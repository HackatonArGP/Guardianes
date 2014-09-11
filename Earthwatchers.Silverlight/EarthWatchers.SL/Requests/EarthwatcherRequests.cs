using System;
using System.Windows;
using Earthwatchers.Models;
using System.Globalization;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class EarthwatcherRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler EarthwatcherReceived;
        private readonly RestClient client;

        public EarthwatcherRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
        }


        public void GetByName(string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest(@"earthwatchers/name=" + username, Method.GET);
            client.ExecuteAsync<Earthwatcher>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        EarthwatcherReceived(
                            response.Data, null)
                    ));
        }


        public void GetById(string id)
        {
            var request = new RestRequest(@"earthwatchers/" + id.ToString(CultureInfo.InvariantCulture), Method.GET);
            client.ExecuteAsync<Earthwatcher>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        EarthwatcherReceived(
                            response.Data, null)
                    ));
        }
    }
}