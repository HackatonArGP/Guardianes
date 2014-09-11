using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Types;

namespace DeforestActionDonations.Models
{
    public class Geom
    {
        public int Id { get; set; }
        public SqlGeometry Geometry { get; set; }
    }
}