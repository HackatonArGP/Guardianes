using System;
using System.Linq;

namespace Earthwatchers.Models
{
    public class Statistic
    {
        public int ShowOrder { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public decimal Percentage { get; set; }
        public string UOM { get; set; }
    }
}
