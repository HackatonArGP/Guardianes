using System.Windows;
using Earthwatchers.UI.Layers;
using Mapsui.Layers;
using System;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class EducationalLayerControl
    {
        private bool isAdded;
        private readonly bool initialized;
        private readonly EducationalLayer educationalLayer;

        public EducationalLayerControl(EducationalLayer educationalLayer, bool isAdded)
        {
            InitializeComponent();
            this.isAdded = isAdded;
            this.educationalLayer = educationalLayer;
            txtName.Text = this.educationalLayer.Name;
            txtInfo.NavigateUri = new Uri(educationalLayer.WikiUrl);
            xhkAdd.IsChecked = this.isAdded;
            initialized = true;
        }

        public bool IsAdded 
        {
            get { return isAdded; }
            set
            {
                if (!initialized) return;
                
                isAdded = value;
                if (value)
                {
                    var tms = new DfaTileSource(educationalLayer.Url);
                    Current.Instance.LayerHelper.AddLayer(new TileLayer(tms) { LayerName = educationalLayer.Name });
                }
                else
                {
                    Current.Instance.LayerHelper.RemoveLayer(educationalLayer.Name);
                }
            }
        }

        private void XhkAddChecked(object sender, RoutedEventArgs e)
        {
            IsAdded = true;
        }

        private void XhkAddUnchecked(object sender, RoutedEventArgs e)
        {
            IsAdded = false;
        }
    }
}
