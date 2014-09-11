using System;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace EarthWatchers.SL.Requests
{
    public class ImageRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler ImageRequestReceived;
        private readonly RestClient client;

        public ImageRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());
        }

        public void GetByExtent(string wkt)
        {
            var request = new RestRequest(@"satelliteimages/intersect=" + wkt, Method.GET);
            client.ExecuteAsync<List<SatelliteImage>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ImageRequestReceived(response.Data, null)
                    ));
        }

        public void GetAllImagery()
        {
            var request = new RestRequest("satelliteimages", Method.GET);
            client.ExecuteAsync<List<SatelliteImage>>(request, response =>
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    ImageRequestReceived(response.Data, null)
                    ));
        }
    }
}
