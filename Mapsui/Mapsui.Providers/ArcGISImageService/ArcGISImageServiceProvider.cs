﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Mapsui.Geometries;
using Mapsui.Rendering;

namespace Mapsui.Providers.ArcGISImageService
{
    public class ArcGISImageServiceProvider : IProvider
    {
        private int srid = -1;
        private readonly ArcGISImageServiceInfo info;
        
        public ArcGISImageServiceProvider(ArcGISImageServiceInfo info, bool continueOnError = false)
        {
            this.info = info;
            ContinueOnError = continueOnError;
        }

        public string ConnectionId
        {
            get { return ""; }
        }

        public bool IsOpen
        {
            get { return true; }
        }

        public int SRID
        {
            get
            {
                return srid;
            }
            set
            {
                srid = value;
            }
        }

        public System.Collections.Generic.IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var features = new Features();
            IRaster raster = null;
            var view = new Viewport { Resolution = resolution, Center = box.GetCentroid(), Width = (box.Width / resolution), Height = (box.Height / resolution) };
            if (TryGetMap(view, ref raster))
            {
                IFeature feature = features.New();
                feature.Geometry = raster;
                features.Add(feature);
            }
            return features;
        }

        public bool TryGetMap(IViewport viewport, ref IRaster raster)
        {
            int width;
            int height;

            try
            {
                width = Convert.ToInt32(viewport.Width);
                height = Convert.ToInt32(viewport.Height);
            }
            catch (OverflowException)
            {
                Trace.Write("Could not convert double to int (ExportMap size)");
                return false;
            }

            var uri = new Uri(GetRequestUrl(viewport.Extent, width, height));
            WebRequest webRequest = WebRequest.Create(uri);

            try
            {
                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                using (var dataStream = webResponse.GetResponseStream())
                {
                    if (!webResponse.ContentType.StartsWith("image")) return false;

                    byte[] bytes = BruTile.Utilities.ReadFully(dataStream);
                    raster = new Raster(new MemoryStream(bytes), viewport.Extent);
                }
                return true;
            }
            catch (WebException webEx)
            {
                if (!ContinueOnError)
                    throw (new RenderException(
                        "There was a problem connecting to the WMS server",
                        webEx));
                Trace.Write("There was a problem connecting to the WMS server: " + webEx.Message);
            }
            catch (Exception ex)
            {
                if (!ContinueOnError)
                    throw (new RenderException("There was a problem while attempting to request the WMS", ex));
                Trace.Write("There was a problem while attempting to request the WMS" + ex.Message);
            }
            return false;
        }
        
        private string GetRequestUrl(BoundingBox boundingBox, int width, int height)
        {
            var url = new StringBuilder(info.Url);

            if (!info.Url.Contains("?")) url.Append("?");
            if (!url.ToString().EndsWith("&") && !url.ToString().EndsWith("?")) url.Append("&");

            url.AppendFormat(CultureInfo.InvariantCulture, "bbox={0},{1},{2},{3}",
                boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.X, boundingBox.Max.Y);
            url.AppendFormat("&size={0},{1}", width, height);
            url.AppendFormat("&interpolation=RSP_{0}", info.Interpolation.ToString());
            url.AppendFormat("&format={0}", info.Format);
            url.AppendFormat("&f={0}", info.F);
            url.AppendFormat("&imageSR={0}", info.ImageSR);
            url.AppendFormat("&bboxSR={0}", info.BBoxSR);
            url.AppendFormat("&time={0}", info.Time);

            return url.ToString();
        }

        public BoundingBox GetExtents()
        {
            return null;
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        public bool ContinueOnError { get; set; }
    }
}
