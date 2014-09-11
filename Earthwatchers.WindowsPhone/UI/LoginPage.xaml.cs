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
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Earthwatchers.WindowsPhone.UI
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = TextBoxUserName.Text.Trim();

            if (!user.Equals(""))
            {
                Settings.Settings.Instance.IsLoggedIn = true;

                //save login name
                if (!PhoneApplicationService.Current.State.ContainsKey("user"))
                {
                    PhoneApplicationService.Current.State.Add("user", user);
                }
                else
                {
                    PhoneApplicationService.Current.State["user"] = user;
                }

                //go back);
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                return;
            }
        }
    }
}