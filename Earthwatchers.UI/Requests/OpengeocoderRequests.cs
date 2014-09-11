using System;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class QueryResult
    {
        public int id { get; set; }
        public string q { get; set; }
        public List<double> bbox{get;set;}
    }


    public class OpengeocoderRequests
    {
        // for the documentation:
        //string searchUrl = @"http://www.opengeocoder.net/api/0.1/Search?query=utrecht";
        // response {"id":301,"q":"utrecht","bbox":[4.9926464,52.0522136,5.1637048,52.1502941]}
        //string autoCompletUrl = @"http://www.opengeocoder.net/API/autocomplete?term=utrecht";
        // response ["utrecht","utrecht, the netherlands"]

        private const string BaseUrl = @"http://www.opengeocoder.net/API";
        //private const string BaseUrl = @"http://localhost/API/";

        public static void GetSuggestions(Action<List<String>> callback, string term)
        {
            var client = new RestClient(BaseUrl);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();

            var request = new RestRequest("autocomplete");
            request.AddParameter("term", term);
            client.ExecuteAsync<List<String>>(request, response => callback(response.Data));
        }

        public static void GetQueryResult(Action<QueryResult> callback, string query)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("search");
            request.AddParameter("query", query);
            client.ExecuteAsync<QueryResult>(request, response => callback(response.Data));
        }
    }
}
