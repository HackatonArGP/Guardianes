using System;
using System.Windows.Browser;
using Earthwatchers.UI.Layers;
using Earthwatchers.UI.Requests;
using System.Collections.Generic;
using System.Windows;
using Earthwatchers.Models;
using System.Windows.Input;
namespace Earthwatchers.UI.GUI.Controls
{
    //TODO: @time Refactor setting of loading/nouser/loggedin

	public partial class UserInfo
	{
        private readonly LandRequests landRequest = new LandRequests(Constants.BaseApiUrl);

	    private Zone zone;
	    
        public UserInfo()
		{
			InitializeComponent();

            
            label.Text = Current.Instance.Earthwatcher.FullName;

            var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            if (flagLayer != null) flagLayer.RequestFlags();

            //RequestFromUsername requesting the land for the user
            landRequest.LandReceived += LandChanged;
            landRequest.GetLandById(Current.Instance.Earthwatcher.LandId.ToString());
		}
        /*
        private void EarthwatcherChanged(object sender, EventArgs e)
        {
            GridLoading.Visibility = Visibility.Collapsed;

            earthwatcher = sender as Earthwatcher;
            Current.Instance.Earthwatcher = earthwatcher;

            if (earthwatcher == null)
            {
                txtIncorrectLogin.Visibility = Visibility.Visible;
                GridNoUser.Visibility = Visibility.Visible;           
                return;
            }

            GridNoUser.Visibility = Visibility.Collapsed;  
            GridLogedIn.Visibility = Visibility.Visible;

            label.Text = earthwatcher.FullName;

            var flagLayer = Current.Instance.LayerHelper.FindLayer(Constants.flagLayerName) as FlagLayer;
            if (flagLayer != null) flagLayer.RequestFlags();

            //RequestFromUsername requesting the land for the user
            landRequest.LandReceived += LandChanged;
            landRequest.GetLandById(earthwatcher.LandId.ToString());

            UserLoggedIn(this, EventArgs.Empty);
        }
         * */

        //Received land of the user, parse the info and show it to the user
        /*
        private void AuthenticationChanged(object sender, EventArgs e)
        {
            Current.Instance.IsAuthenticated = (bool)sender;
            if (!Current.Instance.IsAuthenticated)//if authkey is incorrect do not load userinfo
            {
                txtIncorrectLogin.Visibility = Visibility.Visible;
                SetGuiForNoUser();
                return;
            }

            earthwatcherRequest.EarthwatcherReceived += EarthwatcherChanged;
            earthwatcherRequest.GetByName(Current.Instance.Username, Current.Instance.Password);
        }
         * */

	    //Received land of the user, parse the info and show it to the user
        private void LandChanged(object sender, EventArgs e)
        {
            var land = sender as Land;
            
            if(land == null) 
                return;

            Current.Instance.EarthwatcherLand = land;
            zone = GeoHex.Decode(land.GeohexKey);
                        
            var hexagonLayer = Current.Instance.LayerHelper.FindLayer(Constants.Hexagonlayername) as HexagonLayer;
            
            if (hexagonLayer != null && zone != null) 
                hexagonLayer.AddHexagon(zone, LandStatus.NotChecked, true);

            MapHelper.ZoomToHexagon(Current.Instance.MapControl, land.GeohexKey);
        }

        private void TxtLogoutClick(object sender, RoutedEventArgs e)
        {
            //TODO:
        }

        private void BtnMyLandClick(object sender, RoutedEventArgs e)
        {
            MapHelper.ZoomToHexagon(Current.Instance.MapControl, Current.Instance.EarthwatcherLand.GeohexKey);
        }
    }
}