using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.Layers;
using Mapsui;

namespace Earthwatchers.UI.Layers
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
           
            OnLayersChanged(EventArgs.Empty);
        }

        public void InsertLayer(ILayer layer, int index)
        {
            Layers.Insert(index, layer);
            layerCollection.Insert(index + 1, layer);

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
