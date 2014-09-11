using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class TutorialMenuWindow
    {
        public string SelectedOption { get; set; }

        public TutorialMenuWindow()
        {
            InitializeComponent();

            this.Loaded += TutorialMenuWindow_Loaded;
        }

        void TutorialMenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Current.Instance.Scores.Any(x => x.EarthwatcherId == Current.Instance.Earthwatcher.Id && x.Action == "Tutorial2Completed"))
            {
                this.Status2.Text = "Completado - 500 puntos";
                this.Button2.Background = new SolidColorBrush(Color.FromArgb(255, 241, 251, 187));
            }
            else
            {
                this.Status2.Text = "Incompleto - 500 puntos";
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            this.SelectedOption = border.Name;
            this.Close();
        }
    }
}

