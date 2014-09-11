using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.UI.Resources;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class FindTheJaguar
    {
        public FindTheJaguar()
        {
            InitializeComponent();
            this.TutorialStoryBoard.Completed += TutorialStoryBoard_Completed;
            this.MainStoryBoard.Completed += MainStoryBoard_Completed;
        }

        void MainStoryBoard_Completed(object sender, EventArgs e)
        {
            this.JaguarTutorial.Visibility = System.Windows.Visibility.Collapsed;
        }

        void TutorialStoryBoard_Completed(object sender, EventArgs e)
        {
            this.MainGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BackLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MainGrid.Visibility = System.Windows.Visibility.Visible;
            this.MainStoryBoard.Begin();
        }

        private void ComoBuscarlo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.JaguarTutorial.Visibility = System.Windows.Visibility.Visible;
            this.TutorialStoryBoard.Begin();
        }

    }
}

