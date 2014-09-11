using System;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;

namespace DeforestActionDonations.Resources
{
    [ServiceContract]
    public class HomeResource
    {
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<Home> GetHome(HttpRequestMessage request)
        {
            var home = new Home();
            AddLinks(home,request);
            var response = new HttpResponseMessage<Home>(home) { StatusCode = HttpStatusCode.OK };
            return response;
        }

        private void AddLinks(Home home, HttpRequestMessage request)
        {
            var uriBuilder = new UriBuilder(request.RequestUri);

            uriBuilder.Path = "donations";
            home.AdopterUri = uriBuilder.Uri.ToString();

            uriBuilder.Path = "land";
            home.DonationsLandUri = uriBuilder.Uri.ToString();
        }
    }
}
