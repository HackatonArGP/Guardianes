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
    public partial class FindTheJaguarFound
    {
        public FindTheJaguarFound(string ewMail)
        {
            InitializeComponent();

            winner.Text = ewMail;
        }
    }
}

