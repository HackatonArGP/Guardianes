// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System.Windows;
using BruTile;
using Mapsui;

namespace Mapsui.Windows
{
    public static class MapTransformHelper
    {
        public static void Pan(Viewport view, Point currentMap, Point previousMap)
        {
            Geometries.Point current = view.ScreenToWorld(currentMap.X, currentMap.Y);
            Geometries.Point previous = view.ScreenToWorld(previousMap.X, previousMap.Y);
            double diffX = previous.X - current.X;
            double diffY = previous.Y - current.Y;
            view.Center = new Geometries.Point(view.CenterX + diffX, view.CenterY + diffY);
        }

        public static Rect WorldToMap(Extent extent, IViewport view)
        {
            Geometries.Point min = view.WorldToScreen(extent.MinX, extent.MinY);
            Geometries.Point max = view.WorldToScreen(extent.MaxX, extent.MaxY);
            return new Rect(min.X, max.Y, max.X - min.X, min.Y - max.Y);
        }

        public static Geometries.BoundingBox MapToWorld(Geometries.BoundingBox box, IViewport view)
        {
            Geometries.Point lowerLeft = view.ScreenToWorld(box.BottomLeft);
            Geometries.Point upperRight = view.ScreenToWorld(box.TopRight);
            return new Geometries.BoundingBox(lowerLeft, upperRight);
        }
    }
}
