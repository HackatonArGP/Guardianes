using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using Earthwatchers.WindowsPhone.Helpers;
using System.Diagnostics;

namespace Earthwatchers.WindowsPhone
{
    public partial class MapPage : PhoneApplicationPage
    {
        private bool _isLoggingIn;
        private bool _getHexaCode = true;
        private string _hexa;
        private string _user;
        private Land _land = null;

        public MapPage()
        {
            InitializeComponent();
        }

        private void Init()
        {
            if (!Settings.Settings.Instance.IsLoggedIn && !_isLoggingIn)
            {
                _isLoggingIn = true;
                _hexa = string.Empty;

                string url = string.Format("/UI/LoginPage.xaml");
                Uri uri = new Uri(url, UriKind.Relative);
                if (this.NavigationService != null)
                    this.NavigationService.Navigate(uri);
            }
            else if (Settings.Settings.Instance.IsLoggedIn)
            {
                if (_getHexaCode)
                {
                    _getHexaCode = false;
                }
            }
        }

        void CreateHex()
        {
            if (_hexa.Equals("")) return;
            GeoHex.Zone zone = GeoHex.GeoHex.Decode(_hexa);
            GeoHex.Loc[] locations = zone.getHexCoords();
            if (locations == null || locations.Count() != 6) return;

            map1.Center = new GeoCoordinate(zone.lat, zone.lon);

            var collection = new LocationCollection();

            foreach (var location in locations)
            {
                var geo = new GeoCoordinate(location.lat, location.lon);
                collection.Add(geo);
            }
            mapPolygon.Locations = collection;

            PolygonLayer.Visibility = Visibility.Visible;
        }
       
        private void PhoneApplicationPage_LayoutUpdated(object sender, EventArgs e)
        {
            Init();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (PhoneApplicationService.Current.State.ContainsKey("user"))
            {
                _user = PhoneApplicationService.Current.State["user"].ToString();

                //call service to get the Land object
                //--when no object is returned, or not valid, the login failed
                if (_user != string.Empty && _hexa == string.Empty && _land == null)
                {
                    GetLandHelper landHelper = new GetLandHelper();
                    landHelper.onLandLoaded +=landHelper_onLandLoaded;
                    landHelper.Start(_user);
                }
            }
            else
            {
                _user = string.Empty;
            }
        }

        void landHelper_onLandLoaded(Land land)
        {
            this.Dispatcher.BeginInvoke(delegate()
                                            {
                                                if (land != null)
                                                {
                                                    _land = land;
                                                    _hexa = land.GeohexKey;
                                                    CreateHex();
                                                }
                                                else
                                                {
                                                    //todo : handle land == null
                                                    MessageBox.Show("The Login was invalid. Please try again.");

                                                    string url = string.Format("/UI/LoginPage.xaml");
                                                    Uri uri = new Uri(url, UriKind.Relative);
                                                    NavigationService.Navigate(uri);
                                                }
                                            });

        }

        private void mapPolygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string landAsString = _land.ToString();
            string url = "/UI/MyLandPage.xaml?land="+landAsString;
            Uri uri = new Uri(url,UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}