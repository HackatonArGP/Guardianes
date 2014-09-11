using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using System.Linq;
using RestSharp.Deserializers;


namespace Earthwatchers.UI.Requests
{
    public class ContestRequests
    {

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler ContestReceived;
        private readonly RestClient client;

        public ContestRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetContest()
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest("contest", Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<Contest>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ContestReceived(response.Data, null)
                    ));
        }

        public void GetWinner()
        { 
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest("contest/getwinner", Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<Contest>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ContestReceived(response.Data, null)
                    ));
            
        }
    }
}
