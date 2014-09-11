using BruTile.PreDefined;
using Microsoft.Maps.MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BingMapsTest
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            //this.mapControl.ViewChangeOnFrame += mapControl_ViewChangeOnFrame;
            //this.mapControl.ViewChangeStart += mapControl_ViewChangeStart;
            //this.mapControl.ViewChangeEnd += mapControl_ViewChangeEnd;
            this.Loaded += MainPage_Loaded;
        }

        double startH = 0;
        double endH = 0;
        double startW = 0;
        double endW = 0;
        void mapControl_ViewChangeEnd(object sender, MapEventArgs e)
        {
            endH = this.mapControl.BoundingRectangle.Height;
            endW = this.mapControl.BoundingRectangle.Width;
            
            if (startH != endH || startW != endW)
            {
                var layer = this.mapControl.Children[1] as MapLayer;
                if (layer != null)
                {
                    Image img = layer.Children[0] as Image;
                    img.RenderTransform = new ScaleTransform { ScaleX = startW / endW, ScaleY = startH / endH };
                }
            }
        }

        void mapControl_ViewChangeStart(object sender, MapEventArgs e)
        {
            startH = this.mapControl.BoundingRectangle.Height;
            startW = this.mapControl.BoundingRectangle.Width;
        }

        void mapControl_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            
        }

        MapTileLayer tileLayer;

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            double latitude = -25.292; 
            double longitude = -65.353;

            //Agregar un Hexágono
            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new LinearGradientBrush(new GradientStopCollection() 
            { 
                new GradientStop { Color = Colors.Black, Offset = 0 },
                new GradientStop { Color = Colors.White, Offset = 1 }
            }, 0);
            //polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            polygon.Locations = RenderHexagon(latitude, longitude);

            mapControl.Children.Add(polygon);

            MapLayer imageLayer = new MapLayer();
            imageLayer.Name = "ImageLayer";

            /*
            Pushpin pin = new Pushpin();
            pin.ContentTemplate = this.Resources["PinTemplate"] as DataTemplate;
            pin.Location = new Location(latitude, longitude);
            mapControl.Children.Add(pin);
             * */

            
            Image image = new Image();
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("G.png", UriKind.Relative));
            image.Opacity = 1;
            image.Stretch = System.Windows.Media.Stretch.None;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            imageLayer.AddChild(image, new Location(latitude, longitude), PositionOrigin.Center);
            //Add the image layer to the map
            mapControl.Children.Add(imageLayer);
            

            /*
            //Agrego un Tile Layer
            tileLayer = new MapTileLayer();

            // The source of the overlay.
            LocationRectTileSource tileSource = new LocationRectTileSource();
            tileSource.UriFormat = "https://guardianes.blob.core.windows.net/landsat/20130603_NEW/";
            // The zoom range that the tile overlay is visibile within
            tileSource.ZoomRange = new Range<double>(6, 13);
            // The bounding rectangle area that the tile overaly is valid in.
            tileSource.BoundingRectangle = mapControl.BoundingRectangle;

            // Adds the tile overlay to the map layer
            tileLayer.TileSources.Add(tileSource);

            // Adds the map layer to the map
            if (!mapControl.Children.Contains(tileLayer))
            {
                mapControl.Children.Add(tileLayer);
            }



            var topLeft = FromLonLat(mapControl.BoundingRectangle.West, mapControl.BoundingRectangle.North);
            var bottomRight = FromLonLat(mapControl.BoundingRectangle.East, mapControl.BoundingRectangle.South);

            var extent = new BruTile.Extent(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);

            var schema = new SphericalMercatorWorldSchema();
            schema.Extent = extent;

            var tiles = schema.GetTilesInView(extent, 12);
            foreach (var tile in tiles)
            {

            }
             * */

        }

        private LocationCollection RenderHexagon(double latitude, double longitude)
        {
            var locations = new LocationCollection() { 
                new Location(latitude, longitude - 0.00375),
                new Location(latitude + 0.00335, longitude - 0.002),
                new Location(latitude + 0.00335, longitude + 0.002),
                new Location(latitude, longitude + 0.00375),
                new Location(latitude - 0.00335, longitude + 0.002),
                new Location(latitude - 0.00335, longitude - 0.002),
                new Location(latitude, longitude - 0.00375)
        };
            return locations;
        }

        private const double Radius = 6378137;
        private const double D2R = Math.PI / 180;
        private const double HalfPi = Math.PI / 2;

        public static Point FromLonLat(double lon, double lat)
        {
            var lonRadians = (D2R * lon);
            var latRadians = (D2R * lat);

            var x = Radius * lonRadians;
            var y = Radius * Math.Log(Math.Tan(Math.PI * 0.25 + latRadians * 0.5));

            return new Point(x, y);
        }
    }
}
