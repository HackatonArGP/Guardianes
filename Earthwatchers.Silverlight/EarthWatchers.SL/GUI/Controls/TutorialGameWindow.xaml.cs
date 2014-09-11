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
using EarthWatchers.SL.Requests;
using EarthWatchers.SL.Layers;
using EarthWatchers.SL.Resources;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class TutorialGameWindow
    {
        private int currentStep = 1;
        private Dictionary<string, int> GameImages;
        public int points = 0;

        public TutorialGameWindow()
        {
            InitializeComponent();

            GameImages = new Dictionary<string, int>();
            GameImages.Add("juego1-1.png", 1);
            GameImages.Add("juego1-2.png", 1);
            GameImages.Add("juego2-1.png", 2);
            GameImages.Add("juego2-2.png", 2);
            GameImages.Add("juego3-1.png", 3);
            GameImages.Add("juego3-2.png", 3);

            this.Loaded += TutorialGameWindow_Loaded;
        }

        void TutorialGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Step1.Visibility = System.Windows.Visibility.Visible;

            txtName.Text = Current.Instance.Earthwatcher.FullName;
            txtCountry.Text = Current.Instance.Earthwatcher.Country;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep == 1)
            {
                currentStep = 2;
                this.Title.Text = "Landsat 7";
                this.Text1.Visibility = System.Windows.Visibility.Collapsed;
                this.Text2.Visibility = System.Windows.Visibility.Visible;
            }
            else if (currentStep == 2)
            {
                currentStep = 3;
                this.Title.Text = "Imágenes de Color Real";
                this.Step1.Visibility = System.Windows.Visibility.Collapsed;
                this.Step2.Visibility = System.Windows.Visibility.Visible;
                this.NextButton.Content = "JUGAR";
            }
            else if (currentStep == 3)
            {
                currentStep = 4;
                this.NextButton.Visibility = System.Windows.Visibility.Collapsed;
                this.Step2.Visibility = System.Windows.Visibility.Collapsed;
                this.Step3.Visibility = System.Windows.Visibility.Visible;

                ShuffleImages();
            }
            else
            {
                this.Close();
            }
        }

        List<int> ints = new List<int> { 1, 2, 3 };

        void ShuffleImages()
        {
            Random r = new Random();
            int skip = r.Next(0, ints.Count);
            int number = ints.Skip(skip).FirstOrDefault();
            ints.Remove(number);

            var values = GameImages.Where(x => x.Value.Equals(number));
            skip = r.Next(0, 2);
            for (int i = 0; i < 2; i++)
            {
                KeyValuePair<string, int> value;
                if (i == 0)
                {
                    value = values.Skip(skip).First();
                    this.Image1.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", value.Key));

                    if (value.Key.IndexOf("-1.png") > -1)
                    {
                        this.Image1.Tag = "Correct";
                    }
                    else
                    {
                        this.Image1.Tag = "Incorrect";
                    }
                }
                else
                {
                    if (skip == 0)
                    {
                        value = values.Skip(1).First();
                    }
                    else
                    {
                        value = values.Skip(0).First();
                    }
                    this.Image2.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/{0}", value.Key));

                    if (value.Key.IndexOf("-1.png") > -1)
                    {
                        this.Image2.Tag = "Correct";
                    }
                    else
                    {
                        this.Image2.Tag = "Incorrect";
                    }
                }
            }


        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null && image.Tag != null && image.Tag.ToString().Equals("Correct"))
            {
                points += 500;
            }

            if (currentStep < 6)
            {
                ShuffleImages();

                currentStep++;
            }
            else
            {
                this.NextButton.Visibility = System.Windows.Visibility.Visible;
                this.NextButton.Content = "FINALIZAR";

                this.PointsText.Text = string.Format("{0} puntos", points);

                if (points > 0 && points < 1500)
                {
                    this.ResultText.Text = "¡Bien! Lograste encontrar deforestación. Para ser un experto, ¡volvé a jugar hasta encontrar todos los casos y vas a ganar mas puntos!";
                    this.Tree1.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_on.png");
                    if (points == 1000)
                    {
                        this.Tree2.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_on.png");
                    }
                    else
                    {
                        this.Tree2.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_off.png");
                    }
                    this.Tree3.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_off.png");

                }
                else if (points == 1500)
                {
                    this.ResultText.Text = "¡Excelente! Ya sos un experto en Imágenes de Color Real!";
                    this.Tree1.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_on.png");
                    this.Tree2.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_on.png");
                    this.Tree3.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_on.png");
                }
                else
                {

                    this.ResultText.Text = "No lograste obtener árboles... pero no te preocupes, podés volver a intentar! La práctica hace al maestro.";
                    this.Tree1.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_off.png");
                    this.Tree2.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_off.png");
                    this.Tree3.Source = ResourceHelper.GetBitmap("/Resources/Images/tree_off.png");
                }
                this.Step3.Visibility = System.Windows.Visibility.Collapsed;
                this.Step4.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Twitter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string shareText = ""; //TODO
            HtmlPage.Window.Navigate(new Uri(string.Format("http://twitter.com/share?text={0}", shareText), UriKind.Absolute), "_blank");
        }

        private void Facebook_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(string.Format("http://www.facebook.com/sharer.php?u={0}", "http://bit.ly/13iKeEq"), UriKind.Absolute), "_blank");
        }
    }
}

