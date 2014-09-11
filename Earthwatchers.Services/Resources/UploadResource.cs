using Earthwatchers.Models;
using Earthwatchers.Services.admin;
using Earthwatchers.Services.Security;
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

    public class UploadResource
    {
        //TEST
        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/uploadFile", Method = "POST")]
        public HttpResponseMessage UploadFile()
        {
            try
            {
                UploadFileDotNet up = new UploadFileDotNet();
                //up.UploadGenericFile(HttpContext.Current);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}