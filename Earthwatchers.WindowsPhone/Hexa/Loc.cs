using System;
using System.Net;

namespace GeoHex
{
    public class Loc
    {
        public double lat { get; set; }
        public double lon { get; set; }

        /// <summary>
        /// Represents a world coordinate in longitude latitude
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        public Loc(double lon, double lat)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
