using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Reflection;
using System.IO;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class HomeResource
    {
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<Home> GetHome(HttpRequestMessage request)
        {
            var home = new Home();
            AddLinks(home,request);
            home.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(4) +", date: " + File.GetCreationTime(Assembly.GetExecutingAssembly().Location) ;
            var response = new HttpResponseMessage<Home>(home) { StatusCode = HttpStatusCode.OK };
            return response;
        }

        private void AddLinks(Home home, HttpRequestMessage request)
        {
            var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            var uriBuilder = new UriBuilder(baseUrl) {Path = "api/earthwatchers"};

            // add earthwatchers
            home.EarthwatchersUri = uriBuilder.Uri.ToString();

            // add land
            uriBuilder.Path = "api/land";
            home.LandUri = uriBuilder.Uri.ToString();

            // add satelliteimages
            uriBuilder.Path = "api/satelliteimages";
            home.SatelliteImagesUri = uriBuilder.Uri.ToString();

            // add comments
            uriBuilder.Path = "api/comments";
            home.CommentsUri = uriBuilder.Uri.ToString();

            // add comments
            uriBuilder.Path = "api/flags";
            home.FlagsUri = uriBuilder.Uri.ToString();
        }
    }
}
