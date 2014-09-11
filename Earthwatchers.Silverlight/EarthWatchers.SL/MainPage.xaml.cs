using System;
using System.Threading;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using EarthWatchers.SL.Requests;
using Earthwatchers.Models;
using Mapsui;
using EarthWatchers.SL.Layers;
using EarthWatchers.SL.GUI.Controls;
using System.Windows.Browser;
using System.Collections.Generic;

namespace EarthWatchers.SL
{
    public partial class MainPage
    {
        private LayerHelper layerHelper;
        private HexagonInfo hexagonInfo;
        private bool leftMouseButtonDown;
        private bool loaded;
        private bool rightMenuOpen;
        private DispatcherTimer timer;
        private Land selectedLand;

        public MainPage(Earthwatcher earthwatcher)
        {
            InitializeComponent();

            Current.Instance.Earthwatcher = earthwatcher;
            Current.Instance.Username = earthwatcher.Name;
            Current.Instance.Password = earthwatcher.Password;
            Current.Instance.IsAuthenticated = true;
            Current.Instance.AddScore = new Dictionary<string, int>();
            this.DataContext = earthwatcher;

            scoreRequest.ScoresReceived += scoreRequest_ScoresReceived;
            scoreRequest.ScoreAdded += scoreRequest_ScoreAdded;
            scoreRequest.ScoreUpdated += scoreRequest_ScoreUpdated;

            Loaded += MainPageLoaded;

            Current.Instance.Main = this; //TODO: remove after demo
        }

        [ScriptableMember]
        public void SetCredentials(string username, string password)
        {
            MessageBox.Show("Message from silverlight: Username: " + username);
            // todo: do the same after login method is succeeded: set global username, password for other requests)
        }

        private readonly ScoreRequests scoreRequest = new ScoreRequests(Constants.BaseApiUrl);

        bool isScoreAdding = false;
        void scoreRequest_ScoreAdded(object sender, EventArgs e)
        {
            isScoreAdding = false;
            Score score = sender as Score;
            if (score != null)
            {
                ((Earthwatcher)this.DataContext).TotalScore += score.Points;
                Current.Instance.Scores.Add(score);
            }
            ShowPoints();
        }

        void scoreRequest_ScoreUpdated(object sender, EventArgs e)
        {
            isScoreAdding = false;
            Score score = sender as Score;
            if (score != null)
            {
                var oldScore = Current.Instance.Scores.Where(x => x.EarthwatcherId == score.EarthwatcherId && x.Action == score.Action).FirstOrDefault();
                if (oldScore != null)
                {
                    Current.Instance.Scores.Remove(oldScore);
                }
                Current.Instance.Scores.Add(score);
                ((Earthwatcher)this.DataContext).TotalScore = Current.Instance.Scores.Sum(x => x.Points);
            }
            ShowPoints();
        }

        private void ShowPoints()
        {
            pointsSound.Stop();
            pointsSound.Play();

            PointsStoryBoard.Stop();
            PointsStoryBoard.Begin();
        }

        void scoreRequest_ScoresReceived(object sender, EventArgs e)
        {
            Current.Instance.Scores = sender as List<Score>;

            if (!Current.Instance.TutorialStarted)
            {
                if (Current.Instance.Scores == null || !Current.Instance.Scores.Any(x => x.Action.Equals("TutorialCompleted")))
                {
                    StartTutorial();
                }
                else
                {
                    ShowPoints();
                    Start();
                }
            }
        }

        private readonly LandRequests landRequest = new LandRequests(Constants.BaseApiUrl);

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            landRequest.LandReceived += LandChanged;
            Tutorial0StoryBoard.Completed += Tutorial0StoryBoard_Completed;
            Tutorial3StoryBoard.Completed += Tutorial3StoryBoard_Completed;
            Tutorial25StoryBoard.Completed += Tutorial25StoryBoard_Completed;
            this.OpacitiesStoryBoard.Completed += OpacitiesStoryBoard_Completed;
            this.layerList.AddingLayer += layerList_AddingLayer;
            this.layerList.LayerAdded += layerList_LayerAdded;

            HtmlPage.RegisterScriptableObject("Credentials", this);
            //SizeChanged += MainPageSizeChanged;

            //Create and set map
            var map = new Map();
            mapControl.Map = map;

            //Initialize the singleton
            Current.Instance.MapControl = mapControl;

            //Pass the LayerCollection used on the map to the helper, this is used to distinguish base, normal, ... layers 
            layerHelper = new LayerHelper(map.Layers);

