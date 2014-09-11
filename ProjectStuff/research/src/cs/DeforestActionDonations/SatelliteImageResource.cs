using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Earthwatchers.Services.Projection;

namespace Earthwatchers.Services
{
    [ServiceContract]
    public class SatelliteImageResource
    {
        [WebGet(UriTemplate = "{name}")]
        public SatelliteImage GetImageByName(string name)
        {
            //Todo
            return null;
        }

        [WebGet(UriTemplate = "")]
        public List<SatelliteImage> GetAllImages()
        {
            //Todo
            return new List<SatelliteImage>();
        }

        // Sample: http://localhost:5654/images?latitude=2&longitude=4? 
        [WebGet(UriTemplate = "?latitude={latitude}&longitude={longitude}")]
        public List<SatelliteImage> GetImagesByHexagon(double latitude, double longitude)
        {
            //Get the geohex locations
            string geoHexCode = GeoHex.Encode(longitude, latitude, 7);
            Zone zone = GeoHex.Decode(geoHexCode);
            Loc[] locs = zone.getHexCoords();

            //Create a Polygon
            IEnumerable<XY> pointCollection = GetPointCollection(locs);
            Polygon polygon = new Polygon(pointCollection);

            return GetImagesByPolygon(polygon);
        }

        // Sample: http://localhost:5654/images?minX=2&minY=4&maxX=21&maxY=56 ? 
        [WebGet(UriTemplate = "?minX={minX}&minY={minY}&maxX={maxX}&maxY={maxY}")]
        public List<SatelliteImage> GetImagesByExtent(double minX, double minY, double maxX, double maxY)
        {
            //Create a polygon from the bounding box
            IEnumerable<XY> pointCollection = GetBoundingBox(minX, minY, maxX, maxY);
            Polygon polygon = new Polygon(pointCollection);

            return GetImagesByPolygon(polygon);
        }

        private List<SatelliteImage> GetImagesByPolygon(Polygon polygon)
        {
            //Get WKT
            string wkt = polygon.WKT;

            //Call the stored procedure GetSatelliteImagesInExtent(wkt)

            // return List of sattelite images
            return null;
        }

        private IEnumerable<XY> GetPointCollection(Loc[] locations)
        {
            foreach (Loc location in locations)
            {
                yield return new XY(location.lon, location.lat);
            }
        }

        private IEnumerable<XY> GetBoundingBox(double minX, double minY, double maxX, double maxY)
        {
            yield return new XY(minX, minY);
            yield return new XY(minX, maxY);
            yield return new XY(maxX, maxY);
            yield return new XY(maxX, minY);
        }
    }

    namespace Projection
    {
        public static class SphericalMercator
        {
            private readonly static double radius = 6378137;
            private static double D2R = Math.PI / 180;
            private static double HALF_PI = Math.PI / 2;

            public static XY FromLonLat(double lon, double lat)
            {
                double lonRadians = (D2R * lon);
                double latRadians = (D2R * lat);

                double x = radius * lonRadians;
                double y = radius * Math.Log(Math.Tan(Math.PI * 0.25 + latRadians * 0.5));

                return new XY((float)x, (float)y);
            }

            public static XY ToLonLat(double x, double y)
            {
                double ts;
                ts = Math.Exp(-y / (radius));
                double latRadians = HALF_PI - 2 * Math.Atan(ts);

                double lonRadians = x / (radius);

                double lon = (lonRadians / D2R);
                double lat = (latRadians / D2R);

                return new XY((float)lon, (float)lat);
            }
        }
    }
}