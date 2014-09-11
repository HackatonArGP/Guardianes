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
using DonationsViewer.Request;
using DonationsViewer.Models;
using Layers;

namespace DonationsViewer
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            map.Layers.LayersInitialized += Layers_LayersInitialized;
        }

        private void Layers_LayersInitialized(object sender, EventArgs args)
        {
            DonationsViewerSystem.Instance.MainPage = this;
            map.ZoomTo(new ESRI.ArcGIS.Client.Geometry.Envelope(12369034, -32554, 12410034, -32574));
        }
    }
}
