using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using BruTile.PreDefined;
using BruTile.Web;
using EarthWatchers.SL.Requests;
using Earthwatchers.Models;
using Mapsui.Layers;
using Mapsui.Utilities;

namespace EarthWatchers.SL
{
    public class PermalinkHelper
    {
        private static ImageRequests imageRequest;
        private static string[] _layerlist;
 
        public static void GetPermalinkromUrl()
        {
            GetPermalinkBbox();
            GetPermalinkLayers();
        }

        private static void GetPermalinkLayers()
        {
            string layers;

            try { layers = HtmlPage.Document.QueryString["layers"]; }
            catch (KeyNotFoundException) { return; }

            if (!layers.Equals(""))
            {
                _layerlist = layers.Split(',');
                if (_layerlist.Length == 0) return;

                imageRequest = new ImageRequests(Constants.BaseApiUrl);
                imageRequest.ImageRequestReceived += imageRequest_ImageRequestReceived;
                imageRequest.GetAllImagery();
            }
        }

        static void imageRequest_ImageRequestReceived(object sender, EventArgs e)
        {
            var _satelliteImages = (List<SatelliteImage>)sender;
            foreach (var _satelliteLayer in _satelliteImages)
            {
                var shouldAdd = false;
                foreach (var layer in _layerlist)
                {
                    if (layer.Equals(_satelliteLayer.Name))
                        shouldAdd = true;
                }

                if (!shouldAdd) continue;

                var topLeft = SphericalMercator.FromLonLat(_satelliteLayer.Extent.MinX, _satelliteLayer.Extent.MinY);
                var bottomRight = SphericalMercator.FromLonLat(_satelliteLayer.Extent.MaxX, _satelliteLayer.Extent.MaxY);
                var newExtend = new BruTile.Extent(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

                var schema = new SphericalMercatorWorldSchema {Extent = newExtend};

                var max = _satelliteLayer.MaxLevel + 1;
                while (schema.Resolutions.Count > max)
                {
                    schema.Resolutions.RemoveAt(max);
                }
                var tms = new TmsTileSource(_satelliteLayer.UrlTileCache, schema);
                Current.Instance.LayerHelper.AddLayer(new TileLayer(tms) { LayerName = _satelliteLayer.Name });
            }
        }

        private static void GetPermalinkBbox()
        {
            string bbox;
            try { bbox = HtmlPage.Document.QueryString["bbox"]; }
            catch (KeyNotFoundException) { return; }

            if (!bbox.Equals(""))
            {
                var coordinatesText = bbox.Split(',');
                var coordinates = new List<double>();

                foreach (var coord in coordinatesText)
                {
                    try { coordinates.Add(double.Parse(coord, CultureInfo.InvariantCulture)); }
                    catch (Exception) { }
                }

                if (coordinates.Count != 4) return;

                //var t1 = SphericalMercator.ToLonLat(coordinates[0], coordinates[1]);
                //var t2 = SphericalMercator.ToLonLat(coordinates[2], coordinates[3]);
                //MessageBox.Show(string.Format("x: {0} y: {1} x2: {2} y2: {3}", t1.x, t1.y, t2.x, t2.y));

                var mapcontrol = Current.Instance.MapControl;
                mapcontrol.ZoomToBox(new Mapsui.Geometries.Point(coordinates[0], coordinates[1]), new Mapsui.Geometries.Point(coordinates[2], coordinates[3]));
                mapcontrol.ZoomIn();
                mapcontrol.ZoomOut();
                Current.Instance.MapControl.OnViewChanged(true);

                //double x, y, resolution;
                //var width = Math.Abs(endPoint.X - beginPoint.X);
                //var height = Math.Abs(endPoint.Y - beginPoint.Y);
                //if (width <= 0) return;
                //if (height <= 0) return;

                //ZoomHelper.ZoomToBoudingbox(beginPoint.X, beginPoint.Y, endPoint.X, endPoint.Y, ActualWidth, out x, out y, out resolution);
                //resolution = ZoomHelper.ClipToExtremes(map.Resolutions, resolution);

                //view.Center = new Mapsui.Geometries.Point(x, y);
                //view.Resolution = resolution;

                //toResolution = resolution;

                //OnViewChanged(true, true);
                //Redraw();
                //ClearBBoxDrawing();
            }
        }
    }
}
