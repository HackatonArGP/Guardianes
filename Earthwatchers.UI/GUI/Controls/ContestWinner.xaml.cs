using System;
using System.Net;
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

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class ContestWinner
    {
        private readonly ContestRequests contestRequests;
        private Contest contest;

        public ContestWinner(Contest _contest)
        {
            InitializeComponent();

            contest = _contest;

            this.Title.Text = contest.Title;

            if (Current.Instance.Earthwatcher.Id == contest.WinnerId)
            {
                this.WinnerGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.WinnerName.Text = contest.Description;
                this.AnounceWinnerGrid.Visibility = System.Windows.Visibility.Visible;
            }

            this.Loaded += ContestWinner_Loaded;
            this.ShareStoryBoard.Completed += ShareStoryBoard_Completed;
        }

        void ContestWinner_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void ShareStoryBoard_Completed(object sender, EventArgs e)
        {
            this.WinnerGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.AnounceWinnerGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        string facebookTitle = string.Empty;
        private void ShowShareControl()
        {
            //Facebook y Twitter links
            var longUrl = "http://guardianes.greenpeace.org.ar/?fbshare";

            var title = HttpUtility.UrlEncode(facebookTitle);
            var summary = HttpUtility.UrlEncode(this.shareText.Text);
            var shortUrl = ShortenUrl(longUrl);
            var fbUrl = "http://www.facebook.com/sharer.php?s=100&p[medium]=106&p[title]={0}&p[summary]={1}&p[url]={2}";
            this.FacebookButton.NavigateUri = new Uri(string.Format(fbUrl, title, summary, shortUrl), UriKind.Absolute);

            longUrl = "http://guardianes.greenpeace.org.ar/?twshare";
            shortUrl = ShortenUrl(longUrl);
            var finalText = shareText.Text + " " + shortUrl;
            this.TwitterButton.NavigateUri = new Uri(string.Format("https://twitter.com/intent/tweet?text={0}&data-url={1}", Uri.EscapeUriString(finalText).Replace("#", "%23"), shortUrl), UriKind.Absolute);
            //End Facebook y Twitter

            this.FooterGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.ShareGrid.Visibility = System.Windows.Visibility.Visible;
            this.ShareStoryBoard.Begin();
        }

        private string ShortenUrl(string longUrl)
        {
            var bitly = HtmlPage.Window.Invoke("shorten", new string[] { longUrl }) as string;
            return bitly;
        }

        private void Close2Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.WinnerGrid.Visibility == System.Windows.Visibility.Visible)
            {
                this.shareTitle.Text = Labels.Contest6;
                this.shareText.Text = Labels.Share3;
                facebookTitle = Labels.Contest7;
            }
            else
            {
                this.shareTitle.Text = Labels.Share5;
                this.shareText.Text = Labels.Share4;
                facebookTitle = Labels.Contest8;
            }
            ShowShareControl();
        }
    }
}

