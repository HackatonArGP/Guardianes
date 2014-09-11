using System;
using BruTile;
using BruTile.Web;
using BruTile.PreDefined;

namespace EarthWatchers.SL.Layers
{
    public class DfaTileSource : ITileSource
    {
        public ITileSchema Schema { get; private set; }
        public ITileProvider Provider { get; private set; }

        public DfaTileSource(string url)
        {
            Schema = new SphericalMercatorWorldSchema();
            //Schema.Resolutions.
            Provider = new WebTileProvider(new TmsRequest(new Uri(url), "png"));
        }
    }
}
