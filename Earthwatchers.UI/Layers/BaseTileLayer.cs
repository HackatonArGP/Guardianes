using Mapsui.Layers;
using BruTile;

namespace Earthwatchers.UI.Layers
{
    public class BaseTileLayer : TileLayer
    {
        public string CopyrightText { get; set; }
        public string CopyrightImage { get; set; }
        public string TumbnailPath { get; set; }

        public BaseTileLayer(ITileSource tileSource) : base(tileSource){ }
    }
}
