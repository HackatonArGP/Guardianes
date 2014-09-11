using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Earthwatchers.Data;
using System.Security.Principal;
using System.Linq;

namespace Earthwatchers.Services.Security
{
    public class BasicHttpAuthorizationOperationHandler : HttpOperationHandler<HttpRequestMessage, HttpResponseMessage>
    {
        private readonly BasicHttpAuthorizationAttribute basicHttpAuthorizationAttribute;

        public BasicHttpAuthorizationOperationHandler(BasicHttpAuthorizationAttribute authorizeAttribute): base("response")
        {
            basicHttpAuthorizationAttribute = authorizeAttribute;
        }

        protected override HttpResponseMessage OnHandle(HttpRequestMessage input)
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;

            if (Session.HasLoggedUser())
            {
                var sessionInfo = Session.GetCookieInfo();
                if (sessionInfo.Roles.Contains(basicHttpAuthorizationAttribute.Role.ToString()))
                {
                    if (!(HttpContext.Current.User is GenericPrincipal))
                        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(sessionInfo.EarthwatcherName), sessionInfo.Roles);

                    return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = input };
                }
            }
            else
            {
                if (Authenticator.Authenticate(connectionstring))
                {
                    if (HttpContext.Current.User.IsInRole(basicHttpAuthorizationAttribute.Role.ToString()))
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = input };
                    }
                }
            }

            //return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = input };

            var challengeMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            challengeMessage.ReasonPhrase = "error: you need to be authorized for this operation";
            throw new HttpResponseException(challengeMessage);
        }
    }
}