// Copyright (c) 2011 Barend Gehrels, Amsterdam, the Netherlands

// License? Below is MIT

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


// Adapted from PNL


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows; // for Point

using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;

namespace DeforestAction
{
    public static class GeometryExtensions
    {
        public static SqlGeometry FromWkt(string text, int srid)
        {
            return SqlGeometry.STGeomFromText(new System.Data.SqlTypes.SqlChars(text), 3857);
        }

        // Converts a geometry (ring or linestring) to list of points
        public static List<Point> ToPointList(SqlGeometry sqlGeometry)
        {
            List<Point> points = new List<Point>();

            for (int i = 1; i <= sqlGeometry.STNumPoints(); i++)
            {
                SqlGeometry p = sqlGeometry.STPointN(i);
                points.Add(new Point(p.STX.Value, p.STY.Value));
            }
            return points;
        }

        public static SqlGeometry FromPointList(IList<Point> points, int srid)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var p in points)
            {
                builder.AppendFormat("{0}{1} {2}", first ? "" : ",", p.X, p.Y);
                first = false;
            }

            return FromWkt(String.Format("POLYGON(({0}))", builder.ToString()), srid);
        }

    }
}
