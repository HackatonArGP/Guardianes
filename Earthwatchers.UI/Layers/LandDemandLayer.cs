using System;
using System.Linq;
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
    public class LandDemandLayer : Layer
    {
        private readonly MemoryProvider source;

        public LandDemandLayer(string name)
            : base(name)
        {
            source = new MemoryProvider();
            DataSource = source;
        }

        public void DrawMarkers(IEnumerable<Land> landPieces)
        {
            if (landPieces == null) return;

            ClearGraphics();

            if (Current.Instance.MapControl.Viewport.Resolution > 50)
            {
                return;
            }

            foreach (var land in landPieces.Where(x => x.DemandAuthorities))
            {
                var sphericalMid = SphericalMercator.FromLonLat(land.Longitude, land.Latitude);
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(sphericalMid.x, sphericalMid.y),
                };
                
                var offset = new Offset();
                offset.X = 100;
                double scale = 19.109256744384766 / Current.Instance.MapControl.Viewport.Resolution;
                //var symbolStyle = new SymbolStyle { Symbol = GetSymbol("demandar.png"), SymbolType = SymbolType.Rectangle, SymbolOffset = offset, SymbolScale = scale };
                var symbolStyle = new SymbolStyle { Symbol = GetSymbol("demandar.png"), SymbolScale = 1 };
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
