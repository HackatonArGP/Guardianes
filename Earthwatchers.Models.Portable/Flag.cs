using System;

namespace Earthwatchers.Models
{
    public class Flag
    {
        public Flag() { Published = DateTime.Now; }
        public int Id { get; set; }
        public int EarthwatcherId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime Published { get; set; }
    }
}
