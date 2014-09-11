using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models.KmlModels
{
   public class Polygon
    {



        public int Id { get; set; }
        public String Name { get; set; }
        public List<Location> Locations { get; set; }
        public string PolygonGeom { get; set; }

        
    }


}
