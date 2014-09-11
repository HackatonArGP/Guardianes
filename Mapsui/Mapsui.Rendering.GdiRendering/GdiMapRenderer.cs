﻿// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Copyright 2010 - Paul den Dulk (Geodan) - Adapted SharpMap for Mapsui.
// 
// This file is part of Mapsui.
// Mapsui is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Mapsui is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Mapsui; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Point = Mapsui.Geometries.Point;

namespace Mapsui.Rendering.GdiRendering
{
    public class GdiMapRenderer
    {
        // TODO: derive from IRenderer
        public delegate bool AbortRenderDelegate();

        public static void Render(Graphics graphics, IViewport viewport, Map map, AbortRenderDelegate abortRender)
        {
            foreach (var layer in map.Layers)
            {
                if (layer.Enabled &&
                    layer.MinVisible <= viewport.Resolution &&
                    layer.MaxVisible >= viewport.Resolution)
                {
                    if (layer is LabelLayer)
                    {
                        //!!!GdiLabelRenderer.Render(graphics, view, layer as LabelLayer);
                    }
                    else if (layer is ITileLayer)
                    {
                        var tileLayer = (layer as ITileLayer);
                        GdiTileRenderer.Render(graphics, tileLayer.Schema, viewport, tileLayer.MemoryCache);
                    }
                    else
                    {
                        RenderLayer(graphics, viewport, layer, abortRender);
                    }
                }
                
                if (abortRender != null && abortRender()) return; 
            }
        }

        public static Image RenderMapAsImage(IViewport viewport, Map map)
        {
            if ((viewport.Width <= 0) || (viewport.Height <= 0)) throw new Exception("The view's width or heigh is 0");
            var image = new System.Drawing.Bitmap((int)viewport.Width, (int)viewport.Height);
            var graphics = Graphics.FromImage(image);
#if !PocketPC
            graphics.PageUnit = GraphicsUnit.Pixel;
#endif
            Render(graphics, viewport, map, null);
            return image;
        }

        public byte[] RenderMapAsByteArray(IViewport viewport, Map map)
        {
            Image image = RenderMapAsImage(viewport, map);
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Bmp);
            return memoryStream.ToArray();
        }

        private static void RenderLayer(Graphics graphics, IViewport viewport, ILayer layer, AbortRenderDelegate abortRender)
        {
            int counter = 0;
            const int step = 100;

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var layerStyle in layer.Styles)
            {
                var style = layerStyle;

                var features = layer.GetFeaturesInView(viewport.Extent, viewport.Resolution);

                //Linestring outlines is drawn by drawing the layer once with a thicker line
                //before drawing the "inline" on top.
                var enumerable = features as IList<IFeature> ?? features.ToList();
                foreach (var feature in enumerable)
                {
                    if ((counter++ % step == 0) && abortRender != null && abortRender()) return;
                    if (layerStyle is IThemeStyle) style = (layerStyle as IThemeStyle).GetStyle(feature);

                    if ((style is VectorStyle) && ((style as VectorStyle).Outline != null))
                    {
                        GdiGeometryRenderer.RenderGeometryOutline(graphics, viewport, feature.Geometry, style as VectorStyle);
                    }
                }

                foreach (var feature in enumerable)
                {
                    if ((counter++ % step == 0) && abortRender != null && abortRender()) return;
                    if (layerStyle is IThemeStyle) style = (layerStyle as IThemeStyle).GetStyle(feature);
                    RenderGeometry(graphics, viewport, feature.Geometry, style as VectorStyle);
                }
            }
        }

        private static void RenderGeometry(Graphics graphics, IViewport viewport, IGeometry feature, VectorStyle style)
        {
            if (feature is Point)
                GdiGeometryRenderer.DrawPoint(graphics, (Point)feature, style, viewport);
            else if (feature is MultiPoint)
                GdiGeometryRenderer.DrawMultiPoint(graphics, (MultiPoint) feature, style, viewport);
            else if (feature is LineString)
                GdiGeometryRenderer.DrawLineString(graphics, (LineString)feature, style.Line.Convert(), viewport);
            else if (feature is MultiLineString)
                GdiGeometryRenderer.DrawMultiLineString(graphics, (MultiLineString)feature, style.Line.Convert(), viewport);
            else if (feature is Polygon)
                GdiGeometryRenderer.DrawPolygon(graphics, (Polygon)feature, style.Fill.Convert(), style.Outline.Convert(), viewport);
            else if (feature is MultiPolygon)
                GdiGeometryRenderer.DrawMultiPolygon(graphics, (MultiPolygon)feature, style.Fill.Convert(), style.Outline.Convert(), viewport);
            else if (feature is IRaster)
                GdiGeometryRenderer.DrawRaster(graphics, feature as IRaster, viewport);
        }
    }
}