using System;
using System.Linq;

namespace Earthwatchers.Models
{
    public class LandMini
    {
        public int Id { get; set; }
        public string GeohexKey { get; set; }
        public bool IsUsed { get; set; }
        public string Email { get; set; }
        public int EarthwatcherId { get; set; }
        public int LandId { get; set; }
    }
}
