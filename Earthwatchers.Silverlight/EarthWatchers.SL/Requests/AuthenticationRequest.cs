using System;
using System.Windows;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class AuthenticationRequest
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler AuthenticateReceived;

        public void Authenticate (string baseUrl, string username, string password)
        {
            var client = new RestClient(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };

            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            var request = new RestRequest("authenticate", Method.GET);

            client.ExecuteAsync(request, response =>
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var result = false;
                bool.TryParse(
                    response.Content,
                    out result);
                AuthenticateReceived(result, null);
                    }));
            }
    }
}
