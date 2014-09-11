using System;
using System.Windows;
using System.Windows.Input;
using EarthWatchers.SL.Layers;
using EarthWatchers.SL.Requests;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace EarthWatchers.SL.GUI.Controls
{
    public partial class FlagInfoWindow
    {
        private string _flagId;
        public double longt { get; set; }
        public double lat { get; set; }
        public FlagInfoWindow(string username, string flagId, string text, double longitude, double latitude)
        {           
            InitializeComponent();
            _flagId = flagId;
            txtEarthwacther.Text = username;
            txtComment.Text = text;
            KeyDown += OnKeyDown;
            SetLocation(longitude, latitude);
            longt = longitude;
             lat = latitude;
            if (Current.Instance.Earthwatcher != null && username.Equals(Current.Instance.Earthwatcher.Name))
                btnDelete.Visibility = Visibility.Visible;
        }

        private void SetLocation(double lon, double lat)
        {
            txtLonLat.Text = string.Format("lon: {0} lat: {1}", Math.Round(lon, 5), Math.Round(lat, 5));
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void OpenScreen()
        {
            Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            if (flagLayer != null) flagLayer.DeleteFlag(_flagId);

            Close();
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (Math.Round(longt,5) == 111.70869 && Math.Round(lat,5) == 0.11119)
            {
                image1.Visibility = Visibility.Visible;
            }
            
      else if (Math.Round(longt,5) == 111.72023 && Math.Round(lat,5) == 0.10806)
           {
          image1.Visibility = Visibility.Collapsed;
             image2.Visibility = Visibility.Visible;
         }
            else if (Math.Round(longt, 5) == 111.66165 && Math.Round(lat, 5) == 0.13233)
            {
                image1.Visibility = Visibility.Collapsed;
                image2.Visibility = Visibility.Collapsed;
                image3.Visibility = Visibility.Visible;
            }
            else
           {

               MessageBox.Show("No photos!");
           }
            
        }
    }
}
