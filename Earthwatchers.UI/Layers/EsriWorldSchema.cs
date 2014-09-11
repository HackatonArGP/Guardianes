using System.Globalization;
using BruTile;

namespace Earthwatchers.UI.Layers
{
    public static class EsriWorldSchema
    {
        public static ITileSchema GetSchema()
        {
            var resolutions = new[]
                {
                    156543.033928,
                    78271.5169639999,
                    39135.7584820001,
                    19567.8792409999,
                    9783.93962049996,
                    4891.96981024998,
                    2445.98490512499,
                    1222.99245256249,
                    611.49622628138,
                    305.748113140558,
                    152.874056570411,
                    76.4370282850732,
                    38.2185141425366,
                    19.1092570712683,
                    9.55462853563415,
                    4.77731426794937,
                    2.38865713397468,
                    1.19432856685505,
                    0.597164283559817,
                    0.298582141647617
                };

            var schema = new TileSchema();

            var counter = 0;
            foreach (var resolution in resolutions)
            {
                schema.Resolutions.Add(new Resolution
                    {
                        UnitsPerPixel = resolution, 
                        Id = counter++.ToString(CultureInfo.InvariantCulture)
                    });
            }

            schema.Height = 256;
            schema.Width = 256;
            schema.Extent = new Extent(-20037507.2295943, -19971868.8804086, 20037507.2295943, 19971868.8804086);
            schema.OriginX = -20037508.342787;
            schema.OriginY = 20037508.342787;
            schema.Name = "ESRI";
            schema.Format = "JPEG";
            schema.Axis = AxisDirection.InvertedY;
            schema.Srs = string.Format("EPSG:{0}", 102100);

            return schema;
        }
    }
}
