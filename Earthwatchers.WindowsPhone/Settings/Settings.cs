using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.WindowsPhone.Settings
{
    public sealed class Settings
    {
        static readonly string VERSION = "0.0.01";
        static readonly string BUILD = "2011.19.05";

        static readonly Settings instance = new Settings();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static Settings()
        {
        }

        Settings()
        {
        }

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////

        public bool IsLoggedIn { get; set; }

    }
}
