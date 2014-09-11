// Adapted from Wim Ligtendag's Blog

using System;
using System.Windows.Media;
using Microsoft.SqlServer.Types;
using System.Windows;

namespace DeforestAction
{
    public static class SqlGeometryHelper
    {
        private static double defaultPointSize = 100.0;

        public static Geometry ToWpfGeometry(this SqlGeometry sqlGeometry)
        {
            return ToWpfGeometry(sqlGeometry, defaultPointSize);
        }

        public static Geometry ToWpfGeometry(this SqlGeometry sqlGeometry, double pointSize)
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
            {
                if (sqlGeometry != null && !sqlGeometry.IsNull)
                {
                    for (int geometryIndex = 0; geometryIndex < (int)sqlGeometry.STNumGeometries(); geometryIndex++)
                    {
                        SqlGeometry subGeometry = sqlGeometry.STGeometryN(geometryIndex + 1);
                        if (subGeometry.STGeometryType() == "Polygon" || subGeometry.STGeometryType() == "MultiPolygon")
                        {
                            Point[] points = GetPointsFromSqlGeometry(subGeometry.STExteriorRing());
                            AddSegmentToGeometry(streamGeometryContext, points, true);
                            for (int interiorRingIndex = 0; interiorRingIndex < subGeometry.STNumInteriorRing(); interiorRingIndex++)
                            {
                                points = GetPointsFromSqlGeometry(subGeometry.STInteriorRingN(interiorRingIndex + 1));
                                AddSegmentToGeometry(streamGeometryContext, points, true);
                            }
                        }
                        else if (subGeometry.STGeometryType() == "MultiPoint" || subGeometry.STGeometryType() == "Point")
                        {
                            Point[] points = GetPointsFromSqlGeometry(subGeometry);
                            AddCircleToGeometry(streamGeometryContext, points, pointSize);
                        }
                        else if (subGeometry.STGeometryType() == "LineString" || subGeometry.STGeometryType() == "MultiLineString")
                        {
                            Point[] points = GetPointsFromSqlGeometry(subGeometry);
                            AddSegmentToGeometry(streamGeometryContext, points, false);
                        }
                    }
                }
            }
            return streamGeometry;
        }


        private static void AddCircleToGeometry(StreamGeometryContext streamGeometryContext, Point[] points, double pointSize)
        {
            foreach (Point point in points)
            {
                streamGeometryContext.BeginFigure(new Point(point.X - (pointSize / 2), point.Y - (pointSize / 2)), true, true);
                streamGeometryContext.ArcTo(new Point(point.X - (pointSize / 2) - 0.0001, point.Y - (pointSize / 2)),
                    new Size(pointSize, pointSize), 360, true, SweepDirection.Clockwise, true, false);
            }
        }

        private static void AddSegmentToGeometry(StreamGeometryContext streamGeometryContext, Point[] points, bool close)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (i == 0)
                {
                    streamGeometryContext.BeginFigure(points[i], true, false);
                }
                else
                {
                    streamGeometryContext.LineTo(points[i], true, true);
                }
            }
            if (close)
            {
                streamGeometryContext.LineTo(points[0], true, true);
            }
        }

        private static Point[] GetPointsFromSqlGeometry(SqlGeometry sqlGeometry)
        {
            Point[] points = new Point[(Int32)(sqlGeometry.STNumPoints())];

            for (int i = 0; i < sqlGeometry.STNumPoints(); i++)
            {
                SqlGeometry pointGeometry = sqlGeometry.STPointN(i + 1);
                points[i] = new Point((float)pointGeometry.STX.Value, (float)pointGeometry.STY.Value);
            }
            return points;
        }
    }
}
