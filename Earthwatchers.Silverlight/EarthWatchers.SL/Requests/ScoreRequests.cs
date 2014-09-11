using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class ScoreRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler ScoresReceived;
        public event ChangedEventHandler ScoreAdded;
        public event ChangedEventHandler ScoreUpdated;
        private readonly RestClient client;

        public ScoreRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
        }

        public void GetByUser(int earthwatcherid)
        {
            var request = new RestRequest(@"scores?user=" + earthwatcherid, Method.GET);
            client.ExecuteAsync<List<Score>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ScoresReceived(response.Data, null)
                    ));
        }

        public void GetLeaderBoard(int earthwatcherid)
        {
            var request = new RestRequest(@"scores/leaderboard?user=" + earthwatcherid, Method.GET);
            client.ExecuteAsync<List<Score>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ScoresReceived(response.Data, null)
                    ));
        }

        public void Update(Score score, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("scores/update", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(score);

            client.ExecuteAsync<Score>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ScoreUpdated(response.Data, null)
                    ));
        }

        public void Post(Score score, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("scores", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(score);
            
            client.ExecuteAsync<Score>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ScoreAdded(response.Data, null)
                    ));
        }
    }
}
