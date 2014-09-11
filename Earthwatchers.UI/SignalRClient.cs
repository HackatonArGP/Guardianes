using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;




namespace Earthwatchers.UI
{
    public class SignalRClient
    {
        #region Fields and Properties
        private HubConnection _hubConnection;
        private IHubProxy _hubProxy;
        private string _url;

        public delegate void NotificationReceivedEventHandler(object sender, NotificationReceivedEventArgs e);
        public event NotificationReceivedEventHandler NotificationReceived;

        #endregion
        #region Constructors
        public SignalRClient(string baseUrl)
        {
            _url = baseUrl;
        }
        #endregion
        #region Methods
        public async void RunAsync()
        {
            _hubConnection = new HubConnection(_url);
            _hubConnection.Received += payload =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    OpenNotifications(payload);
                });
            };

            _hubProxy = _hubConnection.CreateHubProxy("hubs");
            await _hubConnection.Start();
        }

        private void OpenNotifications(string data)
        {
            NotificationReceived(this, new NotificationReceivedEventArgs { Data = FromJSon(data) });
        }

        public Notifications FromJSon(string _object)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Notifications>(_object);
        }
        #endregion
    }

    public class Notifications
    {
        public string H { get; set; }
        public string M { get; set; }
        public string[] A { get; set; }
    }

    public class NotificationReceivedEventArgs : System.EventArgs
    {
        public Notifications Data { get; set; }
    }
}
