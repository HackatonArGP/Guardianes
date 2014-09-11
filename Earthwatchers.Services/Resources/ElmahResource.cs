using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;

namespace Earthwatchers.Services.Resources
{

    [ServiceContract]
    public class ElmahResource
    {
        [WebGet(UriTemplate = "")]
        [BasicHttpAuthorization(Role.Admin)]
        public void GetElmah()
        {
            var factory = new Elmah.ErrorLogPageFactory();
            var handler = factory.GetHandler(HttpContext.Current, null, null, null);
            handler.ProcessRequest(HttpContext.Current);
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "{resource}")]
        public void GetElmahResource(string resource)
        {
            var factory = new Elmah.ErrorLogPageFactory();
            HttpContext.Current.RewritePath(FilePath(resource), "/" + resource, HttpContext.Current.Request.QueryString.ToString());
            var handler = factory.GetHandler(HttpContext.Current, null, null, null);
            handler.ProcessRequest(HttpContext.Current);
        }

        private string FilePath(string resource)
        {
            return resource != "stylesheet" ? HttpContext.Current.Request.Path.Replace(String.Format("/{0}", resource), string.Empty) : HttpContext.Current.Request.Path;
        }
    }
}