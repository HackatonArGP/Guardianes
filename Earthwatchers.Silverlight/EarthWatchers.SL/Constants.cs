using System;
using System.Globalization;
using System.Windows;

namespace EarthWatchers.SL
{
    public static class Constants
    {
        public static string BaseApiUrl= new Uri(Application.Current.Host.Source, "/api").ToString();
        // public static string BaseApiUrl = "http://earthwatchers.cloudapp.net/api";
        public static string BaseUrl = "http://guardianes.azurewebsites.net"; 
        public const string Hexagonlayername = "Parcelas";
        public const string AlertedLandLayername = "Alerted Land";
        public const string flagLayerName = "Flags";
        //public const int Hexagonlevel = 7;
    }
}
