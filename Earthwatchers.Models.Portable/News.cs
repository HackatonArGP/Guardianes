using System;

namespace Earthwatchers.Models
{
    public class News
    {
        public int Id { get; set; }
        public int EarthwatcherId { get; set; }
        public string Username { get; set; }
        public string NewsItem { get; set; }
        public DateTime Published { get; set; }
        public string Wkt { get; set; }
    }
}
