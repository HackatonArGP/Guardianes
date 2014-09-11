﻿// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of Mapsui.
// Mapsui is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Mapsui is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Mapsui; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA f

using System.Diagnostics;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Rendering;
using Mapsui.Rendering.XamlRendering;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mapsui.Windows
{
    public class MapControl : Grid
    {
        #region Fields

        private Map map;
        private readonly Viewport viewport = new Viewport();
        private Point previousMousePosition;
        private Point currentMousePosition;
        private Point downMousePosition;
        private string errorMessage;
        private readonly FpsCounter fpsCounter = new FpsCounter();
        private readonly DoubleAnimation zoomAnimation = new DoubleAnimation();
        private readonly Storyboard zoomStoryBoard = new Storyboard();
        private double toResolution = double.NaN;
        private bool mouseDown;
        private bool IsInBoxZoomMode { get; set; }
        private bool viewInitialized;
        private readonly Canvas renderCanvas = new Canvas();
        public IRenderer Renderer { get; set; }
        private bool invalid;
        private readonly Rectangle bboxRect;

        #endregion

        #region EventHandlers

        public event EventHandler ErrorMessageChanged;
        public event EventHandler<ViewChangedEventArgs> ViewChanged;
        public event EventHandler<MouseInfoEventArgs> MouseInfoOver;
        public event EventHandler MouseInfoLeave;
        public event EventHandler<MouseInfoEventArgs> MouseInfoDown;
        public event EventHandler<FeatureInfoEventArgs> FeatureInfo;

        #endregion

        #region Properties

        public IList<ILayer> MouseInfoOverLayers { get; private set; }
        public IList<ILayer> MouseInfoDownLayers { get; private set; }

        public bool ZoomToBoxMode { get; set; }
        public Viewport Viewport { get { return viewport; } }

        public Map Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map != null)
                {
                    var temp = map;
                    map = null;
                    temp.PropertyChanged -= MapPropertyChanged;
                    temp.Dispose();
                }

                map = value;
                //all changes of all layers are returned through this event handler on the map
                if (map != null)
                {
                    map.DataChanged += MapDataChanged;
                    map.PropertyChanged += MapPropertyChanged;
                    map.ViewChanged(true, viewport.Extent, viewport.Resolution);
                }
                OnViewChanged(true);
                RefreshGraphics();
            }
        }

        void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Envelope")
            {
                InitializeView();
                map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            }
        }

        public FpsCounter FpsCounter
        {
            get
            {
                return fpsCounter;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        public bool ZoomLocked { get; set; }

        public Canvas RenderCanvas
        {
            get { return renderCanvas; }
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ResolutionProperty =
          DependencyProperty.Register(
          "Resolution", typeof(double), typeof(MapControl),
          new PropertyMetadata(OnResolutionChanged));

        #endregion

        public MapControl()
        {
            renderCanvas = new Canvas
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.Transparent),
                };
            Children.Add(RenderCanvas);

            bboxRect = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.Red),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 3,
                    RadiusX = 0.5,
                    RadiusY = 0.5,
                    StrokeDashArray = new DoubleCollection { 3.0 },
                    Opacity = 0.3,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Visibility = Visibility.Collapsed
                };
            Children.Add(bboxRect);

            Map = new Map();
            MouseInfoOverLayers = new List<ILayer>();
            MouseInfoDownLayers = new List<ILayer>();
            Loaded += MapControlLoaded;
            KeyDown += MapControlKeyDown;
            KeyUp += MapControlKeyUp;
            MouseLeftButtonDown += MapControlMouseLeftButtonDown;
            MouseLeftButtonUp += MapControlMouseLeftButtonUp;
#if (!WINDOWS_PHONE) //turn off mouse controls
            MouseMove += MapControlMouseMove;
            MouseLeave += MapControlMouseLeave;
            MouseWheel += MapControlMouseWheel;
#endif
            SizeChanged += MapControlSizeChanged;
            CompositionTarget.Rendering += CompositionTargetRendering;
            Renderer = new MapRenderer(RenderCanvas);

