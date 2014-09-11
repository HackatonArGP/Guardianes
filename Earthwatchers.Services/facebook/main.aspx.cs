using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Earthwatchers.Services
{
    public partial class facebook : System.Web.UI.Page
    {
        string publickey = "321672727924360";
        string privatekey = "e7a76fb998a3024ba8a9d0a6211bc9f5";
        string redirecturl = "http://apps.facebook.com/321672727924360/fbmain.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["code"] != null)
            {
                var code = Request.QueryString["code"];

                var url = "https://graph.facebook.com/oauth/access_token?";
                url += "client_id=" + publickey;
                url += "&redirect_uri=" + redirecturl;
                url += "&client_secret=" + privatekey;
                url += "&code=" + code;

                //var i = 0;

                // todo: request access_token
                //Response.Redirect(url);
                //var cl = new FacebookClient();
            }
            else
            {
                Response.Write("Hallo");
            }

            Response.Write("Dit is de facebook earthwatchers applicatie!<br/><br/>" );
            Response.Write("Todo username gegevens...<br/><br/>");
            Response.Write("Todo alot...<br/>");
        }
    }
}