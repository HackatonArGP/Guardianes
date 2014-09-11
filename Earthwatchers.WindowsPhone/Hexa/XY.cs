using System;
using System.Net;

namespace GeoHex
{
    public class XY
    {
        public double x { get; set; }
        public double y { get; set; }

        public XY(double x, double y)
        {
            this.x = x; this.y = y;
        }
    }
}
