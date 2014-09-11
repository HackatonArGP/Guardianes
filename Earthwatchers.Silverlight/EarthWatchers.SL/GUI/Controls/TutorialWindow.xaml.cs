using System;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using System.Collections.Generic;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class TutorialWindow
    {

        public TutorialWindow()
        {
            InitializeComponent();

            if (Current.Instance.Tutorial2Started)
            {
                this.ButtonClose.Content = "CONTINUAR >>";
                this.Title.Text = "¡Bienvenido a la segunda parte del tutorial!";
                this.Body.Text = "Cuando creás una alerta en tu parcela, los otros Guardianes del Bosque se encargarán de comprobar tu reporte, ayudándote así a quitar todas las dudas. Esta acción es crucial para la protección de nuestros bosques.\r\n\r\n¡Vamos a aprender como se hace!";
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            if (Current.Instance.Tutorial2Started && this.Body2.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.Body2.Visibility = System.Windows.Visibility.Visible;
                this.Tutorial2ChangeText.Begin();
            }
            else
            {
                this.Close();
            }
        }

        private void Tutorial2ChangeText_Completed(object sender, EventArgs e)
        {
            this.Body.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}

