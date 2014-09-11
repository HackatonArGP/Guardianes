using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.Models;
using System.Collections.Generic;
using Earthwatchers.UI.Requests;
using System;
using Earthwatchers.UI.Layers;
using System.Linq;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class Ranking
    {
        private readonly ScoreRequests scoreRequests;
        private readonly ContestRequests contestRequests;

        public Ranking()
        {
            InitializeComponent();

            scoreRequests = new ScoreRequests(Constants.BaseApiUrl);
            contestRequests = new ContestRequests(Constants.BaseApiUrl);

            this.Loaded += Ranking_Loaded;
        }

        void Ranking_Loaded(object sender, RoutedEventArgs e)
        {
            scoreRequests.ScoresReceived += scoreRequests_ScoresReceived;
            scoreRequests.ContestLeaderboardReceived += scoreRequests_ContestLeaderboardReceived;
            contestRequests.ContestReceived += contestRequests_ContestReceived;
            scoreRequests.GetLeaderBoard(Current.Instance.Earthwatcher.Id);
            contestRequests.GetContest();
        }

        void contestRequests_ContestReceived(object sender, EventArgs e)
        {
            Contest contest = sender as Contest;
            if (contest != null)
            {
                scoreRequests.GetContestLeaderBoard(Current.Instance.Earthwatcher.Id);
                this.ContestFooterBorder.Visibility = System.Windows.Visibility.Visible;
                this.VerPremios.Visibility = System.Windows.Visibility.Visible;
                this.ContestFooterTextBox.Text = string.Format("{0} {1}", Labels.Ranking3, contest.EndDate.ToString("dd/MM/yyyy HH:mm:ss"));
                this.TitleContestTextBox.Text = string.Format("Ranking {0}", contest.ShortTitle);
                this.contestGrid.Visibility = System.Windows.Visibility.Visible;

                //Gifts Grid
                this.VerPremios.Visibility = Visibility.Visible;
                this.VolverAlRanking.Visibility = Visibility.Collapsed;
                this.TitleGiftsTextBox.Text = "Premios";
                this.TextGiftDescription.Text = contest.Description;
            }
            else
            {
                this.contestGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void scoreRequests_ContestLeaderboardReceived(object sender, EventArgs e)
        {
            List<Score> scores = sender as List<Score>;
            if (scores == null || scores.Count == 0)
            {
                this.contestGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.rankingContestList.ItemsSource = sender as List<Score>;
            }

            this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
        }

        void scoreRequests_ScoresReceived(object sender, EventArgs e)
        {
            this.rankingList.ItemsSource = sender as List<Score>;
        }

        void Premios_Click(object sender, RoutedEventArgs e)
        {
            this.VerPremios.Visibility = Visibility.Collapsed;
            this.VolverAlRanking.Visibility = Visibility.Visible;
            this.contestGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.giftsGrid.Visibility = Visibility.Visible;
        }

        void VolverAlRanking_Click(object sender, RoutedEventArgs e)
        {
            this.VerPremios.Visibility = Visibility.Visible;
            this.VolverAlRanking.Visibility = Visibility.Collapsed;
            this.contestGrid.Visibility = System.Windows.Visibility.Visible;
            this.giftsGrid.Visibility = Visibility.Collapsed;
        }
    }
}

