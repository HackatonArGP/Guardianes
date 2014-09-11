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
    public class JaguarLayer : Layer
    {
        private readonly MemoryProvider _source;

        public JaguarLayer(string name)
            : base(name)
        {
            _source = new MemoryProvider();
            DataSource = _source;

            Current.Instance.MapControl.zoomStarted += MapControlChanged;
            Current.Instance.MapControl.zoomFinished += MapControlChanged;
            Current.Instance.MapControl.panFinished += MapControlChanged;
        }

        void MapControlChanged(object sender, EventArgs e)
        {
            UpdateJaguarInMap();
        }

        public void NotifyJaguarReceived()
        {
            UpdateJaguarInMap();
        }

        public void ClearJaguar()
        {
            _source.Features.Clear();
        }

        private void DrawJaguar()
        {
            var jaguarGame = Current.Instance.JaguarGame;
            if (jaguarGame != null)
            {
                ClearJaguar();
                var sphericalMid = SphericalMercator.FromLonLat(jaguarGame.Longitude, jaguarGame.Latitude);
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(sphericalMid.x, sphericalMid.y)
                };
                var symbolStyle = new SymbolStyle { Symbol = GetSymbol("jaguar.png"), SymbolType = SymbolType.Rectangle };
                feature.Styles.Add(symbolStyle);
                _source.Features.Add(feature);

                Current.Instance.MapControl.OnViewChanged(true);
            }
        }

        private void UpdateJaguarInMap()
        {
            if (Current.Instance.MapControl.Viewport.Resolution <= 2.4)
            {
                DrawJaguar();
            }
            else
            {
                ClearJaguar();
            }
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


    }
}
