using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using System.Net;
using Microsoft.ApplicationServer.Http;
using System.Linq;

namespace Earthwatchers.Services
{
    [ServiceContract]
    public class EarthwatcherResource
    {
        private IEarthwatcherRepository earthwatcherRepository;

        public EarthwatcherResource()
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
            this.earthwatcherRepository = new EarthwatcherRepository(connectionstring);
        }

        // idea: use a formatter to return land in pdf, png, svg, gml, wkt, kml 
        [WebGet(UriTemplate = "{name}")]
        public HttpResponseMessage<Earthwatcher> Get(string name)
        {
            Earthwatcher earthwatcher = earthwatcherRepository.Get(name);
	        if(earthwatcher == null) {
		        return new HttpResponseMessage<Earthwatcher>(HttpStatusCode.NotFound);
	        }
	        return new HttpResponseMessage<Earthwatcher>(earthwatcher){StatusCode = HttpStatusCode.OK};
        }

        //[WebGet(UriTemplate = "")]
        //public List<Earthwatcher> GetAll()
        //{
        //    return earthwatcherRepository.GetAll();
        //}

        // http://localhost:5654/earthwatchers?$orderby=Name desc
        // http://localhost:5654/earthwatchers?$top=1
        // reference all queries: http://www.odata.org/developers/protocols/uri-conventions
        [WebGet(UriTemplate = "")]
        public IQueryable<Earthwatcher> GetAll()
        {
            return earthwatcherRepository.GetAll().AsQueryable();
        }

        // Post Content-type: application/x-www-form-urlencoded
        // return 201: resource was created 
        // return 202: resource will be created
        // 405: method not allowed
        // question: how to call this thing from fiddler/silverlight?
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<Earthwatcher> Post(Earthwatcher earthwatcher)
        {
            Earthwatcher earthwatcherResult = earthwatcherRepository.Post(earthwatcher);
            var response = new HttpResponseMessage<Earthwatcher>(earthwatcherResult);
            response.StatusCode = HttpStatusCode.Created;
            // todo set location in response
            //response.Headers.Location=new UriPathExtensionMapping(string.Format()
            return response;
        }
    }
}