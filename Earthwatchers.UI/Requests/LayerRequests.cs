using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Earthwatchers.Models.KmlModels;
using System.Linq;
using Earthwatchers.Models;
using System.Collections.Generic;

namespace Earthwatchers.UI.Requests
{

    public class LayerRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler LayerRecived;
        public event ChangedEventHandler FincasReceived;
        private readonly RestClient client;

        public LayerRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetLayers(int id)
        {
            var request = new RestRequest("layers/getlayer", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(id.ToString());

            client.ExecuteAsync<Layer>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        LayerRecived(response.Data, null)
                            ));
        }

        public void GetLayersByName(string name)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);

            var request = new RestRequest("layers/getlayerbyname", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(name);
            //request.AddHeader("Accept-Encoding", "gzip, deflate");

            client.ExecuteAsync<Layer>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        LayerRecived(response.Data, null)
                            ));
        }

        public void GetFincas()
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);

            var request = new RestRequest("basecamp", Method.GET) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();

            client.ExecuteAsync<List<Basecamp>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        FincasReceived(response.Data, null)
                            ));
        }

    }
}
