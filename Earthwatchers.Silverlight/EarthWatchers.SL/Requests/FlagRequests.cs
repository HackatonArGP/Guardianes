using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class FlagRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler FlagsReceived;
        public event ChangedEventHandler FlagAdded;
        public event ChangedEventHandler FlagRemoved;
        private readonly RestClient client;

        public FlagRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
        }

        public void GetFlags()
        {
            var request = new RestRequest(@"flags", Method.GET) {RequestFormat = DataFormat.Json};
            client.ExecuteAsync<List<Flag>>(request, response => 
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    FlagsReceived(response.Data, null)
                    ));
        }

        public void Delete(string flagId, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest(@"flags/"+ flagId, Method.DELETE);
            client.ExecuteAsync(request, response => Deployment.Current.Dispatcher.BeginInvoke(() =>
                    FlagRemoved(null, null)
                    ));
        }

        public void Post(Flag flag, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("flags", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(flag);
            client.ExecuteAsync<List<Flag>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    FlagAdded(response.Data, null)
                    ));
        }
    }
}
