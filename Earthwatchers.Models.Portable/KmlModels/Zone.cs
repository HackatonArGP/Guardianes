using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models.KmlModels
{
   public class Zone
    {
        public Zone()
        {

        }

        public Zone(string name, List<Polygon> polygons, string desc = "", string param1 ="")
        {
            Name = name;
            Description = desc;
            Polygons = polygons;
            Param1 = param1;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Param1 { get; set; }
        public List<Polygon> Polygons { get; set; }
    }
}
