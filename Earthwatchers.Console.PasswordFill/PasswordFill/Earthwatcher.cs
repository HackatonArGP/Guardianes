using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PasswordFill
{
    public class Earthwatcher
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string PasswordPrefix { get; set; }
        public string HashedPassword { get; set; }
    }
}
