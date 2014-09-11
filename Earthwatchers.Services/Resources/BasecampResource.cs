using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Models.Portable;
using Earthwatchers.Services.admin;
using Earthwatchers.Services.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class BasecampResource
    {
        private readonly IBasecampRepository basecampRepository;

        public BasecampResource(IBasecampRepository basecampRepository)
        {
            this.basecampRepository = basecampRepository;
        }

        //[BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<Basecamp>> Get(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<Basecamp>>(basecampRepository.Get()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<Basecamp>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [WebGet(UriTemplate = "/bases")]
        public HttpResponseMessage<List<Basecamp>> GetBaseCamps(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<Basecamp>>(basecampRepository.GetBaseCamps()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<Basecamp>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<Basecamp> Post(Basecamp basecamp, HttpRequestMessage<Basecamp> request)
        {
            if (basecamp != null)
            {
                var basecampDb = basecampRepository.Insert(basecamp);

                var response = new HttpResponseMessage<Basecamp>(basecampDb) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<Basecamp>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage Delete(int id, HttpRequestMessage<int> request)
        {
            if (id != 0)
            {
                basecampRepository.Delete(id);
                LayerRepository layerRepository = new LayerRepository(ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
                layerRepository.DeleteZone(id);
                LandRepository landRepository = new LandRepository(ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
                landRepository.LoadLandBasecamp();
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/recalculate", Method = "POST")]
        public HttpResponseMessage Recalculate(int id, HttpRequestMessage<int> request)
        {
            try
            {
                basecampRepository.RecalculateDistance(id);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/getbyid", Method = "POST")]
        public HttpResponseMessage<Basecamp> GetByIdaa(int id, HttpRequestMessage<int> request)
        {
                if (id != 0)
                {
                    var basecamp = basecampRepository.GetById(id);
                    return new HttpResponseMessage<Basecamp>(basecamp) { StatusCode = HttpStatusCode.OK };
                }
                else
            return new HttpResponseMessage<Basecamp>(null) { StatusCode = HttpStatusCode.BadRequest };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/edit", Method = "POST")]
        public HttpResponseMessage<Basecamp> Edit(Basecamp basecamp, HttpRequestMessage<Basecamp> request)
        {
            if (basecamp != null)
            {
                var basecampDb = basecampRepository.Edit(basecamp);

                var response = new HttpResponseMessage<Basecamp>(basecampDb) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<Basecamp>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        //TEST
        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/uploadFile", Method = "POST")]
        public HttpResponseMessage UploadFile()
        {
            try
            {
                var req = HttpContext.Current.Request;
                var resp = HttpContext.Current.Response;
                UploadFileDotNet up = new UploadFileDotNet();
                //up.UploadGenericFile(HttpContext.Current);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/readkml", Method = "POST")]
        public HttpResponseMessage<List<string>> ReadKml(int id)
        {
            List<string> errors = new List<string>();
            if (id == 0)
            {
                errors.Add("No se encontro Id perteneciente a la finca");
                return new HttpResponseMessage<List<string>>(errors) { StatusCode = HttpStatusCode.OK };
            }
            else
            {
                errors.AddRange(KmlParserBasecamp.ReadKmlFile(id));
                return new HttpResponseMessage<List<string>>(errors) { StatusCode = HttpStatusCode.OK };
            }

        }

    }
} 