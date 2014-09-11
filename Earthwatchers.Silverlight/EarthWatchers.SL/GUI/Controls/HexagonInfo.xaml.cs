using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Windows.Browser;
using System.Windows.Controls;
using Earthwatchers.Models;
using EarthWatchers.SL.GUI.Controls.Comments;
using EarthWatchers.SL.Layers;
using EarthWatchers.SL.Requests;
using Mapsui.Providers;
using Mapsui.Windows;
using System.Collections.ObjectModel;
using EarthWatchers.SL.Resources;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class HexagonInfo
    {
        private readonly EarthwatcherRequests    earthwatcherRequest;
        private readonly LandRequests landRequests;

        private readonly CommentRequests commentRequests;
        private readonly HexagonLayer           hexagonLayer;
        private ObservableCollection<Comment>   comments;
        private Earthwatcher                    selectedEarthwatcher;
        private Land                            selectedLand;
        private Boolean                         isShown;

        public delegate void ReportWindowClosedEventHandler(object sender, EventArgs e);
        public event ReportWindowClosedEventHandler ReportWindowClosed;

        public HexagonInfo()
        {
            InitializeComponent();
            earthwatcherRequest = new EarthwatcherRequests(Constants.BaseApiUrl);
            commentRequests = new CommentRequests(Constants.BaseApiUrl);
            landRequests = new LandRequests(Constants.BaseApiUrl);
            hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);

            //Add event listeners
            commentRequests.CommentsByLandReceived += CommentRequestCommentsByLandReceived;
            earthwatcherRequest.EarthwatcherReceived += EarthwatcherChanged;
            Current.Instance.MapControl.zoomFinished += MapControlZoomFinished;
            Current.Instance.MapControl.zoomStarted += MapControlZoomStarted;

            HtmlPage.RegisterScriptableObject("HexagonInfo", this);
        }

        [ScriptableMember]
        public void UpdateText(string result)
        {
            MessageBox.Show(result);
        }

        private void CommentRequestCommentsByLandReceived(object sender, EventArgs e)
        {
            comments = new ObservableCollection<Comment>(sender as List<Comment>);
            comments.CollectionChanged += comments_CollectionChanged;
            if (comments != null)
            {
                NumberOfCommentsText.Text = comments.Count.ToString();
                commentList.ItemsSource = comments.OrderByDescending(x => x.Published).Take(2);
            }
        }

        void comments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NumberOfCommentsText.Text = comments.Count.ToString();
            commentList.ItemsSource = comments.OrderByDescending(x => x.Published).Take(2);
        }

        public Feature GetFeature(double longitude, double latitude, int level)
        {
            //Get the hexcode for the clicked area and try to find if its a feature on the map
            var hexCode = GeoHex.Encode(longitude, latitude, level);
            var feature = hexagonLayer.GetFeatureByHex(hexCode);
            return feature;
        }


        public void ShowInfo(double lon, double lat)
        {
            // first try on level 7...
            var feature = GetFeature(lon, lat, 7);
            var hexCode = GeoHex.Encode(lon, lat, 7);
            if (feature == null)
            {
                // try on level 6...
                hexCode = GeoHex.Encode(lon, lat, 6);
                feature= GetFeature(lon, lat, 6);
                if (feature == null) return;
            }

            UpdateInfo(hexCode, false);
            Move();
            isShown = true;
            Visibility = Visibility.Visible;
        }

        private void UpdateInfo(string hexCode, bool isRefresh)
        {
            if (selectedLand == null || !selectedLand.GeohexKey.Equals(hexCode) || isRefresh)
            {
                if (Current.Instance.EarthwatcherLand != null &&
                    Current.Instance.EarthwatcherLand.GeohexKey.Equals(hexCode))
                {
                    selectedLand = Current.Instance.EarthwatcherLand;
                    selectedEarthwatcher = Current.Instance.Earthwatcher;

                    txtName.Text = selectedEarthwatcher.FullName;
                    txtCountry.Text = selectedEarthwatcher.Country;

                    //this.reportIcon.Source
                    this.reportText.Text = "REPORTAR";
                    this.ActionButton.Visibility = System.Windows.Visibility.Visible;
                    this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/reportar.png");
                }
                else
                {
                    foreach (var land in Current.Instance.LandInView)
                    {
                        if (land.GeohexKey.Equals(hexCode))
                            selectedLand = land;
                    }

                    if (selectedLand == null)
                        return;

                    if (selectedLand.LandStatus == LandStatus.Alert)
                    {
                        this.ActionButton.Visibility = System.Windows.Visibility.Visible;
                        this.reportText.Text = "REVISAR";
                        this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/revisar.png"); 
                    }
                    else
                    {
                        this.ActionButton.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    earthwatcherRequest.GetById(selectedLand.EarthwatcherId.ToString());
                }

                commentRequests.GetCommentsByLand(selectedLand.Id);
            }

            /*
            //A new zone is clicked request data
            if (selectedLand == null || !selectedLand.GeohexKey.Equals(hexCode) || isRefresh)
            {
                ClearInfo();

                //If the zone is the zone of the logged in user););
                if (Current.Instance.EarthwatcherLand != null &&
                    Current.Instance.EarthwatcherLand.GeohexKey.Equals(hexCode))
                {
                    unassignedGrid.Visibility = Visibility.Collapsed;
                    confirmation.Visibility = Visibility.Collapsed;
                    statusChanger.Visibility = Visibility.Visible;

                    selectedLand = Current.Instance.EarthwatcherLand;
                    selectedEarthwatcher = Current.Instance.Earthwatcher;

                    commentRequests.GetCommentsByLand(selectedLand.Id);

                    var selectedIndex = 0;
                    if (((int) selectedLand.LandStatus) - 2 > -1) //-2 cause first 2 can not be set by user
                        selectedIndex = (int) selectedLand.LandStatus - 2;

                    cbSetStatus.SelectedIndex = selectedIndex;
                    txtStatus.Text = selectedLand.LandStatus.ToString();
                
                    txtName.Text = selectedEarthwatcher.FullName;
                    txtCountry.Text = selectedEarthwatcher.Country;
                }
                else
                {
                    foreach (var land in Current.Instance.LandInView)
                    {
                        if (land.GeohexKey.Equals(hexCode))
                            selectedLand = land;
                    }

                    if (selectedLand == null)
                        return;

                    //If there is no user attached to the land
                    if (selectedLand.LandStatus == LandStatus.Unwatched ||
                        selectedLand.LandStatus == LandStatus.Unassigned)
                    {
                        unassignedGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        unassignedGrid.Visibility = Visibility.Collapsed;
                        statusChanger.Visibility = Visibility.Collapsed;

                        if (selectedLand.LandStatus == LandStatus.Alert && Current.Instance.Earthwatcher != null)
                            confirmation.Visibility = Visibility.Visible;
                        if (selectedLand.LandStatus != LandStatus.Alert || Current.Instance.Earthwatcher == null)
                            confirmation.Visibility = Visibility.Collapsed;

                        commentRequests.GetCommentsByLand(selectedLand.Id);
                        earthwatcherRequest.GetById(selectedLand.EarthwatcherId.ToString());
                    }
                }

                if (selectedLand.LandStatus == LandStatus.Alert)//if alerted show confirmers deconfirmers
                {
                    txtConfirmed.Visibility = Visibility.Visible;
                    txtDeconfirmed.Visibility = Visibility.Visible;
                    
                    var countConfirm = selectedLand.DeforestationConfirmers.Split(',');
                    var countDeconfirm = selectedLand.DeforestationDeconfirmers.Split(',');

                    if (countConfirm[0].Equals(""))
                        countConfirm = new string[0];
                    if (countDeconfirm[0].Equals(""))
                        countDeconfirm = new string[0];

                    txtConfirmed.Text = "Confirmed: " + countConfirm.Length;
                    txtDeconfirmed.Text = "Deconfirmed: " + countDeconfirm.Length;
                }
            }
             * */
        }

        private void ClearInfo()
        {
            txtName.Text = "";
            txtCountry.Text = "";
            /*
            txtStatus.Text = "";
            flag.Source = null;
            txtConfirmed.Visibility = Visibility.Collapsed;
            txtDeconfirmed.Visibility = Visibility.Collapsed;
            */
        }

        private void EarthwatcherChanged(object sender, EventArgs e)
        {
            selectedEarthwatcher = sender as Earthwatcher;

            if (selectedEarthwatcher == null)
                return;

            //txtStatus.Text = selectedLand.LandStatus.ToString();
            txtName.Text = selectedEarthwatcher.FullName;
            txtCountry.Text = selectedEarthwatcher.Country;
        }

        private void SetLandStatusStatusChanged(object sender, EventArgs e)
        {
            /*
            var txtBlock = cbSetStatus.SelectedItem as TextBlock;
            if (txtBlock != null)
                txtStatus.Text = txtBlock.Text;

            var alertedLandLayer = Current.Instance.LayerHelper.FindLayer(Constants.AlertedLandLayername) as AlertedLandLayer;
            if (alertedLandLayer != null) 
                alertedLandLayer.RequestAlertedAreas();

            hexagonLayer.UpdateHexagonsInView();
             * */
        }

        private void MapControlZoomFinished(object sender, EventArgs e)
        {
            if (!isShown)
                return;

            Visibility = Visibility.Visible;
            Move();
        }

        private void MapControlZoomStarted(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        public void Move()
        {
            if (selectedLand == null)
                return;

            var spherical = SphericalMercator.FromLonLat(selectedLand.Longitude, selectedLand.Latitude);
            var point = Current.Instance.MapControl.Viewport.WorldToScreen(spherical.x, spherical.y);
            Margin = new Thickness(point.X + 20, point.Y - 30, 0, 0);
        }

        private void BtnCloseMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isShown = false;
            Visibility = Visibility.Collapsed;
        }

        private void TxtCommentsClick(object sender, RoutedEventArgs e)
        {
            if (selectedLand == null) return;

            var cs = new CommentScreen(selectedLand.Id, comments);
            cs.OpenCommentScreen();

            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BtnSetClick(object sender, RoutedEventArgs e)
        {
            /*
            var txtBlock = cbSetStatus.SelectedItem as TextBlock;
            if (txtBlock == null) 
                return;

            var statusNumber = Int32.Parse(txtBlock.Tag.ToString());
            var status = (LandStatus)statusNumber;
            landRequests.StatusChanged += SetLandStatusStatusChanged;
            landRequests.UpdateStatus(selectedLand.Id, status, Current.Instance.Username,Current.Instance.Password);

            Current.Instance.EarthwatcherLand.LandStatus = status;
             * */
        }

        private void BtnSetConfirmClick(object sender, RoutedEventArgs e)
        {
            /*
            //TODO: check for value in dropdown

            //if no user logged in return
            if(Current.Instance.Earthwatcher == null) return;

            ConfirmationSort confirmationSort;
            if (cbConfirmation.SelectedIndex == 0)
                confirmationSort = ConfirmationSort.Confirm;
            else
                confirmationSort = ConfirmationSort.Deconfirm;

            landRequests.ConfirmationAdded += ConfirmationConfirmationAdded;
            landRequests.Confirm(selectedLand.Id, Current.Instance.Earthwatcher.Id, confirmationSort, Current.Instance.Username, Current.Instance.Password);
             * */
        }

        private void BtnTwitterClick(object sender, RoutedEventArgs e)
        {
            
            HtmlPage.Window.Invoke("twitter_click", " " + GetUrlReference() + " %23earthwatchers %23" + selectedLand.GeohexKey);
        }

        private void BtnFacebookClick(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Invoke("fbs_click", GetUrlReference());
        }

        private string GetUrlReference()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}?hexkey={1}", "http://geodan.blob.core.windows.net/earthwatchers/index.html", selectedLand.GeohexKey);
        }

        private void Report_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ReportWindow reportWindow = new ReportWindow(selectedLand, selectedEarthwatcher);
            reportWindow.Closed += reportWindow_Closed;
            reportWindow.ConfirmationEnded += reportWindow_ConfirmationEnded;
            reportWindow.Show();
        }

        void reportWindow_ConfirmationEnded(object sender, EventArgs e)
        {
            UpdateInfo(selectedLand.GeohexKey, true);
        }

        void reportWindow_Closed(object sender, EventArgs e)
        {
            
                ReportWindowClosed(this, EventArgs.Empty);
            

            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}