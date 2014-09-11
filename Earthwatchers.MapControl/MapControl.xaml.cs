// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA f

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Utilities;
using Mapsui.Rendering.XamlRendering;
using System.Windows.Media.Imaging;

namespace Mapsui.Windows
{
    public partial class MapControl
    {
        #region Fields

        private Map map;
        private readonly Viewport viewport = new Viewport();
        private Point previousMousePosition;
        private Point currentMousePosition;
        private Point startMousePosition;
        private string errorMessage;
        private readonly FpsCounter fpsCounter = new FpsCounter();
        private readonly DoubleAnimation zoomAnimation = new DoubleAnimation();
        private readonly Storyboard zoomStoryBoard = new Storyboard();
        private double toResolution;
        private bool mouseDown;
        private MapRenderer renderer;
        private bool IsInBoxZoomMode { get; set; }
        private bool viewInitialized;
        private Canvas renderCanvas = new Canvas();
        private bool invalid;

        #endregion

        #region EventHandlers

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler zoomFinished;
        public event ChangedEventHandler panFinished;
        public event ChangedEventHandler zoomStarted;
        public event EventHandler ErrorMessageChanged;
        public event EventHandler<ViewChangedEventArgs> ViewChanged;
        public event EventHandler<MouseInfoEventArgs> MouseInfoOver;
        public event EventHandler MouseInfoLeave;
        public event EventHandler<MouseInfoEventArgs> MouseInfoDown;

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
                renderer = new MapRenderer(renderCanvas); //todo reset instead of new.
                if (map != null)
                {
                    var temp = map;
                    map = null;
                    temp.Dispose();
                }

                map = value;
                //all changes of all layers are returned through this event handler on the map
                if (map != null)
                {
                    map.DataChanged += map_DataChanged;
                }
                OnViewChanged(true);
                Redraw();
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

        public bool ShowStatistics
        {
            set
            {
                statistics.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void SetCopyrightText(string copyrightText)
        {
            txtCopyright.Text = copyrightText;
        }

        public void SetCopyrightImage(BitmapImage bitmap)
        {
            copyrightImage.Source = bitmap;
        }

        public void clearCopyright()
        {
            txtCopyright.Text = "";
            copyrightImage.Source = null;
        }

        #endregion

        #region Dependency Properties

        private static readonly DependencyProperty ResolutionProperty =
          DependencyProperty.Register(
          "Resolution", typeof(double), typeof(MapControl),
          new PropertyMetadata(new PropertyChangedCallback(OnResolutionChanged)));

        private static readonly DependencyProperty AllowMoveAndZoomProperty =
          DependencyProperty.Register(
          "AllowMoveAndZoom", typeof(bool), typeof(MapControl),
          new PropertyMetadata(true));

        public bool AllowMoveAndZoom
        {
            get { return (bool)GetValue(AllowMoveAndZoomProperty); }
            set { SetValue(AllowMoveAndZoomProperty, value); }
        }

        #endregion

        #region Constructors

        public MapControl()
        {
            InitializeComponent();
            Map = new Map();
            //this.viewport.Resolution = 19;
#if DEBUG
            ShowStatistics = true;
#else
            ShowStatistics = false;
#endif
            MouseInfoOverLayers = new List<ILayer>();
            MouseInfoDownLayers = new List<ILayer>();
            Loaded += MapControl_Loaded;
            KeyDown += MapControl_KeyDown;
            KeyUp += MapControl_KeyUp;
            MouseLeftButtonDown += MapControl_MouseLeftButtonDown;
            MouseLeftButtonUp += MapControl_MouseLeftButtonUp;
            MouseMove += MapControl_MouseMove;
            MouseLeave += MapControl_MouseLeave;
            MouseWheel += MapControl_MouseWheel;
            SizeChanged += MapControlSizeChanged;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            renderer = new MapRenderer();
            canvas.Children.Add(renderCanvas);


#if !SILVERLIGHT
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
#endif
            fps.SetBinding(TextBlock.TextProperty, new Binding("Fps"));
            fps.DataContext = FpsCounter;
        }

        #endregion

        #region Public methods

        public void OnViewChanged(bool changeEnd)
        {
            OnViewChanged(changeEnd, false);
        }

        public void OnViewChanged(bool changeEnd, bool userAction) //should be private soon
        {
            if (map != null)
            {
                //call down
                map.ViewChanged(changeEnd, viewport.Extent, viewport.Resolution);
                //call up
                if (ViewChanged != null)
                {
                    ViewChanged(this, new ViewChangedEventArgs() { View = this.viewport, UserAction = userAction });
                }
            }

        }

        public void Redraw() //should be private soon
        {

#if !SILVERLIGHT
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
            Redraw();
        }

        public void ZoomIn()
        {
            if (toResolution == 0)
                toResolution = viewport.Resolution;

            toResolution = ZoomHelper.ZoomIn(map.Resolutions, toResolution);
            ZoomMiddle();
        }

        public void ZoomOut()
        {
            if (toResolution == 0)
                toResolution = viewport.Resolution;

            toResolution = ZoomHelper.ZoomOut(map.Resolutions, toResolution);
            ZoomMiddle();
        }

        #endregion

        #region Protected and private methods

        protected virtual void OnErrorMessageChanged(EventArgs e)
        {
            if (ErrorMessageChanged != null)
            {
                ErrorMessageChanged(this, e);
            }
        }

        private static void OnResolutionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var newResolution = (double)e.NewValue;
            ((MapControl)dependencyObject).ZoomIn(newResolution);
        }

        private void ZoomIn(double resolution)
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

            OnViewChanged(true);
            Redraw();
        }

