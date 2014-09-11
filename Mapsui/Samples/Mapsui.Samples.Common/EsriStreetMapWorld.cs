﻿using BruTile;
using BruTile.Web;

namespace Mapsui.Samples.Common
{
    public class EsriStreetMapWorld : ITileSource
    {
        public EsriStreetMapWorld()
        {
            Provider = CreateProvider();
            Schema = CreateSchema();
        }

        public ITileProvider Provider { get; private set; }
        public ITileSchema Schema { get; private set; }

        private static ITileProvider CreateProvider()
        {
            return new WebTileProvider(RequestBuilder);
        }

        private static ITileSchema CreateSchema()
        {
            var resoltions = new[] { 
                    0.3515625,
                    0.17578125,
                    0.087890625,
                    0.0439453125,
                    0.02197265625,
                    0.010986328125,
                    0.0054931640625,
                    0.00274658203125,
                    0.001373291015625,
                    0.0006866455078125,
                    0.00034332275390625,
                    0.000171661376953125,
                    0.0000858306884765629,
                    0.0000429153442382814,
                    0.0000214576721191407,
                    0.0000107288360595703 };

            const string format = "jpeg";

            var schema = new TileSchema();
            var count = 0;
            foreach (double resolution in resoltions)
            {
                schema.Resolutions.Add(new Resolution { Id = count.ToString(), UnitsPerPixel = resolution});
                count++;
            }
            schema.Height = 512;
            schema.Width = 512;
            schema.Extent = new Extent(-180, -90, 180, 90);
            schema.OriginX = -180;
            schema.OriginY = 90;
            schema.Name = "ESRI";
            schema.Format = format;
            schema.Axis = AxisDirection.InvertedY;
            schema.Srs = "EPSG:4326";
            return schema;
        }

        private static IRequest RequestBuilder
        {
            get
            {
                const string url = "http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/{0}/{2}/{1}";
                return new BasicRequest(url);
            }
        }
    }
}
