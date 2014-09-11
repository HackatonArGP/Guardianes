using System.Windows;
using Earthwatchers.UI.Resources;
using Mapsui.Layers;
using Earthwatchers.UI.Layers;

namespace Earthwatchers.UI.GUI.Controls
{
	public partial class LayerControl
	{
        private readonly ILayer layer;

        public LayerControl()
        {
            InitializeComponent();
        }

		public LayerControl(ILayer layer)
		{
			InitializeComponent();
            this.layer = layer;
            txtLayerName.Text = layer.LayerName;

            if (layer is HexagonLayer)
            {
                var hexLayer = layer as HexagonLayer;
                sliderOpacity.Value = hexLayer.Opacity * 100;
                btnDelete.Visibility = Visibility.Collapsed;
            }
            else
            {
                sliderOpacity.Value = layer.Opacity * 100;
            }

            if(layer is AlertedLandLayer || layer is FlagLayer)
            {
                btnDelete.Visibility = Visibility.Collapsed;
            }

		    UderlineLegendLayers();
		}

        private void UderlineLegendLayers()
        {
            //TODO: create normal way to set and show layar legends
            if (layer.LayerName.Equals("Oil Palm and Rubber") || layer.LayerName.Equals("Forest and Peat") ||
                layer.LayerName.Equals(Constants.Hexagonlayername))
            {
                txtLayerName.TextDecorations = TextDecorations.Underline;
            }
        }

	    private void SliderOpacityValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderOpacity == null)//not initialized yet
                return;

            var opacity = sliderOpacity.Value / 100;

            //TODO: Refactor
            if (layer is HexagonLayer)
            {
                var hexLayer = layer as HexagonLayer;
                hexLayer.Opacity = opacity;
            }
            else if (layer is FlagLayer)
            {
                layer.Opacity = opacity;
            }
            else if (layer is AlertedLandLayer)
            {
                layer.Opacity = opacity;
            }
            else if(layer is TileLayer)
            {
                layer.Opacity = opacity;
            }

            RefreshMap();
        }

        private static void RefreshMap()
        {
            Current.Instance.MapControl.OnViewChanged(true);
        }

        private void TxtLayerNameMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {      
            /*
            //TODO: create normal way to set and show layar legends
            if (layer.LayerName.Equals("Oil Palm and Rubber"))
            {
                Current.Instance.Main.legendImage.Source = ResourceHelper.GetBitmap("/Resources/Images/palmrubber.png");
                Current.Instance.Main.legendGrid.Visibility = Visibility.Visible;
            }
            else if (layer.LayerName.Equals("Forest and Peat"))
            {
                Current.Instance.Main.legendImage.Source = ResourceHelper.GetBitmap("/Resources/Images/forest_and_peat.png");
                Current.Instance.Main.legendGrid.Visibility = Visibility.Visible;
            }
            else if (layer.LayerName.Equals(Constants.Hexagonlayername))
            {
                Current.Instance.Main.legendImage.Source = ResourceHelper.GetBitmap("/Resources/Images/hexagon_legend.png");
                Current.Instance.Main.legendGrid.Visibility = Visibility.Visible;
            }
             * */
        }

        private void TxtLayerNameMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Current.Instance.Main.legendGrid.Visibility = Visibility.Collapsed;
        }

        private void closeLayerButton(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (layer.LayerName.Equals(Constants.Hexagonlayername) || layer.LayerName.Equals(Constants.AlertedLandLayername) || layer.LayerName.Equals(Constants.ArgentineLawlayername))
                return;

            Current.Instance.LayerHelper.RemoveLayer(layer.LayerName);

            var hexagonLayer = (HexagonLayer)Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername);
            if (hexagonLayer != null)
            {
                hexagonLayer.UpdateHexagonsInView();
            }
        }
	}
}