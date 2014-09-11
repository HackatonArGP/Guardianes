using System;
using System.Linq;

namespace Earthwatchers.Models
{
    public class CollectionItem
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool HasItem { get; set; }
        public string BackgroundImage { get; set; }
    }
}
