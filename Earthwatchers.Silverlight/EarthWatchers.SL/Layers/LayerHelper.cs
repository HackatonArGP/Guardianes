using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.Layers;
using Mapsui;

namespace EarthWatchers.SL.Layers
{
    public class LayerHelper
    {
        private readonly LayerCollection layerCollection;
        public List<ILayer> BaseLayers { get; private set; }
        public List<ILayer> Layers { get; private set; }
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler BaseLayersChanged;
        public event ChangedEventHandler LayersChanged;

        public LayerHelper(LayerCollection layerCollection)
        {
            Layers = new List<ILayer>();
            BaseLayers = new List<ILayer>();
            this.layerCollection = layerCollection;
        }

        public LayerCollection LayerCollection
        {
            get { return layerCollection; }
        }

        public void AddBaseLayer(ILayer layer)
        {
            BaseLayers.Add(layer);
            layerCollection.Add(layer);
            OnBaseLayersChanged(EventArgs.Empty);
        }

        public void AddLayer(ILayer layer, bool init=true)
        {
            if (init)
            {
                Layers.Add(layer);
                layerCollection.Add(layer); 
            }
            else {
                // todo: refactor code, remove hardcoded ints
                // 3 is number of standard layers (hexagons, land, alerts)
                // layers determinines the order in the TOC
                if (Layers.Count == 1)
                {
                    Layers.Add(layer);
                }
                else Layers.Insert(1,layer);

                // layercollection is the order in drawing, draw always above the baselayers
                layerCollection.Insert(2, layer);
            }
            OnLayersChanged(EventArgs.Empty);
        }

        public ILayer FindLayer(string name)
        {
            return Layers.FirstOrDefault(layer => layer.LayerName.Equals(name));
        }

        public void RemoveLayer(string name)
        {
            var layers = layerCollection.FindLayer(name).ToList();
                        
            for (int i = 0; i < layers.Count(); i++)
            {
                layerCollection.Remove(layers[i]);
                Layers.Remove(layers[i]);
            }

            OnLayersChanged(EventArgs.Empty);
        }
        protected virtual void OnBaseLayersChanged(EventArgs e)
        {
            if (BaseLayersChanged != null)
                BaseLayersChanged(this, e);
        }

        protected virtual void OnLayersChanged(EventArgs e)
        {
            if (LayersChanged != null)
                LayersChanged(this, e);
        }
    }
}
