using System;
using System.Net;

namespace Earthwatchers.Models
{
    public class XY
    {
        public double x { get; set; }
        public double y { get; set; }

        public XY()
        {
            x = double.MinValue;
            y = double.MinValue;
        }

        public XY(double x, double y)
        {
            this.x = x; 
            this.y = y;
        }

        public string WKT
        {
            get
            {
                //Todo this should use a proper WKT converter
                if (x != double.MinValue && y != double.MinValue)
                {
                    return string.Format("POINT ({0} {1})", x.ToString(), y.ToString());
                }
                else return "";
            }
        }
    }
}
