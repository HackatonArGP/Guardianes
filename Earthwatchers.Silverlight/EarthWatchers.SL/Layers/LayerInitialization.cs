using System;
using System.Collections.Generic;
using BruTile;
using BruTile.PreDefined;
using BruTile.Web;
using Mapsui.Layers;

namespace EarthWatchers.SL.Layers
{
    public class LayerInitialization
    {
        public static void Initialize(LayerHelper layerHelper)
        {
            //LoadArcGisImageserver(layerHelper);
            LoadEsriImagery(layerHelper);
            LoadOsmMapnik(layerHelper);
            LoadBingHybrid(layerHelper);
            LoadBingRoad(layerHelper);
            LoadHexagon(layerHelper);
            //LoadAlertedLand(layerHelper);
            //LoadFlags(layerHelper);
        }

        public static List<EducationalLayer> GetEducationalLayers()
        {
            
            var educationalLayers = new List<EducationalLayer>()
            {
                new EducationalLayer("Roads", "http://t1.edugis.nl/tiles/1.0.0/dfa_roads/", "http://deforestaction.codeplex.com/"),
                new EducationalLayer("Rivers", "http://t1.edugis.nl/tiles/1.0.0/dfa_rivers/", "http://deforestaction.codeplex.com/"),
                new EducationalLayer("Threat level", "http://t1.edugis.nl/tiles/1.0.0/dfa_threatlevel/", "http://deforestaction.codeplex.com/"),
                new EducationalLayer("Oil Palm and Rubber", "http://t1.edugis.nl/tiles/1.0.0/dfa_palmrubber/", "http://deforestaction.codeplex.com/"),
                new EducationalLayer("Forest and Peat", "http://t1.edugis.nl/tiles/1.0.0/dfa_forestandpeat/", "http://deforestaction.codeplex.com/"),
                new EducationalLayer("Sintang Lestari", "http://research.geodan.nl/tilecache/tilecache.py/1.0.0/htr_kphp_900913/", "http://deforestaction.codeplex.com/"),  
               
            };

            return educationalLayers;
        }

        private static void LoadOsmMapnik(LayerHelper layerHelper)
        {
            var osmLayer = new BaseTileLayer(new OsmTileSource())
            {
                LayerName = "OSM",
                TumbnailPath = "Resources/Images/osm.png",
                CopyrightText = "CC-BY-SA Open Street Map and Contributors",
                Enabled = false
            };

            layerHelper.AddBaseLayer(osmLayer);
        }

        private static void LoadBingHybrid(LayerHelper layerHelper)
        {
            var bingHybrid = new BaseTileLayer(new BingTileSource(new BingRequest("http://h3.ortho.tiles.virtualearth.net/tiles", null, BingMapType.Hybrid)))
            {
                LayerName = "Bing Hybrid",
                TumbnailPath = "Resources/Images/bingHybrid.jpg",
                CopyrightImage = "Resources/Images/bing.png",
                Enabled = false
            };

            layerHelper.AddBaseLayer(bingHybrid);
        }

        private static void LoadBingRoad(LayerHelper layerHelper)
        {
            var bingRoad = new BaseTileLayer(new BingTileSource(new BingRequest("http://h3.ortho.tiles.virtualearth.net/tiles", null, BingMapType.Roads)))
            {
                LayerName = "Bing Road",
                TumbnailPath = "Resources/Images/bingStreet.jpg",
                CopyrightImage = "Resources/Images/bing.png",
                Enabled = false
            };

            layerHelper.AddBaseLayer(bingRoad);
        }

        private static void LoadArcGisImageserver(LayerHelper layerHelper)
        {
           // layerHelper.AddLayer(ArcGISImageServiceSample.Create2());
           // layerHelper.AddLayer(ArcGISImageServiceSample.Create());
           
        }

        private static void LoadEsriImagery(LayerHelper layerHelper)
        {
            var tileSource = new ArcGisTileSource("http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer", EsriWorldSchema.GetSchema());
            var esri = new BaseTileLayer(tileSource)
            {
                LayerName = "World Imagery",
                TumbnailPath = "Resources/Images/world_imagery.png",
                CopyrightImage = "Resources/Images/esriLogo.png",
                Enabled = true
            };

            layerHelper.AddBaseLayer(esri);
        }

        private static void LoadHexagon(LayerHelper layerHelper)
        {
            var layer = new HexagonLayer(Constants.Hexagonlayername) {Enabled = true};
            layerHelper.AddLayer(layer);
        }

        private static void LoadAlertedLand(LayerHelper layerHelper)
        {
            var layer = new AlertedLandLayer(Constants.AlertedLandLayername) { Enabled = true, LayerName = Constants.AlertedLandLayername };
            layerHelper.AddLayer(layer);            
        }

        private static void LoadFlags(LayerHelper layerHelper)
        {
            var layer = new FlagLayer(Constants.flagLayerName) { Enabled = true};
            layerHelper.AddLayer(layer);
        }
    }
}
