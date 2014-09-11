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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Microsoft.SqlServer.Types;


namespace DeforestAction
{
    public class AssignPlotOfLand
    {
        private IList<SqlGeometry> assigned;
        private SqlGeometry assignedUnion;
        private SqlGeometry outerBoundary; // htr_kphp

        // There is ONE starting point now.
        // Possible enhancement: create more starting points, because:
        // 1: they can then be handled parallel
        // 2: this might be an escape for if the process locks (parcel locked in)
        // 3: visually attractive (forest-like)
        private SqlGeometry startingPoint;

        private const double epsilon = 0.00001;


        public AssignPlotOfLand()
        {
            assignedUnion = null;
            startingPoint = null;
            outerBoundary = null;
            assigned = new List<SqlGeometry>();
        }

        public void SetStartingPoint(SqlGeometry point)
        {
            startingPoint = point;
        }

        public void SetOuterBoundary(SqlGeometry boundary)
        {
            outerBoundary = boundary;
        }

        SqlGeometry generateSquare(double x, double y, double width, double height, int srid)
        {
            // Generate a rectangular polygon of specified coordinates with width/height
            SqlGeometry geo = GeometryExtensions.FromPointList
                (
                    new List<Point>() 
                    { 
                            new Point(x, y), 
                            new Point(x, y + width), 
                            new Point(x + width, y + height), 
                            new Point(x + width, y), 
                            new Point(x, y) 
                    }
                    , srid
                );
            return geo;
        }

        SqlGeometry generate(double x, double y, int n, int srid)
        {
            // Generate a polygon starting with (x,y) of area n
            // It follows Anne's snake concept but adds one polygon (rectangle) at a time
            // and it only adds such a rectangle if not overlaying an already assigned area,
            // and if located within the total to-be-forested area
            SqlGeometry total = null;
            int delta = 0;
            int index = 0; // should walk from 0-1,0-1,0-2,0-2,0-3,0-3,0-4,0-4
            int index_count = 1; // should be    1,  1,  2,  2,  3,  3,  4,  4
            int s = 0;
            for (int i = 0; i < n; index++)
            {
                // Just add squares as polygons and union them.
                // [[[Enhancement (hard): generate outline in first pass
                // -> skip first point(s) (at n>=4), generate one, two or three point(s) per box
                // -> not possible, because of other plots of land in neighborhood]]]
                SqlGeometry g = generateSquare(x, y, 1, 1, srid);
                if ((assignedUnion == null || g.STIntersection(assignedUnion).STArea().Value < epsilon)
                    && (outerBoundary == null || Math.Abs(g.STIntersection(outerBoundary).STArea().Value - 1.0) < epsilon)
                    )
                {
                    // TODO: to avoid multi-polygons, calculate the union, get the STNumGeometries,
                    // and only add to total if it is still 1.
                    total = total == null ? g : total.STUnion(g);
                    i++;
                }

                if (index == index_count)
                {
                    index = 0;
                    delta++;
                    delta = delta % 4;
                    if (s == 1)
                    {
                        index_count++;
                    }
                    s = 1 - s;
                }

                switch (delta)
                {
                    case 0: x += 1; break;
                    case 1: y += 1; break;
                    case 2: x -= 1; break;
                    case 3: y -= 1; break;
                }

                // TODO: somehow check if the process locks. The process locks if:
                // a whole circle has been made (delta = 0,1,2,3) and no progress has been made (i still the same)
            }

            // Add to already assigned list
            assignedUnion = assignedUnion == null ? total : assignedUnion.STUnion(total);

            return total;
        }

        private SqlGeometry GenerateStartingPoint(int wishedArea)
        {
            if (assignedUnion == null)
            {
                return startingPoint;
            }

            // 1: retrieve union of all assigned regions
            // 2: get the exterior ring
            // 3: walk through all points of exterior ring to find the closest to the "starting point"
            // 4: return that closest point.

            // Issue: starting point is NOT good if lying close to border. So:
            // 5: starting point may not be located close to border (~ sqrt( area))

            SqlGeometry ring = assignedUnion.STExteriorRing();
            SqlGeometry boundaryOuter = outerBoundary.STBoundary(); // might become member-variable

            int increment = 3; // it is not so important to walk EVERY point of the assigned area, 2 or 3, or even more, will do. This speeds up.
            // Possible performance enhancement: do this iteratively (hard) or reduce the ring by 5 meters (easy)

            SqlGeometry result = null;
            double minDistance = 1e30;
            double bufferDistance = 1.0 + Math.Sqrt(wishedArea); // if 1m^2, it will start >= 2 meters out of the boundary
            for (int i = 1; i <= ring.STNumPoints().Value; i += increment)
            {
                var pointOnRing = ring.STPointN(i);
                double distance = pointOnRing.STDistance(startingPoint).Value;
                if (distance < minDistance && pointOnRing.STDistance(boundaryOuter) >= bufferDistance)
                {
                    minDistance = distance;
                    result = pointOnRing;
                }
            }
            return result;

        }

        public SqlGeometry Assign(int wishedArea, int srid)
        {
            SqlGeometry newStartingPoint = GenerateStartingPoint(wishedArea);
            if (newStartingPoint != null)
            {
                double x = Math.Floor(newStartingPoint.STX.Value);
                double y = Math.Floor(newStartingPoint.STY.Value);
                SqlGeometry assigned = generate(x, y, wishedArea, srid);

                // Defensive check
                if (Math.Abs(assigned.STArea().Value - wishedArea) > epsilon)
                {
                    throw new Exception("Incorrect area");
                }

                return assigned;
            }
            return null;
        }

    }
}
