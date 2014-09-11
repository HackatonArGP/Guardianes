using System.Windows.Input;
using EarthWatchers.SL.Layers;
using EarthWatchers.SL.Resources;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class BaseLayerControl
    {
        private BaseTileLayer currentLayer;

        public BaseLayerControl()
        {
            InitializeComponent();
        }

        public void SetLayerActive(BaseTileLayer layer)
        {
            if (currentLayer != null)
                currentLayer.Enabled = false;            

            layer.Enabled = true;
            currentLayer = layer;

            layerImage.Source = ResourceHelper.GetBitmap(layer.TumbnailPath);

            System.Windows.Controls.ToolTipService.SetToolTip(layerImage, layer.LayerName);
            //txtLayerName.Text = layer.LayerName;

            var mapControl = Current.Instance.MapControl;
            mapControl.clearCopyright();

            if (currentLayer.CopyrightImage != null)
                mapControl.SetCopyrightImage(ResourceHelper.GetBitmap(currentLayer.CopyrightImage));

            if (currentLayer.CopyrightText != null)
                mapControl.SetCopyrightText(currentLayer.CopyrightText);
           
            mapControl.OnViewChanged(true);
        }

        private void BtnSwitchLeftMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var layers = Current.Instance.LayerHelper.BaseLayers;

            if (currentLayer == null || layers.Count <= 1)
                return;

            for (var i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i] != currentLayer) 
                    continue;

                if (i - 1 >= 0)
                    SetLayerActive(layers[i - 1] as BaseTileLayer);
                else
                    SetLayerActive(layers[layers.Count - 1] as BaseTileLayer);

                break;
            }
        }

        private void BtnSwitchRightMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var layers = Current.Instance.LayerHelper.BaseLayers;

            if (currentLayer == null || layers.Count <= 1)
                return;

            for (var i = 0; i < layers.Count; i++)
            {
                if (layers[i] != currentLayer) 
                    continue;

                if ((i + 1) <= (layers.Count - 1))
                    SetLayerActive(layers[i + 1] as BaseTileLayer);
                else
                    SetLayerActive(layers[0] as BaseTileLayer);

                break;
            }
        }
    }
}
