using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class NewsRequests
    {
        public delegate void    ChangedEventHandler(object sender, EventArgs e);
        public event            ChangedEventHandler NewsReceived;
        private readonly RestClient client;

        public NewsRequests(string baseUrl)
        {
            client = new RestClient(baseUrl);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void Getnews()
        {
            var request = new RestRequest("news", Method.GET);

            client.ExecuteAsync<List<News>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    NewsReceived(response.Data, null)
                    ));
        }
    }
}