        private void ZoomMiddle()
        {
            currentMousePosition = new Point(ActualWidth / 2, ActualHeight / 2);
            StartZoomAnimation(viewport.Resolution, toResolution);
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!viewInitialized) InitializeView();
            UpdateSize();
            InitAnimation();

#if !SILVERLIGHT
            Focusable = true;
#else
            IsTabStop = true;
#endif
            Focus();
        }

        private void InitAnimation()
        {
            zoomAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            zoomAnimation.EasingFunction = new QuarticEase();
            Storyboard.SetTarget(zoomAnimation, this);
            Storyboard.SetTargetProperty(zoomAnimation, new PropertyPath("Resolution"));
            zoomStoryBoard.Children.Add(zoomAnimation);
            zoomStoryBoard.Completed += ZoomAnimationCompleted;
        }

        private void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (AllowMoveAndZoom)
            {
                currentMousePosition = e.GetPosition(this); //Needed for both MouseMove and MouseWheel event for mousewheel event

                if (toResolution == 0)
                {
                    toResolution = viewport.Resolution;
                }

                if (e.Delta > 0)
                {
                    toResolution = ZoomHelper.ZoomIn(map.Resolutions, toResolution);
                }
                else if (e.Delta < 0)
                {
                    if (toResolution < 1200)  //Stop zooming out at this resolution
                    {
                        toResolution = ZoomHelper.ZoomOut(map.Resolutions, toResolution);
                    }
                }

                e.Handled = true; //so that the scroll event is not sent to the html page.

                //some cheating for personal gain
                viewport.CenterX += 0.000000001;
                viewport.CenterY += 0.000000001;
                OnViewChanged(false, true);

                StartZoomAnimation(viewport.Resolution, toResolution);
            }
        }

        private void StartZoomAnimation(double begin, double end)
        {
            zoomStarted(true, new EventArgs());
            zoomStoryBoard.Pause(); //using Stop() here causes unexpected results while zooming very fast.
            zoomAnimation.From = begin;
            zoomAnimation.To = end;
            zoomStoryBoard.Begin();
        }

        void ZoomAnimationCompleted(object sender, EventArgs e)
        {
            OnZoomFinished(e);
        }

        protected virtual void OnZoomFinished(EventArgs e)
        {
            zoomFinished(true, e);
        }

        protected virtual void OnPanFinished(EventArgs e)
        {
            panFinished(true, e);
        }

        private void MapControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!viewInitialized) InitializeView();
            UpdateSize();
            OnViewChanged(true);
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

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            previousMousePosition = new Point();
            ReleaseMouseCapture();
        }

        public void map_DataChanged(object sender, DataChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new DataChangedEventHandler(map_DataChanged), new object[] { sender, e });
            }
            else
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    Redraw();
                }
                else if (e.Cancelled)
                {
                    errorMessage = "Cancelled";
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else if (e.Error is System.Net.WebException)
                {
                    errorMessage = "WebException: " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
                else
                {
                    errorMessage = e.Error.GetType() + ": " + e.Error.Message;
                    OnErrorMessageChanged(EventArgs.Empty);
                }
            }
        }

        private void MapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            startMousePosition = e.GetPosition(this);
            previousMousePosition = e.GetPosition(this);
            mouseDown = true;
            CaptureMouse();
            Focus();

            var doubleClick = MouseButtonHelper.IsDoubleClick(sender, e);
            if (doubleClick)
                ZoomIn();
        }

        private void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var location = e.GetPosition(this);
            if (location.X == startMousePosition.X && location.Y == startMousePosition.Y)
            {
                var eventArgs = GetMouseInfoEventArgs(e.GetPosition(this), MouseInfoDownLayers);
                if (eventArgs != null) OnMouseInfoDown(eventArgs);
                else OnMouseInfoDown(new MouseInfoEventArgs());
            }

            if (IsInBoxZoomMode || ZoomToBoxMode)
            {
                ZoomToBoxMode = false;
                Geometries.Point previous = Viewport.ScreenToWorld(previousMousePosition.X, previousMousePosition.Y);
                Geometries.Point current = Viewport.ScreenToWorld(e.GetPosition(this).X, e.GetPosition(this).Y);
                ZoomToBox(previous, current);
            }

            //If moved and mousebutton raised a pan action has taken place
            if (GetDistance(startMousePosition, e.GetPosition(this)) > 4)
                OnPanFinished(new EventArgs());

            OnViewChanged(true, true);
            mouseDown = false;

            previousMousePosition = new Point();
            ReleaseMouseCapture();
        }

        private static double GetDistance(Point point1, Point point2)
        {
            var a = (double)(point2.X - point1.X);
            var b = (double)(point2.Y - point1.Y);

            return Math.Sqrt(a * a + b * b);
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (AllowMoveAndZoom)
            {
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
                    MapTransformHelper.Pan(viewport, currentMousePosition, previousMousePosition);
                    previousMousePosition = currentMousePosition;
                    OnViewChanged(false, true);
                    Redraw();
                }
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
            var margin = 20 * Viewport.Resolution;
            var point = Viewport.ScreenToWorld(new Geometries.Point(mousePosition.X, mousePosition.Y + 15));

            foreach (var layer in layers)
            {
                var feature = layer.GetFeaturesInView(Map.Envelope, 0).FirstOrDefault((f) => f.Geometry.Distance(point) < margin);
                if (feature != null)
                {
                    return new MouseInfoEventArgs() { LayerName = layer.LayerName, Feature = feature };
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
            if (double.IsNaN(ActualWidth) || ActualWidth == 0) return;
            if (map == null || map.Envelope == null || double.IsNaN(map.Envelope.Width) || map.Envelope.Width == 0) return;
            if (map.Envelope.GetCentroid() == null) return;

            if ((viewport.CenterX > 0) && (viewport.CenterY > 0) && (viewport.Resolution > 0))
            {
                viewInitialized = true; //view was already initialized by external code
                return;
            }

            viewport.Center = map.Envelope.GetCentroid();
            //viewport.Resolution = map.Envelope.Width / ActualWidth;
            viewInitialized = true;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!viewInitialized) InitializeView();
            if (!viewInitialized) return; //initialize in the line above failed. 
            if (!invalid) return;

            fpsCounter.FramePlusOne();

            if ((renderer != null) && (map != null))
            {
                renderer.Render(viewport, map.Layers);
                invalid = false;
            }

        }



#if !SILVERLIGHT
        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            if (map != null)
            {
                map.Dispose();
            }
        }
