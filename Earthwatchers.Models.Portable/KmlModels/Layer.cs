using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Earthwatchers.Models;

namespace Earthwatchers.Models.KmlModels
{
    public class Layer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Zone> Zones { get; set; }
    }

    public enum Layers
    {
        [Description("OTBN")]
        LeyBosquesSalta,
        [Description("FincasLayer")]
        Basecamps
    }


}
