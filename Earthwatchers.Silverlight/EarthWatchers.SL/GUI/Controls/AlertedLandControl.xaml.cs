using System;
using System.Windows.Input;
using System.Windows.Media;
using EarthWatchers.SL.Requests;
using Earthwatchers.Models;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class AlertedLandControl
    {
        private readonly Land           land;
        private Earthwatcher            earthwatcher;
        private EarthwatcherRequests     earthwatcherRequest;

        public AlertedLandControl(Land land)
        {
            InitializeComponent();
            this.land = land;
            Init();
        }

        private void Init()
        {
            earthwatcherRequest = new EarthwatcherRequests(Constants.BaseApiUrl);
            earthwatcherRequest.EarthwatcherReceived += EarthwatcherRequestEarthwatcherReceived;
            earthwatcherRequest.GetById(land.EarthwatcherId.ToString());
        }


        private void EarthwatcherRequestEarthwatcherReceived(object sender, EventArgs e)
        {
            earthwatcher = sender as Earthwatcher;
            txtCountry.Text = earthwatcher.Country;
            txtName.Text = string.Format("{0} on {1}", earthwatcher.FullName, land.StatusChangedDateTime.ToShortDateString());

            if (earthwatcher == null) 
                return;

        }

        private void LayoutRootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, land.GeohexKey);
        }

        private void LayoutRootMouseEnter(object sender, MouseEventArgs e)
        {
            bg.Fill = new SolidColorBrush(Color.FromArgb(255, 159, 181, 11));
        }

        private void LayoutRootMouseLeave(object sender, MouseEventArgs e)
        {
            bg.Fill = new SolidColorBrush(Color.FromArgb(38, 255, 255, 255));
        }
    }
}
