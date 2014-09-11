using System;

namespace Earthwatchers.Models
{
    public class Comment
    {
        public Comment() { Published = DateTime.Now; }
        public int Id { get; set; }
        public int EarthwatcherId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int LandId { get; set; }
        public string UserComment { get; set; }
        public DateTime Published { get; set; }

        public string Uri
        {
            get { return @"comments/" +  Id.ToString(); }
        }

    }
}
