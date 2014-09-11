using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class EarthwatchersRestClient
    {
        /**
        static readonly RestClient Client = new RestClient(BaseUrl);
            Client.CookieContainer = RequestsHelper.CookieContainer;

        public static void GetIncidents(NetworkCredential networkCredential, string username, Action<List<Incident>> callback)
        {
            var request = new RestRequest("incidents");
            request.Credentials = networkCredential;
            request.AddParameter("username", username);
            Client.ExecuteAsync<List<Incident>>(request, response => callback(response.Data));
        }
        */
    }
}
