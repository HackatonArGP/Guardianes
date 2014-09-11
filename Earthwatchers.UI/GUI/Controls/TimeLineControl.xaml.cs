using System;
using System.Windows;
using System.Linq;
using Earthwatchers.UI.Resources;
using Mapsui.Layers;
using Earthwatchers.Models;
using Earthwatchers.UI.Layers;
using System.Collections.Generic;
using Earthwatchers.UI.Requests;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BruTile.PreDefined;
using BruTile.Web;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class TimeLineControl
    {
        private List<SatelliteImage> satelliteImages;
        private List<SatelliteImage> includedSatelliteImages;
        private ImageRequests imageRequests;
        private LayerHelper layerHelper;
        LayerControlOnOff hexagonOnOff;
        LayerControlOnOff forestLawOnOff;
        LayerControlOnOff basecampsOnOff;

        public delegate void ChangingOpacityEventHandler(object sender, ChangingOpacityEventArgs e);
        public event ChangingOpacityEventHandler ChangingOpacity;

        public delegate void HexLayerVisibilityChangedEventHandler(object sender, SharedEventArgs e);
        public event HexLayerVisibilityChangedEventHandler HexLayerVisibilityChanged;

        public delegate void ArgentineLawLayerLayerVisibilityChangedEventHandler(object sender, SharedEventArgs e);
        public event ArgentineLawLayerLayerVisibilityChangedEventHandler ArgentineLawLayerLayerVisibilityChanged;

        public delegate void ArgentineLawLoadedEventHandler(object sender, EventArgs e);
        public event ArgentineLawLoadedEventHandler ArgentineLawLoaded;

        public delegate void ArgentineLawLoadingEventHandler(object sender, EventArgs e);
        public event ArgentineLawLoadingEventHandler ArgentineLawLoading;

        public TimeLineControl()
        {
            InitializeComponent();
            satelliteImages = new List<SatelliteImage>();
            includedSatelliteImages = new List<SatelliteImage>();

            //Cuando se inicializa el Timeline Control carga las últimas 1000 imágenes satelitales que encuentra (SP GET ALL)
            imageRequests = new ImageRequests(Constants.BaseApiUrl);
            imageRequests.ImageRequestReceived += imageRequests_ImageRequestReceived;

            this.SliderAnimation1.Completed += SliderAnimation1_Completed;
            this.SliderAnimation2.Completed += SliderAnimation2_Completed;

            this.Loaded += TimeLineControl_Loaded;
        }

        void SliderAnimation2_Completed(object sender, EventArgs e)
        {
            ChangeLayersOpacity();
            hexagonOnOff.ShowHexagonBg();
        }

        void SliderAnimation1_Completed(object sender, EventArgs e)
        {
            ChangeLayersOpacity();
            this.SliderAnimation2.Begin();
        }

        void TimeLineControl_Loaded(object sender, RoutedEventArgs e)
        {
            imageRequests.GetAllImagery();
        }

        public void Highlight()
        {
            this.TutorialStoryBoard.Begin();
        }

        public void SetLayerHelper(LayerHelper layerHelper)
        {
            this.layerHelper = layerHelper;

            var hexLayer = layerHelper.FindLayer(Constants.Hexagonlayername);
            if (hexLayer == null)
                return;

            hexagonOnOff = new LayerControlOnOff(hexLayer, true);
            hexagonOnOff.VisibilityChanged += hexagonLayerOnOff_VisibilityChanged;
            layerWrapper.Children.Add(hexagonOnOff);
        }

        public void AddArgentineLawLayer()
        {
            var lawLayer = layerHelper.FindLayer(Constants.ArgentineLawlayername) as ArgentineLawLayer;
            if (lawLayer == null)
                return;

            lawLayer.Loaded += lawLayer_Loaded;
            lawLayer.Loading += lawLayer_Loading;
            forestLawOnOff = new LayerControlOnOff(lawLayer, false);
            forestLawOnOff.VisibilityChanged += lawLayerOnOff_VisibilityChanged;

            if (!IsLayerAlreadyLoaded(layerWrapper.Children, Constants.ArgentineLawlayername))
                layerWrapper.Children.Add(forestLawOnOff);
        }
        
        public void AddBasecampsLayer()
        {
            var basecamp = layerHelper.FindLayer(Constants.BasecampsLayer) as BasecampLayer;
            if (basecamp == null)
                return;
            basecampsOnOff = new LayerControlOnOff(basecamp, true);

            if (!IsLayerAlreadyLoaded(layerWrapper.Children, Constants.BasecampsLayer))
                layerWrapper.Children.Add(basecampsOnOff);
        }

        public bool IsLayerAlreadyLoaded(UIElementCollection collection, string layerName)
        {
            bool alreadyLoaded = false;
            foreach (var elem in collection)
            {
                if (elem is LayerControlOnOff)
                {
                    if ((elem as LayerControlOnOff).GetLayerName() == layerName)
                    {
                        alreadyLoaded = true;
                        break;
                    }
                }
            }
            return alreadyLoaded;
        }

        void lawLayerOnOff_VisibilityChanged(object sender, SharedEventArgs e)
        {
            ArgentineLawLayerLayerVisibilityChanged(this, e);
        }

        void lawLayer_Loading(object sender, EventArgs e)
        {
            forestLawOnOff.Visibility = System.Windows.Visibility.Collapsed;
            hexagonOnOff.Visibility = System.Windows.Visibility.Collapsed;
            loading.Visibility = System.Windows.Visibility.Visible;

            ArgentineLawLoading(this, EventArgs.Empty);
        }

        void lawLayer_Loaded(object sender, EventArgs e)
        {
            loading.Visibility = System.Windows.Visibility.Collapsed;
            forestLawOnOff.Visibility = System.Windows.Visibility.Visible;
            hexagonOnOff.Visibility = System.Windows.Visibility.Visible;

            ArgentineLawLoaded(this, EventArgs.Empty);
        }

        void hexagonLayerOnOff_VisibilityChanged(object sender, SharedEventArgs e)
        {
            HexLayerVisibilityChanged(this, e);
        }

        public void StartSliderAnimation()
        {
            SliderAnimation1.Begin();
        }

        void imageRequests_ImageRequestReceived(object sender, System.EventArgs e)
        {
            satelliteImages = sender as List<SatelliteImage>;

            if (satelliteImages != null)
            {
                includedSatelliteImages = satelliteImages.Where(x => x.Name == "2008").ToList(); //Muestro solo 5 mas la del 2008, el resto las mando al combo
                includedSatelliteImages.AddRange(satelliteImages.Where(x => x.Name != "2008").OrderByDescending(x => x.Published).Take(3).ToList());
                Current.Instance.LastImageDate = includedSatelliteImages.First().Published.Value.ToString("MMMM yyyy");
                CreateTimeLine();

                if (!Current.Instance.TutorialStarted)
                {
                    var text = this.DatesGrid.Children[this.DatesGrid.ColumnDefinitions.Count - 1] as TextBlock; //Me traigo la ultima fecha del timeline
                    if (text != null)
                        ChangingOpacity(this, new ChangingOpacityEventArgs { Title = text.Text, IsCloudy = includedSatelliteImages.First().IsCloudy, IsInitial = true });
                }
            }
        }

        void CreateTimeLine()
        {
            //Genero los items del combo
            List<SatelliteImage> comboImages = new List<SatelliteImage>();
            foreach (var image in satelliteImages.OrderByDescending(x => x.Published))
            {
                if (!includedSatelliteImages.Any(x => x.Id == image.Id))
                {
                    comboImages.Add(image);
                }
            }
            this.ImagesCombo.ItemsSource = comboImages;

            DatesGrid.Children.Clear();
            DatesGrid.ColumnDefinitions.Clear();

            TimelineMarkersGrid.Children.Clear();
            TimelineMarkersGrid.ColumnDefinitions.Clear();

            int count = 0;
            foreach (var image in includedSatelliteImages.OrderBy(x => x.Published))
            {
                DatesGrid.ColumnDefinitions.Add(new ColumnDefinition());
                TimelineMarkersGrid.ColumnDefinitions.Add(new ColumnDefinition());

                Ellipse ellipse = new Ellipse();
                ellipse.Width = 12;
                ellipse.Height = 12;
                ellipse.StrokeThickness = 1;
                ellipse.IsHitTestVisible = false;
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 170, 203, 13));
                ellipse.Stroke = new SolidColorBrush(Color.FromArgb(255, 106, 126, 11));
                Grid.SetRow(ellipse, 1);

                TextBlock textBlock = new TextBlock();
                if (image.Name.IndexOf("2008") > -1)
                {
                    textBlock.Text = image.Name;
                }
                else
                {
                    textBlock.Text = image.Published.Value.ToString("dd-MMM");
                }
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                textBlock.FontSize = 13;
                textBlock.Tag = image;
                textBlock.TextWrapping = TextWrapping.Wrap;
                if (includedSatelliteImages.Count() > 1 && image.Name.IndexOf("2008") == -1)
                {
                    textBlock.Cursor = System.Windows.Input.Cursors.Hand;
                    textBlock.MouseLeftButtonDown += textBlock_MouseLeftButtonDown;
                    textBlock.MouseEnter += textBlock_MouseEnter;
                    textBlock.MouseLeave += textBlock_MouseLeave;
                    ToolTipService.SetToolTip(textBlock, Labels.Layers10);
                }
                textBlock.FontFamily = new System.Windows.Media.FontFamily("/Earthwatchers.UI;component/Resources/MyriadPro-Regular.otf#Myriad Pro");
                Grid.SetColumn(textBlock, count);
                Grid.SetColumn(ellipse, count);

                if (count % 2 == 0)
                {
                    Grid.SetRow(textBlock, 0);
                }
                else
                {
                    Grid.SetRow(textBlock, 2);
                }

                double opacity = 0;
                if (count == includedSatelliteImages.Count() - 1)
                {
                    textBlock.Margin = new Thickness(0, 0, 5, 0);
                    textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    ellipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    opacity = 1;
                }
                else if (count == 0)
                {
                    textBlock.Margin = new Thickness(5, 0, 0, 0);
                    ellipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                }
                else
                {
                    textBlock.Margin = new Thickness(0, 0, 0, 3);
                    textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    ellipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                }
                this.DatesGrid.Children.Add(textBlock);
                this.TimelineMarkersGrid.Children.Add(ellipse);

                if (Current.Instance.LayerHelper.FindLayer(textBlock.Text) == null)
                {
                    var topLeft = SphericalMercator.FromLonLat(image.Extent.MinX, image.Extent.MinY);
                    var bottomRight = SphericalMercator.FromLonLat(image.Extent.MaxX, image.Extent.MaxY);
                    var newExtend = new BruTile.Extent(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                    var schema = new SphericalMercatorWorldSchema();
                    schema.Extent = newExtend;

                    var max = image.MaxLevel + 1;
                    while (schema.Resolutions.Count > max)
                    {
                        schema.Resolutions.RemoveAt(max);
                    }
                    var tms = new TmsTileSource(image.UrlTileCache, schema);
                    Current.Instance.LayerHelper.InsertLayer(new TileLayer(tms) { LayerName = textBlock.Text, Opacity = opacity, Tag = image.Published.Value }, count);
                }
                count++;
            }

            RefreshMap();
        }

        void textBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = new SolidColorBrush(Colors.White);
        }

        void textBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = new SolidColorBrush(Color.FromArgb(255, 184, 14, 14));
        }

        void textBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (includedSatelliteImages.Count < 2)
            {
                return;
            }

            //Borro la imagen
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null && textBlock.Tag != null)
            {
                includedSatelliteImages.Remove(textBlock.Tag as SatelliteImage);
                Current.Instance.LayerHelper.RemoveLayer(textBlock.Text);
                CreateTimeLine();
            }
        }

        void RefreshMap()
        {
            Current.Instance.MapControl.OnViewChanged(true);
        }

        string title = string.Empty;

        private void ImagesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var image = e.AddedItems[0] as SatelliteImage;
                int order = 0;
                for (int i = 0; i < includedSatelliteImages.Count; i++)
                {
                    if (includedSatelliteImages[i].Published.Value < image.Published.Value)
                    {
                        order = i;
                    }
                    else
                        break;
                }
                includedSatelliteImages.Insert(order, image);
                CreateTimeLine();
            }
        }

        double sliderOldValue = 0;
        private void sliderOpacity_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Relleno los hexagonos
            ChangeLayersOpacity();
            if (hexagonOnOff.IsOn())
            {
            hexagonOnOff.ShowHexagonBg();
        }
            else
            {
                hexagonOnOff.HideHexagonBg();
            }
        }

        private void ChangeLayersOpacity()
        {
            //Oculto los hexagonos
            //Si es distinto al valor inicial entonces ejecuto la lógica
            double newValue = this.sliderOpacity.Value;
            if (newValue != sliderOldValue)
            {
                if (this.DatesGrid != null)
                {
                    int shownImages = this.DatesGrid.ColumnDefinitions.Count - 1;
                    double separator = 100d / shownImages / 100d;

                    double image = newValue / 100d / separator;
                    int i = Convert.ToInt32(Math.Ceiling(image));

                    //System.Diagnostics.Debug.WriteLine(string.Format("i = {0}", i));
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", layer.LayerName, 1 - opacity));

                    //Pongo los demas layers siguientes en 0
                    if (shownImages > i)
                    {
                        for (int x = i + 1; x <= shownImages; x++)
                        {
                            var nextTextBlock = this.DatesGrid.Children[x] as TextBlock;
                            if (nextTextBlock != null)
                            {
                                var nextLayer = Current.Instance.LayerHelper.FindLayer(nextTextBlock.Text);
                                if (nextLayer != null)
                                {
                                    nextLayer.Opacity = 0;
                                    //System.Diagnostics.Debug.WriteLine(string.Format("NEXT {0} {1}", nextLayer.LayerName, 0));
                                }
                            }
                        }
                    }

                    //Pongo los demas layers previos en 0
                    if (i - 1 > 0)
                    {
                        for (int x = 0; x < i - 1; x++)
                        {
                            var prevLTextBlock = this.DatesGrid.Children[x] as TextBlock;
                            if (prevLTextBlock != null)
                            {
                                var prevLLayer = Current.Instance.LayerHelper.FindLayer(prevLTextBlock.Text);
                                if (prevLLayer != null)
                                {
                                    prevLLayer.Opacity = 0;
                                    //System.Diagnostics.Debug.WriteLine(string.Format("PREV PREV {0} {1}", prevLLayer.LayerName, 0));
                                }
                            }
                        }
                    }

                    ChangingOpacity(this, new ChangingOpacityEventArgs { Title = title, IsCloudy = includedSatelliteImages.Any(x => x.Published.Value.ToString("dd-MMM") == title && x.IsCloudy) });

                    RefreshMap();
                }
            }

            sliderOldValue = newValue;
        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (hexagonOnOff != null)
            {
                hexagonOnOff.HideHexagonBg();
            }
            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", e.OldValue, e.NewValue));

            if (this.DatesGrid == null)
                return;

            int shownImages = this.DatesGrid.ColumnDefinitions.Count - 1;
            double separator = 100d / shownImages / 100d;

            double image = e.NewValue / 100d / separator;
            int i = Convert.ToInt32(Math.Ceiling(image));

            var textBlock = this.DatesGrid.Children[i] as TextBlock;
            if (textBlock == null)
                return;

            var layer = Current.Instance.LayerHelper.FindLayer(textBlock.Text);
            if (layer == null)
                return;

            double opacity = i - image;
            layer.Opacity = 1 - opacity;

            if (1 - opacity >= 0.5)
            {
                title = textBlock.Text;
            }

            //System.Diagnostics.Debug.WriteLine(string.Format("i = {0}", i));
            //System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", layer.LayerName, 1 - opacity));

            if (i > 0)
            {
                var prevTextBlock = this.DatesGrid.Children[i - 1] as TextBlock;
                if (prevTextBlock != null)
                {
                    var prevLayer = Current.Instance.LayerHelper.FindLayer(prevTextBlock.Text);
                    if (prevLayer != null)
                    {
                        prevLayer.Opacity = opacity;
                        if (opacity >= 0.5)
                        {
                            title = prevTextBlock.Text;
                        }
                        //System.Diagnostics.Debug.WriteLine(string.Format("PREV {0} {1}", prevLayer.LayerName, opacity));
                    }
                }
            }

            if (e.NewValue == 0)
            {
                var firstTextBlock = this.DatesGrid.Children[0] as TextBlock;
                if (firstTextBlock != null)
                {
                    var firstLayer = Current.Instance.LayerHelper.FindLayer(firstTextBlock.Text);
                    if (firstLayer != null)
                    {
                        firstLayer.Opacity = 1;
                    }
                }

                for (int x = 1; x <= shownImages; x++)
                {
                    var nextTextBlock = this.DatesGrid.Children[x] as TextBlock;
                    if (nextTextBlock != null)
                    {
                        var nextLayer = Current.Instance.LayerHelper.FindLayer(nextTextBlock.Text);
                        if (nextLayer != null)
                        {
                            nextLayer.Opacity = 0;
                        }
                    }
                }
            }

            ChangingOpacity(this, new ChangingOpacityEventArgs { Title = title, IsCloudy = includedSatelliteImages.Any(x => x.Published.Value.ToString("dd-MMM") == title && x.IsCloudy) });

            RefreshMap();
        }
    }
}