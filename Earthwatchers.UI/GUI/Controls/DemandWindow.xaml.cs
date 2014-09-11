using Earthwatchers.Models;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class DemandWindow 
    {
        public delegate void DemandEventHandler(object sender, EventArgs e);
        public event DemandEventHandler Demand;

        private Land selectedLand;
        private Earthwatcher selectedEarthwatcher;
        private readonly HexagonLayer hexagonLayer;
       
        public DemandWindow(Land land, Earthwatcher earthwatcher)
        {
            InitializeComponent();

            selectedLand = land;
            selectedEarthwatcher = earthwatcher;
            hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);
            
            if (Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.DemandAuthorities.ToString()) && (x.LandId == selectedLand.Id)))
            {
                this.DemandIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandarShare.png");
                this.Title.Text = Labels.DemandWindow2;
                this.DemandText.Text = Labels.DemandWindow3;
                this.DemandText2.Text = Labels.DemandWindow4;
                this.DemandTitleText.Text = Labels.DemandWindow2;
            }
            else
            {
                this.DemandIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/demandar.png");
                this.Title.Text = Labels.DemandWindow1;
                this.DemandText.Text = Labels.DemandWindow5;
                this.DemandText2.Text = Labels.DemandWindow6;
                this.DemandTitleText.Text = Labels.DemandWindow1;
            }


        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DemandButton_Click(object sender, RoutedEventArgs e)
        {
            //Al hacer click redirige a la pagina de denuncias
            string basecamp;
            if (selectedLand.BasecampId != null)
            {
                basecamp = selectedLand.BasecampId.ToString();
            }
            else
                basecamp = "salta01";

            Uri demandUri = new Uri(string.Format("http://greenpeace.org.ar/denuncias/index.php?id_ciberaccion={0}&mail={1}&area={2}&GeohexKey={3}&prev=0&lat={4}&long={5}", 5157, Current.Instance.Earthwatcher.Name, basecamp, selectedLand.GeohexKey, Math.Round(selectedLand.Latitude, 4), Math.Round(selectedLand.Longitude, 4)), UriKind.Absolute);
            this.DemandButton.NavigateUri = demandUri;

            //Agrega el score de demanda
            if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.DemandAuthorities.ToString()) && x.LandId == selectedLand.Id))
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.DemandAuthorities.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.DemandAuthorities), Published = DateTime.Now });
                Demand(this, EventArgs.Empty);
                hexagonLayer.UpdateHexagonsInView();
            }
            //Agrega el score de Share
            else if (!Current.Instance.Scores.Any(x => x.Action.Equals(ActionPoints.Action.Shared.ToString()) && x.LandId == selectedLand.Id))
            {
                Current.Instance.AddScore.Add(new Score { EarthwatcherId = Current.Instance.Earthwatcher.Id, Action = ActionPoints.Action.Shared.ToString(), LandId = selectedLand.Id, Points = ActionPoints.Points(ActionPoints.Action.Shared), Published = DateTime.Now });
                Demand(this, EventArgs.Empty);
            }
        }
    }
}

