<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server">  
    private string initParams = string.Empty;
    private string hideHex = System.Configuration.ConfigurationManager.AppSettings.Get("Hexagons.HideByZoom").ToString();
    void Page_Load(object sender, EventArgs e)
        {

        var socialApi = ((System.Web.HttpApplication)(((ASP.home_aspx)(sender)).ApplicationInstance)).Request.Params["social"];
        var mail = "";
        var userId = "";
        var accessToken = "";
        var nickName = "";
        
        if(socialApi == "Facebook")
        {
         mail = ((System.Web.HttpApplication)(((ASP.home_aspx)(sender)).ApplicationInstance)).Request.Params["mail"];
         userId = ((System.Web.HttpApplication)(((ASP.home_aspx)(sender)).ApplicationInstance)).Request.Params["userId"];
         accessToken = ((System.Web.HttpApplication)(((ASP.home_aspx)(sender)).ApplicationInstance)).Request.Params["accessToken"];
         nickName = ((System.Web.HttpApplication)(((ASP.home_aspx)(sender)).ApplicationInstance)).Request.Params["nickName"];
        }
        
        //Twitter API
        if(socialApi == "Twitter")
        {
            btnLogTwitter_Click(null, null);
        }
     
        
        if (Request.QueryString["oauth_token"] != null && Request.QueryString["oauth_verifier"] != null)
        {
            string oauth_token = Request.QueryString["oauth_token"];
            string oauth_verifier = Request.QueryString["oauth_verifier"];

            Earthwatchers.Services.OAuthHelper oauthhelper = new Earthwatchers.Services.OAuthHelper();
            oauthhelper.GetUserTwAccessToken(oauth_token, oauth_verifier);

            if (string.IsNullOrEmpty(oauthhelper.oauth_error))               
            {                                                         //oauth_token= JW2guC2VctxzdBDxiJMt6hJSnZziB6mF   oauth_verifier= FCl8wofiyklyc1vC2vLrJD2ghTrU1bFh (EN URL AL REDIRECCIONAR, CAMBIA)
                Session["twtoken"] = oauthhelper.oauth_access_token;         //Acces_token  1967681070-3kiZPuvlLwW37OX7BnmkvCgUfNpItexbJ6JL6F1
                Session["twsecret"] = oauthhelper.oauth_access_token_secret; //Secret_Token HIukXgkPh2FVeYWjQjfKnXyxQ6cSfrgYw0hVicgGOm53t
                Session["twuserid"] = oauthhelper.user_id;                   //Id del usuario en Twitter  1967681070
                Session["twname"] = oauthhelper.screen_name;                 //Nombre de Twitter  DeepSoftwareARG
            }
            else
                Response.Write(oauthhelper.oauth_error);
        }
        //Twitter API
        
        
        if (!string.IsNullOrEmpty(Request["username"]))
        {
            initParams = ("credentials=" + Request["username"] + ":" + Request["password"] + ":" + Request["geohexcode"]);
        }
        //else if (!string.IsNullOrEmpty(Request["authtoken"]))
        //{
        //    initParams = ("authtoken=" + Request["authtoken"]);
        //}
        else if (Session["twuserid"] != null)
        {
            initParams = ("api=" + "Twitter" + ",userId=" + Session["twuserid"] + ",accessToken=" + Session["twtoken"] + ",nickName=" + Session["twname"]);
        }
        else if (socialApi == "Facebook")
        {
            initParams = ("api=" + "Facebook" + ",userId=" + userId + ",accessToken=" + accessToken + ",mail=" + mail + ",nickName=" + nickName );
        }
        else
        {
            Response.Redirect("/index.html", true);
        }
    }

    void btnLogTwitter_Click(object sender, EventArgs e)
    {
        Earthwatchers.Services.OAuthHelper oauthhelper = new Earthwatchers.Services.OAuthHelper();
        string requestToken = oauthhelper.GetRequestToken();

        if (string.IsNullOrEmpty(oauthhelper.oauth_error))
            Response.Redirect(oauthhelper.GetAuthorizeUrl(requestToken));
        else
            Response.Write(oauthhelper.oauth_error);
    }
    


        
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Guardianes - Greenpeace</title>
    <link href="css/ew.css" rel="stylesheet" />
    <%--<link rel="SHORTCUT ICON" href="http://www.mydomain.com/myicon.ico"/>--%>
    <link rel="icon" href="/favicon.ico" />
