using System;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class HelpWindow
    {

        public HelpWindow()
        {
           
            InitializeComponent();
            hyperlinkButton_ShareTraining.NavigateUri = new Uri("http://shout.tiged.org/dfahq/writing/");
            hyperlinkButton_video.NavigateUri = new Uri("http://www.youtube.com/watch?v=0UnbVj0_MS8");
            hyperlinkButton_deforestaction.NavigateUri = new Uri("http://www.deforestAction.org");
       hyperlinkButton_Geodan.NavigateUri=new Uri("http://www.geodan.com");
      
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }



        private void txtTutorial2_Click(object sender, RoutedEventArgs e)
        {
           // OpenYoutube();
        }

        private void txtWiki2_Click(object sender, RoutedEventArgs e)
        {
           // OpenWiki();
        }

        private void OpenWiki()
        {
            HtmlPage.Window.Navigate(new Uri(string.Format(CultureInfo.InvariantCulture, "http://shout.tiged.org/dfahq/writing/"), UriKind.Absolute), "_blank");
        }

        private void OpenYoutube()
        {
            HtmlPage.Window.Navigate(new Uri(string.Format(CultureInfo.InvariantCulture, "http://www.youtube.com/watch?v=0UnbVj0_MS8"), UriKind.Absolute), "_blank");
        }

        private void get_started_clicked(object sender, MouseEventArgs e)
        {
            #region setting visibility
            hyperlinkButton_deforestaction.Visibility = Visibility.Visible;
            hyperlinkButton_Geodan.Visibility = Visibility.Visible;
            deforestaction_logo.Visibility = Visibility.Visible;
            geodan_logo.Visibility = Visibility.Visible;
            deforestaction_logo.Visibility = Visibility.Visible;
            geodan_logo.Visibility = Visibility.Visible;
            Previous_button.Visibility = Visibility.Visible;
            Next_button.Visibility = Visibility.Visible;
            #endregion setting visibility
        }

        private void OnNext_Click(object sender, RoutedEventArgs e)
        {
            #region load nenxt images
            if (image1.Visibility == Visibility.Visible)
            {
                Previous_button.IsEnabled = true;
                image1.Visibility = Visibility.Collapsed;
                image2.Visibility = Visibility.Visible;
            }

            else if (image2.Visibility == Visibility.Visible)
            {
                Previous_button.IsEnabled = true;
                image2.Visibility = Visibility.Collapsed;
                image3.Visibility = Visibility.Visible;
            }
            else if (image3.Visibility == Visibility.Visible)
            {
                Previous_button.IsEnabled = true;
                image3.Visibility = Visibility.Collapsed;
                image4.Visibility = Visibility.Visible;
            }
            else if (image3.Visibility == Visibility.Visible)
            {
                Previous_button.IsEnabled = true;
                image3.Visibility = Visibility.Collapsed;
                image4.Visibility = Visibility.Visible;
            }
            else if (image4.Visibility == Visibility.Visible)
            {
                Previous_button.IsEnabled = true;
                image4.Visibility = Visibility.Collapsed;
                image5.Visibility = Visibility.Visible;
            }
            else
            {
                Previous_button.IsEnabled = true;
                Next_button.IsEnabled = false;
            }
            #endregion load next images
        }

        private void OnPrevious_Click(object sender, RoutedEventArgs e)
        {
            #region load previous images
            if (image5.Visibility == Visibility.Visible)
            {
                Next_button.IsEnabled = true;
                image5.Visibility = Visibility.Collapsed;
                image4.Visibility = Visibility.Visible;
            }

            else if (image4.Visibility == Visibility.Visible)
            {
                Next_button.IsEnabled = true;
                image4.Visibility = Visibility.Collapsed;
                image3.Visibility = Visibility.Visible;
            }
            else if (image3.Visibility == Visibility.Visible)
            {
                Next_button.IsEnabled = true;
                image3.Visibility = Visibility.Collapsed;
                image2.Visibility = Visibility.Visible;
            }
            else if (image2.Visibility == Visibility.Visible)
            {
                Next_button.IsEnabled = true;
                image2.Visibility = Visibility.Collapsed;
                image1.Visibility = Visibility.Visible;
            }
            else
            {
                Next_button.IsEnabled = true;
                Previous_button.IsEnabled = false;
            }
            #endregion load previous images
        }

        private void DataPartners_click(object sender, MouseEventArgs e)
        {
            #region setting visibility
            hyperlinkButton_deforestaction.Visibility = Visibility.Visible;
            hyperlinkButton_Geodan.Visibility = Visibility.Visible;
            deforestaction_logo.Visibility = Visibility.Visible;
            geodan_logo.Visibility = Visibility.Visible;
            Previous_button.Visibility = Visibility.Collapsed;
            Next_button.Visibility = Visibility.Collapsed;
            #endregion setting visibility
        }

        private void About_Clicked(object sender, MouseEventArgs e)
        {

      //      txtGeodan.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            Previous_button.Visibility = Visibility.Collapsed;
            Next_button.Visibility = Visibility.Collapsed;

        }


        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataPartners_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

       
       
    }
}

