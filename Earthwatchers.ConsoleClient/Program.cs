using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.IO;
using EarthWatchers.SL.Post;
using Newtonsoft.Json;
using RestSharp;

namespace Earthwatchers.ConsoleClient
{
    class Program
    {
        private const string Server = "http://earthwatchers.cloudapp.net/api/";

        static void Main()
        {
            // get land by earthwatchername
            GetLandByEarthwatcher("");

            // add new earthwatcher and assign land
            const string newEarthwatcher = "Batir";
            Console.Write("Land created for earthwatcher" + newEarthwatcher + ": " +
                AddNewEarthwatcherHttpClient(newEarthwatcher, "", ""));
        }

        // client using restsharp, sample for http post with authentication
        private static bool AddNewEarthwatcherRestSharp(string tigUser, string username, string password)
        {
            dynamic tigmember = new ExpandoObject ();
            tigmember.Name=tigUser;
            var client = new RestClient(Server) {Authenticator = new HttpBasicAuthenticator(username, password)};
            var request = new RestRequest("earthwatchers", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(tigmember);
            var response=client.Execute(request);
            return response.StatusCode == HttpStatusCode.Created;
        }

        // client using httpclient, sample for http post with authentication
        private static bool AddNewEarthwatcherHttpClient(string tigUser, string username, string password)
        {
            var httpClient = new HttpClient();
            dynamic tigmember = new ExpandoObject();
            tigmember.Name = tigUser;
            var json=JsonConvert.SerializeObject(tigmember);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var authstring=Base64Helper.ToBase64String(string.Format("{0}:{1}", username,password));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authstring);
            // send the request
            bool isSuccess = httpClient.Post(Server + "earthwatchers", content).IsSuccessStatusCode;
            return isSuccess;
        }

        /// <summary>
        /// Get land by earthwatcher
        /// </summary>
        /// <param name="tigUser"></param>
        private static void GetLandByEarthwatcher(string tigUser)
        {
            // send a http get request
            var httpClient = new HttpClient();
            var response=httpClient.Get(Server+ "land/earthwatcher=" + tigUser).Content.ReadAsString();
            
            // parse xml
            var xdoc = XDocument.Load(new StringReader(response));
            var land=(from d in xdoc.Elements("Land") select d).FirstOrDefault();
            if (land != null)
            {
                // print the result
                if (land.Element("Id") != null)
                {
                    Console.Write("User " + tigUser + " has land id: " + land.Element("Id").Value);
                }
                else
                {
                    Console.Write("User " + tigUser + " has no land yet");
                }
            }
        }
    }
}
