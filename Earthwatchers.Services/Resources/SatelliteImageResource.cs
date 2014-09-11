using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;
using System.Data.SqlTypes;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class SatelliteImageResource
    {
        private readonly ISatelliteImageRepository satelliteImageRepository;

        public SatelliteImageResource(ISatelliteImageRepository satelliteImageRepository)
        {
            this.satelliteImageRepository = satelliteImageRepository;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<SatelliteImage> Get(int id, HttpRequestMessage request)
        {
            var satelliteImage = satelliteImageRepository.Get(id);
            if (satelliteImage == null)
            {
                return new HttpResponseMessage<SatelliteImage>(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage<SatelliteImage>(satelliteImage) { StatusCode = HttpStatusCode.OK };
        }


        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<SatelliteImage>> GetAllSatellites(HttpRequestMessage request)
        {
            var satelliteImageCollection = satelliteImageRepository.GetAll();
            return new HttpResponseMessage<List<SatelliteImage>>(satelliteImageCollection) { StatusCode = HttpStatusCode.OK };
        }

        [WebInvoke(UriTemplate = "/intersect", Method = "POST")]
        public HttpResponseMessage<List<SatelliteImage>> GetSatelliteImageByWkt(string wkt, HttpRequestMessage<string> request)
        {
            if (!string.IsNullOrEmpty(wkt))
            {
                var satelliteImageCollection = satelliteImageRepository.Intersects(wkt);
                return new HttpResponseMessage<List<SatelliteImage>>(satelliteImageCollection) { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage<List<SatelliteImage>>(null) { StatusCode = HttpStatusCode.BadRequest };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<SatelliteImage> Post(SatelliteImage satelliteImage, HttpRequestMessage<SatelliteImage> request)
        {
            if (satelliteImage != null)
            {
                var satelliteImageDb = satelliteImageRepository.Insert(satelliteImage);

                var response = new HttpResponseMessage<SatelliteImage>(satelliteImageDb) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<SatelliteImage>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        public HttpResponseMessage Put(int id, SatelliteImage satelliteImage)
        {
            var satelliteImageDb = satelliteImageRepository.Get(id);

            if (satelliteImageDb != null)
            {
                satelliteImageRepository.Update(id, satelliteImage);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage Delete(int id, HttpRequestMessage<int> request)
        {
            var satelliteImage = satelliteImageRepository.Get(id);

            if (satelliteImage != null)
            {
                satelliteImageRepository.Delete(satelliteImage.Id);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }
    }
}
