using Mapsui.Layers;
using BruTile;
using BruTile.PreDefined;
using BruTile.Web;
using System;
using System.Collections.Generic;
using Earthwatchers.UI.Layers;
using SharpMap.Providers.ArcGISImageService;

namespace Earthwatchers.UI
{
    public static class ArcGISImageServiceSample
    {
        public static Layer Create()
        {
            var provider = CreateProvider();
            var layer = new Layer("AGOL Landsat 2010");
            layer.DataSource = provider;
            layer.Enabled = false;
            return layer;
        }

        public static Layer Create2()
        {
            var provider = CreateProvider2();
            var layer2 = new Layer("AGOL Landsat 1975");
            layer2.DataSource = provider;
            layer2.Enabled = false;
            return layer2;
        }
      
        private static ArcGISImageServiceProvider CreateProvider()
        {
            var info = new ArcGISImageServiceInfo();
            info.Url = "http://imagery.arcgisonline.com/ArcGIS/rest/services/LandsatGLS/FalseColor/ImageServer/exportImage";
            info.Format = "jpgpng";
            info.Interpolation = InterpolationType.NearestNeighbor;
            info.F = "image";
            info.ImageSR = "102100";
            info.BBoxSR = "102100";
            info.Time = "1262304000000,1262304000000";
            return new ArcGISImageServiceProvider(info, true);
        }

        private static ArcGISImageServiceProvider CreateProvider2()
        {
            var info = new ArcGISImageServiceInfo();
            info.Url = "http://imagery.arcgisonline.com/ArcGIS/rest/services/LandsatGLS/FalseColor/ImageServer/exportImage";
            info.Format = "jpgpng";
            info.Interpolation = InterpolationType.NearestNeighbor;
            info.F = "image";
            info.ImageSR = "102100";
            info.BBoxSR = "102100";
            info.Time = "157766400000,157766400000";
            return new ArcGISImageServiceProvider(info, true);
        }
    }
}
