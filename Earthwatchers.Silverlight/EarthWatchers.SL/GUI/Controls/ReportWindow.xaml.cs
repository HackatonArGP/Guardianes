using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using Earthwatchers.Models;
using System.Windows.Controls;
using System.Windows.Media;
using EarthWatchers.SL.Requests;
using EarthWatchers.SL.Layers;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class ReportWindow
    {
        private Land selectedLand;
        private Earthwatcher selectedEarthwatcher;
        private readonly LandRequests landRequests;
        private readonly HexagonLayer hexagonLayer;

        public delegate void ConfirmationEndEventHandler(object sender, EventArgs e);
        public event ConfirmationEndEventHandler ConfirmationEnded;

        public ReportWindow(Land land, Earthwatcher earthwatcher)
        {
            InitializeComponent();

            selectedLand = land;

            string[] confirms = null;
            string[] deconfirms = null;

            if (!string.IsNullOrEmpty(selectedLand.DeforestationConfirmers))
            {
                confirms = selectedLand.DeforestationConfirmers.Split(',');
            }

            if (!string.IsNullOrEmpty(selectedLand.DeforestationDeconfirmers))
            {
                deconfirms = selectedLand.DeforestationDeconfirmers.Split(',');
            }

            if (Current.Instance.Earthwatcher.LandId.HasValue && Current.Instance.Earthwatcher.LandId == selectedLand.Id)
            {
                this.ReportGrid.Visibility = System.Windows.Visibility.Visible;
                this.ConfirmGrid.Visibility = System.Windows.Visibility.Collapsed;

                if (land.LandStatus == LandStatus.Alert)
                {
                    this.AlertButton.BorderThickness = new Thickness(4);
                    this.AlertButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
                }

                if (land.LandStatus == LandStatus.Ok)
                {
                    this.OkButton.BorderThickness = new Thickness(4);
                    this.OkButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
                }
            }
            else
            {
                this.Title.Text = "VALIDAR PARCELA EN ALERTA";
                this.ReportButton.Content = "VALIDAR";

                this.ReportGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.ConfirmGrid.Visibility = System.Windows.Visibility.Visible;

                if (confirms != null && confirms.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.ConfirmButton.BorderThickness = new Thickness(4);
                    this.ConfirmButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));

                    this.ConfirmButton.IsHitTestVisible = false;
                    this.ConfirmButton.Cursor = Cursors.Arrow;
                }

                if (deconfirms != null && deconfirms.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.DeconfirmButton.BorderThickness = new Thickness(4);
                    this.DeconfirmButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));

                    this.DeconfirmButton.IsHitTestVisible = false;
                    this.DeconfirmButton.Cursor = Cursors.Arrow;
                }
            }

            //asocia esto a algún mensaje que muestre confirmación / deconfirmación
            if (land.LandStatus == LandStatus.Alert)
            {
                int countConfirm = 0;
                int countDeconfirm = 0;

                if (confirms != null)
                {
                    countConfirm = confirms.Length;
                }

                if (deconfirms != null)
                {
                    countDeconfirm = deconfirms.Length;
                }

                this.countConfirm1.Text = string.Format("{0} confirmaciones", countConfirm);
                this.countConfirm2.Text = string.Format("{0} confirmaciones", countConfirm);
                this.countDeConfirm1.Text = string.Format("{0} nada sospechoso", countDeconfirm);
                this.countDeConfirm2.Text = string.Format("{0} nada sospechoso", countDeconfirm);
            }
            else
            {
                this.countConfirm1.Visibility = System.Windows.Visibility.Collapsed;
                this.countConfirm2.Visibility = System.Windows.Visibility.Collapsed;
                this.countDeConfirm1.Visibility = System.Windows.Visibility.Collapsed;
                this.countDeConfirm2.Visibility = System.Windows.Visibility.Collapsed;
            }

            selectedEarthwatcher = earthwatcher;
            landRequests = new LandRequests(Constants.BaseApiUrl);
            hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);

            this.Loaded += ReportWindow_Loaded;
        }

        void ReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Text = selectedEarthwatcher.FullName;
            txtCountry.Text = selectedEarthwatcher.Country;

            if (Current.Instance.TutorialStarted)
            {
                this.Overlay.Visibility = System.Windows.Visibility.Visible;
                this.Tutorial1StoryBoard.Begin();
            }

            if (Current.Instance.Tutorial2Started)
            {
                this.Overlay.Visibility = System.Windows.Visibility.Visible;
                this.Tutorial21StoryBoard.Begin();
            }
        }

        private void Step1Click(object sender, RoutedEventArgs e)
        {
            this.Tutorial2StoryBoard.Begin();
        }

        private void Step2Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Step21Click(object sender, RoutedEventArgs e)
        {
            this.Tutorial22StoryBoard.Begin();
        }

        private void Step22Click(object sender, RoutedEventArgs e)
        {
            this.Tutorial23StoryBoard.Begin();
        }

        private void Step23Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        LandStatus status;
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;

            if (Current.Instance.Earthwatcher.LandId.HasValue)
            {
                if (Current.Instance.Earthwatcher.LandId == selectedLand.Id)
                {
                    int statusNumber = 0;
                    if (this.AlertButton.BorderThickness.Left > 0)
                    {
                        statusNumber = 4;
                    }

                    if (this.OkButton.BorderThickness.Left > 0)
                    {
                        statusNumber = 3;
                    }

                    if (statusNumber == 0)
                    {
                        statusNumber = 2; //Not Checked
                    }

                    status = (LandStatus)statusNumber;

                    if (selectedLand.LandStatus != status)
                    {
                        closeWindow = false;
                        landRequests.StatusChanged += SetLandStatusStatusChanged;
                        landRequests.UpdateStatus(selectedLand.Id, status, Current.Instance.Username, Current.Instance.Password);

                        Current.Instance.EarthwatcherLand.LandStatus = status;
                    }
                }
                else
                {
                    ConfirmationSort confirmationSort = ConfirmationSort.Confirm;
                    bool hasAction = false;
                    if (this.ConfirmButton.BorderThickness.Left > 0)
                    {
                        if (!selectedLand.DeforestationConfirmers.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                        {
                            confirmationSort = ConfirmationSort.Confirm;
                            hasAction = true;
                        }
                    }

                    if (this.DeconfirmButton.BorderThickness.Left > 0)
                    {
                        if (!selectedLand.DeforestationDeconfirmers.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                        {
                            confirmationSort = ConfirmationSort.Deconfirm;
                            hasAction = true;
                        }
                    }

                    if (hasAction)
                    {
                        closeWindow = false;

                        landRequests.ConfirmationAdded += landRequests_ConfirmationAdded;
                        landRequests.Confirm(selectedLand.Id, Current.Instance.Earthwatcher.Id, confirmationSort, Current.Instance.Username, Current.Instance.Password);
                    }
                }
            }

            if (closeWindow)
            {
                this.Close();
            }
        }

        void landRequests_ConfirmationAdded(object sender, EventArgs e)
        {
            if ((ConfirmationSort)sender == ConfirmationSort.Confirm)
            {
                if (!string.IsNullOrEmpty(selectedLand.DeforestationConfirmers))
                {
                    selectedLand.DeforestationConfirmers += ",";
                }
                selectedLand.DeforestationConfirmers += Current.Instance.Earthwatcher.Id.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedLand.DeforestationDeconfirmers))
                {
                    selectedLand.DeforestationDeconfirmers += ",";
                }
                selectedLand.DeforestationDeconfirmers += Current.Instance.Earthwatcher.Id.ToString();
            }

            if (!Current.Instance.Scores.Any(x => x.Action.Equals("ConfirmationAdded") && x.Published > selectedLand.LastReset))
            {
                Current.Instance.AddScore.Add("ConfirmationAdded", 500);
            }

            var topLeft = Current.Instance.MapControl.Viewport.Extent.TopLeft;
            var bottomLeft = Current.Instance.MapControl.Viewport.Extent.BottomLeft;
            var bottomRight = Current.Instance.MapControl.Viewport.Extent.BottomRight;
            var topRight = Current.Instance.MapControl.Viewport.Extent.TopRight;

            var llTopLeft = SphericalMercator.ToLonLat(topLeft.X, topLeft.Y);
            var llbottomLeft = SphericalMercator.ToLonLat(bottomLeft.X, bottomLeft.Y);
            var llbottonRight = SphericalMercator.ToLonLat(bottomRight.X, bottomRight.Y);
            var llTopRight = SphericalMercator.ToLonLat(topRight.X, topRight.Y);

            var wkt = String.Format(CultureInfo.InvariantCulture, "POLYGON(({0} {1},{2} {3},{4} {5},{6} {7},{0} {1}))", llTopLeft.x, llTopLeft.y, llbottomLeft.x, llbottomLeft.y, llbottonRight.x, llbottonRight.y, llTopRight.x, llTopRight.y);

            landRequests.LandInViewReceived += LivLandInViewReceived;
            landRequests.GetLandByWkt(wkt);
        }

        private void LivLandInViewReceived(object sender, EventArgs e)
        {
            var landPieces = sender as List<Land>;
            Current.Instance.LandInView = landPieces;
            ConfirmationEnded(this, EventArgs.Empty);

            this.Close();
        }

        private void SetLandStatusStatusChanged(object sender, EventArgs e)
        {
            if (status == LandStatus.Alert || status == LandStatus.Ok)
            {
                if (!Current.Instance.Scores.Any(x => x.Action.Equals("LandStatusChanged") && x.Published > selectedLand.LastReset))
                {
                    Current.Instance.AddScore.Add("LandStatusChanged", 500);
                }
            }

            var alertedLandLayer = Current.Instance.LayerHelper.FindLayer(Constants.AlertedLandLayername) as AlertedLandLayer;
            if (alertedLandLayer != null)
                alertedLandLayer.RequestAlertedAreas();

            hexagonLayer.UpdateHexagonsInView();

            this.Close();
        }

        private void Demand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TODO:
        }

        private void Action_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                bool ischecked = false;
                if (border.BorderThickness.Left > 0)
                {
                    border.BorderThickness = new Thickness(0);
                    border.Background = new SolidColorBrush(Colors.Transparent);
                }
                else
                {
                    ischecked = true;
                    border.BorderThickness = new Thickness(4);
                    border.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
                }

                if (border.Name.Equals("OkButton") && ischecked)
                {
                    this.AlertButton.BorderThickness = new Thickness(0);
                    this.AlertButton.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (border.Name.Equals("AlertButton") && ischecked)
                {
                    this.OkButton.BorderThickness = new Thickness(0);
                    this.OkButton.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (border.Name.Equals("ConfirmButton") && ischecked)
                {
                    this.DeconfirmButton.BorderThickness = new Thickness(0);
                    this.DeconfirmButton.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (border.Name.Equals("DeconfirmButton") && ischecked)
                {
                    this.ConfirmButton.BorderThickness = new Thickness(0);
                    this.ConfirmButton.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        private void Twitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(string.Format("http://twitter.com/share?text={0}", shareText.Text), UriKind.Absolute), "_blank");
        }

        private void Facebook_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(string.Format("http://www.facebook.com/sharer.php?u={0}", "http://bit.ly/13iKeEq"), UriKind.Absolute), "_blank");
        }
    }
}

