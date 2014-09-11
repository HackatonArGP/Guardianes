using System;

namespace Earthwatchers.Models
{
    public class Contest
    {
        public int Id { get; set; }
        public string ShortTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? WinnerId { get; set; }
    }
}
