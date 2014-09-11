using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Earthwatchers.Models;
using Earthwatchers.Data;
using System.Data;
using Earthwatchers.Services.Security;

namespace Earthwatchers.Services
{
    [HubName("hubs")]
    public class Hubs : Hub
    {
        public void LandChanged(string hexCode, int earthwatcherId)
        {
            Clients.All.LandChanged(hexCode, earthwatcherId);
        }

        public void LandVerified(int earthwatcherId)
        {
            Clients.All.LandVerified(earthwatcherId);
        }

        public static List<string> Users = new List<string>();

        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
            context.Clients.All.updateUsersOnlineCount(count);
        }

        public override Task OnConnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            //TODO: Cuando dispongamos del servicio azure cambiar de lugar esta logica.
            IJaguarRepository repo = new JaguarRepository(System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
            List<JaguarGame> positions = repo.Get();
            var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
            var game = positions.FirstOrDefault(g => g.IsAvailable());
            if (game != null)
            {
                context.Clients.All.FindTheJaguar(game.Id);
            }
            else
            {
                context.Clients.All.FindTheJaguarFinished();
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnReconnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnDisconnected();
        }

        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }
    }

}