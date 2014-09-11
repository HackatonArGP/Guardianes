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
    public class CommentRequests
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler CommentsByLandReceived;
        private readonly RestClient client;

        public CommentRequests(string url)
        {
            client = new RestClient(url);
            client.ClearHandlers();
            client.AddHandler("application/json", new JsonDeserializer());

            client.CookieContainer = RequestsHelper.GetCookieContainer();
        }

        public void GetCommentsByLand(int landId)
        {
            var request = new RestRequest(@"comments/land=" + landId, Method.GET) {RequestFormat = DataFormat.Json};
            client.ExecuteAsync<List<Comment>>(request, response => 
                Deployment.Current.Dispatcher.BeginInvoke(() => 
                    CommentsByLandReceived(response.Data, null)
                    ));
        }

        public void Delete(Comment comment, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("comments/del", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(comment);
            client.ExecuteAsync(request, response => { });
        }


        public void Post(Comment comment, string username, string password)
        {
            client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest("comments", Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new JsonSerializer();
            request.AddBody(comment);
            client.ExecuteAsync(request, response => {});
        }
    }
}
