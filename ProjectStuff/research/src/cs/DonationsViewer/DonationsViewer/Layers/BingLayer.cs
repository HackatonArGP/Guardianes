using System.Text;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

namespace Layers
{
    public class BingLayer : TiledMapServiceLayer
    {
        private const double cornerCoordinate = 20037508.3427892;
        private const int WKID = 102113;
        private string _url = "http://ecn.t2.tiles.virtualearth.net/tiles/";
        private string _generation = "700";
        private string _bingType = "h";

        public override void Initialize()
        {
            //Full extent fo the layer
            FullExtent = new Envelope(-cornerCoordinate, -cornerCoordinate, cornerCoordinate, cornerCoordinate)
            {
                SpatialReference = new SpatialReference(WKID)
            };
            SpatialReference = new SpatialReference(WKID);
            TileInfo = new TileInfo
            {
                Height = 256,
                Width = 256,
                Origin = new MapPoint(-cornerCoordinate, cornerCoordinate) { SpatialReference = new SpatialReference(WKID) },
                Lods = new Lod[19]
            };

            var resolution = cornerCoordinate * 2 / 256;
            for (var i = 0; i < TileInfo.Lods.Length; i++)
            {
                TileInfo.Lods[i] = new Lod { Resolution = resolution };
                resolution /= 2;
            }
            base.Initialize();
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string Generation
        {
            get { return _generation; }
            set { _generation = value; }
        }

        public string BingType
        {
            get { return _bingType; }
            set { _bingType = value; }
        }

        private static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            var quadKey = new StringBuilder();
            for (var i = levelOfDetail; i > 0; i--)
            {
                var digit = '0';
                var mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        public override string GetTileUrl(int level, int row, int col)
        {
            var QuadKey = TileXYToQuadKey(col, row, level);
            return string.Format("{0}{1}{2}?g={3}", _url, _bingType, QuadKey, _generation);
        }
    }
}
