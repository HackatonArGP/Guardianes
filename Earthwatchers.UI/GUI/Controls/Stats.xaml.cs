using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
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
using System.Windows.Threading;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class Stats : UserControl
    {
        private readonly LandRequests landRequest; //test
        private List<Statistic> stats;
        private DispatcherTimer myTimer;
        private System.Resources.ResourceManager rm;

        public Stats()
        {
            InitializeComponent();
            landRequest = new LandRequests(Constants.BaseApiUrl);
            landRequest.StatsReceived += landRequest_StatsReceived;
            this.FadeOut1.Completed += FadeOut1_Completed;
            this.FadeOut2.Completed += FadeOut2_Completed;
            this.FadeOut3.Completed += FadeOut3_Completed;

        }

        public void Show()
        {
            rm = new System.Resources.ResourceManager(typeof(Earthwatchers.UI.Resources.Labels));

            landRequest.GetStats();
        }

        void landRequest_StatsReceived(object sender, EventArgs e)
        {
            stats = sender as List<Statistic>;
            RenderOnlineUsers();

            if (isFirstTime)
            {
                this.StartStoryBoard.Begin();

                myTimer = new DispatcherTimer();
                myTimer.Interval = TimeSpan.FromMilliseconds(15000); // 15 segundos
                myTimer.Tick += myTimer_Tick;
                myTimer.Start();
            }

            this.HighlightStoryBoard.Begin();
            RotateData();
        }

        int seconds = 0;
        bool isFirstTime = true;
        void myTimer_Tick(object sender, EventArgs e)
        {
            seconds += 15;

            //Roto los datos
            RotateData();

            if (seconds == 600)
            {
                //Refresco los datos del server cada 10 minutos
                landRequest.GetStats();
            }
        }

        int usersOnlineNow = 0;
        public void UpdateOnlineUsers(string userCount)
        {
            usersOnlineNow = Convert.ToInt32(userCount);

            RenderOnlineUsers();
        }
        CompleteStats sta;
        private void RenderOnlineUsers()
        {
            sta = new CompleteStats();

            //if (stats != null && usersOnlineNow > 0)
            //{
            //    var onlineUserStat = stats.Where(x => x.Name == "StatsUsersOnline").FirstOrDefault();
            //    if (onlineUserStat != null)
            //    {
            //        onlineUserStat.Number = usersOnlineNow;
            //    }
            //    else
            //    {
            //        stats.Add(new Statistic { Name = "StatsUsersOnline", Number = usersOnlineNow, ShowOrder = 1 });
            //    }
            //}
            sta.UpdateOnlineUsers(usersOnlineNow);
        }

        int showOrder = 1;
        int prevShowOrder = 1;
        void RotateData()
        {
            if (stats == null || stats.Count == 0)
                return;

            prevShowOrder = showOrder;

            if (showOrder > 1)
            {
                if (this.LandStats1.Visibility == System.Windows.Visibility.Visible)
                {
                    this.FadeOut1.Begin();
                }

                if (this.LandStats2.Visibility == System.Windows.Visibility.Visible)
                {
                    this.FadeOut2.Begin();
                }

                if (this.LandStats3.Visibility == System.Windows.Visibility.Visible)
                {
                    this.FadeOut3.Begin();
                }
            }
            else
            {
                int count = stats.Where(x => x.ShowOrder == showOrder).Count();
                if (count > 0)
                {
                    this.FadeOut1.Begin();
                }

                if (count > 1)
                {
                    this.FadeOut2.Begin();
                }

                if (count > 2)
                {
                    this.FadeOut3.Begin();
                }
            }

            showOrder++;
            if (!stats.Any(x => x.ShowOrder == showOrder))
            {
                showOrder = 1;
            }
        }

        void FadeOut3_Completed(object sender, EventArgs e)
        {
            var stat = stats.Where(x => x.ShowOrder == prevShowOrder).Skip(2).FirstOrDefault();
            if (stat != null)
            {
                this.LandStats3.Visibility = System.Windows.Visibility.Visible;
                this.LandStats31.Text = string.Format("{0}:", rm.GetString(stat.Name));
                this.LandStats32.Text = string.Format(" {0}{1}{2}", stat.Number, !string.IsNullOrEmpty(stat.UOM) ? " " + stat.UOM : string.Empty, stat.Percentage > 0 ? " / " + stat.Percentage.ToString("P1") : string.Empty);
            }
            else
            {
                this.LandStats3.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.FadeIn3.Begin();
        }

        void FadeOut2_Completed(object sender, EventArgs e)
        {
            var stat = stats.Where(x => x.ShowOrder == prevShowOrder).Skip(1).FirstOrDefault();
            if (stat != null)
            {
                this.LandStats2.Visibility = System.Windows.Visibility.Visible;
                this.LandStats21.Text = string.Format("{0}:", rm.GetString(stat.Name));
                this.LandStats22.Text = string.Format(" {0}{1}{2}", stat.Number, !string.IsNullOrEmpty(stat.UOM) ? " " + stat.UOM : string.Empty, stat.Percentage > 0 ? " / " + stat.Percentage.ToString("P1") : string.Empty);
            }
            else
            {
                this.LandStats2.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.FadeIn2.Begin();
        }

        void FadeOut1_Completed(object sender, EventArgs e)
        {
            var stat = stats.Where(x => x.ShowOrder == prevShowOrder).FirstOrDefault();
            if (stat != null)
            {
                this.LandStats1.Visibility = System.Windows.Visibility.Visible;
                this.LandStats11.Text = string.Format("{0}:", rm.GetString(stat.Name));
                this.LandStats12.Text = string.Format(" {0}{1}{2}", stat.Number, !string.IsNullOrEmpty(stat.UOM) ? " " + stat.UOM : string.Empty, stat.Percentage > 0 ? " / " + stat.Percentage.ToString("P1") : string.Empty);
            }
            else
            {
                this.LandStats1.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.FadeIn1.Begin();
        }

        private void Clicked(object sender, MouseButtonEventArgs e)
        {
            sta.UpdateOnlineUsers(usersOnlineNow);
            sta.Show();
        }
    }
}
