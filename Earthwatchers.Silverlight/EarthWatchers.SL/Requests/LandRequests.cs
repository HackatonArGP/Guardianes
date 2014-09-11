using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class LandRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler LandInViewReceived;
        public event ChangedEventHandler StatusChanged;
        public event ChangedEventHandler LandByStatusReceived;
        public event ChangedEventHandler LandReceived;
        public event ChangedEventHandler ConfirmationAdded;
        private readonly RestClient client;

        public LandRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
        }

        public void GetLandById(string landId)
        {
            var request = new RestRequest("land/"+ landId, Method.GET);

            client.ExecuteAsync<Land>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    LandReceived(response.Data, null)
                    ));
        }


        public void GetLandByWkt(string wktString)
        {
            var request = new RestRequest("land/intersect=" + wktString, Method.GET);
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


        public void UpdateStatus(int landId, LandStatus landStatus, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);

            var land = new Land { Id = landId, LandStatus = landStatus };
            var request = new RestRequest("land/" + landId.ToString(CultureInfo.InvariantCulture) + @"/updatestatus", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new JsonSerializer();

            request.AddBody(land);
            client.ExecuteAsync(request, response =>
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    StatusChanged(null, null)
            ));
        }


        public void Confirm(int landId, int userId, ConfirmationSort confirmationSort, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);

            var earthwatcher = new Earthwatcher { Id = userId };
            var request = new RestRequest("land/" + landId.ToString(CultureInfo.InvariantCulture) + @"/" + confirmationSort.ToString(), Method.PUT);
            request.AddBody(earthwatcher);
            request.RequestFormat = DataFormat.Json;
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
