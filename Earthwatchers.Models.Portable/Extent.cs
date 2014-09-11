using System;

namespace Earthwatchers.Models
{
    public class Extent
    {
        public Extent()
        {
        }

        public Extent(double minX,double minY, double maxX, double maxY, long srid)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            SRID = srid;
        }

        public long SRID { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
    }
}
