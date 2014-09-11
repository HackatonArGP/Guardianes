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
using System.Collections.Generic;
using DonationsViewer.Models;
using System.Net.Browser;
using System.Globalization;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace DonationsViewer.Request
{
    public class LandRequest
    {       
        private List<DonationsLand> _land;
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler LandReceived;

        public LandRequest()
        {
            var httpResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
        }

        /// <summary>
        /// Starts requesting the database for a piece of land based on the id
        /// </summary>
        /// <param name="landId">id of the land in the database</param>
        public void GetLand()
        {
            var wc = new WebClient();
            wc.Headers[HttpRequestHeader.Accept] = "application/json";

            //TODO: random caching hack, find other way
            var random = new Random();
            wc.OpenReadAsync(new Uri(String.Format(CultureInfo.InvariantCulture, "{0}/{1}?rand={2}", Constants.ServiceUrl, "land", random.NextDouble())));
            wc.OpenReadCompleted += OnOpenLandCompleted;
        }    

        private void OnOpenLandCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                //with the serializer we can create a Land object
                var stream = e.Result;
                var sr = new StreamReader(stream);
                _land = Deserialize<List<DonationsLand>>(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            OnLandReceived(EventArgs.Empty);
        }

        public static T Deserialize<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        protected virtual void OnLandReceived(EventArgs e)
        {
            if (LandReceived != null)
                LandReceived(_land, e);
        }
    }
}
