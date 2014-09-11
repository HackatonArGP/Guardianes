using System.Net;
using System.Windows;
using System.Windows.Browser;
using Earthwatchers.UI.Requests;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class PublishMap
    {
        private bool urlReceived = false;

        public PublishMap()
        {
            InitializeComponent();
        }

        private static string GetBboxString()
        {
            var topLeft = Current.Instance.MapControl.Viewport.Extent.TopLeft;
            var bottomRight = Current.Instance.MapControl.Viewport.Extent.BottomRight;
            var bboxString = string.Format("bbox={0},{1},{2},{3}", topLeft.X.ToString().Replace(',', '.'), topLeft.Y.ToString().Replace(',', '.'), bottomRight.X.ToString().Replace(',', '.'), bottomRight.Y.ToString().Replace(',', '.'));

            return bboxString;
        }

        private static string GetLayerString()
        {
            var layerString = "layers=";
            var layers = Current.Instance.LayerHelper.Layers;
            foreach(var layer in layers)
            {
                if(layer.LayerName.Equals(Constants.Hexagonlayername) || layer.LayerName.Equals(Constants.AlertedLandLayername))
                    continue;

                if (!layerString.Equals("layers="))
                    layerString += ",";

                layerString += layer.LayerName;
            }
            return layerString;
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnCopyUrl_Click(object sender, RoutedEventArgs e)
        {
            var access = AccessGranted();
            if (!access) return;

            Clipboard.SetText(txtBitLy.Text);
            Close();
        }

        private bool AccessGranted()
        {
            if (!urlReceived)
            {
                var warning = new WarningScreen(Labels.PublishMap3);
                warning.Show();

                return false;
            }

            return true;
        }

        private void btnFacebook_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //HtmlPage.Window.Invoke("fbs_click", shortenedUrl);
        }

        private void btnTwitter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //HtmlPage.Window.Invoke("twitter_click", " " + shortenedUrl + " %23earthwatchers");
        }
    }
}

