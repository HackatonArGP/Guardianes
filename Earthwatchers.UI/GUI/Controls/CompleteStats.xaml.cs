using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class CompleteStats
    {
        private readonly LandRequests landRequest;
        private List<Statistic> stats;

        public CompleteStats()
        {
            InitializeComponent();
            landRequest = new LandRequests(Constants.BaseApiUrl);
            landRequest.StatsReceived += landRequest_StatsReceived;
            landRequest.GetStats();
        }

        int usersOnlineNow = 0;
        public void UpdateOnlineUsers(int userCount)
        {
            usersOnlineNow = userCount;

            RenderOnlineUsers();
        }

        private void RenderOnlineUsers()
        {
            if (stats == null || stats.Count == 0)
            {
                landRequest.GetStats();
            }
            else
            {
                this.AddOtherStats();
                this.AssignValues();
            }
        }

        private void AddOtherStats()
        {
            if (stats != null && usersOnlineNow > 0)
            {
                var onlineUserStat = stats.Where(x => x.Name == "StatsUsersOnline").FirstOrDefault();
                if (onlineUserStat != null)
                {
                    onlineUserStat.Number = usersOnlineNow;
                }
                else
                {
                    stats.Add(new Statistic { Name = "StatsUsersOnline", Number = usersOnlineNow, ShowOrder = 1 });
                }
            }
        }

        void landRequest_StatsReceived(object sender, EventArgs e)
        {
            stats = sender as List<Statistic>;
            if(stats != null && this.Visibility == Visibility.Visible)
            {
                this.AddOtherStats();
                this.AssignValues();
            }
        }


        public void AssignValues()
        {
            var stat = stats.FirstOrDefault(x => x.Name == "StatsActivePlayers");
            //if (stat != null)
            //    StatsActivePlayers.Text = stat.Number.ToString();

            stat = stats.FirstOrDefault(x => x.Name == "StatsTotalPlayers");
            if (stat != null)
                StatsTotalPlayers.Text = stat.Number.ToString();

            //stat = stats.FirstOrDefault(x => x.Name == "StatsUsersOnline");
            //if (stat != null)
            //    StatsUsersOnline.Text = stat.Number.ToString();

            //stat = stats.FirstOrDefault(x => x.Name == "StatsGreenPlots");
            //if (stat != null)
            //    StatsGreenPlots.Text = stat.Number.ToString();

            //stat = stats.FirstOrDefault(x => x.Name == "StatsRedPlots");
            //if (stat != null)
            //    StatsRedPlots.Text = stat.Number.ToString();

            stat = stats.FirstOrDefault(x => x.Name == "StatsVerifiedPlots");
            if (stat != null)
                StatsVerifiedPlots.Text = stat.Number.ToString();

            stat = stats.FirstOrDefault(x => x.Name == "StatsAlertedAreaConfirmed");
            if (stat != null)
                StatsAlertedAreaConfirmed.Text = stat.Number.ToString() + " km2";

            stat = stats.FirstOrDefault(x => x.Name == "StatsDenouncesCreated");
            if (stat != null)
                StatsDenouncesCreated.Text = stat.Number.ToString();

            //stat = stats.FirstOrDefault(x => x.Name == "StatsAlertedArea");
            //if (stat != null)
            //    StatsAlertedArea.Text = stat.Number.ToString() + " km2";

            

            this.loadinAnim.Visibility = Visibility.Collapsed;
            this.MainGrid.Visibility = Visibility.Visible;
        }

    }
}

