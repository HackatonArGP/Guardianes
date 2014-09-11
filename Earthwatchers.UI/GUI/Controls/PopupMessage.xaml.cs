using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class PopupMessageWindow
    {

        private readonly PopupMessageRequests popupRequests;
        string imageUrl = null;

        public PopupMessageWindow(PopupMessage messageInfo)
        {
            InitializeComponent();

            popupRequests = new PopupMessageRequests(Constants.BaseApiUrl);

            this.Title = messageInfo.ShortTitle;
            this.TitleTextBox.Text = messageInfo.Title;
            this.Message.Text = string.Format(messageInfo.Description, Environment.NewLine);

            if (!string.IsNullOrEmpty(messageInfo.ImageURL))
            {
                this.ImageStackPanel.Visibility = Visibility.Visible;
                imageUrl = messageInfo.ImageURL;
                this.Image1.Source = new BitmapImage(new Uri(string.Format("{0}/messages/{1}", Configuration.ImagesPath, messageInfo.ImageURL), UriKind.Absolute));
            }
        }

        private void Image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void ImageToNewTab_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                HtmlPage.Window.Navigate(new Uri("http://www.guardianes.greenpeace.org.ar/SatelliteImages/messages/" + imageUrl), "_blank");
            }
        }
    }
}

