using System;
using ESRI.ArcGIS.Client.Geometry;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DonationsViewer.Helpers
{
    public class WktConverter
    {
        public static Polygon PolygonWktToPolygon(string wkt)
        {
            if (wkt.Contains("MULTIPOLYGON ((")) 
                return MultiPolygonWktToPolygon(wkt);
            else if(wkt.Contains("POLYGON (("))
                return SinglePolygonWktToPolygon(wkt);

            return null;
        }

        private static Polygon SinglePolygonWktToPolygon(string wkt)
        {
            var polygon = new Polygon();
            var pointCollection = new PointCollection();
            var removed = wkt.Replace("POLYGON ((", "").Replace("))", "");
            var coords = removed.Split(',');

            foreach (var coord in coords)
            {
                var xy = coord.TrimStart().Split(' ');
                if (xy.Length != 2)
                    continue;

                pointCollection.Add(new MapPoint(double.Parse(xy[0], CultureInfo.InvariantCulture), double.Parse(xy[1], CultureInfo.InvariantCulture)));
            }

            polygon.Rings.Add(pointCollection);

            return polygon;
        }

        private static Polygon MultiPolygonWktToPolygon(string wkt)
        {
            var polygon = new Polygon();

            var pointCollection = new PointCollection();
            var removed = wkt.Replace("MULTIPOLYGON (", "");
            var preSplit = removed.Replace(")), ((", "|");
            var rings = preSplit.Split('|');

            foreach (var r in rings)
            {
                PointCollection pc = new PointCollection();

                var r1 = r.Replace("(", "");
                var r2 = r1.Replace(")", "");
                var r3 = r2.Trim();

                var coords = r3.Split(',');
                foreach(var coord in coords)
                {
                    coord.Trim();
                    var xy = coord.Trim().Split(' ');
                    if (xy.Length != 2)
                    continue;

                    pc.Add(new MapPoint(double.Parse(xy[0], CultureInfo.InvariantCulture), double.Parse(xy[1], CultureInfo.InvariantCulture)));
                }

                polygon.Rings.Add(pc);
            }

            return polygon;
        }
    }
}
