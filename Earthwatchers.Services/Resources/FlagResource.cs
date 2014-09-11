using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class FlagResource
    {
        private readonly IFlagRepository flagRepository;

        public FlagResource(IFlagRepository flagRepository)
        {
            this.flagRepository = flagRepository;
        }

        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<Flag>> GetFlags(HttpRequestMessage request)
        {
            var flagCollection = flagRepository.GetFlags();
            return flagCollection != null ?
                new HttpResponseMessage<List<Flag>>(flagCollection) { StatusCode = HttpStatusCode.OK } :
                new HttpResponseMessage<List<Flag>>(null) { StatusCode = HttpStatusCode.BadRequest };
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<Flag> PostFlag(Flag flag, HttpRequestMessage<Flag> request)
        {
            if (flag.EarthwatcherId != 0 && flag.Longitude != 0 & flag.Latitude != 0)
            {
                var newflag = flagRepository.PostFlag(flag);
                var response = new HttpResponseMessage<Flag>(newflag) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return null;
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public void DeleteComment(int id, HttpRequestMessage request)
        {
            flagRepository.DeleteFlag(id);
        }
    }
}