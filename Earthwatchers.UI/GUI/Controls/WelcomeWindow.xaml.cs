using System;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using Earthwatchers.UI.Resources;
using Earthwatchers.Models;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class WelcomeWindow
    {
        public delegate void PointsAddedEventHandler(object sender, EventArgs e);
        public event PointsAddedEventHandler PointsAdded;

        public WelcomeWindow()
        {
            InitializeComponent();

            this.StartStoryBoard.Completed += WelcomeStoryBoard_Completed;
            this.Welcome2StoryBoard.Completed += WelcomeStoryBoard_Completed;
            this.Welcome3StoryBoard.Completed += WelcomeStoryBoard_Completed;
            this.Welcome4StoryBoard.Completed += WelcomeStoryBoard_Completed;

            //Logo
            this.logo.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", Labels.LogoPathMini));

            this.Loaded += WelcomeWindow_Loaded;
        }

        void WelcomeStoryBoard_Completed(object sender, EventArgs e)
        {
            this.Next1.IsHitTestVisible = true;
        }

        void WelcomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.WelcomeStoryBoard.Begin();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;
            this.StartStoryBoard.Begin();

            Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.PreTutorialStep1.ToString(), Points = ActionPoints.Points(ActionPoints.Action.PreTutorialStep1), Published = DateTime.Now });
            PointsAdded(this, EventArgs.Empty);
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        int currentStep = 0;
        private void Next1_Click(object sender, RoutedEventArgs e)
        {
            this.Next1.IsHitTestVisible = false;

            currentStep++;
            if (currentStep == 2)
            {
                this.Welcome2.Visibility = System.Windows.Visibility.Visible;
                this.Welcome2StoryBoard.Begin();

                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.PreTutorialStep2.ToString(), Points = ActionPoints.Points(ActionPoints.Action.PreTutorialStep2), Published = DateTime.Now });
                PointsAdded(this, EventArgs.Empty);
            }
            else if (currentStep == 3)
            {
                this.Welcome1.Visibility = System.Windows.Visibility.Collapsed;
                this.Welcome3.Visibility = System.Windows.Visibility.Visible;
                this.Welcome3StoryBoard.Begin();

                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.PreTutorialStep3.ToString(), Points = ActionPoints.Points(ActionPoints.Action.PreTutorialStep3), Published = DateTime.Now });
                PointsAdded(this, EventArgs.Empty);
            }
            else if (currentStep == 4)
            {
                this.Welcome2.Visibility = System.Windows.Visibility.Collapsed;
                this.Welcome4.Visibility = System.Windows.Visibility.Visible;
                this.Welcome4StoryBoard.Begin();

                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.PreTutorialStep4.ToString(), Points = ActionPoints.Points(ActionPoints.Action.PreTutorialStep4), Published = DateTime.Now });
                PointsAdded(this, EventArgs.Empty);
            }
            else
            {
                this.Close();
            }
        }
    }
}

