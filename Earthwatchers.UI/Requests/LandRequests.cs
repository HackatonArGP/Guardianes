using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace Earthwatchers.UI.Requests
{
    public class LandRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler LandInViewReceived;
        public event ChangedEventHandler LandsReceived;
        public event ChangedEventHandler StatusChanged;
        public event ChangedEventHandler LandByStatusReceived;
        public event ChangedEventHandler LandReceived;
        public event ChangedEventHandler ConfirmationAdded;
        public event ChangedEventHandler ActivityReceived;
        public event ChangedEventHandler StatsReceived;
        public event ChangedEventHandler VerifiedLandCodesReceived;
        public event ChangedEventHandler PollAdded;
        private readonly RestClient client;

        public LandRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetLandById(string landId)
        {
            var request = new RestRequest("land/" + landId, Method.GET);

            client.ExecuteAsync<Land>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    LandReceived(response.Data, null)
                    ));
        }

        public void GetAll(int? landId)
        {
            var request = new RestRequest("land/all", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddHeader("Accept-Encoding", "gzip, deflate");
            
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(landId.HasValue ? landId.Value : 0);

            client.ExecuteAsync<List<Land>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    LandsReceived(response.Data, null)
                    ));

        }

        public void GetStats()
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);

            var request = new RestRequest("land/stats", Method.GET);

            client.ExecuteAsync<List<Statistic>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    StatsReceived(response.Data, null)
                    ));
        }

        public void GetLandByWkt(string wkt, int landId)
        {
            var request = new RestRequest("land/intersect", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();

            var earthwatcher = new Earthwatcher { Name = wkt, Id = landId };
            request.AddBody(earthwatcher);

            client.ExecuteAsync<List<Land>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    LandInViewReceived(response.Data, null)
                    ));

        }

        public void GetLandByStatus(LandStatus status)
        {
            var request = new RestRequest("land/status=" + status, Method.GET);
            client.ExecuteAsync<List<Land>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    LandByStatusReceived(response.Data, null)
                    ));
        }

        public void GetVerifiedLandsGeoHexCodes(int earthwatcherId, bool isPoll)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);

            var request = new RestRequest("land/verifiedlandscodes", Method.POST) { RequestFormat = DataFormat.Json };
            var earthwatcher = new Earthwatcher { Id = earthwatcherId, IsPowerUser = isPoll };
            request.AddBody(earthwatcher);

            client.ExecuteAsync<List<string>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    VerifiedLandCodesReceived(response.Data, null)
                    ));
        }

        public void AddPoll(LandMini land)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);

            var request = new RestRequest("land/addpoll", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(land);

            client.ExecuteAsync(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    PollAdded(null, null)
                    ));
        }

        public void GetActivity(int landId)
        {
            client.Authenticator = new HttpBasicAuthenticator(Current.Instance.Username, Current.Instance.Password);
            var request = new RestRequest("land/activity=" + landId, Method.GET) { RequestFormat = DataFormat.Json };
            client.ExecuteAsync<List<Score>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ActivityReceived(response.Data, null)
                    ));
        }

        public void UpdateStatus(int landId, LandStatus landStatus, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);

            var land = new Land { Id = landId, LandStatus = landStatus };
            var request = new RestRequest("land/updatestatus", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();

            request.AddBody(land);
            client.ExecuteAsync(request, response =>
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    StatusChanged(null, null)
            ));
        }

        public void Confirm(Land land, ConfirmationSort confirmationSort, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var landMini = new LandMini { LandId = land.Id, EarthwatcherId = land.EarthwatcherId.Value, Id = Current.Instance.Earthwatcher.Id, GeohexKey = land.GeohexKey };
            var request = new RestRequest("land/" + confirmationSort.ToString().ToLower(), Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();

            request.AddBody(landMini);
            client.ExecuteAsync(request, response =>
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ConfirmationAdded(confirmationSort, null)
            ));
        }
    }

    public enum ConfirmationSort
    {
        Confirm,
        Deconfirm
    }
}
