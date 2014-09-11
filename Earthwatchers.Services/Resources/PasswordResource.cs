using Earthwatchers.Data;
using Earthwatchers.Models;
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
    public class PasswordResource
    {
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<Earthwatcher> ChangePassword(Earthwatcher earthwatcher, HttpRequestMessage<Earthwatcher> request)
        {
            if (!String.IsNullOrEmpty(earthwatcher.Name) && (!String.IsNullOrEmpty(earthwatcher.Password)))
            {
                    GenerateAndUpdatePassword(earthwatcher);
                    var response = new HttpResponseMessage<Earthwatcher>(earthwatcher) { StatusCode = HttpStatusCode.Accepted };
                    return response;
            }
            else
            {
                var response = new HttpResponseMessage<Earthwatcher>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "request parameters not correct"};
                return response;
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/admin", Method = "POST")]
        public HttpResponseMessage<Earthwatcher> ChangePasswordAdmin(Earthwatcher earthwatcher, HttpRequestMessage<Earthwatcher> request)
        {
            if (!String.IsNullOrEmpty(earthwatcher.Name) && (!String.IsNullOrEmpty(earthwatcher.Password)))
            {
                    GenerateAndUpdatePassword(earthwatcher);
                    var response = new HttpResponseMessage<Earthwatcher>(earthwatcher) { StatusCode = HttpStatusCode.Accepted };
                    return response;
            }
            else
            {
                var response = new HttpResponseMessage<Earthwatcher>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "request parameters not correct" };
                return response;
            }
        }

        public void GenerateAndUpdatePassword(Earthwatcher earthwatcher)
        {
            var prefix = PasswordChecker.GeneratePrefix();
            var hashedPassword = PasswordChecker.GenerateHashedPassword(earthwatcher.Password, prefix);

            // store in database
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
            var repos = new EarthwatcherRepository(connectionstring);
            repos.UpdatePassword(earthwatcher, prefix, hashedPassword);
        }
    }
}