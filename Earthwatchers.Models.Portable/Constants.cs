using System;
using System.Net;

namespace Earthwatchers.Models
{
    internal class Constants
    {
        public static readonly String h_key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static readonly double h_base = 20037508.34; //MaxExtend Spherical mercator
        public static readonly double h_deg = Math.PI * (30.0 / 180.0); //Used to calculate Graphine, 180 / 30 = 6 coordinates for honeycomb
        public static readonly double h_k = Math.Tan(h_deg);
    }
}
