using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models.KmlModels
{
    public class Location
    {
        public Location(double? lon, double? lat)
        {
            this.Longitude = lon;
            this.Latitude = lat;
        }

        public Location()
        {

        }

        public int Id { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Index { get; set; }
    }
}
