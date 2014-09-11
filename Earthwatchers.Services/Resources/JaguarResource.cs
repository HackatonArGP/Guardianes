using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using System.ServiceModel;
using Earthwatchers.Services.Security;
using System.ServiceModel.Web;
using System.Net.Http;
using System.Net;
using Microsoft.AspNet.SignalR;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class JaguarResource
    {
        private readonly IJaguarRepository jaguarRepository;

        public JaguarResource(IJaguarRepository jaguarRepository)
        {
            this.jaguarRepository = jaguarRepository;
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<JaguarGame>> Get(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<JaguarGame>>(jaguarRepository.Get()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<JaguarGame>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<JaguarGame> Post(JaguarGame jaguarPos, HttpRequestMessage<JaguarGame> request)
        {
            if (jaguarPos != null)
            {
                var jaguarDB = jaguarRepository.Insert(jaguarPos);

                var response = new HttpResponseMessage<JaguarGame>(jaguarDB) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<JaguarGame>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage Delete(int id, HttpRequestMessage<int> request)
        {
            if (id != 0)
            {
                jaguarRepository.Delete(id);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/updateWinner", Method = "POST")]
        public HttpResponseMessage Update(LandMini landMini, HttpRequestMessage<LandMini> request)
        {
            try
            {
                jaguarRepository.Update(landMini.EarthwatcherId, landMini.LandId);

                var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
                context.Clients.All.JaguarFound(landMini.Email, landMini.EarthwatcherId);

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [WebInvoke(UriTemplate = "/getById", Method = "POST")]
        public HttpResponseMessage<JaguarGame> GetPositionById(int id, HttpRequestMessage<int> request)
        {
            var pos = jaguarRepository.GetPos(id);
            if (pos == null)
            {
                return new HttpResponseMessage<JaguarGame>(HttpStatusCode.NotFound);
            }
            else
            {
                return new HttpResponseMessage<JaguarGame>(pos) { StatusCode = HttpStatusCode.OK };
            }
        }

    }
}