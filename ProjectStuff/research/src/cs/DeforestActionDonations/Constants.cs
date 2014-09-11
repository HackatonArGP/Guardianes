using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeforestActionDonations
{
    public class Constants
    {
        public static readonly string LandTable = "tembak_hutan";
        public static readonly string AdoptersTable = "htr_adopters";
        public static readonly string SRID = "3857";
        public static readonly int SRID_INT = 3857;
        public static readonly string[] AllowedIps = new string[] { "::1", "127.0.0.1", "212.178.99.194", "141.101.126.95" };
    }
}