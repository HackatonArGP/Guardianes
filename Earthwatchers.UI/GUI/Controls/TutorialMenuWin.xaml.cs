using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.UI.Resources;
using Earthwatchers.Models;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class TutorialMenuWin
    {
        public string SelectedOption { get; set; }

        public delegate void PointsAddedEventHandler(object sender, EventArgs e);
        public event PointsAddedEventHandler PointsAdded;

        public TutorialMenuWin()
        {
            InitializeComponent();

            this.Loaded += TutorialMenuWindow_Loaded;

            this.txtPointsLogin.Text = ActionPoints.Points(ActionPoints.Action.Login).ToString();
            this.txtPointsTutorialCompleted.Text = ActionPoints.Points(ActionPoints.Action.TutorialCompleted).ToString();
            this.txtPointsConfirmationAdded.Text = ActionPoints.Points(ActionPoints.Action.ConfirmationAdded).ToString();
            this.txtPointsLandStatusChanged.Text = ActionPoints.Points(ActionPoints.Action.LandStatusChanged).ToString();
            this.txtPointsShared.Text = ActionPoints.Points(ActionPoints.Action.Shared).ToString();
            this.txtPointsDemandAuthorities.Text = ActionPoints.Points(ActionPoints.Action.DemandAuthorities).ToString();
            this.txtPointsGames.Text = ActionPoints.Points(ActionPoints.Action.MiniJuegoI).ToString();
        }

        void TutorialMenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var score = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.MiniJuegoI.ToString()).OrderByDescending(x => x.Points).FirstOrDefault();
            var scoreScoring = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.ScoringHelp.ToString()).FirstOrDefault();
            if (score != null)
            {
                this.PlayMg1.Visibility = System.Windows.Visibility.Collapsed;
                this.MiniGame1Points.Text = string.Format("{0} {1}", score.Points, Labels.NavBar9);
            }
            else
            {
                this.PlayMg1.Visibility = System.Windows.Visibility.Visible;
                
            }

        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement elem = sender as FrameworkElement;

            if (elem.Name == "HelpButton")
            {
                this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.FAQGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else if (elem.Name == "ScoresButton")
            {
                this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.ScoringGrid.Visibility = System.Windows.Visibility.Visible;
                if (!Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.ScoringHelp.ToString()))
                {
                    Current.Instance.AddScore.Add(new Models.Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.ScoringHelp.ToString(), Points = ActionPoints.Points(ActionPoints.Action.ScoringHelp), Published = DateTime.Now });
                    PointsAdded(this, EventArgs.Empty);
                }
            }
            else if (elem.Name == "AboutButton")
            {
                this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.AboutGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.SelectedOption = elem.Name;
                this.Close();
            }
        }

        private void BackToMainHelph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            ScoringGrid.Visibility = Visibility.Collapsed;
            FAQGrid.Visibility = Visibility.Collapsed;
            AboutGrid.Visibility = Visibility.Collapsed;
        }
    }
}



