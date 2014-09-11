using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using System.Linq;
using RestSharp.Deserializers;

namespace Earthwatchers.UI.Requests
{
    public class PopupMessageRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler MessageReceived;
        private readonly RestClient client;

        public PopupMessageRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetMessage()
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest("popupmessages", Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<List<PopupMessage>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    MessageReceived(response.Data, null)                    
                    ));
        }
    }
}
