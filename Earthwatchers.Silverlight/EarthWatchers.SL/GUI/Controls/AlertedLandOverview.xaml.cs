using System.Collections.Generic;
using EarthWatchers.SL.Requests;
using Earthwatchers.Models;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class AlertedLandOverview
    {
        private LandRequests landRequest;
        private int alertedLandAmountToShow = 20;

        public AlertedLandOverview()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            landRequest = new LandRequests(Constants.BaseApiUrl);
            landRequest.LandByStatusReceived += LandRequestLandByStatusReceived;
            landRequest.GetLandByStatus(LandStatus.Alert);
        }

        private void LandRequestLandByStatusReceived(object sender, System.EventArgs e)
        {
            var alertedLand = (List<Land>)sender;
            
            if (alertedLand == null)
                return;

            if (alertedLand.Count < alertedLandAmountToShow)
                alertedLandAmountToShow = alertedLand.Count;

            for (var i = 0; i < alertedLandAmountToShow; i++)
            {
                panel.Children.Add(new AlertedLandControl(alertedLand[i]));
            }
        }
    }
}
