using Earthwatchers.Models;
using EarthWatchers.SL.GUI.Controls;
using EarthWatchers.SL.Layers;
using Mapsui.Windows;

namespace EarthWatchers.SL
{
    public class MapHelper
    {
        public static void ZoomToHexagon(MapControl mapcontrol, string geohex)
        {
            var zone = GeoHex.Decode(geohex);

            if (zone == null)
            {
                var ws = new WarningScreen("Sorry this land is not available");
                ws.Show();
                return;
            }

            var coordinates = zone.getHexCoords();
            var sphericalTopLeft = SphericalMercator.FromLonLat(coordinates[0].Longitude, coordinates[1].Latitude);
            var sphericalBottomRight = SphericalMercator.FromLonLat(coordinates[3].Longitude, coordinates[4].Latitude);
            const int marge = 8000;

            mapcontrol.ZoomToBox(new Mapsui.Geometries.Point(sphericalTopLeft.x - marge, sphericalTopLeft.y - marge), new Mapsui.Geometries.Point(sphericalBottomRight.x + marge, sphericalBottomRight.y + marge));
            
            //TODO: Blergh awfull dirty dirty hack to show hexagon after zoomToHexagon (problem = Extend is a center point after ZoomToBox)
            mapcontrol.ZoomIn();
            mapcontrol.ZoomOut();

            var hexLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);
            hexLayer.UpdateHexagonsInView();
            Current.Instance.MapControl.OnViewChanged(true);
        }
    }
}
