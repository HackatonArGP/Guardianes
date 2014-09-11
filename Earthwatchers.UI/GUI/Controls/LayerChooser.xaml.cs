using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.Models;
using System.Collections.Generic;
using Earthwatchers.UI.Requests;
using System;
using Earthwatchers.UI.Layers;
using System.Linq;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class LayerChooser
    {
        private readonly ImageRequests imageRequest = new ImageRequests(Constants.BaseApiUrl);
        private List<SatelliteImage> satelliteImages;        

        public LayerChooser()
        {
            InitializeComponent();

            if (Current.Instance.TutorialStarted)
            {
                TutorialText.Visibility = System.Windows.Visibility.Visible;
            }

            imageRequest.ImageRequestReceived += ImageRequestImageRequestReceived;

            var topLeft = Current.Instance.MapControl.Viewport.Extent.TopLeft;
            var bottomLeft = Current.Instance.MapControl.Viewport.Extent.BottomLeft;
            var bottomRight = Current.Instance.MapControl.Viewport.Extent.BottomRight;
            var topRight = Current.Instance.MapControl.Viewport.Extent.TopRight;

            var llTopLeft = SphericalMercator.ToLonLat(topLeft.X, topLeft.Y);
            var llbottomLeft = SphericalMercator.ToLonLat(bottomLeft.X, bottomLeft.Y);
            var llbottonRight = SphericalMercator.ToLonLat(bottomRight.X, bottomRight.Y);
            var llTopRight = SphericalMercator.ToLonLat(topRight.X, topRight.Y);

            var wkt = String.Format(CultureInfo.InvariantCulture, "POLYGON(({0} {1},{2} {3},{4} {5},{6} {7},{0} {1}))", llTopLeft.x, llTopLeft.y, llbottomLeft.x, llbottomLeft.y, llbottonRight.x, llbottonRight.y, llTopRight.x, llTopRight.y);

            //imageRequest.GetByExtent(wkt);
            imageRequest.GetAllImagery();
        }

        private void ImageRequestImageRequestReceived(object sender, EventArgs e)
        {
            satelliteImages = (List<SatelliteImage>)sender;
            
            //SQL error hemisphere.... too much zoomed out
            if (satelliteImages == null)
            {
                satelliteInfoHeader.Visibility = Visibility.Collapsed;
                satelliteLayers.Children.Add(new TextBlock { Margin = new Thickness(0, 3, 0, 0), HorizontalAlignment = HorizontalAlignment.Center, Text = "La imágenes satelitales no soportan este nivel de zoom. Aumentá el nivel de zoom y probá nuevamente", Foreground = new SolidColorBrush(Colors.White) });
                return;
            }

            //Sort list by Aquisition DateTime
            satelliteImages.Sort((a, b) => DateTime.Compare(b.Published.Value, a.Published.Value));

            //No images at this location
            if (satelliteImages.Count == 0)
            {
                satelliteInfoHeader.Visibility = Visibility.Collapsed;
                satelliteLayers.Children.Add(new TextBlock {Margin = new Thickness(0,3,0,0), HorizontalAlignment = HorizontalAlignment.Center, Text = "No se encontraron capas en esta región. Dirigite a otra región y probá nuevamente", Foreground = new SolidColorBrush(Colors.White)});
                return;
            } 

            foreach(var si in satelliteImages)
            {
                AddSatelliteImagery(si);
            }            
        }

        private void BtnDoneClick(object sender, RoutedEventArgs e)
        {
            if ((Current.Instance.TutorialStarted) && Current.Instance.LayerHelper.LayerCollection.Count != 4)
            {
                this.TutorialTextBlock.Text = "En el tutorial tenés que seleccionar una imagen satelital para continuar";
                this.TutorialTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                DialogResult = true;
            }
        }

        private void ChildWindowLoaded(object sender, RoutedEventArgs e)
        {
            //LoadEducationalLayers();
        }

        private void AddSatelliteImagery(SatelliteImage satelliteImage)
        {
            var isAdded = true;
            var result = Current.Instance.LayerHelper.LayerCollection.FindLayer(satelliteImage.DateName);
            
            var size = result.Count();
            if (size == 0 )
                isAdded = false;

            satelliteLayers.Children.Add(new SatelliteLayerControl(satelliteImage, isAdded));
        }

        private void LoadEducationalLayers()
        {
            /*
            var educationalLayers = LayerInitialization.GetEducationalLayers();
           
            foreach (var eduLayer in educationalLayers)
            {
                var isAdded = false;

                foreach (var layer in Current.Instance.LayerHelper.LayerCollection)
                {
                    if(layer.LayerName.Equals(eduLayer.Name))
                        isAdded = true;                    
                }

                eduLayerWrapper.Children.Add(new EducationalLayerControl(eduLayer, isAdded));
              
            }
             * */
          
        }
    }
}

