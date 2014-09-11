using System;
using System.Linq;
using System.Windows;
using EarthWatchers.SL.Layers;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class LayerList
    {
        private LayerHelper layerHelper;
        public delegate void AddingLayerEventHandler(object sender, EventArgs e);
        public delegate void LayerAddedEventHandler(object sender, EventArgs e);
        public event AddingLayerEventHandler AddingLayer;
        public event LayerAddedEventHandler LayerAdded;
        
        public LayerList()
        {
            InitializeComponent();
        }

        public void SetLayerHelper(LayerHelper layerHelper)
        {
            this.layerHelper = layerHelper;
            layerHelper.BaseLayersChanged += LayerHelperBaseLayersChanged;
            layerHelper.LayersChanged += LayerHelperLayersChanged;
        }

        private void LayerHelperLayersChanged(object sender, EventArgs e)
        {
            layerWrapper.Children.Clear();

            //Add standardlayers
            var hexLayer = layerHelper.FindLayer(Constants.Hexagonlayername);
            //var alertedlandLayer = layerHelper.FindLayer(Constants.AlertedLandLayername);
            //var flagLayer = layerHelper.FindLayer(Constants.flagLayerName);

            //if (hexLayer == null || alertedlandLayer == null || flagLayer == null)
            //    return;

            if (hexLayer == null)
                return;

            layerWrapper.Children.Add(new LayerControl(hexLayer));
            //layerWrapper.Children.Add(new LayerControl(alertedlandLayer));
            //layerWrapper.Children.Add(new LayerControl(flagLayer));


            //Add other layers
            for (var i = layerHelper.Layers.Count - 1; i >= 0; i--)
            {
                //if (layerHelper.Layers[i] == hexLayer || layerHelper.Layers[i] == alertedlandLayer || layerHelper.Layers[i] == flagLayer)
                //    continue;

                if (layerHelper.Layers[i] == hexLayer)
                    continue;

                var layerControl = new LayerControl(layerHelper.Layers[i]);
                layerWrapper.Children.Add(layerControl);
            }
        }

        private void LayerHelperBaseLayersChanged(object sender, EventArgs e)
        {
            foreach (var layer in layerHelper.BaseLayers.Cast<BaseTileLayer>().Where(layer => layer.Enabled))
            {
                baselayerControl.SetLayerActive(layer);
            }
        }

        private void addLayerButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Current.Instance.TutorialStarted || Current.Instance.Tutorial2Started)
            {
                AddingLayer(this, EventArgs.Empty);
            }

            var lc = new LayerChooser();
            lc.Closed += lc_Closed;
            lc.Show();
        }

        void lc_Closed(object sender, EventArgs e)
        {
            if (Current.Instance.TutorialStarted || Current.Instance.Tutorial2Started)
            {
                LayerAdded(this, EventArgs.Empty);
            }
        }
    }
}