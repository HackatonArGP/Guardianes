using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;
using System.Net;
using Earthwatchers.Data;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class AuthenticatorResource
    {
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<bool> Authenticate(HttpRequestMessage request)
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
            var isOkay=Authenticator.Authenticate(connectionstring);

            if(isOkay)
            {
                return new HttpResponseMessage<bool>(isOkay) { StatusCode = HttpStatusCode.OK};
            }
            else
            {
                return new HttpResponseMessage<bool>(isOkay) { StatusCode = HttpStatusCode.Forbidden };
            }
        }

        [WebGet(UriTemplate = "/login")]
        public HttpResponseMessage<bool> Login(HttpRequestMessage request)
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
            var isOkay = Authenticator.Authenticate(connectionstring);

            if (isOkay)
            {
                
                //Guardo un nuevo Scoring
                ScoreRepository scoreRepository = new ScoreRepository(connectionstring);
                scoreRepository.AddLoginScore(System.Web.HttpContext.Current.User.Identity.Name);

                if (!Session.HasLoggedUser())
                {
                    EarthwatcherRepository ewRepo = new EarthwatcherRepository(connectionstring);
                    var ew = ewRepo.GetEarthwatcher(System.Web.HttpContext.Current.User.Identity.Name, false);
                    if (ew != null)
                    {
                        Session.GenerateCookie(ew);
                    }
                }

                return new HttpResponseMessage<bool>(isOkay) { StatusCode = HttpStatusCode.OK };
            }
            else
            {
                return new HttpResponseMessage<bool>(isOkay) { StatusCode = HttpStatusCode.Forbidden };
            }
        }

        [WebGet(UriTemplate = "/logout")]
        public HttpResponseMessage Logout(HttpRequestMessage request)
        {
            
            try
            {
                Session.Logout();
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
         
    }
}