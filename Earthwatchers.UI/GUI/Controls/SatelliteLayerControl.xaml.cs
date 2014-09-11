using System;
using System.Windows;
using BruTile.PreDefined;
using BruTile.Web;
using Earthwatchers.Models;
using Mapsui.Layers;
using Extent = BruTile.Extent;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class SatelliteLayerControl
    {
        private bool isAdded;
        private readonly bool initialized;
        private readonly SatelliteImage satelliteLayer;

        public SatelliteLayerControl(SatelliteImage satelliteLayer, bool isAdded)
        {
            InitializeComponent();
            this.isAdded = isAdded;
            this.satelliteLayer = satelliteLayer;

            xhkAdd.IsChecked = this.isAdded;
            initialized = true;
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            txtName.Text = satelliteLayer.Name;
            txtProvider.Text = satelliteLayer.Provider;
            txtType.Text = satelliteLayer.ImageType.ToString();

            if (!string.IsNullOrEmpty(satelliteLayer.UrlMetadata))
                txtInfo.NavigateUri = new Uri(satelliteLayer.UrlMetadata);

            txtPublished.Text = satelliteLayer.DateName;

            if (satelliteLayer.UrlMetadata == null || satelliteLayer.UrlMetadata.Equals(""))
                txtInfo.Content = ""; 
        }

        public bool IsAdded 
        {
            get { return isAdded; }
            set
            {
                if (!initialized) return;
                
                isAdded = value;
                if (value)
                {
                   // var tms = new DfaTileSource("http://geodan.blob.core.windows.net/landsat/test/LE71200602008218EDC00");

                    var topLeft = SphericalMercator.FromLonLat(satelliteLayer.Extent.MinX, satelliteLayer.Extent.MinY);
                    var bottomRight = SphericalMercator.FromLonLat(satelliteLayer.Extent.MaxX, satelliteLayer.Extent.MaxY);
                    var newExtend = new Extent(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                    var schema = new SphericalMercatorWorldSchema();
                    schema.Extent = newExtend;

                    //TODO: Hack need to get this back from service
                    //int max = 12 + 1;
                    var max = satelliteLayer.MaxLevel + 1;
                    while (schema.Resolutions.Count > max)
                    {
                        schema.Resolutions.RemoveAt(max);
                    }
                    var tms = new TmsTileSource(satelliteLayer.UrlTileCache, schema);
                    Current.Instance.LayerHelper.AddLayer(new TileLayer(tms) { LayerName = satelliteLayer.DateName, Opacity = Current.Instance.TutorialStarted ? 0 : 1 }, false);
                    
                    //var newExtend = new Extent(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
                    //Current.Instance.LayerHelper.AddLayer(new TileLayer(tms) { LayerName = _satelliteLayer.Name, Schema = newExtend });
                }
                else
                {
                    Current.Instance.LayerHelper.RemoveLayer(satelliteLayer.DateName);
                }
            }
        }

        private void XhkAddChecked(object sender, RoutedEventArgs e)
        {
            IsAdded = true;
        }

        private void XhkAddUnchecked(object sender, RoutedEventArgs e)
        {
            IsAdded = false;
        }
    }
}
