namespace Earthwatchers.Models
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Location(double lon, double lat)
        {
            Latitude = lat;
            Longitude = lon;
        }
    }
}
