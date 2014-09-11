using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Resources;
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
    public partial class DailySummary
    {

        private readonly ScoreRequests scoreRequest;
        private readonly ContestRequests contestRequests;

        public DailySummary(string contestText)
        {
            InitializeComponent();

            scoreRequest = new ScoreRequests(Constants.BaseApiUrl);
            contestRequests = new ContestRequests(Constants.BaseApiUrl);

            if (Current.Instance.Scores != null && Current.Instance.Lands != null)
            {
                List<Score> ewScores = Current.Instance.Scores;
                this.Title.Text = Labels.DailySummaryTitle;
                this.HiUser.Text = String.Format("{0} {1}", Labels.DailySummaryHiUser, Current.Instance.Earthwatcher.FullName);

                this.YourPoints.Text = Labels.DailySummaryYourPoints;
                this.Points.Text = (ewScores.Sum(x => x.Points)).ToString();

                this.MorePoints.Text = Labels.DailySummaryMorePoints;


                //Denunciar (Primer Punto)
                this.Denounce1.Text = Labels.DailySummaryDenounce1;
                this.Denounce2.Text = (ewScores.Count(x => x.Action == ActionPoints.Action.DemandAuthorities.ToString())).ToString();
                this.Denounce3.Text = Labels.DailySummaryDenounce3;
                this.Denounce4.Text = (Current.Instance.Lands.Count(x => x.DemandAuthorities == true)).ToString() + ".";
                this.Denounce5.Text = Labels.DailySummaryDenounce5;

                //Verificar (Segundo Punto)
                this.Verify1.Text = Labels.DailySummaryVerify1;
                this.Verify2.Text = (ewScores.Count(x => x.Action == ActionPoints.Action.ConfirmationAdded.ToString())).ToString();
                this.Verify3.Text = Labels.DailySummaryVerify3;
                this.Verify4.Text = (Current.Instance.Lands.Count(x => x.LandStatus == LandStatus.Ok || x.LandStatus == LandStatus.Alert)).ToString() + ".";
                this.Verify5.Text = Labels.DailySummaryVerify5;

                //Reportar (Tercer Punto)
                this.Report1.Text = Labels.DailySummaryReport1;
                this.Report2.Text = (ewScores.Count(x => x.Action == ActionPoints.Action.LandStatusChanged.ToString())).ToString();
                this.Report3.Text = Labels.DailySummaryReport3;

                //Compartir (Cuarto Punto)
                this.Share1.Text = Labels.DailySummaryShare1;
                this.Share2.Text = (ewScores.Count(x => x.Action == ActionPoints.Action.Shared.ToString())).ToString();
                this.Share3.Text = Labels.DailySummaryShare3;

                this.Remember.Text = Labels.DailySummaryRemember;
                this.Contest.Text = contestText;
            }
        }
    }
}

