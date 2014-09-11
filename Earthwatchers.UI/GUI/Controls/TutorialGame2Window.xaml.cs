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
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Resources;
using System.Windows.Threading;
using System.Net;
using System.Windows.Media.Imaging;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class TutorialGame2Window
    {
        private int currentStep = 0;
        public int points = 0;
        private DispatcherTimer myTimer;
        private int totalImages = 3;

        public TutorialGame2Window()
        {
            InitializeComponent();

            LoadImages();

            //Logo
            this.logo.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", Labels.LogoPathMini));

            this.Step1Storyboard.Completed += Step1Storyboard_Completed;
            this.Step2Storyboard.Completed += Step2Storyboard_Completed;
            this.Step3Storyboard.Completed += Step3Storyboard_Completed;
            this.Step4Storyboard.Completed += Step4Storyboard_Completed;
        }

        void LoadImages()
        {
            Random random = new Random();
            int ok = 0;
            int alert = 0;
            int rnd = 0;
            List<int> oks = new List<int>();
            List<int> alerts = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                oks.Add(i + 1);
                alerts.Add(i + 1);
            }

            for (int i = 0; i < totalImages; i++)
            {
                ok = oks[random.Next(0, 15 - i)];
                alert = alerts[random.Next(0, 15 - i)];

                oks.Remove(ok);
                alerts.Remove(alert);

                rnd = random.Next(1, 3);

                //Ok
                Image image = this.FindName(string.Format("Image{0}{1}", i + 1, rnd == 1 ? 1 : 2)) as Image;
                image.Tag = "Incorrect";
                image.Source = new BitmapImage(new Uri(string.Format("{0}/game1/Oks/{1}.jpg", Configuration.ImagesPath, ok), UriKind.Absolute));

                //Alert
                Image imageAlert = this.FindName(string.Format("Image{0}{1}", i + 1, rnd == 1 ? 2 : 1)) as Image;
                imageAlert.Tag = "Correct";
                imageAlert.Source = new BitmapImage(new Uri(string.Format("{0}/game1/Alerts/{1}.jpg", Configuration.ImagesPath, alert), UriKind.Absolute));
            }

            this.Step0Storyboard.Begin();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        int imagesLoaded = 0;
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            imagesLoaded++;
            if (imagesLoaded == totalImages)
            { 
                this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
                this.NextButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void Step3Storyboard_Completed(object sender, EventArgs e)
        {
            this.Step2.Visibility = System.Windows.Visibility.Collapsed;

            myTimer = new DispatcherTimer();
            myTimer.Interval = TimeSpan.FromMilliseconds(100); // 0.1 second
            myTimer.Tick += myTimer_Tick;
            myTimer.Start();

            this.secondsText.Text = "19";
        }

        int seconds = 19;
        int miliseconds = 10;
        private SolidColorBrush redBrush = new SolidColorBrush(Color.FromArgb(255, 179, 30, 37));

        void myTimer_Tick(object sender, EventArgs e)
        {
            miliseconds--;
            this.milisecondsText.Text = miliseconds.ToString();
            if (miliseconds == 0)
            {
                seconds--;
                this.secondsText.Text = seconds.ToString().PadLeft(2, '0');
                miliseconds = 10;

                if (seconds == 4)
                {
                    this.secondsText.Foreground = redBrush;
                    this.milisecondsText.Foreground = redBrush;
                    this.separatorText.Foreground = redBrush;
                }
            }

            if (seconds <= 0) //20 segundos
            {
                //Termina el juego
                myTimer.Stop();
                EndGame(true);
            }
        }

        void Step2Storyboard_Completed(object sender, EventArgs e)
        {
            this.Step1.Visibility = System.Windows.Visibility.Collapsed;
            
            this.Step3.Visibility = System.Windows.Visibility.Visible;
            this.Step3Storyboard.Begin();
        }

        void Step1Storyboard_Completed(object sender, EventArgs e)
        {
            this.Step0.Visibility = System.Windows.Visibility.Collapsed;
            this.NextButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentStep++;

            if (currentStep == 1)
            {
                this.NextButton.Visibility = System.Windows.Visibility.Collapsed;
                this.Step1.Visibility = System.Windows.Visibility.Visible;
                this.Step1Storyboard.Begin();
            }
            else if (currentStep == 2)
            {
                this.NextButton.Visibility = System.Windows.Visibility.Collapsed;
                this.Step2.Visibility = System.Windows.Visibility.Visible;
                this.Step2Storyboard.Begin();
            }
            else
            {
                this.Close();
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        int answers = 1;
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null && image.Tag != null && image.Tag.ToString().Equals("Correct"))
            {
                points += 500;
                pointsText.Text = string.Format("{0} {1}", points, Labels.NavBar9);
            }

            image.Visibility = System.Windows.Visibility.Collapsed;
            Image otherImage = this.FindName(string.Format("Image{0}{1}", answers, image.Name.EndsWith("1") ? 2 : 1)) as Image;
            if (otherImage != null)
                otherImage.Visibility = System.Windows.Visibility.Collapsed;

            answers++;
            if ((totalImages + 1) > answers)
            {
                Image newimage1 = this.FindName(string.Format("Image{0}1", answers)) as Image;
                if (newimage1 != null)
                    newimage1.Visibility = System.Windows.Visibility.Visible;

                Image newimage2 = this.FindName(string.Format("Image{0}2", answers)) as Image;
                if (newimage2 != null)
                    newimage2.Visibility = System.Windows.Visibility.Visible;
            }
            else
            { 
                //Termino el juego
                myTimer.Stop();
                EndGame(false);
            }
        }

        private bool _endedPrematurely = false;
        private void EndGame(bool endedPrematurely)
        {
            _endedPrematurely = endedPrematurely;
            this.PointsText.Text = points.ToString();
            this.ShareText.Text = string.Format("{0} {1} {2}", Labels.TutorialMiniGame8, points, Labels.TutorialMiniGame9);
            this.Step4.Visibility = System.Windows.Visibility.Visible;
            this.Step4Storyboard.Begin();
        }

        void Step4Storyboard_Completed(object sender, EventArgs e)
        {
            this.Step3.Visibility = System.Windows.Visibility.Collapsed;

            //Time Bonus
            if (!_endedPrematurely)
            {
                this.TimeBonusText.Visibility = System.Windows.Visibility.Visible;
                this.TimeBonusPoints.Text = (seconds * 50).ToString();
                points += (seconds * 50);
                this.TotalPointsText.Text = points.ToString();
                this.TimeBonusStoryboard.Begin();
            }

            this.NextButton.Visibility = System.Windows.Visibility.Visible;
            this.NextButton.Content = Labels.TutorialGame4;
        }

        private void Twitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string shareText = string.Format("{0} {1} {2}", Labels.TutorialMiniGame8, points, Labels.TutorialMiniGame9);
            HtmlPage.Window.Navigate(new Uri(string.Format("http://twitter.com/share?text={0}", shareText), UriKind.Absolute), "_blank");
        }

        private void Facebook_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(string.Format("http://www.facebook.com/sharer.php?u={0}", "http://bit.ly/13iKeEq"), UriKind.Absolute), "_blank");
        }
    }
}

