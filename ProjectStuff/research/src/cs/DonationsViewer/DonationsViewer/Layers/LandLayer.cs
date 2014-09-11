using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using DonationsViewer.Request;
using System.Collections.Generic;
using DonationsViewer.Models;
using DonationsViewer.Helpers;
using ESRI.ArcGIS.Client.Symbols;

namespace Layers
{
    public class LandLayer : GraphicsLayer
    {
        private LandRequest _landRequest;

        public LandLayer()
        {
            _landRequest = new LandRequest();
            _landRequest.LandReceived += _landRequest_DonationsReceived;
            _landRequest.GetLand();
        }

        private void _landRequest_DonationsReceived(object sender, EventArgs e)
        {
            var land = sender as List<DonationsLand>;
            if (land == null) return;

            foreach (var l in land)
            {
                var geom = WktConverter.PolygonWktToPolygon(l.geom);
                if (geom == null)
                    continue;
                //FFA52A2A
                var fillSymbol = new SimpleFillSymbol { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = 1, Fill = new SolidColorBrush(Color.FromArgb(80, 176, 196, 222)) };
                var g = new Graphic { Geometry = geom, Symbol = fillSymbol };
                Graphics.Add(g);
            }
        }  
    }
}
