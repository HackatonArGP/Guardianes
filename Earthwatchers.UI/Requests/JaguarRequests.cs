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
using Earthwatchers.Models;
using System.Collections.Generic;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class JaguarRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler PositionReceived;
        private readonly RestClient client;

        public JaguarRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }



        public void UpdateWinner(int earthWatcherId, int posId, string nickName)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest("jaguarpositions/updateWinner", Method.POST) { RequestFormat = DataFormat.Json };
            LandMini earthwatcher = new LandMini { EarthwatcherId = earthWatcherId, LandId = posId, Email = nickName };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(earthwatcher);

            client.ExecuteAsync(request, response =>
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine("Ocurrio un error en la comunicación"); //TODO: tirar cartel al usuario.
                    }
                });
        }

        public void GetJaguarGameByID(int posId)
        {
            var request = new RestRequest("jaguarpositions/getById", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(posId);

            client.ExecuteAsync<JaguarGame>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                     PositionReceived(response.Data, null)
                     ));
        }
    }
}
