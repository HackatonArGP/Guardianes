namespace EarthWatchers.SL
{
    using System;
    using System.Windows;
    using EarthWatchers.SL.Requests;
    using Earthwatchers.Models;

    public partial class App
    {
        public App()
        {
            Startup += ApplicationStartup;
            Exit += ApplicationExit;
            UnhandledException += ApplicationUnhandledException;

            InitializeComponent();
        }

        string password = string.Empty;
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if (e.InitParams.ContainsKey("credentials"))
            {
                string[] credentials = e.InitParams["credentials"].Split(':');
                if (credentials.Length == 2 && !string.IsNullOrEmpty(credentials[0]) && !string.IsNullOrEmpty(credentials[1]))
                {
                    EarthwatcherRequests earthwatcherRequest = new EarthwatcherRequests(new Uri(Application.Current.Host.Source, "/api").ToString());
                    earthwatcherRequest.EarthwatcherReceived += earthwatcherRequest_EarthwatcherReceived;
                    password = credentials[1].Trim();
                    earthwatcherRequest.GetByName(credentials[0].Trim(), password);
                }
                else
                {
                    BackToLoginPage();
                }
            }
            else
            {
                BackToLoginPage();
            }
        }

        void earthwatcherRequest_EarthwatcherReceived(object sender, EventArgs e)
        {
            Earthwatcher earthwatcher = sender as Earthwatcher;
            if (earthwatcher != null)
            {
                earthwatcher.Password = password;
                RootVisual = new MainPage(earthwatcher);
            }
            else
            {
                BackToLoginPage();
            }
        }

        public static void BackToLoginPage()
        {
            //Back to login
            string host = Application.Current.Host.Source.AbsoluteUri.Substring(0, Application.Current.Host.Source.AbsoluteUri.Length - 31);
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(host + "/index.html", UriKind.Absolute), "_self");
        }

        private static void ApplicationExit(object sender, EventArgs e)
        {

        }

        private static void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (System.Diagnostics.Debugger.IsAttached) 
                return;

            // NOTE: This will allow the application to continue running after an exception has been thrown
            // but not handled. 
            // For production applications this error handling should be replaced with something that will 
            // report the error to the website and stop the application.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(() => ReportErrorToDom(e));
        }

        private static void ReportErrorToDom(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                var errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
