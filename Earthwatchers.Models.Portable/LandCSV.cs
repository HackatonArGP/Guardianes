using System;
using System.Linq;

namespace Earthwatchers.Models
{
    public class LandCSV
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GeohexKey { get; set; }
        public LandThreat LandThreat { get; set; }
        public LandStatus LandStatus { get; set; }
        public int? EarthwatcherId { get; set; }
        public string EarthwatcherName { get; set; }
        public string LastUsersWithActivity { get; set; }
        public string StatusChangedDateTime { get; set; }
        public bool DemandAuthorities { get; set; }
        public string LastReset { get; set; }
        public bool IsLocked { get; set; }
        public bool? Confirmed { get; set; }
        
        public int OKs { get; set; }
        public int Suspicious { get; set; }

        public string OKsDetail { get; set; }
        public string SuspiciousDetail { get; set; }

        public int Positives { get; set; }
        public int Negatives { get; set; }

        public int PositivesV { get; set; }
        public int NegativesV { get; set; }
    }
}
