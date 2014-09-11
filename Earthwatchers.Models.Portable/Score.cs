using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int EarthwatcherId { get; set; }
        public DateTime Published { get; set; }
        public string Action { get; set; }
        public int Points { get; set; }
        public int? LandId { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }

        public string Uri
        {
            get { return @"scores/" + Id.ToString(); }
        }
    }
}
