using Earthwatchers.Data;
using Earthwatchers.Models.KmlModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class LayersResource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILayerRepository layersRepository;

        public LayersResource(ILayerRepository repo)
        {
            this.layersRepository = repo;
        }

        [WebInvoke(UriTemplate = "/getlayer", Method = "POST")]
        public HttpResponseMessage<Layer> GetLayerById(string id, HttpRequestMessage<string> request)
        {
            Layer lay = layersRepository.GetLayer(int.Parse(id));
            if (lay == null)
            {
                return new HttpResponseMessage<Layer>(HttpStatusCode.NotFound);
            }
            return new HttpResponseMessage<Layer>(lay) { StatusCode = HttpStatusCode.OK };
        }

        [WebInvoke(UriTemplate = "/getlayerbyname", Method = "POST")]
        public HttpResponseMessage<Layer> GetLayerByName(string name, HttpRequestMessage<string> request)
        {
            Layer lay = layersRepository.GetLayerByName(name);
            if (lay == null)
            {
                return new HttpResponseMessage<Layer>(HttpStatusCode.NotFound);
                logger.Error("LayersResource, ");
            }

            HttpResponseMessage<Layer> message = new HttpResponseMessage<Layer>(lay) { StatusCode = HttpStatusCode.OK };
            //message.Content.Headers.Add("Content-Encoding", "gzip");
            return message;
        }
    }
}