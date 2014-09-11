using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Requests;
using Earthwatchers.Models;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class Flag
    {
        private bool _ignoreClick;
        private bool _isMouseCaptured;
        private double _mouseVerticalPosition;
        private double _mouseHorizontalPosition;

        public Flag()
        {
            InitializeComponent();
        }

        #region drag drop

        public void Handle_MouseDown(object sender, MouseEventArgs args)
        {
            if (_ignoreClick)
            {
                _ignoreClick = !_ignoreClick;
                return;
            }

            imgFlagNoShadow.Visibility = Visibility.Collapsed;
            flagPanel.Visibility = Visibility.Visible;   

            var item = sender as Flag;
            _mouseVerticalPosition = args.GetPosition(null).Y;
            _mouseHorizontalPosition = args.GetPosition(null).X;
            _isMouseCaptured = true;
            if (item != null)
                flagPanel.CaptureMouse();         
        }

        public void Handle_MouseMove(object sender, MouseEventArgs args)
        {
            var item = sender as StackPanel;
            if (!_isMouseCaptured) return;

            // Calculate the current position of the object.
            var deltaV = args.GetPosition(null).Y - _mouseVerticalPosition;
            var deltaH = args.GetPosition(null).X - _mouseHorizontalPosition;
            var oldTop = (double)item.GetValue(Canvas.TopProperty);
            var oldLeft = (double)item.GetValue(Canvas.LeftProperty);
            var newTop = deltaV + (double) item.GetValue(Canvas.TopProperty);
            var newLeft = deltaH + (double) item.GetValue(Canvas.LeftProperty);

            // Set new position of object.
            item.SetValue(Canvas.TopProperty, newTop);
            item.SetValue(Canvas.LeftProperty, newLeft);

            var generalTransform = flagPanel.TransformToVisual(Current.Instance.MapControl);
            var childToParentCoordinates = generalTransform.Transform(new Point(0, 0));

            if(childToParentCoordinates.X <= 0)
                item.SetValue(Canvas.LeftProperty, oldLeft);
            if ((childToParentCoordinates.X + 1) + flagPanel.ActualWidth >= Current.Instance.MapControl.ActualWidth)
                item.SetValue(Canvas.LeftProperty, oldLeft);
            if (childToParentCoordinates.Y <= 0)
                item.SetValue(Canvas.TopProperty, oldTop);
            if ((childToParentCoordinates.Y + 1) + flagPanel.ActualHeight >= Current.Instance.MapControl.ActualHeight)
                item.SetValue(Canvas.TopProperty, oldTop);

            // Update position global variables.
            _mouseVerticalPosition = args.GetPosition(null).Y;
            _mouseHorizontalPosition = args.GetPosition(null).X;
        }

        public void Handle_MouseUp(object sender, MouseEventArgs args)
        {            
            var item = sender as StackPanel;
            _isMouseCaptured = false;
            if (item != null) item.ReleaseMouseCapture();
            _mouseVerticalPosition = -1;
            _mouseHorizontalPosition = -1;

            if (editPanel.Visibility != Visibility.Collapsed) return;

            editPanel.Visibility = Visibility.Visible;
            var left = (double) flagPanel.GetValue(Canvas.LeftProperty);
            flagPanel.SetValue(Canvas.LeftProperty, left - flagPanel.ActualWidth);
        }

        #endregion

        private void BtnDeleteMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reset();
            _ignoreClick = true;
        }

        private void Reset()
        {
            txtComment.Text = "";
            editPanel.Visibility = Visibility.Collapsed;
            flagPanel.Visibility = Visibility.Collapsed;
            imgFlagNoShadow.Visibility = Visibility.Visible;
            flagPanel.SetValue(Canvas.LeftProperty, 0d);
            flagPanel.SetValue(Canvas.TopProperty, 0d);
        }

        private void BtnOkMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Current.Instance.Username == null || Current.Instance.Password == null || Current.Instance.Earthwatcher == null)
            {
                var ws = new WarningScreen(Labels.Flag1);
                ws.Show();

                _ignoreClick = true;
                Reset();
                return;
            }
            
            var comment = txtComment.Text;
            var eId = Current.Instance.Earthwatcher.Id;

            //Get lat lon for flag
            var generalTransform = imgFlagShadow.TransformToVisual(Current.Instance.MapControl);
            var childToParentCoordinates = generalTransform.Transform(new Point(15, imgFlagShadow.ActualHeight));
            var spherical = Current.Instance.MapControl.Viewport.ScreenToWorld(childToParentCoordinates.X, childToParentCoordinates.Y);
            var lonLat = SphericalMercator.ToLonLat(spherical.X, spherical.Y);

            //Post
            var flagPost = new FlagRequests(Constants.BaseApiUrl);
            flagPost.FlagAdded += OnFlagAdded;  
            var flag = new Earthwatchers.Models.Flag { Comment = comment, EarthwatcherId = eId, Latitude = lonLat.y, Longitude = lonLat.x };
            flagPost.Post(flag, Current.Instance.Username, Current.Instance.Password);

            _ignoreClick = true;
            Reset();
        }

        private static void OnFlagAdded(object sender, System.EventArgs e)
        {
            var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            if (flagLayer != null) flagLayer.RequestFlags();
        }
    }
}