<link rel="shortcut icon" href="/favicon.ico" />
    <style type="text/css">
        html, body { height: 100%; overflow: hidden; }

        body { padding: 0; margin: 0; }

        #silverlightControlHost { height: 100%; text-align: center; }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script src="Scripts/jquery-1.6.4.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            alert(errMsg);
        }
    </script>
</head>
<body scroll="no">
    <script>
        (function (i, s, o, g, r, a, m)
        {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function ()
            {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-47414806-1', 'greenpeace.org.ar');
        ga('send', 'pageview');

    </script>
    <form id="form1" runat="server" style="height: 100%">
        <div id="silverlightControlHost">
            <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
                <param name="source" value="ClientBin/Earthwatchers.UI.xap?v=1.145" /> 
                <param name="onError" value="onSilverlightError" />
                <param name="background" value="white" />
                <param name="minRuntimeVersion" value="5.0.61118.0" />
                <param name="autoUpgrade" value="true" />
                <param name="hideHexagon" value="<% System.Configuration.ConfigurationManager.AppSettings.Get("Hexagons.HideByZoom"); %>" />
                <param name="initParams" value="<%=initParams %>" />
                 <asp:Literal ID="ParamInitParams" runat="server"></asp:Literal>
                <div class="floater">
                    <div class="contents">
                        <div>
                            <img src="Images/logo.png" />
                        </div>
                        <div style="margin-top: 15px">
                            <img src="Images/guardianes.png" />
                        </div>
                        <div class="container">
                            <div id="logindiv">

                                <div class="content">
                                    <div class="slHeader">
                                        Estás a punto de convertirte en Guardián!
                                    </div>
                                    <div class="slText">Instalá el plugin the Microsoft Silverlight, sólo lleva unos segundos</div>
                                    <div style="margin-top: 30px">
                                        <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration: none" class="button" onclick="logSlInstall()">INSTALAR SILVERLIGHT</a>
                                    </div>
                                    <div style="margin-top: 30px">
                                        <a class="slText" href="http://www.greenpeace.org.ar/denuncias/index.php?id=0" target="_blank" style="text-decoration: none">No puedo instalarlo, pero quiero participar</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </object>
            <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>
        </div>

        <script>
            function logSlInstall() //Loguear install de Silverlight
            {

                $.ajax({
                    type: 'POST',
                    url: "../api/earthwatchers/logSlInstall",
                    success: function ()
                    {
                    },
                    error: function ()
                    {
                    }
                });
            }

            function shorten(url) {
                var access_token = 'c31a6068bb170d6b9c50ac625c59e25888abb0e0';
                var api_req = 'https://api-ssl.bitly.com/v3/shorten?access_token=' + access_token + '&longUrl=' + encodeURIComponent(url);
                var res = null;

                $.ajax({
                    dataType: "json",
                    async: false,
                    url: api_req,
                    success: function (result) {
                        res = result.data.url;
                    }
                });

                return res;
            }
            
                function deleteCookie(cookieName) {
                    var exp = new Date();
                    exp.setTime(exp.getTime() - 1);
                    document.cookie = cookieName + "=;expires=" + exp.toGMTString();
                }

                function logout() {
                    $.get('../api/authenticate/logout', function () {
                        deleteCookie('authtoken');
                        window.location.href = "/index.html?action=noreturn";
                    });
                }

                $(document).ajaxError(function (event, jqxhr, settings, exception)
                {
                    if (console && console.log)
                    {
                        console.log(exception);
                    }
                    else
                    {
                        alert('Exception');
                    }

                
                });

        </script>
    </form>
</body>
</html>
