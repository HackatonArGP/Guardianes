using System;

namespace Earthwatchers.Models
{
    public class Calculations
    {
        public static double CalcHexSize(int level)
        {            
            return Constants.h_base / Math.Pow(3.0, level + 1);
        }

        public static XY Loc2Xy(double lon, double lat)
        {
            double x = lon * Constants.h_base / 180.0;
            double y = Math.Log(Math.Tan((90.0 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);
            y *= Constants.h_base / 180.0;

            return new XY(x, y);
        }

        public static Location XyToLoc(double x, double y)
        {
            double lon = (x / Constants.h_base) * 180.0;
            double lat = (y / Constants.h_base) * 180.0;
            lat = 180 / Math.PI * (2.0 * Math.Atan(Math.Exp(lat * Math.PI / 180.0)) - Math.PI / 2.0);

            return new Location(lon, lat);
        }
    }
}
