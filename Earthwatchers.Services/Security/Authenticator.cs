using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Principal;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security.MembershipProviders;

namespace Earthwatchers.Services.Security
{
    public class Authenticator
    {
        public static bool Authenticate(string connectionString)
        {
            //if (!HttpContext.Current.Request.IsSecureConnection && !HttpContext.Current.Request.IsLocal) return false;

            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization")) return false;

            string authHeader = HttpContext.Current.Request.Headers["Authorization"];

            //Para probar performance. Sacar despues
            //return true;

            IPrincipal principal;
            if (TryGetPrincipal(connectionString, authHeader, out principal))
            {
                HttpContext.Current.User = principal;

                return true;
            }
            return false;
        }

        

        private static bool TryGetPrincipal(string connectionString, string authHeader, out IPrincipal principal)
        {
            var creds = ParseAuthHeader(authHeader);
            if (creds != null)
            {
                if (TryGetPrincipal(connectionString, creds[0], creds[1], out principal)) return true;
            }

            principal = null;
            return false;
        }

        public static string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header 
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic")) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded 
            var base64Credentials = authHeader.Substring(6);
            var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

            // Okay this is the credentials 
            return credentials;
        }


        private static bool TryGetPrincipal(string connectionString, string userName, string password, out IPrincipal principal)
        {
            var earthwatcher = new EarthwatcherRepository(connectionString).GetEarthwatcher(userName, false);
            if (earthwatcher != null)
            {
                var membershipProvider = new EarthwatchersMembershipProvider(connectionString);
                var result = membershipProvider.ValidateUser(userName, password, earthwatcher.ApiEwId);
                if (result)
                {
                    principal = new GenericPrincipal(new GenericIdentity(userName), earthwatcher.GetRoles());
                    return true;
                }
            }
            principal = null;
            return false;
        }

        

        public static Earthwatcher CurrentUser()
        {
            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization")) return null;

            string authHeader = HttpContext.Current.Request.Headers["Authorization"];
            var creds = ParseAuthHeader(authHeader);
            if(creds.Length < 2)
                return null;

            EarthwatcherRepository repo = new EarthwatcherRepository(System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
            var user = repo.GetEarthwatcher(creds[0], false);

            return user;
        }

    }
}