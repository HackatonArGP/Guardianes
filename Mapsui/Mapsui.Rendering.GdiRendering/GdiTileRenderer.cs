﻿// Copyright 2008 - Paul den Dulk (Geodan)
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

using BruTile;
using BruTile.Cache;
using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Point = Mapsui.Geometries.Point;

namespace Mapsui.Rendering.GdiRendering
{
    public static class GdiTileRenderer
    {
        public static void Render(Graphics graphics, ITileSchema schema,
            IViewport viewport, MemoryCache<Feature> cache)
        {
            int level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, viewport.Resolution);
            DrawRecursive(graphics, schema, viewport, cache, schema.GetExtentOfTilesInView(viewport.Extent.ToExtent(), level), level);
        }

        private static void DrawRecursive(Graphics graphics, ITileSchema schema, IViewport viewport, 
            MemoryCache<Feature> cache, Extent extent, int level)
        {
            var tileInfos = schema.GetTilesInView(extent, level);

            foreach (TileInfo info in tileInfos)
            {
                var feature = cache.Find(info.Index);
                if (feature == null)
                {
                    if (level > 0) DrawRecursive(graphics, schema, viewport, cache, info.Extent.Intersect(extent), level - 1);
                }
                else
                {
                    var image = ((IRaster)feature.Geometry).Data;
                    RectangleF dest = WorldToView(info.Extent, viewport);
                    dest = RoundToPixel(dest);
                    RectangleF clip = WorldToView(extent, viewport);
                    clip = RoundToPixel(clip);

                    if (!Contains(clip, dest))
                    {
                        clip = Intersect(clip, dest);
                        if (clip.IsEmpty) continue;
                        DrawImage(graphics, new Bitmap(image), dest, clip);
                    }
                    else
                    {
                        //Not using a clip at all sometimes performs better than using screenwide clip.
                        DrawImage(graphics, new Bitmap(image), dest);
                    }
                }
            }
        }

        private static RectangleF RoundToPixel(RectangleF dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            dest = new RectangleF(
                (float)Math.Round(dest.Left),
                (float)Math.Round(dest.Top),
                (float)(Math.Round(dest.Right) - Math.Round(dest.Left)),
                (float)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
            return dest;
        }

        private static bool Contains(RectangleF clip, RectangleF dest)
        {
            return ((clip.Left <= dest.Left) && (clip.Top <= dest.Top) &&
              (clip.Right >= dest.Right) && (clip.Bottom >= dest.Bottom));
        }

        private static RectangleF Intersect(RectangleF clip, RectangleF dest)
        {
            float left = Math.Max(clip.Left, dest.Left);
            float top = Math.Max(clip.Top, dest.Top);
            float right = Math.Min(clip.Right, dest.Right);
            float bottom = Math.Min(clip.Bottom, dest.Bottom);
            return new RectangleF(left, top, right - left, bottom - top);
        }

        private static void DrawImage(Graphics graphics, Bitmap bitmap, RectangleF dest)
        {
            var rectDest = new Rectangle((int)dest.X, (int)dest.Y, (int)dest.Width, (int)dest.Height);
            var rectSrc = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(bitmap, rectDest, rectSrc.X, rectSrc.Y, rectSrc.Width, rectSrc.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        private static void DrawImage(Graphics graphics, Bitmap bitmap, RectangleF dest, RectangleF clip)
        {
            //todo: clipping like this is very inefficient. Find a faster way (use a smaller srcRect instead of a clip).
            graphics.Clip = new Region(new Rectangle((int)clip.X, (int)clip.Y, (int)clip.Width, (int)clip.Height));
            DrawImage(graphics, bitmap, dest);
            graphics.ResetClip();
        }

        private static RectangleF WorldToView(Extent extent, IViewport viewport)
        {
            Point min = viewport.WorldToScreen(new Point(extent.MinX, extent.MinY));
            Point max = viewport.WorldToScreen(new Point(extent.MaxX, extent.MaxY));
            return new RectangleF((float)min.X, (float)max.Y, 
                (float)(max.X - min.X), (float)(min.Y - max.Y));
        }

    }
}