#if (!SILVERLIGHT && !WINDOWS_PHONE)
            Dispatcher.ShutdownStarted += DispatcherShutdownStarted;
            RenderCanvas.IsManipulationEnabled = true;
            RenderCanvas.ManipulationDelta += OnManipulationDelta;
            RenderCanvas.ManipulationCompleted += OnManipulationCompleted;
            RenderCanvas.ManipulationInertiaStarting += OnManipulationInertiaStarting;
#endif

#if (WINDOWS_PHONE)
            RenderCanvas.ManipulationDelta += WP_OnManipulationDelta;
            RenderCanvas.ManipulationCompleted += WP_OnManipulationCompleted;
            RenderCanvas.ManipulationStarted += WP_OnManipulationStarted;
#endif
        }

        #region Public methods

        public void OnViewChanged(bool changeEnd)
        {
            OnViewChanged(changeEnd, false);
        }

        private void OnViewChanged(bool changeEnd, bool userAction)
        {
            if (map != null)
            {
                if (ViewChanged != null)
                {
                    ViewChanged(this, new ViewChangedEventArgs { Viewport = viewport, UserAction = userAction });
                }
            }
        }

        public void Refresh()
        {
            map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            RefreshGraphics();
        }

        private void RefreshGraphics() //should be private soon
        {
#if (!SILVERLIGHT && !WINDOWS_PHONE)
            InvalidateVisual();
#endif
            InvalidateArrange();
            invalid = true;
        }

        public void Clear()
        {
            if (map != null)
            {
                map.ClearCache();
            }
            RefreshGraphics();
        }

        public void ZoomIn()
        {
            if (ZoomLocked)
                return;

            if (double.IsNaN(toResolution))
                toResolution = viewport.Resolution;

            toResolution = ZoomHelper.ZoomIn(map.Resolutions, toResolution);
            ZoomMiddle();
        }

        public void ZoomOut()
        {
            if (double.IsNaN(toResolution))
                toResolution = viewport.Resolution;

            toResolution = ZoomHelper.ZoomOut(map.Resolutions, toResolution);
            ZoomMiddle();
        }

        #endregion

        #region Protected and private methods

        protected void OnErrorMessageChanged(EventArgs e)
        {
            if (ErrorMessageChanged != null)
            {
                ErrorMessageChanged(this, e);
            }
        }

        private static void OnResolutionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var newResolution = (double)e.NewValue;
            ((MapControl)dependencyObject).ZoomToResolution(newResolution);
        }

        private void ZoomToResolution(double resolution)
        {
            Point mousePosition = currentMousePosition;
            // When zooming we want the mouse position to stay above the same world coordinate.
            // We calcultate that in 3 steps.

            // 1) Temporarily center on the mouse position
            viewport.Center = viewport.ScreenToWorld(mousePosition.X, mousePosition.Y);

            // 2) Then zoom 
            viewport.Resolution = resolution;

            // 3) Then move the temporary center of the map back to the mouse position
            viewport.Center = viewport.ScreenToWorld(
              viewport.Width - mousePosition.X,
              viewport.Height - mousePosition.Y);

            map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            OnViewChanged(true);
            RefreshGraphics();
        }

        private void ZoomMiddle()
        {
            currentMousePosition = new Point(ActualWidth / 2, ActualHeight / 2);
            StartZoomAnimation(viewport.Resolution, toResolution);
        }

        private void MapControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!viewInitialized) InitializeView();
            UpdateSize();
            InitAnimation();

#if (!SILVERLIGHT && !WINDOWS_PHONE)
            Focusable = true;
