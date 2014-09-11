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
using Earthwatchers.WindowsPhone.Helpers;
using Microsoft.Phone.Controls;

namespace Earthwatchers.WindowsPhone.UI
{
    public partial class MyLandPage : PhoneApplicationPage
    {
        public MyLandPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string landAsString = NavigationContext.QueryString.ContainsKey("land") ? NavigationContext.QueryString["land"] : "";
            if (landAsString != "")
            {
                Land land = new Land();
                land.FromString(landAsString);

                TextBlockId.Text = "Id=" + land.Id.ToString();
                TextBlockLandType.Text ="LandType=" + land.LandType.ToString();
                TextBlockLandThreat.Text = "LandThreat=" + land.LandThreat.ToString();
                TextBlockGeohexKey.Text = "GeohexKey="+ land.GeohexKey;
                TextBlockEarthwatcherGuid.Text = "EarthwatcherGuid=" +land.EarthwatcherGuid;
            }

        }
    }
}