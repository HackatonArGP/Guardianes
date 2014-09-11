using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class CollectionRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler ItemsReceived;
        public event ChangedEventHandler NewItemReceived;
        public event ChangedEventHandler ItemsCountReceived;
        private readonly RestClient client;

        public CollectionRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetCollectionItemsByEarthwatcher(int earthwatcherid)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest(@"collections/" + earthwatcherid, Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<List<CollectionItem>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ItemsReceived(response.Data, null)
                    ));
        }

        public void GetNewCollectionItem(int earthwatcherid)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest(@"collections/newitem=" + earthwatcherid, Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<CollectionItem>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    NewItemReceived(response.Data, null)
                    ));
        }

        public void GetTotalItems(int earthwatcherid)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest(@"collections/totalitems=" + earthwatcherid, Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<int>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ItemsCountReceived(response.Content, null)
                    ));
        }
    }
}
