using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace EarthWatchers.SL.Requests
{
    public class EarthwatchersRestClient
    {
        /**
        static readonly RestClient Client = new RestClient(BaseUrl);

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
