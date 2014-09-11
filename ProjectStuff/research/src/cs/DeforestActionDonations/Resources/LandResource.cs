using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeforestActionDonations.Models;
using DeforestActionDonations.Repositories;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;
using System.Net;

namespace DeforestActionDonations.Resources
{
    [ServiceContract]
    public class LandResource
    {
        private readonly LandRepository _landRepository;

        public LandResource()
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
            _landRepository = new LandRepository(connectionstring);
        }

        //Get all adopters
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<DonationsLand>> GetAll(HttpRequestMessage request)
        {
            var land = _landRepository.GetAll();
            return new HttpResponseMessage<List<DonationsLand>>(land) { StatusCode = HttpStatusCode.OK };
        } 
    }
}