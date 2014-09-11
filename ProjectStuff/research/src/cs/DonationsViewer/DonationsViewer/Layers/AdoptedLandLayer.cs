using System;
using ESRI.ArcGIS.Client;
using DonationsViewer.Models;
using System.Collections.Generic;
using DonationsViewer.Helpers;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Media;
using ESRI.ArcGIS.Client.Geometry;
using DonationsViewer.Request;
using DonationsViewer;

namespace Layers
{
    public class AdoptedLandLayer : GraphicsLayer
    {
        private Random random;
        private DonationsRequest _donationsRequest;

        public AdoptedLandLayer()
        {
            random = new Random();

            _donationsRequest = new DonationsRequest();
            _donationsRequest.DonationsReceived += _donationsRequest_DonationsReceived;
            _donationsRequest.GetDonations();
        }

        private void _donationsRequest_DonationsReceived(object sender, EventArgs e)
        {            
            var adopters = sender as List<Adopter>;
            if (adopters == null) return;

            DonationsViewerSystem.Instance.MainPage.overviewControl.UpdateOverview(adopters);

            foreach (var adopter in adopters)
            {
                //Create symbols and place it on the map
                var geom = WktConverter.PolygonWktToPolygon(adopter.geom);
                if (geom == null)
                    continue;

                var fillSymbol = new SimpleFillSymbol { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = 1, Fill = new SolidColorBrush(GetRandomColor()) };
                var g = new Graphic { Geometry = geom, Symbol = fillSymbol };
                g.Attributes.Add("username", adopter.username);
                g.Attributes.Add("area", adopter.area);
                g.Attributes.Add("amount", adopter.amount);
                Graphics.Add(g);
            }
        }          

        private Color GetRandomColor()
        {
            return Color.FromArgb(120, byte.Parse(random.Next(0, 255).ToString()), byte.Parse(random.Next(0, 255).ToString()), byte.Parse(random.Next(0, 255).ToString()));
        }
    }
}
