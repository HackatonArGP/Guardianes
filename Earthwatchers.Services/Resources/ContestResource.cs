using System;
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
    public class ContestResource
    {
        private readonly IContestRepository contestRepository;

        public ContestResource(IContestRepository _contestRepository)
        {
            this.contestRepository = _contestRepository;
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "/getall")]
        public HttpResponseMessage<List<Contest>> GetAllContests(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<Contest>>(contestRepository.GetAllContests()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<Contest>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/getwinner")]
        public HttpResponseMessage<Contest> GetWinner(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<Contest>(contestRepository.GetWinner()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<Contest>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/add", Method = "POST")]
        public HttpResponseMessage<Contest> Post(Contest contest, HttpRequestMessage<Contest> request)
        {
            if (contest != null)
            {
                var contestDB = contestRepository.Insert(contest);

                var response = new HttpResponseMessage<Contest>(contestDB) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<Contest>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage Delete(int id, HttpRequestMessage<int> request)
        {
            if (id != 0)
            {
                contestRepository.Delete(id);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<Contest> Get(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<Contest>(contestRepository.GetContest()) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<Contest>(null) { StatusCode = HttpStatusCode.InternalServerError };
            }
        }


    }
}