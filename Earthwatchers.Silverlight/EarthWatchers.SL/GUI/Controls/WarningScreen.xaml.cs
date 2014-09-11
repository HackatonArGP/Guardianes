using System.Windows;
using System;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class WarningScreen
    {
        public WarningScreen(String warning)
        {
            InitializeComponent();
            txtWarning.Text = warning;
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