#endif
        }

        private void InitAnimation()
        {
            zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 1000));
            zoomAnimation.EasingFunction = new QuarticEase();
            Storyboard.SetTarget(zoomAnimation, this);
            Storyboard.SetTargetProperty(zoomAnimation, new PropertyPath("Resolution"));
            zoomStoryBoard.Children.Add(zoomAnimation);
        }

        private void MapControlMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ZoomLocked)
                return;

            currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event

            if (double.IsNaN(toResolution))
            {
                toResolution = viewport.Resolution;
            }

            if (e.Delta > 0)
            {
                toResolution = ZoomHelper.ZoomIn(map.Resolutions, toResolution);
            }
            else if (e.Delta < 0)
            {
                toResolution = ZoomHelper.ZoomOut(map.Resolutions, toResolution);
            }

            e.Handled = true; //so that the scroll event is not sent to the html page.

            //some cheating for personal gain
            viewport.CenterX += 0.000000001;
            viewport.CenterY += 0.000000001;
            map.ViewChanged(false, viewport.Extent, viewport.Resolution);
            OnViewChanged(false, true);

            StartZoomAnimation(viewport.Resolution, toResolution);
        }

        private void StartZoomAnimation(double begin, double end)
        {
            zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            zoomAnimation.From = begin;
            zoomAnimation.To = end;
            zoomAnimation.Completed += ZoomAnimationCompleted;
            zoomStoryBoard.Begin();
        }

        private void ZoomAnimationCompleted(object sender, EventArgs e)
        {
            toResolution = double.NaN;
        }

        private void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!viewInitialized) InitializeView();
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
            UpdateSize();
            map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            OnViewChanged(true);
            Refresh();
        }

        private void UpdateSize()
        {
            var rect = new RectangleGeometry();
            rect.Rect = new Rect(0f, 0f, ActualWidth, ActualHeight);

            if (Viewport != null)
            {
                viewport.Width = ActualWidth;
                viewport.Height = ActualHeight;
            }
        }

        private void MapControlMouseLeave(object sender, MouseEventArgs e)
        {
            previousMousePosition = new Point();
            ReleaseMouseCapture();
        }

        public void MapDataChanged(object sender, DataChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new DataChangedEventHandler(MapDataChanged), new[] { sender, e });
            }
            else
            {
                if (e.Cancelled)
                {
                    errorMessage = "Cancelled";
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is System.Net.WebException)
                {
                    errorMessage = "WebException: " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error != null)
                {
                    errorMessage = e.Error.GetType() + ": " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else // no problems
                {
                    RefreshGraphics();
                }

            }
        }

        private void MapControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var eventArgs = GetMouseInfoEventArgs(e.GetPosition(this), MouseInfoDownLayers);
            OnMouseInfoDown(eventArgs ?? new MouseInfoEventArgs());
            previousMousePosition = e.GetPosition(this);
            downMousePosition = e.GetPosition(this);
            mouseDown = true;
            CaptureMouse();
        }

        private void MapControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsInBoxZoomMode || ZoomToBoxMode)
            {
                ZoomToBoxMode = false;
                Geometries.Point previous = Viewport.ScreenToWorld(previousMousePosition.X, previousMousePosition.Y);
                Geometries.Point current = Viewport.ScreenToWorld(e.GetPosition(this).X, e.GetPosition(this).Y);
                ZoomToBox(previous, current);
            }
            else
            {
                HandleFeatureInfo(e);
            }

            map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            OnViewChanged(true, true);
            mouseDown = false;

            previousMousePosition = new Point();
            ReleaseMouseCapture();
        }

        private void HandleFeatureInfo(MouseButtonEventArgs e)
        {
            if (FeatureInfo == null) return; // don't fetch if you the call back is not set.

            if (downMousePosition == e.GetPosition(this))
            {
                foreach (var layer in Map.Layers)
                {
                    if (layer is IFeatureInfo)
                    {
                        (layer as IFeatureInfo).GetFeatureInfo(viewport, downMousePosition.X, downMousePosition.Y, OnFeatureInfo);
                    }
                }
            }
        }

        private void OnFeatureInfo(IDictionary<string, IEnumerable<IFeature>> features)
        {
            if (FeatureInfo != null)
            {
                FeatureInfo(this, new FeatureInfoEventArgs { FeatureInfo = features });
            }
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("MapControlMouseMove");

            if (IsInBoxZoomMode || ZoomToBoxMode)
            {
                DrawBbox(e.GetPosition(this));
                return;
            }

            if (!mouseDown) RaiseMouseInfoEvents(e.GetPosition(this));

            if (mouseDown)
            {
                if (previousMousePosition == new Point())
                {
                    return; // It turns out that sometimes MouseMove+Pressed is called before MouseDown
                }

                currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event
                viewport.Transform(currentMousePosition.X, currentMousePosition.Y, previousMousePosition.X, previousMousePosition.Y);
                previousMousePosition = currentMousePosition;
                map.ViewChanged(false, viewport.Extent, viewport.Resolution);
                OnViewChanged(false, true);
                RefreshGraphics();
            }
        }

        private void RaiseMouseInfoEvents(Point mousePosition)
        {
            if (!mouseDown)
            {
                var mouseEventArgs = GetMouseInfoEventArgs(mousePosition, MouseInfoOverLayers);
                if (mouseEventArgs == null) OnMouseInfoLeave();
                else OnMouseInfoOver(mouseEventArgs);
            }
        }

        private MouseInfoEventArgs GetMouseInfoEventArgs(Point mousePosition, IEnumerable<ILayer> layers)
        {
            var margin = 8 * Viewport.Resolution;
            var point = Viewport.ScreenToWorld(new Geometries.Point(mousePosition.X, mousePosition.Y));

            foreach (var layer in layers)
            {
                var feature = layer.GetFeaturesInView(Map.Envelope, 0).FirstOrDefault(f =>
                    f.Geometry.GetBoundingBox().GetCentroid().Distance(point) < margin);
                if (feature != null)
                {
                    return new MouseInfoEventArgs { LayerName = layer.LayerName, Feature = feature };
                }
            }
            return null;
        }

        protected void OnMouseInfoLeave()
        {
            if (MouseInfoLeave != null)
            {
                MouseInfoLeave(this, new EventArgs());
            }
        }

        protected void OnMouseInfoOver(MouseInfoEventArgs e)
        {
            if (MouseInfoOver != null)
            {
                MouseInfoOver(this, e);
            }
        }

        protected void OnMouseInfoDown(MouseInfoEventArgs e)
        {
            if (MouseInfoDown != null)
            {
                MouseInfoDown(this, e);
            }
        }

        private void InitializeView()
        {
            if (ActualWidth.IsNanOrZero()) return;
            if (map == null) return;
            if (map.Envelope == null) return;
            if (map.Envelope.Width.IsNanOrZero()) return;
            if (map.Envelope.Height.IsNanOrZero()) return;
            if (map.Envelope.GetCentroid() == null) return;

            if ((viewport.CenterX > 0) && (viewport.CenterY > 0) && (viewport.Resolution > 0))
            {
                viewInitialized = true; //view was already initialized
                return;
            }

            viewport.Center = map.Envelope.GetCentroid();
            viewport.Resolution = map.Envelope.Width / ActualWidth;
            viewInitialized = true;
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (!viewInitialized) InitializeView();
            if (!viewInitialized) return; //stop if the line above failed. 
            if (!invalid) return;

            if ((Renderer != null) && (map != null))
            {
                Renderer.Render(viewport, map.Layers);
                fpsCounter.FramePlusOne();
                invalid = false;
            }
        }

