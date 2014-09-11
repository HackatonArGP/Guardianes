using System;
using System.Net;
using System.Linq;
using System.Windows.Browser;
using System.Collections.Generic;
using System.Linq;


namespace Earthwatchers.UI.Requests
{
    public class RequestsHelper
    {
        public static CookieContainer GetCookieContainer()
        {
            var container = new CookieContainer();
            var cookies = RequestsHelper.GetAllCookieList();
            foreach (var c in cookies)
            {
                container.Add(GetCurrentDomain(), c);
                //container.Add(new Uri("http://localhost:2084"), c);
            }

            return container;
        }

        private static CookieCollection GetAllCookieCollection()
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            CookieCollection cookieCollection = new CookieCollection();
            foreach (string cookie in cookies)
            {
                string[] cookieParts = cookie.Split('=');
                if (cookieParts.Count() >= 1)
                {
                    cookieCollection.Add(new Cookie(cookieParts[0].Trim(), cookieParts[1].Trim()));
                }
            }
            return cookieCollection; 

        }

        public static List<Cookie> GetAllCookieList()
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            List<Cookie> cookieList = new List<Cookie>();
            foreach (string cookie in cookies)
            {
                string[] cookieParts = cookie.Split('=');
                if (cookieParts.Count() > 1)
                {
                    cookieList.Add(new Cookie(cookieParts[0].Trim(), cookieParts[1].Trim()));
                }
            }

            return cookieList; 
        }

        private static Uri GetCurrentDomain()
        {
            string scheme = App.Current.Host.Source.Scheme;
            string host = App.Current.Host.Source.Host;
            string port = App.Current.Host.Source.Port.ToString();

            return new Uri(string.Format("{0}://{1}:{2}", scheme, host, port));
        }
    }
}