            Current.Instance.LayerHelper = layerHelper;
            layerList.SetLayerHelper(layerHelper);

            //Load the preset layers
            LayerInitialization.Initialize(layerHelper);

            hexagonInfo = new HexagonInfo();
            hexagonInfo.ReportWindowClosed += hexagonInfo_ReportWindowClosed;
            landInfoWrapper.Children.Add(hexagonInfo);

            var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            if (flagLayer != null) flagLayer.RequestFlags();

            scoreRequest.GetByUser(Current.Instance.Earthwatcher.Id);
        }

        void Tutorial25StoryBoard_Completed(object sender, EventArgs e)
        {
            if (!Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == "Tutorial2Completed"))
            {
                AddPoints("Tutorial2Completed", 500);
            }
        }

        void AddPoints(string action, int points)
        {
            var score = new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = action, Points = points };
            isScoreAdding = true;
            scoreRequest.Post(score, Current.Instance.Username, Current.Instance.Password);
        }

        void UpdatePoints(string action, int points)
        {
            var score = new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = action, Points = points };
            isScoreAdding = true;
            scoreRequest.Update(score, Current.Instance.Username, Current.Instance.Password);
        }

        void OpacitiesStoryBoard_Completed(object sender, EventArgs e)
        {
            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay1.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay2.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay3.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay4.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay5.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay6.Visibility = System.Windows.Visibility.Collapsed;
            this.Overlay7.Visibility = System.Windows.Visibility.Collapsed;

            this.Tutorial0.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial1.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial2.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial3.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial4.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial5.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial6.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial7.Visibility = System.Windows.Visibility.Collapsed;

            this.Tutorial21.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial22.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial23.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial24.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial25.Visibility = System.Windows.Visibility.Collapsed;
        }

        void hexagonInfo_ReportWindowClosed(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                CompleteTutorialStep(5);
            }

            if (Current.Instance.Tutorial2Started)
            {
                this.Tutorial24StoryBoard.Begin();
            }

            //TODO: logica para grabar nuevos scores. Definir la dispersión de fechas entre una nueva acción y la anterior
            if (Current.Instance.AddScore.Count > 0)
            {
                var score = Current.Instance.AddScore.First();

                AddPoints(score.Key, score.Value);

                Current.Instance.AddScore.Clear();
            }
        }

        void layerList_LayerAdded(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                CompleteTutorialStep(3);
            }
            if (Current.Instance.Tutorial2Started)
            {
                this.Tutorial22StoryBoard.Begin();
            }
        }

        void layerList_AddingLayer(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                this.Tutorial3.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (Current.Instance.Tutorial2Started)
            {
                this.Tutorial21.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void Tutorial0StoryBoard_Completed(object sender, EventArgs e)
        {
            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;

            //Navego hacia Salta
            var sphericalTopLeft = SphericalMercator.FromLonLat(-65.742188, -21.988895);
            var sphericalBottomRight = SphericalMercator.FromLonLat(-62.3364, -26.335268);
            mapControl.ZoomToBox(new Mapsui.Geometries.Point(sphericalTopLeft.x, sphericalTopLeft.y), new Mapsui.Geometries.Point(sphericalBottomRight.x, sphericalBottomRight.y));

            //TODO: Blergh awfull dirty dirty hack to show hexagon after zoomToHexagon (problem = Extend is a center point after ZoomToBox)
            mapControl.ZoomIn();
            mapControl.ZoomOut();
        }

        void Tutorial3StoryBoard_Completed(object sender, EventArgs e)
        {
            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;
        }

        void Start()
        {
            tutorialCompletePhase1 = true;
            tutorialCompletePhase2 = true;
            Current.Instance.TutorialStarted = false;
            Current.Instance.Tutorial2Started = false;

            if (Current.Instance.EarthwatcherLand == null)
            {
                landRequest.GetLandById(Current.Instance.Earthwatcher.LandId.ToString());
            }

            OpacitiesStoryBoard.Begin();
            //menuRight.Visibility = System.Windows.Visibility.Visible;

            StartTimer(null, null);
        }

        private void Step0Click(object sender, RoutedEventArgs e)
        {
            //Termina Mapa
            CompleteTutorialStep(0);
        }

        private void Step1Click(object sender, RoutedEventArgs e)
        {
            //Termina Mi Parcela

            //RequestFromUsername requesting the land for the user
            landRequest.GetLandById(Current.Instance.Earthwatcher.LandId.ToString());

            CompleteTutorialStep(1);
        }

        private void Step2Click(object sender, RoutedEventArgs e)
        {
            //Termina Zoom
            CompleteTutorialStep(2);
        }

        private void Step4Click(object sender, RoutedEventArgs e)
        {
            //Termina Capas
            CompleteTutorialStep(4);
        }

        private void Step6Click(object sender, RoutedEventArgs e)
        {
            //Termina Puntajes
            CompleteTutorialStep(6);
        }

        private void Step22Click(object sender, RoutedEventArgs e)
        {
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, selectedLand.GeohexKey);

            this.tutorialCompletePhase2 = true;
            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial23StoryBoard.Begin();
        }

        private void Step24Click(object sender, RoutedEventArgs e)
        {
            this.Tutorial25StoryBoard.Begin();
        }

        private void Step7Click(object sender, RoutedEventArgs e)
        {
            //Vuelvo a su parcela
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.EarthwatcherLand.GeohexKey);

            //Termina el tutorial
            Start();
        }

        TutorialWindow tutorialWindow;

        private void StartTutorial()
        {
            Current.Instance.TutorialStarted = true;
            tutorialWindow = new TutorialWindow();
            tutorialWindow.Closed += tutorialWindow_Closed;
            tutorialWindow.Show();
        }

        void tutorialWindow_Closed(object sender, EventArgs e)
        {
            if (tutorialWindow != null)
            {
                this.Overlay.Visibility = System.Windows.Visibility.Visible;
                this.Overlay1.Visibility = System.Windows.Visibility.Visible;
                this.Overlay2.Visibility = System.Windows.Visibility.Visible;
                this.Overlay3.Visibility = System.Windows.Visibility.Visible;
                this.Overlay4.Visibility = System.Windows.Visibility.Visible;
                this.Overlay5.Visibility = System.Windows.Visibility.Visible;
                this.Overlay6.Visibility = System.Windows.Visibility.Visible;
                this.Overlay7.Visibility = System.Windows.Visibility.Visible;

                if (!Current.Instance.Tutorial2Started)
                {
                    this.Tutorial0StoryBoard.Begin();

                    this.Tutorial0.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial1.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial2.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial3.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial4.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial5.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial6.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial7.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;

                    this.Tutorial21.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial22.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial23.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial24.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial25.Visibility = System.Windows.Visibility.Visible;

                    this.tutorialCompletePhase2 = false;
                    this.Overlay.Visibility = System.Windows.Visibility.Collapsed;

                    //Movimiento hacia parcela en alerta (dummy user)
                    landRequest.GetLandById("1"); //TODO: cambiar por ID correspondiente al Dummy user
                }
            }
        }

        void CompleteTutorialStep(int step)
        {
            //Sumo los puntos
            if (step == 5)
            {
                if (!Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == "TutorialCompleted"))
                {
                    AddPoints("TutorialCompleted", 500);
                }
            }

            switch (step)
            {
                case 0:
                    this.Overlay.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial1StoryBoard.Begin();
                    break;
                case 1:
                    this.Overlay2.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial2StoryBoard.Begin();
                    break;
                case 2:
                    this.Overlay3.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial3StoryBoard.Begin();
                    break;
                case 3:
                    this.Tutorial4StoryBoard.Begin();
                    break;
                case 4:
                    tutorialCompletePhase1 = true;
                    MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.EarthwatcherLand.GeohexKey);
                    this.Tutorial5StoryBoard.Begin();
                    break;
                case 5:
                    this.Tutorial6StoryBoard.Begin();
                    break;
                case 6:
                    this.Tutorial7StoryBoard.Begin();
                    break;
            }
        }

        private Zone zone;
        private void LandChanged(object sender, EventArgs e)
        {
            var land = sender as Land;
            selectedLand = land;

            if (land == null)
                return;

            if (!Current.Instance.Tutorial2Started)
            {
                Current.Instance.EarthwatcherLand = land;
                zone = GeoHex.Decode(land.GeohexKey);

                var hexagonLayer = Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername) as HexagonLayer;

                if (hexagonLayer != null && zone != null)
                    hexagonLayer.AddHexagon(zone, LandStatus.NotChecked, true);
            }

            MapHelper.ZoomToHexagon(Current.Instance.MapControl, land.GeohexKey);

            if (Current.Instance.Tutorial2Started)
            {
                this.Tutorial21StoryBoard.Begin();
            }
        }

        private void TxtLogoutClick(object sender, RoutedEventArgs e)
        {
            //TODO:
            App.BackToLoginPage();
        }

        private void RankingClick(object sender, RoutedEventArgs e)
        {
            var rankingWindow = new Ranking();
            rankingWindow.Show();
        }

        private void BtnMyLandClick(object sender, RoutedEventArgs e)
        {
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.EarthwatcherLand.GeohexKey);
        }

        public void SetLegendGraphic(BitmapImage image)
        {
            legendGrid.Visibility = Visibility.Visible;
            legendImage.Source = image;
        }

        public void StartTimer(object o, RoutedEventArgs sender)
        {
            timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1, 0) };
            timer.Tick += InitHack;
            timer.Start();
        }

        // Hack, map has no initialised event so delay reading of send params
        public void InitHack(object o, EventArgs sender)
        {
            if (!loaded)
            {
                loaded = !loaded;
                timer.Stop();
                try
                {
                    PermalinkHelper.GetPermalinkromUrl();
                    var referedGeoHex = HtmlPage.Document.QueryString["hexkey"];
                    MapHelper.ZoomToHexagon(mapControl, referedGeoHex);
                }
                catch (KeyNotFoundException) { }
            }
        }

        /*
         * The map is shown in the middle of the screen, since Silverlight is unable to clipToBounds
         * the grid needs Clip, the rect has to change when the window is resized.
         */
        private void MainPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //clipMap.Rect = new Rect(0, 0, mapWrapper.ActualWidth, mapWrapper.ActualHeight);
        }

        #region button handlers

        #endregion

        #region mouse events

        bool tutorialCompletePhase1 = false;
        bool tutorialCompletePhase2 = false;
        private void MapControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isScoreAdding) //esto evita que abran el hexagono nuevamente mientras se está guardando un puntaje
            {
                if ((Current.Instance.TutorialStarted && tutorialCompletePhase1) || (Current.Instance.Tutorial2Started && tutorialCompletePhase2) || (!Current.Instance.TutorialStarted && !Current.Instance.Tutorial2Started))
                {
                    leftMouseButtonDown = true;

                    if (!layerHelper.FindLayer(Constants.Hexagonlayername).Enabled)
                        return;

                    var mousePos = e.GetPosition(mapControl);
                    var sphericalCoordinate = mapControl.Viewport.ScreenToWorld(mousePos.X, mousePos.Y); //TODO: posicionar bien el tooltip
                    var lonLat = SphericalMercator.ToLonLat(sphericalCoordinate.X, sphericalCoordinate.Y);

                    var feature = hexagonInfo.GetFeature(lonLat.x, lonLat.y, 7);
                    var hexCode = GeoHex.Encode(lonLat.x, lonLat.y, 7);

                    if (feature == null)
                    {
                        // try on level 6...
                        hexCode = GeoHex.Encode(lonLat.x, lonLat.y, 6);
                    }

                    bool showHex = true;
                    if ((Current.Instance.TutorialStarted || Current.Instance.Tutorial2Started) && !selectedLand.GeohexKey.Equals(hexCode))
                    {
                        showHex = false;
                    }

                    if (showHex)
                    {
                        if (Current.Instance.TutorialStarted)
                        {
                            this.Tutorial5.Visibility = System.Windows.Visibility.Collapsed;
                            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        if (Current.Instance.Tutorial2Started)
                        {
                            this.Tutorial23.Visibility = System.Windows.Visibility.Collapsed;
                            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        hexagonInfo.ShowInfo(lonLat.x, lonLat.y);
                    }
                }
            }
        }

        private void MapControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseButtonDown = false;
        }

        private void MapControlMouseLeave(object sender, MouseEventArgs e)
        {
            leftMouseButtonDown = true;
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            if (!leftMouseButtonDown)
                return;

            hexagonInfo.Move();
        }

        #endregion

        private void BtnLeftMenuMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rightMenuOpen)
                OpenRightMenu.Begin();
            else
                CloseRightMenu.Begin();

            rightMenuOpen = !rightMenuOpen;
        }

        private void BtnLeftMenuMouseEnter(object sender, MouseEventArgs e)
        {
            //txtInfo.Foreground = new SolidColorBrush(Color.FromArgb(255, 159, 181, 11));
        }

        private void BtnLeftMenuMouseLeave(object sender, MouseEventArgs e)
        {
            //txtInfo.Foreground = new SolidColorBrush(Colors.Black);
        }

        List<String> adresses_list = new List<string>();
        private void callbackSuggestion(List<String> suggestions)
        {
            if (suggestions != null)
            {
                //var i = 0;

                adresses_list = suggestions;
            }

        }
        string address = " ";

        private void onText_Changed(object sender, EventArgs e)
        {
            address = txt_search.Text;
            List<string> suggested_addresses = new List<string>();
            OpengeocoderRequests.GetSuggestions(callbackSuggestion, address);
            combo_search.ItemsSource = adresses_list;
            address = combo_search.SelectedItem.ToString();
        }


        private void on_Selection_changed(object sender, SelectionChangedEventArgs e)
        {
            txt_search.Text = combo_search.SelectedValue.ToString();

        }
        private void on_CheckBox_Checked(object sender, EventArgs e)
        {
            txt_search.Visibility = Visibility.Visible;
            combo_search.Visibility = Visibility.Visible;

        }
        private void on_CheckBox_UnChecked(object sender, EventArgs e)
        {
            txt_search.Text = "";
            txt_search.Visibility = Visibility.Collapsed;
            combo_search.Visibility = Visibility.Collapsed;

        }
        private void SearchClick(object sender, MouseButtonEventArgs e)
        {

            OpengeocoderRequests.GetQueryResult(callback, address);

        }

        private void callback(QueryResult queryResult)
        {
            if (queryResult != null)
            {
                var xyStart = SphericalMercator.FromLonLat(queryResult.bbox[0], queryResult.bbox[1]);
                var xyEnd = SphericalMercator.FromLonLat(queryResult.bbox[2], queryResult.bbox[3]);

                var beginPoint = new Mapsui.Geometries.Point(xyStart.x, xyStart.y);
                var endPoint = new Mapsui.Geometries.Point(xyEnd.x, xyEnd.y);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    mapControl.ZoomToBox(beginPoint, endPoint)
                );
            }
        }

        private void zoomInButton_click(object sender, MouseButtonEventArgs e)
        {
            mapControl.ZoomIn();
        }

        private void zoomOutButton_click(object sender, MouseButtonEventArgs e)
        {
            mapControl.ZoomOut();
        }

        private void fullScreenButton_click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        private void helpButton_click(object sender, MouseButtonEventArgs e)
        {
            var tutorialMenuWindow = new TutorialMenuWindow();
            tutorialMenuWindow.Closed += tutorialMenuWindow_Closed;
            tutorialMenuWindow.Show();
        }

        void tutorialMenuWindow_Closed(object sender, EventArgs e)
        {
            var tutorialMenuWindow = sender as TutorialMenuWindow;
            if (tutorialMenuWindow != null && !string.IsNullOrEmpty(tutorialMenuWindow.SelectedOption))
            {
                if (tutorialMenuWindow.SelectedOption.Equals("Button2"))
                {
                    Current.Instance.Tutorial2Started = true;
                    tutorialWindow = new TutorialWindow();
                    tutorialWindow.Closed += tutorialWindow_Closed;
                    tutorialWindow.Show();
                }

                if (tutorialMenuWindow.SelectedOption.Equals("Button3"))
                {
                    TutorialGameWindow gameWindow = new TutorialGameWindow();
                    gameWindow.Closed += gameWindow_Closed;
                    gameWindow.Show();
                }
            }
        }

        void gameWindow_Closed(object sender, EventArgs e)
        {
            TutorialGameWindow gameWindow = sender as TutorialGameWindow;
            if (gameWindow != null)
            {
                var score = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == "TrueColorGame").FirstOrDefault();
                if (score != null)
                {
                    if (score.Points < gameWindow.points)
                    {
                        UpdatePoints("TrueColorGame", gameWindow.points);
                    }
                }
                else
                { 
                    AddPoints("TrueColorGame", gameWindow.points);
                }
            }
        }

        private void shareButton_click(object sender, MouseButtonEventArgs e)
        {
            var pb = new PublishMap();
            pb.Show();
        }

        private void reshuffleButton_click(object sender, MouseButtonEventArgs e)
        {
            //TODO:
        }
        

        private void rulerButton_click(object sender, MouseButtonEventArgs e)
        {
            distanceCalculator.Visibility = Visibility.Visible;
        }

        private void button_Enter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                ((SolidColorBrush)border.Background).Opacity = 0.2d;
            }
        }

        private void button_Leave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                ((SolidColorBrush)border.Background).Opacity = 0;
            }
        }

        private void pointsSound_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