#if !SILVERLIGHT 
        private void DispatcherShutdownStarted(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTargetRendering;
            if (map != null)
            {
                map.Dispose();
            }
        }
#endif

        #endregion

        #region Bbox zoom

        public void ZoomToBox(Geometries.Point beginPoint, Geometries.Point endPoint)
        {
            double x, y, resolution;
            var width = Math.Abs(endPoint.X - beginPoint.X);
            var height = Math.Abs(endPoint.Y - beginPoint.Y);
            if (width <= 0) return;
            if (height <= 0) return;

            ZoomHelper.ZoomToBoudingbox(beginPoint.X, beginPoint.Y, endPoint.X, endPoint.Y, ActualWidth, out x, out y, out resolution);
            resolution = ZoomHelper.ClipToExtremes(map.Resolutions, resolution);

            viewport.Center = new Geometries.Point(x, y);
            viewport.Resolution = resolution;
            toResolution = resolution;

            map.ViewChanged(true, viewport.Extent, viewport.Resolution);
            OnViewChanged(true, true);
            RefreshGraphics();
            ClearBBoxDrawing();
        }

        private void ClearBBoxDrawing()
        {
            bboxRect.Margin = new Thickness(0, 0, 0, 0);
            bboxRect.Width = 0;
            bboxRect.Height = 0;
        }

        private void MapControlKeyUp(object sender, KeyEventArgs e)
        {
            String keyName = e.Key.ToString().ToLower();
            if (keyName.Equals("ctrl") || keyName.Equals("leftctrl") || keyName.Equals("rightctrl"))
            {
                IsInBoxZoomMode = false;
            }
        }

        private void MapControlKeyDown(object sender, KeyEventArgs e)
        {
            String keyName = e.Key.ToString().ToLower();
            if (keyName.Equals("ctrl") || keyName.Equals("leftctrl") || keyName.Equals("rightctrl"))
            {
                IsInBoxZoomMode = true;
            }
        }

        private void DrawBbox(Point newPos)
        {
            if (mouseDown)
            {
                Point from = previousMousePosition;
                Point to = newPos;

                if (from.X > to.X)
                {
                    Point temp = from;
                    from.X = to.X;
                    to.X = temp.X;
                }

                if (from.Y > to.Y)
                {
                    Point temp = from;
                    from.Y = to.Y;
                    to.Y = temp.Y;
                }

                bboxRect.Width = to.X - from.X;
                bboxRect.Height = to.Y - from.Y;
                bboxRect.Margin = new Thickness(from.X, from.Y, 0, 0);
            }
        }

        #endregion

        public void ZoomToFullEnvelope()
        {
            if (Map.Envelope == null) return;
            if (ActualWidth.IsNanOrZero()) return;
            viewport.Resolution = Map.Envelope.Width / ActualWidth;
            viewport.Center = Map.Envelope.GetCentroid();
        }

        #region WPF4 Touch Support

