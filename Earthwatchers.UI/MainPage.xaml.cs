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
using Earthwatchers.UI.Requests;
using Earthwatchers.Models;
using Mapsui;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.GUI.Controls;
using System.Windows.Browser;
using System.Collections.Generic;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI
{
    public partial class MainPage
    {
        private LayerHelper layerHelper;
        private HexagonInfo hexagonInfo;
        private bool leftMouseButtonDown;
        private bool rightMenuOpen;
        private Land selectedLand;
        private string geohexcode;

        public MainPage(Earthwatcher earthwatcher, string _geohexcode)
        {
            InitializeComponent();

            geohexcode = _geohexcode;

            //Logo
            this.logo.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", Labels.LogoPath));

            Current.Instance.Earthwatcher = earthwatcher;
            Current.Instance.Username = earthwatcher.Name;
            Current.Instance.Password = earthwatcher.Password;
            Current.Instance.IsAuthenticated = true;
            Current.Instance.AddScore = new List<Score>();
            Current.Instance.MapControl = mapControl;
            this.DataContext = earthwatcher;

            if (earthwatcher.IsPowerUser)
            {
                this.badgeIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/badgej.png");
                ToolTipService.SetToolTip(this.badgeIcon, "Jaguar");
            }

            if (this.UserFullName.Text.Length > 14)
            {
                UserFullName.Text = Current.Instance.Earthwatcher.FullName.Substring(0, 13) + "...";
            }
            else
                UserFullName.Text = Current.Instance.Earthwatcher.FullName;

            scoreRequest.ScoresReceived += scoreRequest_ScoresReceived;
            scoreRequest.ScoreAdded += scoreRequest_ScoreAdded;
            scoreRequest.ScoreUpdated += scoreRequest_ScoreUpdated;
            scoreRequest.ServerDateTimeReceived += scoreRequest_ServerDateTimeReceived;
            Current.Instance.MapControl.zoomStarted += MapControl_zoomStarted;
            Current.Instance.MapControl.zoomFinished += MapControl_zoomFinished;

            Loaded += MainPageLoaded;
           
           


            //Current.Instance.Main = this; //TODO: remove after demo
        }

        [ScriptableMember]
        public void SetCredentials(string username, string password)
        {
            MessageBox.Show(Labels.Main1 + username);
            // todo: do the same after login method is succeeded: set global username, password for other requests)
        }

        private readonly ScoreRequests scoreRequest = new ScoreRequests(Constants.BaseApiUrl);

        bool isScoreAdding = false;
        void scoreRequest_ScoreAdded(object sender, EventArgs e)
        {
            isScoreAdding = false;
            List<Score> scores = sender as List<Score>;
            if (scores != null)
            {
                Current.Instance.Scores.AddRange(scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id));
                UpdateHelpNotifications();

                if (scores.Count > 0 && (scores.First().Action.StartsWith(ActionPoints.Action.LandReassigned.ToString()) || scores.Sum(x => x.Points) == 0))
                    return;

                ShowPoints();

                //loggear si se desbloqueo algun nuevo feature
                var toLog = Current.Instance.Features.GetNewUnlocksToLog();
                if (toLog.Any())
                {
                    this.AddPoints(toLog);
                }

                //chequear si hay algo para habilitar
                this.TurnOnUnlockedFeatures();
            }
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

                ShowPoints();

                //loggear si se desbloqueo algun nuevo feature
                var toLog = Current.Instance.Features.GetNewUnlocksToLog();
                if (toLog.Any())
                {
                    this.AddPoints(toLog);
                }

                //chequear si hay algo para habilitar
                this.TurnOnUnlockedFeatures();
            }

        }

        private void ShowPoints()
        {
            this.TotalScore.Text = Current.Instance.Scores.Sum(x => x.Points).ToString();

            pointsSound.Stop();
            pointsSound.Play();

            PointsStoryBoard.Stop();
            PointsStoryBoard.Begin();

            UpdateHelpNotifications();
        }

        void UpdateHelpNotifications()
        {
            int helpNotifications = 0;
            if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.TutorialCompleted.ToString())))
            {
                helpNotifications += 2;
            }

            if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.MiniJuegoI.ToString())))
            {
                helpNotifications++;
            }

            if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.ScoringHelp.ToString())))
            {
                helpNotifications++;
            }

            if (helpNotifications > 0)
            {
                this.HelpNotifications.Visibility = System.Windows.Visibility.Visible;
                this.HelpNotificationsText.Text = helpNotifications.ToString();
            }
            else
            {
                this.HelpNotifications.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        // OJO: Tener en cuenta que este metodo se ejecuta varias veces durante la ejecucion
        //   Poner alguna restriccion si se van a agregar elementos a una lista, ya que puede repetirlos varias veces.
        public void TurnOnUnlockedFeatures() 
        {
            if (Current.Instance.Features.IsUnlocked(EwFeature.ForestLaw))
            {
            }

            if(Current.Instance.Earthwatcher.Role == Role.Admin)
            { 
                AddArgentineLawLayer();
            }

            AddBasecampsLayer();

            if (Current.Instance.Features.IsUnlocked(EwFeature.Image2008Warning)) //Para que no la muestre siempre
            {
                if (!Current.Instance.Scores.Any(x => x.Action == ActionPoints.Action.FeatureUnlocked_Image2008Warning.ToString()))
                AddImage2008Warning();
            }
        }

        void scoreRequest_ScoresReceived(object sender, EventArgs e)
        {
            Current.Instance.Scores = sender as List<Score>;
            this.TotalScore.Text = Current.Instance.Scores.Sum(x => x.Points).ToString();

            //initialize features and turn on unlocked ones
            Current.Instance.Features = new Features(Current.Instance.Scores, Current.Instance.Earthwatcher.Id);
            TurnOnUnlockedFeatures();

            //Correccion para desloqueos por puntos que se obtienen mediante acciones de terceros.
            var toLog = Current.Instance.Features.GetNewUnlocksToLog();
            if (toLog.Any())
            {
                this.AddPoints(toLog);
            }
            
            //chequear si hay algo para habilitar
            this.TurnOnUnlockedFeatures();

            UpdateHelpNotifications();

            if (!Current.Instance.TutorialStarted)
            {
                if (Current.Instance.Scores == null || !Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.TutorialCompleted.ToString())))
                {
                    StartTutorial();
                }
                else
                {
                    //Corro la animación solo si el login le dio puntos
                    var lastLogin = Current.Instance.Scores.Where(x => x.Action.Equals(ActionPoints.Action.Login.ToString())).FirstOrDefault();
                    if (lastLogin != null && lastLogin.Points > 0)
                    {
                        ShowPoints();
                    }
                    Start(false);
                }
            }
            //En cuanto se carga el score, verifico si cumple los requisitos para tener un jaguar badge
            if (Current.Instance.Scores.Count(x => x.Action == ActionPoints.Action.FoundTheJaguar.ToString()) != 0)
            {
                this.jaguarbadge.Visibility = Visibility.Visible;
            }

            if (Current.Instance.Scores.Any(x => x.Action.StartsWith(ActionPoints.Action.ContestWon.ToString())))
            {
                this.ContestWinnerBadge.Visibility = Visibility.Visible;
            }
        }

        private readonly LandRequests landRequest = new LandRequests(Constants.BaseApiUrl);
        private readonly JaguarRequests jaguarRequests = new JaguarRequests(Constants.BaseApiUrl);
        private readonly PopupMessageRequests popupMessageRequests = new PopupMessageRequests(Constants.BaseApiUrl);
        private readonly ContestRequests contestRequests = new ContestRequests(Constants.BaseApiUrl);

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            landRequest.LandReceived += LandChanged;
            landRequest.LandsReceived += landRequest_LandsReceived;
            landRequest.VerifiedLandCodesReceived += landRequest_VerifiedLandCodesReceived;
            jaguarRequests.PositionReceived += jaguarRequests_PositionReceived;
            popupMessageRequests.MessageReceived += popupMessageRequests_MessageReceived;
            contestRequests.ContestReceived += contestRequests_ContestReceived;

            Tutorial0StoryBoard.Completed += Tutorial0StoryBoard_Completed;
            Tutorial0LoadingStoryBoard.Completed += Tutorial0LoadingStoryBoard_Completed;
            Tutorial1StoryBoard.Completed += Tutorial1StoryBoard_Completed;
            Tutorial3StoryBoard.Completed += Tutorial3StoryBoard_Completed;
            this.OpacitiesStoryBoard.Completed += OpacitiesStoryBoard_Completed;
            this.layerList.ChangingOpacity += layerList_ChangingOpacity;
            this.layerList.HexLayerVisibilityChanged += layerList_HexLayerVisibilityChanged;
            this.layerList.ArgentineLawLayerLayerVisibilityChanged += layerList_ArgentineLawLayerLayerVisibilityChanged;
            this.layerList.ArgentineLawLoaded += layerList_ArgentineLawLoaded;
            this.layerList.ArgentineLawLoading += layerList_ArgentineLawLoading;
            this.ChangeOpacityStoryBoard.Completed += ChangeOpacityStoryBoard_Completed;

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

            //Load the preset layers
            LayerInitialization.Initialize(layerHelper);
            layerList.SetLayerHelper(layerHelper);

            hexagonInfo = new HexagonInfo();
            hexagonInfo.ReportWindowClosed += hexagonInfo_ReportWindowClosed;
            hexagonInfo.CollectionCompleted += hexagonInfo_CollectionCompleted;
            hexagonInfo.ReportWindowDemand += hexagonInfo_ReportWindowDemand;
            hexagonInfo.Shared += hexagonInfo_Shared;
            hexagonInfo.ReportWindowConfirmationEnded += hexagonInfo_ReportWindowConfirmationEnded;
            hexagonInfo.ReportWindowLandStatusChanged += hexagonInfo_ReportWindowLandStatusChanged;
            landInfoWrapper.Children.Add(hexagonInfo);

            //var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            //if (flagLayer != null) flagLayer.RequestFlags();
            MapControl_zoomFinished(null, null);
            MapControl_zoomStarted(null, null);
            scoreRequest.GetByUser(Current.Instance.Earthwatcher.Id);

      
        

        }

        void Tutorial0LoadingStoryBoard_Completed(object sender, EventArgs e)
        {
            loadinAnim.Visibility = Visibility.Collapsed;
            this.loadingAnimText.Text = Labels.Cargando;
        }

        void hexagonInfo_ReportWindowLandStatusChanged(object sender, EventArgs e)
        {
            if (Current.Instance.AddScore.Count > 0)
            {
                AddPoints(Current.Instance.AddScore);
            }
        }

        void hexagonInfo_ReportWindowConfirmationEnded(object sender, EventArgs e)
        {
            //TODO: logica para grabar nuevos scores. Definir la dispersión de fechas entre una nueva acción y la anterior
            if (Current.Instance.AddScore.Count > 0)
            {
                AddPoints(Current.Instance.AddScore);
            }
        }

        void hexagonInfo_ReportWindowDemand(object sender, EventArgs e)
        {
            if (Current.Instance.AddScore.Count > 0)
            {
                AddPoints(Current.Instance.AddScore);
            }
        }


        void layerList_ArgentineLawLayerLayerVisibilityChanged(object sender, SharedEventArgs e)
        {
            if (e.Action == "False")
            {
                this.ArgentineLawLayerLegend.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.ArgentineLawLayerLegend.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void layerList_HexLayerVisibilityChanged(object sender, SharedEventArgs e)
        {
            if (e.Action == "False")
            {
                this.HexagonsLegend.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.HexagonsLegend.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void hexagonInfo_CollectionCompleted(object sender, CollectionCompleteEventArgs e)
        {
            if (Current.Instance.Scores.Any(x => x.Action == ActionPoints.Action.CollectionComplete.ToString() + " " + e.CollectionId))
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.CollectionComplete.ToString() + " " + e.CollectionId, Points = e.Points, Published = DateTime.Now });
                AddPoints(Current.Instance.AddScore);
            }
        }

        void landRequest_VerifiedLandCodesReceived(object sender, EventArgs e)
        {
            List<string> codes = sender as List<string>;
            if (codes.Count > 0)
            {
                PollsWindow pollsWindow = new PollsWindow(codes);
                pollsWindow.BonusReached += pollsWindow_BonusReached;
                pollsWindow.Show();
            }
        }

        void pollsWindow_BonusReached(object sender, SharedEventArgs e)
        {
            Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = e.Action, Points = e.Points, Published = DateTime.Now });
            AddPoints(Current.Instance.AddScore);
        }

        void ChangeOpacityStoryBoard_Completed(object sender, EventArgs e)
        {
            this.NotificationText.Visibility = System.Windows.Visibility.Collapsed;
        }

        void layerList_ChangingOpacity(object sender, ChangingOpacityEventArgs e)
        {
            string newTitle = string.Format("{0} {1}{2}", Labels.Timeline1, e.Title, e.Title.Equals("2008") ? "\n(los desmontes de esta imagen son previos a la ley de bosques)" : e.IsCloudy ? Labels.Main2 : string.Empty);
            if (this.NotificationText.Visibility == System.Windows.Visibility.Collapsed || this.NotificationText.Text != newTitle)
            {
                this.NotificationText.Visibility = System.Windows.Visibility.Visible;
                this.NotificationText.Text = newTitle;
                if (!e.IsInitial)
                {
                    ShowSatelliteImageText();
                }
            }
        }

        void ShowSatelliteImageText()
        {
            this.ChangeOpacityStoryBoard.Begin();
        }

        void landRequest_LandsReceived(object sender, EventArgs e)
        {
            loadinAnim.Visibility = Visibility.Collapsed;
            ShowSatelliteImageText();
            List<Land> lands = sender as List<Land>;
            if (lands != null)
            {
                Current.Instance.Lands = lands;
            }

            //Inicializo el Reporte Diario (HAY QUE HACERLO ACA QUE ES CUANDO SI O SI TIENE LAS LANDS CARGADAS)
            this.showDailySummary(contestText);
        }

        SharedEventArgs sharedEventArgs;
        void hexagonInfo_Shared(object sender, SharedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Action))
            {
                sharedEventArgs = e;

                //Veo el time del server
                scoreRequest.GetServerTime();
            }
        }

        void scoreRequest_ServerDateTimeReceived(object sender, EventArgs e)
        {
            Score score = sender as Score;
            if (score != null && sharedEventArgs != null)
            {
                int timesPerDayAllowed = 10;
                int timesPerDay = 0;
                if (sharedEventArgs.Action.StartsWith(ActionPoints.Action.DemandAuthorities.ToString()))
                {
                    //Si ya le dí puntaje por demandar esa parcela entonces no le vuelvo a dar
                    if (Current.Instance.Scores.Any(x => x.Action.Equals(sharedEventArgs.Action)))
                    {
                        return;
                    }
                    timesPerDay = Current.Instance.Scores.Count(x => x.Action.StartsWith(ActionPoints.Action.DemandAuthorities.ToString()) && x.Published.Date == score.Published.Date);
                    timesPerDayAllowed = 5;
                }
                else
                {
                    timesPerDay = Current.Instance.Scores.Count(x => x.Action.Equals(sharedEventArgs.Action) && x.Published.Date == score.Published.Date);
                }
                if (timesPerDay < timesPerDayAllowed)
                {
                    Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = sharedEventArgs.Action, Points = sharedEventArgs.Points, Published = score.Published.Date });
                    AddPoints(Current.Instance.AddScore);
                }
            }
        }

        public void AddPoints(List<Score> scores)
        {
            //var score = new Score { EarthwatcherId = earthwatcherId, Action = action, Points = points, Published = DateTime.Now };
            isScoreAdding = true;
            scoreRequest.Post(scores, Current.Instance.Username, Current.Instance.Password);
            Current.Instance.AddScore.Clear();
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
            this.Overlay7.Visibility = System.Windows.Visibility.Collapsed;

            this.Tutorial0.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial1.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial3.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial5.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial6.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial7.Visibility = System.Windows.Visibility.Collapsed;
            this.Tutorial8.Visibility = System.Windows.Visibility.Collapsed;

            this.Tutorial23.Visibility = System.Windows.Visibility.Collapsed;
        }

        void hexagonInfo_ReportWindowClosed(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                CompleteTutorialStep();
            }
        }

        void layerList_LayerAdded(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                CompleteTutorialStep();
            }
        }

        void layerList_AddingLayer(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted)
            {
                this.Tutorial3.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void Tutorial0StoryBoard_Completed(object sender, EventArgs e)
        {
            if (!replayTutorial)
            {
                Tutorial0LoadingStoryBoard.Begin();
            }

            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;

            //Navego hacia Salta
            var sphericalTopLeft = SphericalMercator.FromLonLat(-66.693003, -21.758917);
            var sphericalBottomRight = SphericalMercator.FromLonLat(-61.30395, -26.567339);
            mapControl.ZoomToBox(new Mapsui.Geometries.Point(sphericalTopLeft.x, sphericalTopLeft.y), new Mapsui.Geometries.Point(sphericalBottomRight.x, sphericalBottomRight.y));

            //TODO: Blergh awfull dirty dirty hack to show hexagon after zoomToHexagon (problem = Extend is a center point after ZoomToBox)
            mapControl.ZoomIn();
            mapControl.ZoomOut();
            mapControl.ZoomIn();  //Extra zoom to see the plots
        }

        void Tutorial1StoryBoard_Completed(object sender, EventArgs e)
        {
            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;
            this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
            this.layerList.StartSliderAnimation();
        }

        void Tutorial3StoryBoard_Completed(object sender, EventArgs e)
        {
            this.MiParcelaStoryBoard.Begin();
        }

        private void Step0Click(object sender, RoutedEventArgs e)
        {
            //Termina Mapa
            mapControl.ZoomIn();
            mapControl.ZoomIn();
            CompleteTutorialStep();
        }

        private void Step1Click(object sender, RoutedEventArgs e)
        {
            //Termina Timeline
            CompleteTutorialStep();
        }

        private void Step2Click(object sender, MouseButtonEventArgs e)
        {
            //Termina Mi Parcela
            LandChanged(Current.Instance.Earthwatcher.Lands.First());
            this.MiParcelaStoryBoard.Stop();

            CompleteTutorialStep();
        }

        private void Step4Click(object sender, RoutedEventArgs e)
        {
            //Termina Capas
            CompleteTutorialStep();
        }

        private void Step6Click(object sender, RoutedEventArgs e)
        {
            //Termina Puntajes
            CompleteTutorialStep();
        }

        private void Step24Click(object sender, RoutedEventArgs e)
        {
            CompleteTutorialStep();
        }

        private void Step7Click(object sender, RoutedEventArgs e)
        {
            CompleteTutorialStep();
        }

        private void Step8Click(object sender, RoutedEventArgs e)
        {
            this.Tutorial8StoryBoard.Stop();

            this.Overlay.Visibility = System.Windows.Visibility.Collapsed;

            //habilitar el Move y el Zoom del mapa
            this.mapControl.AllowMoveAndZoom = true;

            //Vuelvo a su parcela
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.Earthwatcher.Lands.First().GeohexKey);

            //Vuelvo a mostrar el timeline
            this.layerList.Visibility = System.Windows.Visibility.Visible;

            //Termina el tutorial
            Start(replayTutorial);
        }

        void CompleteTutorialStep()
        {
            //Sumo los puntos
            if (Current.Instance.TutorialCurrentStep == 6)
            {
                if (!Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.TutorialCompleted.ToString()))
                {
                    Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.TutorialCompleted.ToString(), Points = ActionPoints.Points(ActionPoints.Action.TutorialCompleted), Published = DateTime.Now });
                    AddPoints(Current.Instance.AddScore);
                }
            }

            switch (Current.Instance.TutorialCurrentStep)
            {
                case 0:
                    this.Overlay.Visibility = System.Windows.Visibility.Visible;
                    //Deshabilitar el Move y el Zoom del mapa
                    this.mapControl.AllowMoveAndZoom = false;
                    this.layerList.Visibility = System.Windows.Visibility.Visible;                                        
                    this.Tutorial1StoryBoard.Begin();
                    LogStep(0);
                    break;
                case 1:
                    this.Overlay2.Visibility = System.Windows.Visibility.Visible;
                    this.layerList.Visibility = System.Windows.Visibility.Collapsed;
                    this.Tutorial3StoryBoard.Begin();
                    this.TutorialMyPlotButton.Visibility = System.Windows.Visibility.Visible;
                    LogStep(1);
                    break;
                case 2:
                    this.TutorialMyPlotButton.Visibility = System.Windows.Visibility.Collapsed;
                    this.Overlay3.Visibility = System.Windows.Visibility.Visible;
                    this.Tutorial5StoryBoard.Begin();
                    LogStep(2);
                    break;
                case 3:
                    landRequest.GetLandById(Configuration.TutorLandId.ToString()); //TODO: cambiar por ID correspondiente al Dummy user
                    LogStep(3);
                    break;
                case 4:
                    this.Overlay.Visibility = System.Windows.Visibility.Visible;
                    this.Overlay.Opacity = 1;
                    this.Tutorial6StoryBoard.Begin();
                    LogStep(4);
                    break;
                case 5:
                    this.Tutorial7StoryBoard.Begin();
                    LogStep(5);
                    break;
                case 6:
                    this.Tutorial8StoryBoard.Begin();
                    LogStep(6);
                    break;
            }

            Current.Instance.TutorialCurrentStep++;
        }

        private void LogStep(int step)
        {
            ActionPoints.Action stepAction = ActionPoints.Action.TutorialStep0;

            string[] list = { ActionPoints.Action.TutorialCompleted.ToString() };
            if (!Current.Instance.Scores.Any(x => list.Contains(x.Action))) // solo si nunca lo completo
            {
                switch (step)
                {
                    case 0: stepAction = ActionPoints.Action.TutorialStep0; break;
                    case 1: stepAction = ActionPoints.Action.TutorialStep1; break;
                    case 2: stepAction = ActionPoints.Action.TutorialStep2; break;
                    case 3: stepAction = ActionPoints.Action.TutorialStep3; break;
                    case 4: stepAction = ActionPoints.Action.TutorialStep4; break;
                    case 5: stepAction = ActionPoints.Action.TutorialStep5; break;
                    case 6: stepAction = ActionPoints.Action.TutorialStep6; break;
                }

                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = stepAction.ToString(), Points = ActionPoints.Points(stepAction), Published = DateTime.Now });
                AddPoints(Current.Instance.AddScore);
            }

        }

        private SignalRClient signalRClient;

        void Start(bool replay)
        {
            Current.Instance.TutorialStarted = false;
            OpacitiesStoryBoard.Begin();

            if (!replay)
            {
                this.StatsControl.Show();

                //SignalR Push Notificacionts
                signalRClient = new SignalRClient(Constants.BaseUrl);
                signalRClient.NotificationReceived += signalRClient_NotificationReceived;
                signalRClient.RunAsync();
                LandChanged(Current.Instance.Earthwatcher.Lands.First());

                //menuRight.Visibility = System.Windows.Visibility.Visible;

                //Cargo todos los lands en Background
                if (Current.Instance.Lands == null)
                {
                    this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
                    landRequest.GetAll(Current.Instance.Earthwatcher.Id);
                }

                //Cargo el mensaje del día
                popupMessageRequests.GetMessage();

                //Veo si tengo que notificar el ganador de un concurso
                contestRequests.GetWinner();

                //Cargo los concursos para mostrar la fecha del resumen diario
                contestRequests.GetContest();
            }
        }

        string contestText;
        void contestRequests_ContestReceived(object sender, EventArgs e)
        {
            Contest contest = sender as Contest;
            if (contest != null && !Current.Instance.Scores.Any(x => (x.Action == ActionPoints.Action.ContestWinnerAnnounced.ToString() && x.Param1 == contest.Id.ToString()) || (x.Action == ActionPoints.Action.ContestWon.ToString() && x.Param1 == contest.Id.ToString())))
            {
                if (contest.WinnerId != null)
                {
                    //Le doy los 10.000 puntos o registro que ya se lo anuncié
                    if (contest.WinnerId.Value == Current.Instance.Earthwatcher.Id)
                    {
                        Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.ContestWon.ToString(), Param1 = contest.Id.ToString(), Points = ActionPoints.Points(ActionPoints.Action.ContestWon), Published = DateTime.UtcNow });
                    }
                    else
                    {
                        Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.ContestWinnerAnnounced.ToString(), Param1 = contest.Id.ToString(), Points = ActionPoints.Points(ActionPoints.Action.ContestWinnerAnnounced), Published = DateTime.UtcNow });
                    }

                    AddPoints(Current.Instance.AddScore);

                    ContestWinner winner = new ContestWinner(contest);
                    winner.Show();
                }
            }
            if (contest != null)
            {
                contestText = string.Format("{0} {1}", Labels.DailySummaryContest, contest.EndDate.ToString("dd/MM/yyyy")); // HH:mm:ss
            }
        }

        void signalRClient_NotificationReceived(object sender, NotificationReceivedEventArgs e)
        {
            //Si alguien tomo la parcela de este usuario conectado
            if (e.Data.M == "LandChanged")
            {
                //Logueo el Land Changed
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.Log.ToString(), Param1 = "Llega a signalRClient_NotificationReceived POR LandChanged", Points = ActionPoints.Points(ActionPoints.Action.Log), Published = DateTime.UtcNow });
                AddPoints(Current.Instance.AddScore);

                if (!Current.Instance.Earthwatcher.Id.ToString().Equals(e.Data.A.Last()) && Current.Instance.Earthwatcher.Lands.Any(x => x.GeohexKey.Equals(e.Data.A.First())))
                {
                    NotificationsWindow notificationsWindow = new NotificationsWindow("ChangeLand");
                    notificationsWindow.LandReassigned += notificationsWindow_LandReassigned;
                    notificationsWindow.Show();
                }
            }
            else if (e.Data.M == ActionPoints.Action.LandVerified.ToString())
            {
                if (Current.Instance.Earthwatcher.Id.ToString().Equals(e.Data.A.First()))
                {
                    NotificationsWindow notificationsWindow = new NotificationsWindow(ActionPoints.Action.LandVerified.ToString());
                    notificationsWindow.LandVerified += notificationsWindow_LandVerified;
                    notificationsWindow.Show();
                }
            }
            else if (e.Data.M == "updateUsersOnlineCount")
            {
                this.StatsControl.UpdateOnlineUsers(e.Data.A.First());

            }
            else if (e.Data.M == "FindTheJaguar")
            {
                int jagPosId = Convert.ToInt32(e.Data.A.First());

                JaguarGameInitialize(jagPosId);
            }
            else if (e.Data.M == "JaguarFound")
            {
                JaguarGameFinalize();

                if (e.Data.A.Last() != Current.Instance.Earthwatcher.Id.ToString())
                {
                    ShowFindTheJaguarFoundPopUp(e.Data.A.First());
                }
            }
            else if (e.Data.M == "FindTheJaguarFinished")
            {
                JaguarGameFinalize();
            }
        }

        void notificationsWindow_LandVerified(object sender, EventArgs e)
        {
            Current.Instance.Scores.Add(new Score { Action = ActionPoints.Action.LandVerified.ToString(), EarthwatcherId = Current.Instance.Earthwatcher.Id, Points = ActionPoints.Points(ActionPoints.Action.LandVerified), Published = DateTime.UtcNow });

            Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.LandVerifiedInformed.ToString(), Points = ActionPoints.Points(ActionPoints.Action.LandVerifiedInformed), Published = DateTime.UtcNow });
            AddPoints(Current.Instance.AddScore);
        }

        private void StartTutorial()
        {
            Current.Instance.TutorialStarted = true;

            WelcomeWindow welcomeWindow = new WelcomeWindow();
            welcomeWindow.PointsAdded += welcomeWindow_PointsAdded;
            welcomeWindow.Closed += welcomeWindow_Closed;
            welcomeWindow.Show();
        }

        void welcomeWindow_Closed(object sender, EventArgs e)
        {
            WelcomeWindowClosed();
        }

        void WelcomeWindowClosed()
        {
            Current.Instance.TutorialCurrentStep = 0;
            this.layerList.Visibility = System.Windows.Visibility.Collapsed;

            this.Overlay.Visibility = System.Windows.Visibility.Visible;
            this.Overlay1.Visibility = System.Windows.Visibility.Visible;
            this.Overlay2.Visibility = System.Windows.Visibility.Visible;
            this.Overlay3.Visibility = System.Windows.Visibility.Visible;
            this.Overlay4.Visibility = System.Windows.Visibility.Visible;
            this.Overlay5.Visibility = System.Windows.Visibility.Visible;
            this.Overlay7.Visibility = System.Windows.Visibility.Visible;

            this.Overlay.Opacity = 1;
            this.Overlay1.Opacity = 1;
            this.Overlay2.Opacity = 1;
            this.Overlay3.Opacity = 1;
            this.Overlay4.Opacity = 1;
            this.Overlay5.Opacity = 1;
            this.Overlay7.Opacity = 1;

            this.Tutorial0.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial1.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial3.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial5.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial6.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial7.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial23.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;
            this.Tutorial8.Visibility = System.Windows.Visibility.Visible;

            this.Tutorial0StoryBoard.Begin();

            //Pongo un loading de unos segundos para darle un feedback al usuario mientras se carga el mapa
            if (!replayTutorial)
            {
                this.loadingAnimText.Text = string.Format("Cargando mapas...{0}Esperá unos segundos para que se termine de cargar el mapa", Environment.NewLine);
                this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void LandChanged(Land land)
        {
            selectedLand = land;

            if (land == null)
                return;

            bool openPoll = true;

            if (!Current.Instance.TutorialStarted)
            {
                //Chequeo Scoring para abrir Modals
                var logins = Current.Instance.Scores.Where(x => x.Action.Equals(ActionPoints.Action.Login.ToString())).OrderByDescending(x => x.Published);
                Score lastLogin = null;
                if (logins.Count() > 1)
                {
                    lastLogin = logins.Skip(1).First();
                }
                if (lastLogin != null)
                {
                    if (Current.Instance.Scores.Any(x => x.Action.Equals("DemandAuthoritiesApproved") && x.Published > lastLogin.Published))
                    {
                        NotificationsWindow notificationsWindow = new NotificationsWindow("DemandAuthoritiesApproved");
                        notificationsWindow.Show();
                        openPoll = false;
                    }

                    if (selectedLand.LastReset > lastLogin.Published)
                    {
                        NotificationsWindow notificationsWindow = new NotificationsWindow("NewLand");
                        notificationsWindow.Show();
                        openPoll = false;
                    }
                }

                var last2 = Current.Instance.Scores.OrderByDescending(x => x.Published).Take(2);
                if (last2.Any(x => x.Action.Equals(ActionPoints.Action.LandVerified.ToString())) && !last2.Any(x => x.Action.Equals(ActionPoints.Action.LandVerifiedInformed.ToString())))
                {
                    NotificationsWindow notificationsWindow = new NotificationsWindow(ActionPoints.Action.LandVerifiedInformed.ToString());
                    notificationsWindow.Show();
                    openPoll = false;
                }

                //Si el ultimo login es de más de 2 horas, abrir el poll
                //if (openPoll && lastLogin != null && DateTime.UtcNow.AddHours(-2) >= lastLogin.Published)
                //{
                //    //TODO: DB. verificar con lucas 
                //    //agrego como condicion final que el feature de poll este desbloqueado.
                //    if(Current.Instance.Features.IsUnlocked(EwFeature.Polls))
                //    {
                //        landRequest.GetVerifiedLandsGeoHexCodes(Current.Instance.Earthwatcher.Id, true);
                //    }
                //}
            }

            if (!Current.Instance.TutorialStarted || Current.Instance.TutorialCurrentStep < 4)
            {
                var hexagonLayer = Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername) as HexagonLayer;

                if (hexagonLayer != null)
                    hexagonLayer.AddHexagon(selectedLand);

                //Inicializar fincas paito
                var basecampLayer = Current.Instance.LayerHelper.FindLayer(Constants.BasecampsLayer) as BasecampLayer;

                if (basecampLayer != null)
                    basecampLayer.LoadData();
            }

            if (string.IsNullOrEmpty(geohexcode))
            {
                if(!string.IsNullOrEmpty(land.GeohexKey))
                {
                    MapHelper.ZoomToHexagon(Current.Instance.MapControl, land.GeohexKey);
                }
                else
                {
                    MapHelper.ZoomToHexagon(Current.Instance.MapControl, "NY8582044"); // Hack: Si viene nulo por algun motivo lo mando a la land del tutor(siempre verde, lockeada)
                    //MessageBox.Show("TEST: MAPA CHICO, LLENDO A PARCELA DEL TUTOR");
                }
            }
            else
            {
                MapHelper.ZoomToHexagon(Current.Instance.MapControl, geohexcode);
            }

            if (Current.Instance.TutorialStarted && Current.Instance.TutorialCurrentStep == 4)
            {
                this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Visible;
                this.Tutorial23StoryBoard.Begin();
            }

          
        }

        private void LandChanged(object sender, EventArgs e)
        {
            var land = sender as Land;
            LandChanged(land);
        }

        private void TxtLogoutClick(object sender, RoutedEventArgs e)
        {
            App.Logout();
            //Earthwatchers.UI.App.BackToLoginPage();
        }

        private void RankingClick(object sender, RoutedEventArgs e)
        {
            var rankingWindow = new Ranking();
            rankingWindow.Show();       
        }

        private void BtnMyLandClick(object sender, RoutedEventArgs e)
        {
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.Earthwatcher.Lands.First().GeohexKey);
        }

        public void SetLegendGraphic(BitmapImage image)
        {
            legendGrid.Visibility = Visibility.Visible;
            legendImage.Source = image;
        }

        /*
         * The map is shown in the middle of the screen, since Silverlight is unable to clipToBounds
         * the grid needs Clip, the rect has to change when the window is resized.
         */
        private void MainPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //clipMap.Rect = new Rect(0, 0, mapWrapper.ActualWidth, mapWrapper.ActualHeight);
        }

        #region mouse events

        Point currentMousePos;
        private void MapControlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentMousePos = e.GetPosition(mapControl);
        }

        private double GetDistanceBetweenPoints(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        private void MapControlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Si movi el mouse mientras apreté entonces es un drag
            var mousePos = e.GetPosition(mapControl);
            if (GetDistanceBetweenPoints(currentMousePos, mousePos) > 5)
            {
                return;
            }

            //Find the Jaguar Logic
            bool isClickingJaguar = false;
            if (Current.Instance.JaguarGame != null) //esto significa que hay un juego findthejaguar corriendo
            {
                if (Current.Instance.MapControl.Viewport.Resolution <= 2.4)
                {
                    var jaguar = Current.Instance.JaguarGame;
                    var sphericalMid = SphericalMercator.FromLonLat(jaguar.Longitude, jaguar.Latitude);
                    var jaguarPoint = new System.Windows.Point(sphericalMid.x, sphericalMid.y);

                    var mouseSpherical = mapControl.Viewport.ScreenToWorld(mousePos.X, mousePos.Y);
                    var mousePoint = new System.Windows.Point(mouseSpherical.X, mouseSpherical.Y);

                    if (GetDistanceBetweenPoints(mousePoint, jaguarPoint) <= 300)
                    {
                        isClickingJaguar = true;
                        var posId = Current.Instance.JaguarGame.Id;
                        Current.Instance.JaguarGame = null; //game over.
                        JaguarRequests requests = new JaguarRequests(Constants.BaseApiUrl);
                        requests.UpdateWinner(Current.Instance.Earthwatcher.Id, posId, Current.Instance.Earthwatcher.FullName);  //Updatear el que lo encontro     

                        Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.FoundTheJaguar.ToString(), Points = ActionPoints.Points(ActionPoints.Action.FoundTheJaguar), Published = DateTime.UtcNow });
                        AddPoints(Current.Instance.AddScore);
                        ShowFindTheJaguarCongratsPopUp();
                    }
                }
            }

            if (!isClickingJaguar)
            {
                if (!isScoreAdding) //esto evita que abran el hexagono nuevamente mientras se está guardando un puntaje
                {
                    if (!Current.Instance.TutorialStarted || (Current.Instance.TutorialStarted && (Current.Instance.TutorialCurrentStep == 3 || Current.Instance.TutorialCurrentStep == 4)))
                    {
                        leftMouseButtonDown = true;

                        hexagonInfo.Initialize();

                        //INTERSECT BASECAMPS  
                        if (layerHelper.FindLayer(Constants.BasecampsLayer).Enabled) // si esta activado el layer de basecamps
                        {

                            var bsphericalCoordinate = mapControl.Viewport.ScreenToWorld(mousePos.X, mousePos.Y);
                            var blonLat = SphericalMercator.ToLonLat(bsphericalCoordinate.X, bsphericalCoordinate.Y);

                            var bfeature = hexagonInfo.GetFeature(blonLat.x, blonLat.y, 7);
                            var bhexCode = GeoHex.Encode(blonLat.x, blonLat.y, 7);
                        }
                        if (!layerHelper.FindLayer(Constants.Hexagonlayername).Enabled)
                            return;

                        var sphericalCoordinate = mapControl.Viewport.ScreenToWorld(mousePos.X, mousePos.Y);
                        var lonLat = SphericalMercator.ToLonLat(sphericalCoordinate.X, sphericalCoordinate.Y);

                        // first try on level 7...
                        var feature = hexagonInfo.GetFeature(lonLat.x, lonLat.y, 7);
                        var hexCode = GeoHex.Encode(lonLat.x, lonLat.y, 7);

                        if (feature == null)
                        {
                            // try on level 6...
                            hexCode = GeoHex.Encode(lonLat.x, lonLat.y, 6);
                            feature = hexagonInfo.GetFeature(lonLat.x, lonLat.y, 6);
                            if (feature == null)
                            {
                                hexagonInfo.Visibility = Visibility.Collapsed;
                                return;
                            }
                        }

                        if (Current.Instance.TutorialStarted)
                        {
                            if (selectedLand != null && selectedLand.GeohexKey.Equals(hexCode))
                            {
                                if (Current.Instance.TutorialCurrentStep == 3)
                                {
                                    this.Tutorial5.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    this.Tutorial23.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                this.Tutorial5Arrow.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            else
                                return;
                        }

                        hexagonInfo.ShowInfo(lonLat.x, lonLat.y, hexCode);
                    }
                }
            }

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
           if (Current.Instance.MapControl.Viewport.Resolution < 1200) //Stop zooming out at this resolution
           {
             mapControl.ZoomOut();
           }
        }

        private void fullScreenButton_click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        private void helpButton_click(object sender, MouseButtonEventArgs e)
        {
            var tutorialMenuWindow = new TutorialMenuWin();
            tutorialMenuWindow.Closed += tutorialMenuWindow_Closed;
            tutorialMenuWindow.PointsAdded += tutorialMenuWindow_PointsAdded;
            tutorialMenuWindow.Show();
        }

        void tutorialMenuWindow_PointsAdded(object sender, EventArgs e)
        {
            AddPoints(Current.Instance.AddScore);
        }

        bool replayTutorial = false;
        void tutorialMenuWindow_Closed(object sender, EventArgs e)
        {
            var tutorialMenuWindow = sender as TutorialMenuWin;
            if (tutorialMenuWindow != null && !string.IsNullOrEmpty(tutorialMenuWindow.SelectedOption))
            {
                if (tutorialMenuWindow.SelectedOption.Equals("PreTutorial") || tutorialMenuWindow.SelectedOption.Equals("PreTutorial1"))
                {
                    WelcomeWindow welcomeWindow = new WelcomeWindow();
                    welcomeWindow.PointsAdded += welcomeWindow_PointsAdded;
                    welcomeWindow.Show();
                }

                if (tutorialMenuWindow.SelectedOption.Equals("Tutorial") || tutorialMenuWindow.SelectedOption.Equals("Tutorial1"))
                {
                    Current.Instance.TutorialStarted = true;
                    replayTutorial = true;
                    WelcomeWindowClosed();
                }

                if (tutorialMenuWindow.SelectedOption.Equals("MiniGame1") || tutorialMenuWindow.SelectedOption.Equals("MiniGame01"))
                {
                    /*
                    TutorialGameWindow gameWindow = new TutorialGameWindow();
                    gameWindow.Closed += gameWindow_Closed;
                    gameWindow.Show();
                     * */
                    TutorialGame2Window gameWindow = new TutorialGame2Window();
                    gameWindow.Closed += gameWindow_Closed;
                    gameWindow.Show();
                }
            }
        }

        void welcomeWindow_PointsAdded(object sender, EventArgs e)
        {
            AddPoints(Current.Instance.AddScore);
        }

        void gameWindow_Closed(object sender, EventArgs e)
        {
            TutorialGame2Window gameWindow = sender as TutorialGame2Window;
            if (gameWindow != null && gameWindow.points > 0)
            {
                var score = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.MiniJuegoI.ToString()).FirstOrDefault();
                if (score != null)
                {
                    if (score.Points < gameWindow.points)
                    {
                        UpdatePoints(ActionPoints.Action.MiniJuegoI.ToString(), gameWindow.points);
                    }
                }
                else
                {
                    Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.MiniJuegoI.ToString(), Points = gameWindow.points, Published = DateTime.Now });
                    AddPoints(Current.Instance.AddScore);
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
            NotificationsWindow notificationsWindow = new NotificationsWindow(ActionPoints.Action.LandReassigned.ToString());
            notificationsWindow.LandReassigned += notificationsWindow_LandReassigned;
            notificationsWindow.Show();
        }

        void notificationsWindow_LandReassigned(object sender, EventArgs e)
        {
            Land land = sender as Land;
            if (land != null)
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.LandReassigned.ToString(), LandId = land.Id, Points = ActionPoints.Points(ActionPoints.Action.LandReassigned), Published = land.StatusChangedDateTime });
                AddPoints(Current.Instance.AddScore);
            }
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

        private void UserControlPanelGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
                UserPanel userPanelWindow = new UserPanel();
                userPanelWindow.Show();
        }


        private void JaguarGameInitialize(int jaguarPositionId)
        {
            if (Current.Instance.Features.IsUnlocked(EwFeature.JaguarGame))
            {
                if (!Current.Instance.JaguarGameStarted)
                {
                    jaguarRequests.GetJaguarGameByID(jaguarPositionId);
                    this.JaguarIcon.Visibility = System.Windows.Visibility.Visible;
                    this.ToggleJaguarIcon.Begin();

                    Current.Instance.JaguarGameStarted = true;
                }
            }
        }

        private void JaguarGameFinalize()
        {
            if (Current.Instance.JaguarGameStarted)
            {
                this.JaguarIcon.Visibility = System.Windows.Visibility.Collapsed;
                this.ToggleJaguarIcon.Stop();
                JaguarLayer jaguarLayer = Current.Instance.LayerHelper.FindLayer(Constants.jaguarLayerName) as JaguarLayer;
                jaguarLayer.ClearJaguar();

                Current.Instance.JaguarGameStarted = false;
            }
        }

        private void ShowFindTheJaguarPopUp()
        {
            var jaguarWindow = new FindTheJaguar();
            jaguarWindow.Show();
        }

        private void ShowFindTheJaguarFoundPopUp(string ewMail)
        {
            var jaguarWindow = new FindTheJaguarFound(ewMail);
            jaguarWindow.Show();
        }

        private void ShowFindTheJaguarCongratsPopUp()
        {
            var jaguarWindow = new FindTheJaguarCongrats();
            jaguarWindow.Show();
        }

        private void jaguarRequests_PositionReceived(object sender, EventArgs e)
        {
            JaguarLayer jaguarLayer = Current.Instance.LayerHelper.FindLayer(Constants.jaguarLayerName) as JaguarLayer;
            Current.Instance.JaguarGame = sender as JaguarGame;
            ToolTipService.SetToolTip(this.JaguarIcon, string.Format(Labels.Main3 , Current.Instance.JaguarGame.GetFinalizationTime().ToString("hh:mm")));
            jaguarLayer.NotifyJaguarReceived();
        }

        private void JaguarIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowFindTheJaguarPopUp();
        }

        
        bool flag = false;
        private void MapControl_zoomFinished(object sender, EventArgs e)
        {
            if (flag == false)
            {
                if (mapControl.Viewport.Resolution <= 2.4)
                {
                    JaguarImg.Visibility = Visibility.Collapsed;
                    BinocularsImg.Visibility = Visibility.Visible;

                }
                else
                {
                    JaguarImg.Visibility = Visibility.Visible;
                    BinocularsImg.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (mapControl.Viewport.Resolution <= 2.4)
                {
                    JaguarImg.Visibility = Visibility.Collapsed;
                    BinocularsImg.Visibility = Visibility.Visible;
                }
                else
                {
                    JaguarImg.Visibility = Visibility.Visible;
                    BinocularsImg.Visibility = Visibility.Collapsed;
                }
            }
            flag = true;
        }

        private void MapControl_zoomStarted(object sender, EventArgs e)
        {
            if (mapControl.Viewport.Resolution <= 2.4)
            {
                JaguarImg.Visibility = Visibility.Collapsed;
                BinocularsImg.Visibility = Visibility.Visible;
            }
            else
            {
                JaguarImg.Visibility = Visibility.Visible;
                BinocularsImg.Visibility = Visibility.Collapsed;
            }
        }

        //LUCAS: al desbloequear este feature se tiene que llamar a este método
        private void AddArgentineLawLayer()
        {
            this.layerList.AddArgentineLawLayer();
            if (Current.Instance.Features.IsNew(EwFeature.ForestLaw))
            {
                this.ArgentineLawTutorial.Begin();
            }
        }

        private void AddBasecampsLayer()
        {
            this.layerList.AddBasecampsLayer();
        }
        
        bool shown = false;

        private void AddImage2008Warning()
        {
            if (Current.Instance.Features.IsNew(EwFeature.Image2008Warning) && shown == false) 
            {
                 shown = true;
                 Image2008Warning popup = new Image2008Warning();
                 popup.Show();
            }
        }

        private void TutorialArgentineLawClick(object sender, RoutedEventArgs e)
        {
            this.TutorialArgentineLaw.Visibility = System.Windows.Visibility.Collapsed;
        }

        void layerList_ArgentineLawLoaded(object sender, EventArgs e)
        {
            //this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
        }

        void layerList_ArgentineLawLoading(object sender, EventArgs e)
        {
            //this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
        }

        private void popupMessageRequests_MessageReceived(object sender, EventArgs e)
        {
            var messageInfo = sender as List<PopupMessage>;
            var read1 = true;
            foreach (var m in messageInfo)
            {
                if (m != null)
                {
                    if (!Current.Instance.Scores.Any(x => x.Action == ActionPoints.Action.DailyMessage.ToString() && x.Param1 == m.Id.ToString()))
                    {
                        if (read1 == true)
                        {
                            read1 = false;
                            //loguear que recibio el dailyMessage
                            Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.DailyMessage.ToString(), Param1 = m.Id.ToString(), Points = ActionPoints.Points(ActionPoints.Action.DailyMessage), Published = DateTime.Now });
                            AddPoints(Current.Instance.AddScore);

                            PopupMessageWindow popup = new PopupMessageWindow(m);
                            popup.Show();
                        }
                    }
                }
            }
        }

        private void showDailySummary(string contestText)
        {
            if(!Current.Instance.Scores.Any(x => x.Action == ActionPoints.Action.DailySummary.ToString() && x.Published.Date == DateTime.Now.Date))
            {
                 //loguear que recibio el DailySummary
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.DailySummary.ToString(), Points = ActionPoints.Points(ActionPoints.Action.DailySummary), Published = DateTime.Now });
                AddPoints(Current.Instance.AddScore);

                var dailySummary = new DailySummary(contestText);
                dailySummary.Show();

            }
        }


        private void _3D_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TestGeoHexTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MapHelper.ZoomToHexagon(Current.Instance.MapControl, this.TestGeoHexTextBox.Text);
            }
        }
    }
}