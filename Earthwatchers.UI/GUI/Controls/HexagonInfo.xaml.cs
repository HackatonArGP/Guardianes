using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Windows.Browser;
using System.Windows.Controls;
using Earthwatchers.Models;
using Earthwatchers.UI.GUI.Controls.Comments;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Requests;
using Mapsui.Providers;
using Mapsui.Windows;
using System.Collections.ObjectModel;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class HexagonInfo
    {
        private readonly LandRequests landRequests;
        private int _total;
        private readonly CommentRequests commentRequests;
        private readonly CollectionRequests collectionRequests;
        private readonly HexagonLayer hexagonLayer;
        private readonly BasecampLayer bcLayer; //TEST
        private ObservableCollection<Comment> comments;
        private Earthwatcher selectedEarthwatcher;
        private Land selectedLand;
        private Boolean isShown;

        public delegate void ReportWindowClosedEventHandler(object sender, EventArgs e);
        public event ReportWindowClosedEventHandler ReportWindowClosed;

        public delegate void ReportWindowConfirmationEndedEventHandler(object sender, EventArgs e);
        public event ReportWindowConfirmationEndedEventHandler ReportWindowConfirmationEnded;

        public delegate void ReportWindowDemandEventHandler(object sender, EventArgs e);
        public event ReportWindowDemandEventHandler ReportWindowDemand;

        public delegate void ReportWindowLandStatusChangedEventHandler(object sender, EventArgs e);
        public event ReportWindowLandStatusChangedEventHandler ReportWindowLandStatusChanged;

        public delegate void SharedEventHandler(object sender, SharedEventArgs e);
        public event SharedEventHandler Shared;

        public delegate void CollectionCompleteEventHandler(object sender, CollectionCompleteEventArgs e);
        public event CollectionCompleteEventHandler CollectionCompleted;

        public HexagonInfo()
        {
            InitializeComponent();

            commentRequests = new CommentRequests(Constants.BaseApiUrl);
            collectionRequests = new CollectionRequests(Constants.BaseApiUrl);
            landRequests = new LandRequests(Constants.BaseApiUrl);
            hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);
            bcLayer  = (BasecampLayer)Current.Instance.LayerHelper.FindLayer(Constants.BasecampsLayer); //TEST
            //Add event listeners
            commentRequests.CommentsByLandReceived += CommentRequestCommentsByLandReceived;
            collectionRequests.NewItemReceived += collectionRequests_NewItemReceived;
            collectionRequests.ItemsCountReceived += collectionRequests_ItemsCountReceived;
            Current.Instance.MapControl.zoomFinished += MapControlZoomFinished;
            Current.Instance.MapControl.zoomStarted += MapControlZoomStarted;

            HtmlPage.RegisterScriptableObject("HexagonInfo", this);
            collectionRequests.GetTotalItems(Current.Instance.Earthwatcher.Id);
        }

        [ScriptableMember]
        public void UpdateText(string result)
        {
            MessageBox.Show(result);
        }

        private void CommentRequestCommentsByLandReceived(object sender, EventArgs e)
        {
            comments = new ObservableCollection<Comment>(sender as List<Comment>);
            //comments.CollectionChanged += comments_CollectionChanged;
            if (comments != null)
            {
                this.commentsBorder.Visibility = System.Windows.Visibility.Visible;
                NumberOfCommentsText.Text = comments.Count.ToString();
                commentList.ItemsSource = comments.OrderByDescending(x => x.Published).Take(2);
            }
        }

        /*
        void comments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NumberOfCommentsText.Text = comments.Count.ToString();
            commentList.ItemsSource = comments.OrderByDescending(x => x.Published).Take(2);
        }
         * */

        bool first = true;
        public Feature GetFeature(double longitude, double latitude, int level)
        {
            //Get the hexcode for the clicked area and try to find if its a feature on the map
            var hexCode = GeoHex.Encode(longitude, latitude, level);

            Feature feature = null; //TEST
            if (first)
            {
                feature = bcLayer.GetFeatureByHex(hexCode); //TEST
                first = false;
            }
            else
                feature = hexagonLayer.GetFeatureByHex(hexCode);
            //var feature = hexagonLayer.GetFeatureByHex(hexCode);
            
            return feature;
        }

        public void Initialize()
        {
            if (Current.Instance.TutorialStarted)
            {
                this.btnClose.Visibility = System.Windows.Visibility.Collapsed;
                this.commentsBorder.Visibility = System.Windows.Visibility.Collapsed;
                this.rectHighlight.Visibility = System.Windows.Visibility.Visible;
                this.HighlightStoryBoard.Begin();
            }
            else
            {
                this.btnClose.Visibility = System.Windows.Visibility.Visible;
                this.commentsBorder.Visibility = System.Windows.Visibility.Collapsed;
                this.rectHighlight.Visibility = System.Windows.Visibility.Collapsed;

                txtName.Text = string.Empty;
                txtName.Visibility = System.Windows.Visibility.Collapsed;
                txtBasecampName.Text = string.Empty;
                txtBasecampName.Visibility = System.Windows.Visibility.Collapsed;
                txtBasecamp.Text = string.Empty;
                txtBasecamp.Visibility = System.Windows.Visibility.Collapsed;
                this.LastUsersWithActivityText.Visibility = System.Windows.Visibility.Collapsed;
                this.LastUsersWithActivityText.MouseLeftButtonDown -= LastUsersWithActivityText_MouseLeftButtonDown;
                this.LastUsersWithActivityText.MouseEnter -= LastUsersWithActivityText_MouseEnter;
                this.LastUsersWithActivityText.MouseLeave -= LastUsersWithActivityText_MouseLeave;
                //txtCountry.Text = string.Empty;
            }
        }

        public void ShowInfo(double lon, double lat, string hexCode)
        {
            if (!string.IsNullOrEmpty(hexCode))
            {
                UpdateInfo(hexCode, false);
                Move();
                isShown = true;
                Visibility = Visibility.Visible;
            }
        }

        private void UpdateInfo(string hexCode, bool isRefresh)
        {
            if (Current.Instance.Earthwatcher.Lands != null &&
                Current.Instance.Earthwatcher.Lands.Any(x => x.GeohexKey == hexCode))
            {
                selectedLand = Current.Instance.Earthwatcher.Lands.Where(x => x.GeohexKey == hexCode).First();
                selectedEarthwatcher = Current.Instance.Earthwatcher;

                this.reportText.Text = Labels.HexInfo1;
                this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/reportar.png");

                this.ActionButton.Visibility = System.Windows.Visibility.Visible;
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

                selectedEarthwatcher = new Earthwatcher { Name = selectedLand.EarthwatcherName, Id = selectedLand.EarthwatcherId.Value, IsPowerUser = selectedLand.IsPowerUser.Value };

                this.reportText.Text = Labels.HexInfo2;
                this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/revisar.png");

                if (selectedLand.LandStatus == LandStatus.Alert || selectedLand.LandStatus == LandStatus.Ok)
                {
                    this.ActionButton.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    this.ActionButton.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            RenderActivity();
            commentRequests.GetCommentsByLand(selectedLand.Id);
        }

        private void RenderActivity()
        {
            if (selectedLand == null)
                return;

            this.btnDemand.MouseLeftButtonDown -= Report_MouseLeftButtonDown;
            this.btnDemand.MouseLeftButtonDown += Report_MouseLeftButtonDown;

            if (selectedLand.DemandAuthorities)
            {
                this.reportText.Text = Labels.HexInfo5;
                this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/demandaricon.png");
                
                //DB: Deshabilito el boton demandar por ahora. Para volver a habilitar las denuncias eliminar las 3 lineas siguientes:
                //this.reportIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/demandaricon_off.png");
                //this.btnDemand.MouseLeftButtonDown -= Report_MouseLeftButtonDown;
                //ToolTipService.SetToolTip(this.btnDemand, "Las denuncias están momentáneamente deshabilitadas por mantenimiento en los servidores. Disculpe las molestias.");

                //Deshabilito los comentarios
                this.commentsBorder.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (selectedEarthwatcher.IsPowerUser)
            {
                this.badgeIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/badgej.png");
                ToolTipService.SetToolTip(this.badgeIcon, "Jaguar");
            }
            else
            {
                this.badgeIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/badge.png");
                ToolTipService.SetToolTip(this.badgeIcon, null);
            }
            if(Current.Instance.Scores.Count(x => x.Action == ActionPoints.Action.FoundTheJaguar.ToString()) != 0)
            {
                JaguarBadge.Visibility = Visibility.Visible;
            }

            //Mostrar la finca a la que pertenece
            if (selectedLand.BasecampName != null)
            {
                txtBasecamp.Text = Labels.HexInfo9;
                txtBasecampName.Text = selectedLand.BasecampName;
                ToolTipService.SetToolTip(this.txtBasecampName, null);
                if (this.txtBasecampName.Text.Length > 60)
                {
                    txtBasecampName.Text = selectedLand.BasecampName.Substring(0, 58) + "...";
                    ToolTipService.SetToolTip(this.txtBasecampName, selectedLand.BasecampName);
                }
                txtBasecamp.Visibility = System.Windows.Visibility.Visible;
                txtBasecampName.Visibility = System.Windows.Visibility.Visible;
            }

            txtName.Text = selectedEarthwatcher.FullName;
            ToolTipService.SetToolTip(this.txtName, null);
            if (this.txtName.Text.Length > 10)
            {
                txtName.Text = selectedEarthwatcher.FullName.Substring(0, 10) + "...";
                ToolTipService.SetToolTip(this.txtName, selectedEarthwatcher.FullName);
            }
            txtName.Visibility = System.Windows.Visibility.Visible;

            //Activity
            if (selectedLand != null && !string.IsNullOrEmpty(selectedLand.LastUsersWithActivity))
            {
                char[] charsep = new char[] { ',' };
                bool moreThan5 = selectedLand.OKs.Split(charsep, StringSplitOptions.RemoveEmptyEntries).Length + selectedLand.Alerts.Split(charsep, StringSplitOptions.RemoveEmptyEntries).Length > 5 ? true : false;
                var lstUsersWithActivity = selectedLand.LastUsersWithActivity.Split(charsep, StringSplitOptions.RemoveEmptyEntries);
                var lstFirst5 = lstUsersWithActivity.Where(x => x != Configuration.GreenpeaceName).Take(5).ToList();
                if (lstUsersWithActivity.Contains(Configuration.GreenpeaceName))
                {
                    lstFirst5.Insert(0, Configuration.GreenpeaceName);
                    lstFirst5.Take(5);
                }
                this.LastUsersWithActivityText.Text = string.Join(",", lstFirst5) + (moreThan5 ? ", ..." : string.Empty);

                if (moreThan5)
                {
                    this.LastUsersWithActivityText.Cursor = System.Windows.Input.Cursors.Hand;
                    this.LastUsersWithActivityText.MouseLeftButtonDown += LastUsersWithActivityText_MouseLeftButtonDown;
                    this.LastUsersWithActivityText.MouseEnter += LastUsersWithActivityText_MouseEnter;
                    this.LastUsersWithActivityText.MouseLeave += LastUsersWithActivityText_MouseLeave;
                    ToolTipService.SetToolTip(this.LastUsersWithActivityText, Labels.HexInfo6);
                }
                else
                {
                    this.LastUsersWithActivityText.Cursor = System.Windows.Input.Cursors.Arrow;
                    ToolTipService.SetToolTip(this.LastUsersWithActivityText, Labels.HexInfo7);
                }
                this.LastUsersWithActivityText.Visibility = System.Windows.Visibility.Visible;
                this.LastUsersWithActivityTextTitle.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.LastUsersWithActivityTextTitle.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (selectedLand != null && selectedLand.IsLocked)
            {
                btnComments.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                btnComments.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void LastUsersWithActivityText_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.LastUsersWithActivityText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 65, 65, 65));
        }

        void LastUsersWithActivityText_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.LastUsersWithActivityText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 105, 125, 0));
        }

        void LastUsersWithActivityText_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (selectedLand != null)
            {
                this.LastUsersWithActivityText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 65, 65, 65));
                LandActivity landActivity = new LandActivity(selectedLand.Id);
                landActivity.Show();
            }
        }

        private void MapControlZoomFinished(object sender, EventArgs e)
        {
            if (!isShown)
                return;

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
            Margin = new Thickness(point.X + 70, point.Y - 30, 0, 0);
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
            this.HighlightStoryBoard.Stop();
            if (selectedLand.DemandAuthorities)
            {
                DemandWindow demandWindow = new DemandWindow(selectedLand, selectedEarthwatcher);
                demandWindow.Demand += reportWindow_Demand;
                demandWindow.Show();
            }
            else
            {
                ReportWindow reportWindow = new ReportWindow(selectedLand, selectedEarthwatcher);
                reportWindow.Closed += reportWindow_Closed;
                reportWindow.ConfirmationEnded += reportWindow_ConfirmationEnded;
                reportWindow.Demand += reportWindow_Demand;
                reportWindow.LandStatusChanged += reportWindow_LandStatusChanged;
                reportWindow.Shared += reportWindow_Shared;
                reportWindow.Show();
            }
        }

        void reportWindow_LandStatusChanged(object sender, EventArgs e)
        {
            if (Current.Instance.Features.IsUnlocked(EwFeature.Collections))
            {
                //Abro la ventana de collections para entregarle un item al usuario
                if (Current.Instance.AddScore.Any(x => x.Points > 0))
                {
                    collectionRequests.GetNewCollectionItem(Current.Instance.Earthwatcher.Id);
                }
            }

            ReportWindowLandStatusChanged(this, EventArgs.Empty);
        }

        void reportWindow_Shared(object sender, SharedEventArgs e)
        {
            Shared(sender, e);
        }

        void reportWindow_ConfirmationEnded(object sender, EventArgs e)
        {
            UpdateInfo(selectedLand.GeohexKey, true);
            ReportWindowConfirmationEnded(this, EventArgs.Empty);
        }

        void reportWindow_Demand(object sender, EventArgs e)
        {
            ReportWindowDemand(this, EventArgs.Empty);
        }

        void reportWindow_Closed(object sender, EventArgs e)
        {
            ReportWindowClosed(this, EventArgs.Empty);
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        void collectionRequests_NewItemReceived(object sender, EventArgs e)
        {
            CollectionItem item = sender as CollectionItem;
          
            if (item != null)
            {
                Collections collections = new Collections(item);
                if (_total == 0)
                {
                    collections.CollectionsTutorial.Visibility = Visibility.Visible;
                    collections.ContinueButton.Visibility = Visibility.Visible;
                    collections.Show();
                }
                else
                {
                    collections.loadCollections();
                    collections.CollectionCompleted += collections_CollectionCompleted;
                    collections.Show();
                }
            }
        }
        
        void collections_CollectionCompleted(object sender, CollectionCompleteEventArgs e)
        {
            CollectionCompleted(this, e);
        }

        void collectionRequests_ItemsCountReceived(object sender, EventArgs e)
        {
                int items = Convert.ToInt32(sender);
                if (items != -1)
                _total = items;
        }

    }
}