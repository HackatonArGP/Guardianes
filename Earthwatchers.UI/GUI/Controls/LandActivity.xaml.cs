using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.Models;
using System.Collections.Generic;
using Earthwatchers.UI.Requests;
using System;
using Earthwatchers.UI.Layers;
using System.Linq;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class LandActivity
    {
        private readonly LandRequests landRequests;

        public LandActivity(int landId)
        {
            InitializeComponent();

            landRequests = new LandRequests(Constants.BaseApiUrl);
            landRequests.ActivityReceived += landRequests_ActivityReceived;
            landRequests.GetActivity(landId);
        }

        void landRequests_ActivityReceived(object sender, EventArgs e)
        {
            this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
            var list = sender as List<Score>;

            var gp = list.SingleOrDefault(x => x.Action == Configuration.GreenpeaceName);
            if(gp != null)
            {
                list.Remove(gp);
                list.Insert(0, gp);
            }
            
            this.activityList.ItemsSource = list;
        }
    }
}

