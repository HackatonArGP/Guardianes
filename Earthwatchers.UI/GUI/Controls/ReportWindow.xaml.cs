using System;
using System.Net;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using Earthwatchers.Models;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class ReportWindow
    {
        private Land selectedLand;
        private Earthwatcher selectedEarthwatcher;
        private readonly LandRequests landRequests;
        private readonly HexagonLayer hexagonLayer;

        public delegate void ConfirmationEndEventHandler(object sender, EventArgs e);
        public event ConfirmationEndEventHandler ConfirmationEnded;

        public delegate void DemandEventHandler(object sender, EventArgs e);
        public event DemandEventHandler Demand;

        public delegate void LandStatusChangedEventHandler(object sender, EventArgs e);
        public event LandStatusChangedEventHandler LandStatusChanged;

        public delegate void SharedEventHandler(object sender, SharedEventArgs e);
        public event SharedEventHandler Shared;

        public ReportWindow(Land land, Earthwatcher earthwatcher)
        {
            InitializeComponent();

            this.ShareStoryBoard.Completed += ShareStoryBoard_Completed;

            selectedLand = land;

            string[] oks = null;
            string[] alerts = null;
            char[] charsep = new char[] { ',' };

            if (!string.IsNullOrEmpty(selectedLand.OKs))
            {
                oks = selectedLand.OKs.Split(charsep, StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrEmpty(selectedLand.Alerts))
            {
                alerts = selectedLand.Alerts.Split(charsep, StringSplitOptions.RemoveEmptyEntries);
            }

            if (Current.Instance.Earthwatcher.Lands.Any(x => x.Id == selectedLand.Id))
            {
                //NO SELECCIONAR LA MANITO OK o ALERT SEGUN EL STATUS, SINO LO QUE EL USER REPORTO
                this.ReportGrid.Visibility = System.Windows.Visibility.Visible;
                this.ConfirmGrid.Visibility = System.Windows.Visibility.Collapsed;

                //if (land.LandStatus == LandStatus.Alert)
                if (alerts != null && alerts.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.AlertButton.BorderThickness = new Thickness(4);
                    this.AlertButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
                }

                //if (land.LandStatus == LandStatus.Ok)
                else if (oks != null && oks.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.OkButton.BorderThickness = new Thickness(4);
                    this.OkButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
                }
            }
            else
            {
                this.Title.Text = Labels.Report5;
                this.ReportButton.Content = Labels.Report11;

                this.ReportGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.ConfirmGrid.Visibility = System.Windows.Visibility.Visible;

                if (oks != null && oks.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.ConfirmButton.BorderThickness = new Thickness(4);
                    this.ConfirmButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));

                    this.ConfirmButton.IsHitTestVisible = false;
                    this.ConfirmButton.Cursor = Cursors.Arrow;
                }

                if (alerts != null && alerts.Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                {
                    this.DeconfirmButton.BorderThickness = new Thickness(4);
                    this.DeconfirmButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));

                    this.DeconfirmButton.IsHitTestVisible = false;
                    this.DeconfirmButton.Cursor = Cursors.Arrow;
                }
            }

            int countConfirm = 0;
            int countDeconfirm = 0;

            if (oks != null)
            {
                countConfirm = oks.Length;
            }

            if (alerts != null)
            {
                countDeconfirm = alerts.Length;
            }

            //asocia esto a algún mensaje que muestre confirmación / deconfirmación
            this.countConfirm1.Text = string.Format("{0} {1}", countConfirm + countDeconfirm, Labels.Report12);
            this.countConfirm2.Text = string.Format("{0} {1}", countConfirm + countDeconfirm, Labels.Report12);

            //Limite de 30 verificaciones
            if (selectedLand.IsLocked || selectedLand.DemandAuthorities)
            {
                DisableActions();
            }
