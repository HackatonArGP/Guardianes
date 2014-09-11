using System;
using System.Linq;

namespace Earthwatchers.Models
{
    public class Land
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GeohexKey { get; set; }
        public LandThreat LandThreat { get; set; }
        public LandStatus LandStatus { get; set; }
        public int? EarthwatcherId { get; set; }
        public string EarthwatcherName { get; set; }
        public bool? IsPowerUser { get; set; }
        public string OKs { get; set; }
        public string Alerts { get; set; }
        public string LastUsersWithActivity { get; set; }
        public DateTime StatusChangedDateTime { get; set; }
        public bool DemandAuthorities { get; set; }
        public string DemandUrl { get; set; }
        public DateTime LastReset { get; set; }
        public bool? Reset { get; set; }

        public bool IsLocked { get; set; }
        public bool? Confirmed { get; set; }

        public int? BasecampId { get; set; }
        public string BasecampName { get; set; }
        public string ShortText { get; set; }

        //public int ConfirmCount { get; set; }
        //public int DeConfirmCount { get; set; }
        //public int VerificationsCount { get; set; }
    }
}
