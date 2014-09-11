using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.Models;
using System.Collections.Generic;
using EarthWatchers.SL.Requests;
using System;
using EarthWatchers.SL.Layers;
using System.Linq;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class Ranking
    {
        private readonly ScoreRequests scoreRequests;

        public Ranking()
        {
            InitializeComponent();

            scoreRequests = new ScoreRequests(Constants.BaseApiUrl);

            this.Loaded += Ranking_Loaded;
        }

        void Ranking_Loaded(object sender, RoutedEventArgs e)
        {
            scoreRequests.ScoresReceived += scoreRequests_ScoresReceived;
            scoreRequests.GetLeaderBoard(Current.Instance.Earthwatcher.Id);
        }

        void scoreRequests_ScoresReceived(object sender, EventArgs e)
        {
            this.rankingList.ItemsSource = sender as List<Score>;
        }
    }
}

