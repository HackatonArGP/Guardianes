using System;
using System.Windows;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;
using Earthwatchers.Models;
using Mapsui.Styles;
using System.Globalization;
using Earthwatchers.UI.Requests;

namespace Earthwatchers.UI.Layers
{
    public class AlertedLandLayer : Layer
    {
        private readonly MemoryProvider source;
        private readonly LandRequests    landRequest;

        public AlertedLandLayer(string name)
            : base(name)
        {
            source = new MemoryProvider();
            DataSource = source;
            
            landRequest = new LandRequests(Constants.BaseApiUrl);
            landRequest.LandByStatusReceived += LandRequestLandByStatusReceived;
            RequestAlertedAreas();
        }

        /// <summary>
        /// Get all land with status Alert and show a marker in the middle
        /// </summary>
        public void RequestAlertedAreas()
        {
            landRequest.GetLandByStatus(LandStatus.Alert);
        }

        private void LandRequestLandByStatusReceived(object sender, EventArgs e)
        {
            var land = sender as List<Land>;
            if(land != null)
                DrawMarkers(land);
        }

        private void DrawMarkers(IEnumerable<Land> landPieces)
        {
            if (landPieces == null) return;

            ClearGraphics();

            foreach (var land in landPieces)
            {
                var sphericalMid = SphericalMercator.FromLonLat(land.Longitude, land.Latitude);
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(sphericalMid.x, sphericalMid.y)
                };
                var symbolStyle = new SymbolStyle { Symbol = GetSymbol("warning.png"), SymbolType = SymbolType.Rectangle};
                feature.Styles.Add(symbolStyle);

                source.Features.Add(feature);
            }

            Current.Instance.MapControl.OnViewChanged(true);
        }

        private static Bitmap GetSymbol(string bitmapname)
        {
            Bitmap bitmap = null;
            try
            {
                var filename = string.Format(CultureInfo.InvariantCulture, "Earthwatchers.UI;component/Resources/Images/{0}", bitmapname);
                var resourceInfo = Application.GetResourceStream(new Uri(filename, UriKind.Relative));
                bitmap = new Bitmap { Data = resourceInfo.Stream };
            }
            catch
            {
                //TODO: error handling
            }

            return bitmap;
        }

        private void ClearGraphics()
        {
            source.Features.Clear();
        }
    }
}
