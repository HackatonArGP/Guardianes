using System.Linq;
using Earthwatchers.UI.Extensions;
using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Geometries;
using System.Collections.Generic;
using Mapsui.Styles;
using System;
using System.Globalization;

namespace Earthwatchers.UI.Layers
{
    public class HexagonLayer : Layer
    {
        private readonly MemoryProvider source;
        private double opacity = 0.6;
        private readonly LandRequests landRequest = new LandRequests(Constants.BaseApiUrl);

        public HexagonLayer(string name)
            : base(name)
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
                        (style as VectorStyle).SetOpacity(opacity);
                }
            }
        }

        public void AddHexagon(Land land)
        {
            var zone = GeoHex.Decode(land.GeohexKey);

            var isOwn = Current.Instance.Earthwatcher.Lands.Any(x => x.GeohexKey == land.GeohexKey);
            var sphericalCoordinates = ConvertHexCoordinates(zone.getHexCoords());
            var polygon = new Polygon { ExteriorRing = new LinearRing(sphericalCoordinates) };
            var feature = new Feature { Geometry = polygon };
            feature["isGreenpeaceUser"] = land.EarthwatcherId.HasValue && land.EarthwatcherId.Value == Configuration.GreenpeaceId ? "True" : "False";
            feature["hexcode"] = zone.code;

            bool ischecked = false;
            if (land.OKs.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())) || land.Alerts.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
            {
                ischecked = true;
            }

            bool denouncedByMe = false;
            if(Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.DemandAuthorities.ToString()) && (x.LandId == land.Id)))
            {
                denouncedByMe = true;
            }

            bool islockedOnly = land.IsLocked == true && land.DemandAuthorities == false ? true : false;

            feature.Styles.Add(GetVectorStyle(land.LandStatus, isOwn, land.DemandAuthorities, ischecked, land.EarthwatcherId.HasValue && land.EarthwatcherId.Value == Configuration.GreenpeaceId ? true : false, islockedOnly, denouncedByMe));

            source.Features.Add(feature);
        }

        public Feature GetFeatureByHex(string hexcode)
        {
            if (source == null)
                return null;

            foreach (Feature feature in source.Features)
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
            /*
            if (Current.Instance.MapControl.Viewport.Resolution > 50)
            {
                ClearGraphics();
                return;
            }
             * */

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

            //Esto se ejecuta al principio o cuando haces una actualizacion
            landRequest.GetLandByWkt(wkt, Current.Instance.Earthwatcher.Lands.First().Id);
        }

        private void MapControlViewChanged(object sender, EventArgs e)
        {
            if (Configuration.HideHexagons == true)
            {
                if (Current.Instance.MapControl.Viewport.Resolution > 306)  //A este nivel de zoom, las parcelas dejan de mostrarse
                {                                                           // ocultar al siguiente nivel de zoom menor = 156
                    ClearGraphics();
                }
                else
                {
                    UpdateHexagonsFromMemory();
                }
            }
            else
            {
                UpdateHexagonsFromMemory();
            }
        }


        public void UpdateHexagonsFromMemory()
        {
            if (Current.Instance.Lands == null)
                return;


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

            var landPieces = Current.Instance.Lands.Where(x => x.Latitude >= llbottomLeft.y && x.Latitude <= llTopLeft.y && x.Longitude >= llTopLeft.x && x.Longitude <= llTopRight.x);
            UpdateLands(landPieces.ToList());
        }

        private void UpdateLands(List<Land> landPieces)
        {
            if (landPieces != null && landPieces.Count > 0)
            {
                Current.Instance.LandInView = landPieces;
                LandDemandLayer landDemandLayer = Current.Instance.LayerHelper.FindLayer(Constants.DemandAuthoritiesLayerName) as LandDemandLayer;
                if (landDemandLayer != null)
                {
                    landDemandLayer.DrawMarkers(landPieces);
                }

                ClearGraphics();

                foreach (var landPiece in landPieces)
                {
                    AddHexagon(landPiece);
                }

                Current.Instance.MapControl.OnViewChanged(true);
            }
        }

        private void LandRequestLandInViewReceived(object sender, EventArgs e)
        {
            var landPieces = sender as List<Land>;
            if (landPieces != null)
            {
                //Actualizo los hexágonos que están en memoria
                if (Current.Instance.Lands != null)
                {
                    foreach (Land land in landPieces)
                    {
                        var oldLand = Current.Instance.Lands.Where(x => x.Id == land.Id).FirstOrDefault();
                        if (oldLand != null)
                        {
                            Current.Instance.Lands.Remove(oldLand);
                        }
                        Current.Instance.Lands.Add(land);
                    }
                }

                UpdateLands(landPieces);
            }

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

        private VectorStyle GetVectorStyle(LandStatus status, bool isOwnLand, bool demandAuthorities, bool ischecked, bool isGreenpeaceUser, bool islockedOnly, bool denouncedByMe)
        {
            Brush fill = null;
            Pen outline = null;

            switch (status)
            {
                case LandStatus.NotChecked:  //STATUS 2 SIN CHEQUEAR
                    fill = new Brush { Color = Color.FromArgb(255, 255, 255, 255) };
                    outline = new Pen { Color = Color.Black, Width = 1 };
                    break;
                case LandStatus.Ok: //STATUS 3 SIN DESMONTES
                    fill = new Brush { Color = Color.FromArgb(143, 121, 136, 35) };
                    outline = new Pen { Color = Color.FromArgb(255, 169, 183, 41), Width = 4 };
                    break;
                case LandStatus.Alert:  //STATUS 4 CON DESMONTES
                     fill = new Brush { Color = Color.FromArgb(143, 255, 236, 0) };
                outline = new Pen { Color = Color.FromArgb(255, 255, 242, 0), Width = 4 };
                    
                    break;
                default: //DEFAULT, IDEM SIN CHEQUEAR
                    fill = new Brush { Color = Color.FromArgb(255, 255, 255, 255) };
                    outline = new Pen { Color = Color.Black, Width = 1 };
                    break;
            }

            if (ischecked && (status == LandStatus.Alert || status == LandStatus.Ok)) //VERIFICADA POR MI DESMONTADA O SIN DESMONTE
            {
                outline = new Pen { Color = Color.FromArgb(0, 0, 0, 0), Width = 1 };
            }

            if (isOwnLand && (status != LandStatus.Alert && status != LandStatus.Ok))  //MI PROPIA LAND
            {
                outline = new Pen { Color = Color.White, Width = 4 };
            }

            if (demandAuthorities)  //LISTA PARA DEMANDAR
            {
                 fill = new Brush { Color = Color.FromArgb(143, 167, 11, 10) };
                 outline = new Pen { Color = Color.FromArgb(255, 217, 7, 7), Width = 4 };

                if (denouncedByMe)
                {
                    outline = new Pen { Color = Color.FromArgb(0, 0, 0, 0), Width = 1 };

                }
            }

            if (!demandAuthorities && islockedOnly)
            {
                outline = new Pen { Color = Color.FromArgb(0, 0, 0, 0), Width = 1 };
            }

            var vStyle = new VectorStyle
            {
                Fill = fill,
                Outline = outline,
            };

            if (opacity == 0)
            {
                vStyle.SetOpacity(0);
            }
            else
            {
                if (Current.Instance.MapControl.Viewport.Resolution > 2.4 || Current.Instance.MapControl.Viewport.Resolution == 0)
                {
                        vStyle.SetOpacity(opacity);
                }
                else
                {
                    vStyle.SetOpacity(0);
                }
            } 

            return vStyle;
        }
    }
}
