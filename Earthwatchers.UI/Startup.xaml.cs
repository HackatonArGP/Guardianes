using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Earthwatchers.Models;
using Earthwatchers.UI.Requests;
using System.Globalization;
using System.Windows.Browser;

namespace Earthwatchers.UI
{
    public partial class Startup : UserControl
    {
        string password = string.Empty;
        string geohexcode = string.Empty;

        public Startup(IDictionary<string,string> initParams)
        {
            InitializeComponent();

            if (initParams.ContainsKey("authtoken"))
            {
                string[] authtoken = initParams["authtoken"].Split(':');
                if (authtoken[0] == "true")
                {
                    EarthwatcherRequests earthwatcherRequest = new EarthwatcherRequests(new Uri(Application.Current.Host.Source, "/api").ToString());
                    earthwatcherRequest.EarthwatcherReceived += earthwatcherRequest_EarthwatcherReceived;
                    earthwatcherRequest.GetLogged();
                }
            }
            else
                if (initParams.ContainsKey("api"))
                {
                    ApiEw ew = new ApiEw();

                    ew.Api = initParams["api"];
                    ew.UserId = initParams["userId"];
                    ew.NickName = initParams["nickName"];
                    ew.AccessToken = initParams["accessToken"];
                    //password = ew.UserId.ToString();

                    EarthwatcherRequests earthwatcherRequest = new EarthwatcherRequests(new Uri(Application.Current.Host.Source, "/api").ToString());
                    earthwatcherRequest.LoginWithApi(ew);
                    earthwatcherRequest.ApiEwReceived += earthwatcherRequest_ApiEwReceived;
                }
            if (initParams.ContainsKey("credentials"))
                {
                    string[] credentials = initParams["credentials"].Split(':');
                    if (credentials.Length >= 2 && !string.IsNullOrEmpty(credentials[0]) && !string.IsNullOrEmpty(credentials[1]))
                    {
                        geohexcode = credentials[2].Trim();

                        EarthwatcherRequests earthwatcherRequest = new EarthwatcherRequests(new Uri(Application.Current.Host.Source, "/api").ToString());
                        earthwatcherRequest.EarthwatcherReceived += earthwatcherRequest_EarthwatcherReceived;
                        password = credentials[1].Trim();
                        earthwatcherRequest.GetByName(credentials[0].Trim(), password);
                    }
                    else
                    {
                        Earthwatchers.UI.App.BackToLoginPage();
                    }
                }
        }

        void earthwatcherRequest_EarthwatcherReceived(object sender, EventArgs e)
        {
            Earthwatcher earthwatcher = sender as Earthwatcher;
            if (earthwatcher != null)
            {
                if (earthwatcher.Language == null || earthwatcher.Language.Equals("Spanish", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-AR");
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-AR");
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }

                earthwatcher.Password = password;
                SwitchControl(new MainPage(earthwatcher, geohexcode));
            }
            else
            {
                Earthwatchers.UI.App.BackToLoginPage();
                //string host = Application.Current.Host.Source.AbsoluteUri.Substring(0, Application.Current.Host.Source.AbsoluteUri.Length - 39);
                //System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("/index.html?action=logout", UriKind.Absolute), "_self");
            }
        }

        public void SwitchControl(UserControl newControl)
        {
            LayoutRoot.Children.Clear();
            if (newControl != null)
            {
                LayoutRoot.Children.Add(newControl);
            }
        }

        void earthwatcherRequest_ApiEwReceived(object sender, EventArgs e)
        {
            Earthwatcher earthwatcher = sender as Earthwatcher;
            if (earthwatcher != null)
            {
                if (earthwatcher.Language == null || earthwatcher.Language.Equals("Spanish", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-AR");
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-AR");
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }

                //earthwatcher.Password = password;
                SwitchControl(new MainPage(earthwatcher, geohexcode));
            }
            else
            {
                Earthwatchers.UI.App.BackToLoginPage();
            }
        }
    }

}
