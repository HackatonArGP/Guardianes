using System;
using System.Globalization;
using System.Windows;

namespace Earthwatchers.UI
{
    public static class Constants
    {
        public static string BaseApiUrl= new Uri(Application.Current.Host.Source, "/api").ToString();
        public static string BaseUrl = new Uri(Application.Current.Host.Source, "/").ToString();
        public const string Hexagonlayername = "Parcelas";
        public const string ArgentineLawlayername = "Ley de Bosques";
        public const string BasecampsLayer = "Basecamps";
        public const string AlertedLandLayername = "Alerted Land";
        public const string DemandAuthoritiesLayerName = "Demandar a las autoridades";
        public const string flagLayerName = "Flags";
        public const string jaguarLayerName = "Jaguar";
        public const string fincasLayerName = "NombreFincas";
        //public const int Hexagonlevel = 7;
    }
}