/*
            if (land.DemandAuthorities)
            {
                this.shareText.Text = Labels.Share5;
            }
 * */

            selectedEarthwatcher = earthwatcher;
            if (earthwatcher.IsPowerUser)
            {
                this.badgeIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/badgej.png");
                ToolTipService.SetToolTip(this.badgeIcon, "Jaguar");
            }

            if (Current.Instance.Scores.Count(x => x.Action == ActionPoints.Action.FoundTheJaguar.ToString()) != 0)
            {
                JaguarBadge.Visibility = Visibility.Visible;
            }
            if (Current.Instance.Scores.Any(x => x.Action.StartsWith(ActionPoints.Action.ContestWon.ToString())))
            {
                this.ContestWinnerBadge.Visibility = Visibility.Visible;
            }
            landRequests = new LandRequests(Constants.BaseApiUrl);
            landRequests.StatusChanged += SetLandStatusStatusChanged;
            landRequests.ConfirmationAdded += landRequests_ConfirmationAdded;

            hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);

            this.Loaded += ReportWindow_Loaded;
        }


        private void DemandIcon_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
           
            if (selectedLand.DemandAuthorities)
            {
                image.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandar.png");

                //if (Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.DemandAuthorities.ToString()) && (x.LandId == selectedLand.Id)))
                //{
                //    if (image != null)
                //        image.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandarShare.png");
                //}
                //else
                //{
                //    if (image != null)
                //        image.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandar.png");
                //        //.Text = Labels.Report4;
                //}
            }
            else
            {
                if (image != null)
                    image.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandar_off.png");
            }
        }

        void ShareStoryBoard_Completed(object sender, EventArgs e)
        {
            this.MainGrid.IsHitTestVisible = true;
            this.ReportGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.ConfirmGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        void DisableActions()
        {
            this.ConfirmButton.IsHitTestVisible = false;
            this.ConfirmButton.Cursor = Cursors.Arrow;
            this.ConfirmIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/confirm_off.png");

            this.DeconfirmButton.IsHitTestVisible = false;
            this.DeconfirmButton.Cursor = Cursors.Arrow;
            this.DeconfirmIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/deconfirm_off.png");

            this.OkButton.IsHitTestVisible = false;
            this.OkButton.Cursor = Cursors.Arrow;
            this.OkButtonIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/confirm_off.png");

            this.AlertButton.IsHitTestVisible = false;
            this.AlertButton.Cursor = Cursors.Arrow;
            this.AlertButtonIcon.Source = ResourceHelper.GetBitmap("/Resources/Images/deconfirm_off.png");

            this.ReportButton.IsEnabled = false;
        }

        void ReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Text = selectedEarthwatcher.FullName;
            if (this.txtName.Text.Length > 16)
            {
                txtName.Text = selectedEarthwatcher.FullName.Substring(0, 15) + "...";
                ToolTipService.SetToolTip(this.txtName, selectedEarthwatcher.FullName);
            }
            //txtCountry.Text = selectedEarthwatcher.Country;

            if (Current.Instance.TutorialStarted)
            {
                this.Overlay.Visibility = System.Windows.Visibility.Visible;
                if (Current.Instance.TutorialCurrentStep < 4)
                {
                    this.Tutorial1StoryBoard.Begin();
                }
                else
                {
                    this.Tutorial21StoryBoard.Begin();
                }
            }
            else
            {
                if (selectedLand.DemandAuthorities)
                {
                    string basecamp;
                    if (selectedLand.BasecampId != null)
                    {
                        basecamp = selectedLand.BasecampId.ToString();
                    }
                    else
                        basecamp = "salta01";

                    Uri demandUri = new Uri(string.Format("http://greenpeace.org.ar/denuncias/index.php?id_ciberaccion={0}&mail={1}&area={2}&GeohexKey={3}&prev=0&lat={4}&long={5}", 5157, Current.Instance.Earthwatcher.Name, basecamp, selectedLand.GeohexKey, Math.Round(selectedLand.Latitude, 4), Math.Round(selectedLand.Longitude, 4)), UriKind.Absolute);
                    this.DemandButton1.NavigateUri = demandUri;
                    this.DemandButton2.NavigateUri = demandUri;
                }
                else
                {
                    this.DemandButton1.IsEnabled = false;
                    this.DemandButton2.IsEnabled = false;
                }
            }
        }



        private void Step1Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Step21Click(object sender, RoutedEventArgs e)
        {
            this.Overlay2.Visibility = System.Windows.Visibility.Visible;
            this.Overlay3.Visibility = System.Windows.Visibility.Visible;
            this.Overlay4.Visibility = System.Windows.Visibility.Visible;
            this.Overlay5.Visibility = System.Windows.Visibility.Visible;
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
            this.ValidateMessageText.Visibility = System.Windows.Visibility.Collapsed;

            if (Current.Instance.Earthwatcher.Lands != null)
            {
                if (Current.Instance.Earthwatcher.Lands.Any(x => x.Id == selectedLand.Id))
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
                        loadinAnim.Visibility = System.Windows.Visibility.Visible;
                        this.MainGrid.IsHitTestVisible = false;
                        landRequests.UpdateStatus(selectedLand.Id, status, Current.Instance.Username, Current.Instance.Password);

                        Current.Instance.Earthwatcher.Lands.Where(x => x.Id == selectedLand.Id).First().LandStatus = status;
                    }
                    else
                    {
                        this.ValidateMessageText.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    ConfirmationSort confirmationSort = ConfirmationSort.Confirm;
                    bool hasAction = false;
                    if (this.ConfirmButton.BorderThickness.Left > 0)
                    {
                        if (!selectedLand.OKs.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                        {
                            confirmationSort = ConfirmationSort.Confirm;
                            hasAction = true;
                        }
                    }

                    if (this.DeconfirmButton.BorderThickness.Left > 0)
                    {
                        if (!selectedLand.Alerts.Split(',').Any(x => x.Equals(Current.Instance.Earthwatcher.Id.ToString())))
                        {
                            confirmationSort = ConfirmationSort.Deconfirm;
                            hasAction = true;
                        }
                    }

                    if (hasAction)
                    {
                        loadinAnim.Visibility = System.Windows.Visibility.Visible;
                        this.MainGrid.IsHitTestVisible = false;
                        landRequests.Confirm(selectedLand, confirmationSort, Current.Instance.Username, Current.Instance.Password);
                    }
                    else
                    {
                        this.ValidateMessageText.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        void landRequests_ConfirmationAdded(object sender, EventArgs e)
        {
            if ((ConfirmationSort)sender == ConfirmationSort.Confirm)
            {
                if (!string.IsNullOrEmpty(selectedLand.OKs))
                {
                    selectedLand.OKs += ",";
                }
                selectedLand.OKs += Current.Instance.Earthwatcher.Id.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedLand.Alerts))
                {
                    selectedLand.Alerts += ",";
                }
                selectedLand.Alerts += Current.Instance.Earthwatcher.Id.ToString();
            }

            if (!string.IsNullOrEmpty(selectedLand.LastUsersWithActivity))
            {
                selectedLand.LastUsersWithActivity += ", ";
            }
            selectedLand.LastUsersWithActivity += Current.Instance.Earthwatcher.FullName;

            if (!Current.Instance.Earthwatcher.Lands.Any(x => x.Id == selectedLand.Id)
                    && !Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.ConfirmationAdded.ToString()) && x.LandId == selectedLand.Id && x.Published > selectedLand.LastReset)
                    && !Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.ConfirmationAdded.ToString()) && x.LandId == selectedLand.Id && x.Published.AddHours(2) > DateTime.Now))
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.ConfirmationAdded.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.ConfirmationAdded), Published = DateTime.Now });

                /*
                //Puntos recibidos por el dueño de la parcela
                int receivedPoints = (int)Math.Pow((selectedLand.DeforestationConfirmers.Split(',').Count() + selectedLand.DeforestationDeconfirmers.Split(',').Count()) * 10, 2);
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = selectedEarthwatcher.Id, Action = "ConfirmationReceived", Points = receivedPoints, Published = DateTime.Now });
                 * */
            }

            hexagonLayer.UpdateHexagonsInView();
            ConfirmationEnded(this, EventArgs.Empty);

            if (this.ShareGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                ShowShareControl();
            }
            //this.Close();
        }

        private void SetLandStatusStatusChanged(object sender, EventArgs e)
        {
            if (status == LandStatus.Alert || status == LandStatus.Ok)
            {
                if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.LandStatusChanged.ToString()) && x.LandId == selectedLand.Id && x.Published > selectedLand.LastReset)
                    && !Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.LandStatusChanged.ToString()) && x.LandId == selectedLand.Id && x.Published.AddMinutes(10) > DateTime.Now))
                {
                    Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.LandStatusChanged.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.LandStatusChanged), Published = DateTime.Now });
                }
            }

            //Agrego tambien la verificacion como primer verificacion
            ConfirmationSort confirmationSort = status == LandStatus.Alert ? ConfirmationSort.Deconfirm : ConfirmationSort.Confirm;
            landRequests.Confirm(selectedLand, confirmationSort, Current.Instance.Username, Current.Instance.Password);

            LandStatusChanged(this, EventArgs.Empty);

            if (this.ShareGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                ShowShareControl();
            }
            //this.Close();
        }

        private void Demand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedLand.DemandAuthorities)
            {
                this.shareText.Text = Labels.Share5;
                AddSharedPoints(string.Format(ActionPoints.Action.DemandAuthorities.ToString(), selectedLand.GeohexKey), ActionPoints.Points(ActionPoints.Action.DemandAuthorities));
                
            }
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
                    this.shareText.Text = string.Format("{0} {1}", Labels.Share1, "#SoyGuardianDelBosque");
                }

                if (border.Name.Equals("AlertButton") && ischecked)
                {
                    this.OkButton.BorderThickness = new Thickness(0);
                    this.OkButton.Background = new SolidColorBrush(Colors.Transparent);
                    this.shareText.Text = string.Format("{0} {1}", Labels.Share2, "#SoyGuardianDelBosque");
                    if (selectedLand.ShortText != null || !string.IsNullOrEmpty(selectedLand.ShortText))
                    {
                        shareText.Text = String.Format("Encontré un desmonte en {0}. ¿Me ayudás a verificarlo? {1}", selectedLand.ShortText, "#SoyGuardianDelBosque");
                    }
                }

                if (border.Name.Equals("ConfirmButton") && ischecked)
                {
                    this.DeconfirmButton.BorderThickness = new Thickness(0);
                    this.DeconfirmButton.Background = new SolidColorBrush(Colors.Transparent);
                    this.shareText.Text = string.Format("{0} {1}", Labels.Share1, "#SoyGuardianDelBosque");
                }

                if (border.Name.Equals("DeconfirmButton") && ischecked)
                {
                    this.ConfirmButton.BorderThickness = new Thickness(0);
                    this.ConfirmButton.Background = new SolidColorBrush(Colors.Transparent);
                    this.shareText.Text = string.Format("{0} {1}", Labels.Share2, "#SoyGuardianDelBosque");
                    if (selectedLand.ShortText != null || !string.IsNullOrEmpty(selectedLand.ShortText))
                    {
                        shareText.Text = String.Format("Encontré un desmonte en {0}. ¿Me ayudás a verificarlo? {1}", selectedLand.ShortText, "#SoyGuardianDelBosque");
                    }
                }
            }
        }

        private void ShowShareControl()
        {
            //Facebook y Twitter links
            var geohexcode = selectedLand.GeohexKey;
            var longUrl = string.Format("http://guardianes.greenpeace.org.ar/?fbshare&geohexcode={0}", geohexcode);

            var title = HttpUtility.UrlEncode("¡Parcela Revisada!");
            var summary = HttpUtility.UrlEncode(this.shareText.Text);
            var shortUrl = ShortenUrl(longUrl);
            var fbUrl = "http://www.facebook.com/sharer.php?s=100&p[medium]=106&p[title]={0}&p[summary]={1}&p[url]={2}";
            this.FacebookButton.NavigateUri = new Uri(string.Format(fbUrl, title, summary, shortUrl), UriKind.Absolute);

            longUrl = string.Format("http://guardianes.greenpeace.org.ar/?twshare&geohexcode={0}", geohexcode);
            shortUrl = ShortenUrl(longUrl);
            var finalText = shareText.Text + " " + shortUrl;
            this.TwitterButton.NavigateUri = new Uri(string.Format("https://twitter.com/intent/tweet?text={0}&data-url={1}", Uri.EscapeUriString(finalText).Replace("#", "%23"), shortUrl), UriKind.Absolute);
            //End Facebook y Twitter

            this.FooterGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.ShareGrid.Visibility = System.Windows.Visibility.Visible;
            this.ShareStoryBoard.Begin();
        }

        private string ShortenUrl(string longUrl)
        {
            var bitly = HtmlPage.Window.Invoke("shorten", new string[] { longUrl }) as string;
            return bitly;
        }
        
        private void AddSharedPoints(string action, int points)
        {
            Shared(action, new SharedEventArgs { Action = action, Points = points });
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
           // AddSharedPoints(string.Format(ActionPoints.Action.Shared.ToString(), selectedLand.Id), ActionPoints.Points(ActionPoints.Action.Shared));
            if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.Shared.ToString()) && x.LandId == selectedLand.Id))
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.Shared.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.Shared), Published = DateTime.Now });
                Demand(this, EventArgs.Empty);
            }
        }

        private void DemandButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLand.DemandAuthorities)
            {
                if(!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.DemandAuthorities.ToString()) && x.LandId == selectedLand.Id))
                {
                    Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.DemandAuthorities.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.DemandAuthorities), Published = DateTime.Now });
                    //AddSharedPoints(string.Format(ActionPoints.Action.DemandAuthorities.ToString(), selectedLand.GeohexKey), ActionPoints.Points(ActionPoints.Action.DemandAuthorities));
                    Demand(this, EventArgs.Empty);
                    hexagonLayer.UpdateHexagonsInView();

                }
            }
        }

  

     
    }
}

