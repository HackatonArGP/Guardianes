using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class PollsWindow
    {
        private readonly LandRequests landRequests;
        private List<string> codes;
        private string code;
        private bool moreThan5 = false;

        public delegate void BonusReachedHandler(object sender, SharedEventArgs e);
        public event BonusReachedHandler BonusReached;

        public PollsWindow(List<string> _codes)
        {
            InitializeComponent();

            codes = _codes;

            if (codes.Count > 5)
            {
                this.BonusGrid.Visibility = System.Windows.Visibility.Visible;
                this.BonusText.Visibility = System.Windows.Visibility.Visible;
                this.Bonus.Visibility = System.Windows.Visibility.Visible;
                moreThan5 = true;
            }

            if (!string.IsNullOrEmpty(Current.Instance.LastImageDate))
            {
                this.LastSatImgText.Text = Current.Instance.LastImageDate;
            }
            else
            {
                this.LastSatImgText.Text = Labels.Polls6;
            }

            landRequests = new LandRequests(Constants.BaseApiUrl);
            landRequests.PollAdded += landRequests_PollAdded;

            LoadNextImage();
        }

        int answers = 0;
        void landRequests_PollAdded(object sender, EventArgs e)
        {
            answers++;
            if (moreThan5)
            {
                //Pinto de verde los circulos
                var ellipse = this.FindName("Circle" + answers) as System.Windows.Shapes.Ellipse;
                if (ellipse != null)
                {
                    ellipse.Fill = App.Current.Resources["BackGroundEllipseGreen"] as System.Windows.Media.RadialGradientBrush;
                }
                var rectangle = this.FindName("Rectangle" + answers) as System.Windows.Shapes.Rectangle;
                if (rectangle != null)
                {
                    rectangle.Fill = App.Current.Resources["BackGroundRectGreen"] as System.Windows.Media.LinearGradientBrush;
                }

                //Si llegué a las 5 le doy un puntaje
                if (answers == 5)
                {
                    this.BonusStoryBoard.Begin();

                    BonusReached(this, new SharedEventArgs { Action = ActionPoints.Action.FivePollAnswersBonus.ToString(), Points = ActionPoints.Points(ActionPoints.Action.FivePollAnswersBonus) });
                }
            }

            LoadNextImage();
        }

        int countImage = 0;
        int countErrors = 0;
        private void LoadNextImage()
        {
            countImage = 0;
            countErrors = 0;

            if (codes.Count == 0)
            {
                this.Close();
            }
            else
            {
                this.loadinAnim.Visibility = System.Windows.Visibility.Visible;

                //Random
                Random rnd = new Random();
                int index = rnd.Next(codes.Count);


                code = codes[index];

                this.BeforeImage.Source = new BitmapImage(new Uri(string.Format("{0}/demand/{1}-a.jpg", Configuration.ImagesPath, code), UriKind.Absolute));
                this.AfterImage.Source = new BitmapImage(new Uri(string.Format("{0}/demand/{1}-d.jpg", Configuration.ImagesPath, code), UriKind.Absolute));

                codes.RemoveAt(index);
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            countErrors++;
            //Esto esta haciendo que se cierre la ventana porque no encuentra las imágenes
            /*
            if (countErrors == 2)
            {
                this.Close();
            }
             * */
        }

        
        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            countImage++;
            if (countImage == 2)
            {
                this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            if (button.Name == "ButtonDontKnow")
            {
                LoadNextImage();
            }
            else
            {
                LandMini land = new LandMini { GeohexKey = code, EarthwatcherId = Current.Instance.Earthwatcher.Id, IsUsed = button.Name == "ButtonYes" ? true : false };
                landRequests.AddPoll(land);
            }
        }
    }
}

