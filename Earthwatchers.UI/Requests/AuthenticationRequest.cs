using System;
using System.Windows;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class AuthenticationRequest
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler AuthenticateReceived;
        public event ChangedEventHandler LogoutFinished;
        private readonly RestClient client;

        public AuthenticationRequest(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void Authenticate (string baseUrl, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
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

        public void Logout()
        {
            var request = new RestRequest("authenticate/logout", Method.GET);
            client.ExecuteAsync(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                        LogoutFinished(null, null)
                    ));
        }
    }
}
