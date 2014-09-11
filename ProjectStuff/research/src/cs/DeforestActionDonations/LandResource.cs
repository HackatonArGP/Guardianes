using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Models;
using System.Collections.Generic;

namespace Earthwatchers.Services
{
    [ServiceContract]
    public class LandResource
    {
        [WebGet(UriTemplate = "{name}")]
        public Land GetLandByEathWatcherName(string name)
        {
            return null;
        }
    }
}