#if (!SILVERLIGHT && !WINDOWS_PHONE)

        private static void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 25 * 96.0 / (1000.0 * 1000.0);
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var previous = viewport.ScreenToWorld(e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            var current = viewport.ScreenToWorld(e.ManipulationOrigin.X + e.DeltaManipulation.Translation.X, e.ManipulationOrigin.Y + e.DeltaManipulation.Translation.Y);

            double scale = (e.DeltaManipulation.Scale.X != 1d && !ZoomLocked) ? 
                ((e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2) : 1.0;
            PanAndZoom(current, previous, scale);

            invalid = true;
            // not calling map.ViewChanged(false, view.Extent, view.Resolution); for smoother panning/zooming
            OnViewChanged(false, true);            
        }
               
        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Refresh();
        }

#endif

#if (WINDOWS_PHONE)
        private void WP_OnManipulationStarted(object sender, ManipulationStartedEventArgs manipulationStartedEventArgs)
        {
        }

        private void WP_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs manipulationCompletedEventArgs)
        {
            Refresh();
        }

        private void WP_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var previous = viewport.ScreenToWorld(e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            var current = viewport.ScreenToWorld(e.ManipulationOrigin.X + e.DeltaManipulation.Translation.X, e.ManipulationOrigin.Y + e.DeltaManipulation.Translation.Y);

            double scale = (e.DeltaManipulation.Scale.X != 1d && !ZoomLocked) ?
                ((e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2) : 1.0;
            PanAndZoom(current, previous, (scale <= 0) ? 1 : scale);

            invalid = true;
            // not calling map.ViewChanged(false, view.Extent, view.Resolution); for smoother panning/zooming
            OnViewChanged(false, true);
        }
#endif

        //end of windows phone support
        private void PanAndZoom(Geometries.Point current, Geometries.Point previous, double deltaScale)
        {
            var diffX = previous.X - current.X;
            var diffY = previous.Y - current.Y;
            var newX = viewport.CenterX + diffX;
            var newY = viewport.CenterY + diffY;
            var zoomCorrectionX = (1 - deltaScale) * (current.X - viewport.CenterX);
            var zoomCorrectionY = (1 - deltaScale) * (current.Y - viewport.CenterY);
            viewport.Resolution = viewport.Resolution / deltaScale;

            viewport.Center = new Geometries.Point(newX - zoomCorrectionX, newY - zoomCorrectionY);
        }

        #endregion
    }

    public class ViewChangedEventArgs : EventArgs
    {
        public Viewport Viewport { get; set; }
        public bool UserAction { get; set; }
    }

    public class MouseInfoEventArgs : EventArgs
    {
        public MouseInfoEventArgs()
        {
            LayerName = string.Empty;
        }

        public string LayerName { get; set; }
        public IFeature Feature { get; set; }
    }

    public class FeatureInfoEventArgs : EventArgs
    {
        public IDictionary<string, IEnumerable<IFeature>> FeatureInfo { get; set; }
    }
}