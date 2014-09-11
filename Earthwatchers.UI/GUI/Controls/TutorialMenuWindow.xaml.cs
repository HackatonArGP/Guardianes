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
    public partial class TutorialMenuWindow
    {
        public string SelectedOption { get; set; }

        public delegate void PointsAddedEventHandler(object sender, EventArgs e);
        public event PointsAddedEventHandler PointsAdded;

        public TutorialMenuWindow()
        {
            InitializeComponent();

            this.Loaded += TutorialMenuWindow_Loaded;
        }

        void TutorialMenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Status1.Text = string.Format("{0} - {1} {2}", Labels.Help5, 1000, Labels.NavBar9);
            this.Status2.Text = string.Format("{0} - {1} {2}", Labels.Help5, 1000, Labels.NavBar9);

            var score = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.MiniJuegoI.ToString()).OrderByDescending(x => x.Points).FirstOrDefault();
            var scoreScoring = Current.Instance.Scores.Where(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == "ScoringHelp").FirstOrDefault();
            if (score != null)
            {
                this.Status3.Text = string.Format("{0} - {1} {2}", Labels.Help5, score.Points, Labels.NavBar9);
                this.Button3.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
            }
            else
            {
                this.Status3.Text = Labels.Help9;
            }

            if (scoreScoring != null)
            {
                this.ScoresButton.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border.Name == "HelpButton")
            {
                this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.FAQGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else if (border.Name == "ScoresButton")
            {
                this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.ScoringGrid.Visibility = System.Windows.Visibility.Visible;
                if (!Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == ActionPoints.Action.ScoringHelp.ToString()))
                {
                    Current.Instance.AddScore.Add(new Models.Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.ScoringHelp.ToString(), Points = ActionPoints.Points(ActionPoints.Action.ScoringHelp), Published = DateTime.Now });
                    PointsAdded(this, EventArgs.Empty);
                }
            }
            else
            {
                this.SelectedOption = border.Name;
                this.Close();
            }
        }
    }
}

