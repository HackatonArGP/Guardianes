using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Earthwatchers.Models;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class DistanceCalculationControl
    {
        private List<Point> _pointList;
 
        public DistanceCalculationControl()
        {
            InitializeComponent();
            _pointList = new List<Point>();            
        }

        private void AddPoint(Point position)
        {
            _pointList.Add(position);
            Update();
        }

        private void Update()
        {
            Draw();
            CalculateDistance();
        }

        private void Draw()
        {
            drawCanvas.Children.Clear();
            var polyline = new Polyline { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 4, Opacity = 0.3 };            

            foreach (var point in _pointList)
                polyline.Points.Add(point);                

            drawCanvas.Children.Add(polyline);
            
            const int ellipseWidth = 12;
            foreach (var point in _pointList)
                drawCanvas.Children.Add(new Ellipse { Margin = new Thickness(point.X - ellipseWidth / 2, point.Y - ellipseWidth / 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Width = ellipseWidth, Height = ellipseWidth, Fill = new SolidColorBrush(Colors.Orange) });         
        }

        private void CalculateDistance()
        {
            var distance = 0d;

            for (var i = 0; i < _pointList.Count; i++)
            {
                if (i + 1 >= _pointList.Count)
                    break;

                var world1 = Current.Instance.MapControl.Viewport.ScreenToWorld(_pointList[i].X, _pointList[i].Y);
                var world2 = Current.Instance.MapControl.Viewport.ScreenToWorld(_pointList[i + 1].X, _pointList[i + 1].Y);

                var lonLat1 = SphericalMercator.ToLonLat(world1.X, world1.Y);
                var lonLat2 = SphericalMercator.ToLonLat(world2.X, world2.Y);

                distance += Distance(lonLat1.y, lonLat1.x, lonLat2.y, lonLat2.x);
            }

            txtDistance.Text = string.Format("Distance: {0} KM", Math.Round(distance, 4));
        }

        private static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            var theta = lon1 - lon2;
            var dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;

            return (dist);
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private void GridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddPoint(e.GetPosition(drawCanvas));
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {
            _pointList = new List<Point>();
            Update();
        }

        private void BtnStop(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            _pointList = new List<Point>();
            Update();
        }
    }
}
