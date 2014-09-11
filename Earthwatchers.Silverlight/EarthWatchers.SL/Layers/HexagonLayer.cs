using System.Linq;
using EarthWatchers.SL.Extensions;
using Earthwatchers.Models;
using EarthWatchers.SL.Requests;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Geometries;
using System.Collections.Generic;
using Mapsui.Styles;
using System;
using System.Globalization;

namespace EarthWatchers.SL.Layers
{
    public class HexagonLayer : Layer
    {
        private readonly MemoryProvider source;
        private double opacity = 0.5;
        private readonly LandRequests landRequest = new LandRequests(Constants.BaseApiUrl);

        public HexagonLayer(string name) 
            : base (name)
        {
            source = new MemoryProvider();
            DataSource = source;
            landRequest.LandInViewReceived += LandRequestLandInViewReceived;
            Current.Instance.MapControl.zoomFinished += MapControlViewChanged;
            Current.Instance.MapControl.panFinished += MapControlViewChanged;
        }        

        public new double Opacity
        {
            set 
            {  
                opacity = value;
                ChanceFeatureOpacity();
            }
            get { return opacity; }
        }

        private void ChanceFeatureOpacity()
        {
            foreach (Feature feature in source.Features)
            {
                foreach (var style in feature.Styles)
                {
                    if (style is VectorStyle) (style as VectorStyle).SetOpacity(opacity);                    
                }
            }
        }

        public void AddHexagon(Zone zone, LandStatus landStatus, bool isOwnLand)
        {
            var sphericalCoordinates = ConvertHexCoordinates(zone.getHexCoords());
            var polygon = new Polygon { ExteriorRing = new LinearRing(sphericalCoordinates) };
            var feature = new Feature {Geometry = polygon};
            feature["hexcode"] = zone.code;

            feature.Styles.Add(GetVectorStyle(landStatus, isOwnLand));
            
            source.Features.Add(feature);
        }

        public void ChangeHexagon(string hexcode, LandStatus landStatus)
        {
            var feature = GetFeatureByHex(hexcode);
            feature.Styles.Add(GetVectorStyle(landStatus, false));
        }

        public Feature GetFeatureByHex(string hexcode)
        {
            if (source == null)
                return null;

            foreach(Feature feature in source.Features)
            {
                if (feature["hexcode"] == null)
                    continue;

                if (feature["hexcode"].Equals(hexcode))
                    return feature;
            }

            return null;
        }

        public void UpdateHexagonsInView()
        {
            if (Current.Instance.MapControl.Viewport.Resolution > 50)
            {
                ClearGraphics();
                return;
            }

            if (!Enabled)
                return;

            var topLeft = Current.Instance.MapControl.Viewport.Extent.TopLeft;
            var bottomLeft = Current.Instance.MapControl.Viewport.Extent.BottomLeft;
            var bottomRight = Current.Instance.MapControl.Viewport.Extent.BottomRight;
            var topRight = Current.Instance.MapControl.Viewport.Extent.TopRight;

            var llTopLeft = SphericalMercator.ToLonLat(topLeft.X, topLeft.Y);
            var llbottomLeft = SphericalMercator.ToLonLat(bottomLeft.X, bottomLeft.Y);
            var llbottonRight = SphericalMercator.ToLonLat(bottomRight.X, bottomRight.Y);
            var llTopRight = SphericalMercator.ToLonLat(topRight.X, topRight.Y);

            var wkt = String.Format(CultureInfo.InvariantCulture, "POLYGON(({0} {1},{2} {3},{4} {5},{6} {7},{0} {1}))", llTopLeft.x, llTopLeft.y, llbottomLeft.x, llbottomLeft.y, llbottonRight.x, llbottonRight.y, llTopRight.x, llTopRight.y);

            landRequest.GetLandByWkt(wkt);
        }

        private void MapControlViewChanged(object sender, EventArgs e)
        {
            UpdateHexagonsInView();
        }

        private void LandRequestLandInViewReceived(object sender, EventArgs e)
        {
            var landPieces = sender as List<Land>;
            Current.Instance.LandInView = landPieces;

            ClearGraphics();

            if (landPieces == null || Current.Instance.MapControl.Viewport.Resolution > 50)
                return;
            
            foreach (var landPiece in landPieces)
            {
                var zone = GeoHex.Decode(landPiece.GeohexKey);

                var isOwn = Current.Instance.EarthwatcherLand != null && Current.Instance.EarthwatcherLand.GeohexKey.Equals(landPiece.GeohexKey);

                AddHexagon(zone, landPiece.LandStatus, isOwn);
            }

            Current.Instance.MapControl.OnViewChanged(true);
        }

        public void ClearGraphics()
        {
            source.Features.Clear();
        }

        //Convert lon lat coordinates into spherical for use on map
        private static List<Point> ConvertHexCoordinates(IList<Location> locations)
        {
            if (locations == null || locations.Count == 0)
                return null;

            var newLocations = locations.Select(location => SphericalMercator.FromLonLat(location.Longitude, location.Latitude)).Select(spherical => new Point(spherical.x, spherical.y)).ToList();

            var sphericalClosing = SphericalMercator.FromLonLat(locations[0].Longitude, locations[0].Latitude);
            newLocations.Add(new Point(sphericalClosing.x, sphericalClosing.y));

            return newLocations;
        }

        private VectorStyle GetVectorStyle(LandStatus status, bool isOwnLand)
        {
            Brush fill;
            Pen outline;

            switch (status)
            {
                case LandStatus.NotChecked:
                    fill = new Brush { Color = Color.White };
                    outline = new Pen { Color = Color.Black, Width = 2 };
                    break;
                case LandStatus.Ok:
                    fill = new Brush { Color = Color.Green };
                    outline = new Pen { Color = Color.Black, Width = 2 };
                    break;
                case LandStatus.Alert:
                    fill = new Brush { Color = Color.Red };
                    outline = new Pen { Color = Color.Black, Width = 2 };
                    break;
                default:
                    fill = new Brush { Color = Color.Orange };
                    outline = new Pen { Color = Color.Black, Width = 2 };
                    break;
            }

            if (isOwnLand)
            {
                outline = new Pen { Color = Color.Orange, Width = 4 };
            }

            var vStyle = new VectorStyle
            {
                Fill = fill,
                Outline = outline,
            };
            vStyle.SetOpacity(opacity);
            return vStyle;
        }
    }
}
