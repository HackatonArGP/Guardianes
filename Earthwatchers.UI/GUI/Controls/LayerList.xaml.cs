using System;
using System.Linq;
using System.Windows;
using Earthwatchers.UI.Layers;

namespace Earthwatchers.UI.GUI.Controls
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
            layerWrapper.Children.Add(new TimeLineControl());

            var hexLayer = layerHelper.FindLayer(Constants.Hexagonlayername);
            if (hexLayer == null)
                return;

            layerWrapper.Children.Add(new LayerControlOnOff(hexLayer));


            //layerHelper.BaseLayersChanged += LayerHelperBaseLayersChanged;
            //layerHelper.LayersChanged += LayerHelperLayersChanged;
        }

        public void StartSliderAnimation()
        {
            var l = layerWrapper.Children.Last() as LayerControl;
            if (l != null)
            {
                l.SliderAnimation.Begin();
            }
        }

        private void LayerHelperLayersChanged(object sender, EventArgs e)
        {
            //layerWrapper.Children.Clear();

            //Add standardlayers
            
            //var alertedlandLayer = layerHelper.FindLayer(Constants.AlertedLandLayername);
            //var flagLayer = layerHelper.FindLayer(Constants.flagLayerName);

            //if (hexLayer == null || alertedlandLayer == null || flagLayer == null)
            //    return;

            
            //layerWrapper.Children.Add(new LayerControl(alertedlandLayer));
            //layerWrapper.Children.Add(new LayerControl(flagLayer));

            /*
            //Add other layers
            for (var i = layerHelper.Layers.Count - 1; i >= 0; i--)
            {
                //if (layerHelper.Layers[i] == hexLayer || layerHelper.Layers[i] == alertedlandLayer || layerHelper.Layers[i] == flagLayer)
                //    continue;

                if (layerHelper.Layers[i] == hexLayer || layerHelper.Layers[i] is LandDemandLayer)
                    continue;

                var layerControl = new LayerControl(layerHelper.Layers[i]);
                layerWrapper.Children.Add(layerControl);
            }
             * */
        }

        /*
        private void LayerHelperBaseLayersChanged(object sender, EventArgs e)
        {
            foreach (var layer in layerHelper.BaseLayers.Cast<BaseTileLayer>().Where(layer => layer.Enabled))
            {
                baselayerControl.SetLayerActive(layer);
            }
        }
         * */
    }
}