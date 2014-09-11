using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class NotificationsWindow
    {
        public delegate void LandReassignedEventHandler(object sender, EventArgs e);
        public event LandReassignedEventHandler LandReassigned;

        public delegate void LandVerifiedEventHandler(object sender, EventArgs e);
        public event LandVerifiedEventHandler LandVerified;

        private readonly ScoreRequests scoreRequests;

        private string _action;
        public NotificationsWindow(string action)
        {
            InitializeComponent();

            _action = action;

            scoreRequests = new ScoreRequests(Constants.BaseApiUrl);
            scoreRequests.ServerDateTimeReceived += scoreRequests_ServerDateTimeReceived;

            //Logo
            this.logo.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", Labels.LogoPath));

            if (action.Equals("DemandAuthoritiesApproved"))
            {
                this.Title.Text = Labels.Notifications3;
                this.Body.Text = Labels.Notifications5;
            }

            if (action.Equals("NewLand"))
            {
                this.Title.Text = Labels.Notifications6;
                this.Body.Text = Labels.Notifications7;
            }

            if (action.Equals(ActionPoints.Action.LandReassigned.ToString()))
            {
                //Veo el DateTime del server
                scoreRequests.GetServerTime();
            }

            if (action.Equals("ChangeLand"))
            {
                this.Title.Text = Labels.Notifications13;
                this.Body.Text = Labels.Notifications14;
                this.ButtonClose.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonContinue.Visibility = System.Windows.Visibility.Visible;
            }

            if (action.Equals(ActionPoints.Action.LandVerified.ToString()))
            {
                this.Title.Text = Labels.Notifications3;
                this.Body.Text = Labels.Notifications15;
                this.ButtonClose.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonContinue.Visibility = System.Windows.Visibility.Visible;
            }

            if (action.Equals(ActionPoints.Action.LandVerifiedInformed.ToString()))
            {
                this.Title.Text = Labels.Notifications3;
                this.Body.Text = Labels.Notifications16;
                this.ButtonClose.Visibility = System.Windows.Visibility.Visible;
                this.ButtonContinue.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void scoreRequests_ServerDateTimeReceived(object sender, EventArgs e)
        {
            Score score = sender as Score;
            if (Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action.StartsWith(ActionPoints.Action.LandReassigned.ToString()) && x.Published > score.Published.AddMinutes(-10)))
            {
                
                this.Title.Text = Labels.Notifications8;
                this.Body.Text = Labels.Notifications9;
                this.ButtonClose.Content = Labels.Notifications10;
            }
            else
            {
                this.Title.Text = Labels.Notifications8;
                this.Body.Text = Labels.Notifications11;

                this.ReassignButtons.Visibility = System.Windows.Visibility.Visible;
                this.ButtonClose.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void earthwatcherRequest_LandReassigned(object sender, EventArgs e)
        {
            try
            {
                Land land = sender as Land;
                if (land != null)
                {
                    //TODO: ver como queda esto después de los multiple plots
                    var oldLand = Current.Instance.Lands.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id).FirstOrDefault();
                    if (oldLand != null)
                    {
                        //Si no está en amarillo o en verde la saco 
                        if (oldLand.LandStatus != LandStatus.Alert && oldLand.LandStatus != LandStatus.Ok)
                        {
                            Current.Instance.Lands.Remove(oldLand);
                        }
                        else
                        {   //Si es amarilla o verde la paso a greenpeace 
                            oldLand.EarthwatcherId = Configuration.GreenpeaceId;
                            oldLand.EarthwatcherName = Configuration.GreenpeaceName;
                            oldLand.IsPowerUser = true;
                        }
                        //Saco del current instance del Ew la land que quiero cambiar
                        var oldEarthwatcherLand = Current.Instance.Earthwatcher.Lands.Where(x => x.Id == oldLand.Id).FirstOrDefault();
                        Current.Instance.Earthwatcher.Lands.Remove(oldEarthwatcherLand);
                    }
                    //si la land que me asignaron no esta en el current instance de lands, la agrega y se la agrega al Ew
                    if (!Current.Instance.Lands.Any(x => x.Id == land.Id))
                    {
                        Current.Instance.Lands.Add(land);
                        Current.Instance.Earthwatcher.Lands.Add(land);
                    }

                    if (_action.Equals(ActionPoints.Action.LandVerified.ToString()))
                    {
                        LandVerified(land, EventArgs.Empty);
                    }
                    else
                    {// Recibe el evento en mainpage y le asigna el puntaje
                        LandReassigned(land, EventArgs.Empty);
                    }

                    var hexagonLayer = Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername) as Earthwatchers.UI.Layers.HexagonLayer;

                    if (hexagonLayer != null)
                        hexagonLayer.AddHexagon(land);

                    this.Title.Text = Labels.Notifications3;
                    this.Body.Text = Labels.Notifications12;

                    MapHelper.ZoomToHexagon(Current.Instance.MapControl, land.GeohexKey);
                }
            }
            finally {
                this.ReassignButtons.Visibility = System.Windows.Visibility.Collapsed;
                this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonContinue.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonClose.Visibility = System.Windows.Visibility.Visible;
            }
        }

        //Button Yes
        private void ReassignLandClick(object sender, RoutedEventArgs e)
        {
            ChangeLand();
        }

        void ChangeLand()
        {
            this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
            this.ReassignButtons.IsHitTestVisible = false;
            EarthwatcherRequests earthwatcherRequest = new EarthwatcherRequests(Constants.BaseApiUrl);
            earthwatcherRequest.LandReassigned += earthwatcherRequest_LandReassigned;

            //TODO: cuando este funcionando multiple plots hay que especificar cual reasigno
            earthwatcherRequest.ReassignLand(Current.Instance.Earthwatcher.Id);
            //earthwatcherRequest.ReassignLand(Current.Instance.Earthwatcher.Lands.First());  PAITO
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            ChangeLand();
        }
    }
}

