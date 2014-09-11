using System;
using System.Web.Routing;
using DeforestActionDonations.Resources;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;

namespace WebRole1
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
          //  var config = HttpHostConfiguration.Create().AddFormatters(new WktFormatter());
            RouteTable.Routes.MapServiceRoute<DonationsResource>("donations");
            RouteTable.Routes.MapServiceRoute<LandResource>("land");
            RouteTable.Routes.MapServiceRoute<HomeResource>("");
        }
    }
}