#endif

        #endregion

        #region Bbox zoom

        public void ZoomToBox(Mapsui.Geometries.Point beginPoint, Mapsui.Geometries.Point endPoint)
        {
            double x, y, resolution;
            var width = Math.Abs(endPoint.X - beginPoint.X);
            var height = Math.Abs(endPoint.Y - beginPoint.Y);
            if (width <= 0) return;
            if (height <= 0) return;

            ZoomHelper.ZoomToBoudingbox(beginPoint.X, beginPoint.Y, endPoint.X, endPoint.Y, ActualWidth, out x, out y, out resolution);
            resolution = ZoomHelper.ClipToExtremes(map.Resolutions, resolution);

            viewport.Center = new Mapsui.Geometries.Point(x, y);
            viewport.Resolution = resolution;

            toResolution = resolution;

            OnViewChanged(true, true);
            Redraw();
            ClearBBoxDrawing();
        }

        private void ClearBBoxDrawing()
        {
            bboxRect.Margin = new Thickness(0, 0, 0, 0);
            bboxRect.Width = 0;
            bboxRect.Height = 0;
        }

        private void MapControl_KeyUp(object sender, KeyEventArgs e)
        {
            String keyName = e.Key.ToString().ToLower();
            if (keyName.Equals("ctrl") || keyName.Equals("leftctrl") || keyName.Equals("rightctrl"))
            {
                IsInBoxZoomMode = false;
            }
        }

        private void MapControl_KeyDown(object sender, KeyEventArgs e)
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

        public void ZoomToExtent()
        {
            viewport.Resolution = Width / Map.Envelope.Width;
            viewport.Center = Map.Envelope.GetCentroid();
        }
    }

    public class ViewChangedEventArgs : EventArgs
    {
        public Viewport View { get; set; }
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
}