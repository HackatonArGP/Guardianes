using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Models;

namespace Earthwatchers.Services
{
    [ServiceContract]
    public class LandService
    {
        [WebGet(UriTemplate = "{name}")]
        public Land Get(string name)
        {
            var land = new Land { Latitude = 5, Longitude = 10};
            return land;
        }
    }
}