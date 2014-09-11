using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models
{
    public class PetitionsSigned
    {
        public string Earthwatcher { get; set; }
        public int PetitionId { get; set; }
        public bool Signed { get; set; }
    }
}
