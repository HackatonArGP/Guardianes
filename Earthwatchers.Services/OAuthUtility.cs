using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
namespace Earthwatchers.Services
{
    public class OAuthUtility
    {
        public OAuthUtility()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region *******Common Methods**********
        protected static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            Random random = new Random();
            return random.Next(123400, 9999999).ToString();
        }
        public static Dictionary<string, string> GetQueryParameters(string dataWithQuery)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] parts = dataWithQuery.Split('?');
            if (parts.Length > 0)
            {
                string QueryParameter = parts.Length > 1 ? parts[1] : parts[0];
                if (!string.IsNullOrEmpty(QueryParameter))
                {
                    string[] p = QueryParameter.Split('&');
                    foreach (string s in p)
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(temp[0], temp[1]);
                        }
                        else
                        {
                            result.Add(s, string.Empty);
                        }
                    }
                }
            }
            return result;
        }
        #endregion Common Methods

        public static string GetAuthorizationHeaderForPost_OR_QueryParameterForGET(Uri url, string callbackUrl, string httpMethod, string consumerKey, string consumerSecret, string token, string tokenSecret, out string normalizedUrl)
        {
            string normalizedParameters = "";

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("oauth_version", "1.0");
            if (token != "")
                parameters.Add("oauth_token", token);
            parameters.Add("oauth_nonce", GenerateNonce()); //Random String
            parameters.Add("oauth_timestamp", GenerateTimeStamp()); // Current Time Span
            parameters.Add("oauth_consumer_key", consumerKey); //Customer Consumer Key
            parameters.Add("oauth_signature_method", "HMAC-SHA1"); //Singnatur Encription Method
            parameters.Add("oauth_callback", UrlEncode(callbackUrl)); //return url

            Dictionary<string, string> drQuery = GetQueryParameters(url.Query);
            foreach (string key in drQuery.Keys)
                parameters.Add(key, drQuery[key]);

            if (url.Query != "")
                normalizedUrl = url.AbsoluteUri.Replace(url.Query, "");
            else
                normalizedUrl = url.AbsoluteUri;

            List<string> li = parameters.Keys.ToList();
            li.Sort();

            StringBuilder sbOAuthHeader = new StringBuilder("OAuth ");
            StringBuilder sbSignatureBase = new StringBuilder();
            foreach (string k in li)
            {
                sbSignatureBase.AppendFormat("{0}={1}&", k, parameters[k]); // For Signature and Get Date (QueryString)
                sbOAuthHeader.AppendFormat("{0}=\"{1}\", ", k, parameters[k]); // For Post Request (Post Data)
            }

            string signature = GenerateSignatureBySignatureBase(httpMethod, consumerSecret, tokenSecret, normalizedUrl, sbSignatureBase);

            if (httpMethod == "POST")
            {
                string OAuthHeader = sbOAuthHeader.Append("oauth_signature=\"" + UrlEncode(signature) + "\"").ToString();
                normalizedParameters = OAuthHeader;
            }
            else if (httpMethod == "GET")
            {
                normalizedParameters = sbSignatureBase.AppendFormat("{0}={1}", "oauth_signature", signature).ToString(); ;
            }
            return normalizedParameters;
        }
        private static string GenerateSignatureBySignatureBase(string httpMethod, string consumerSecret, string tokenSecret, string normalizedUrl, StringBuilder sbSignatureBase)
        {
            string normalizedRequestParameters = sbSignatureBase.ToString().TrimEnd('&');
            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToString());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), UrlEncode(tokenSecret)));
            byte[] hashBytes = hmacsha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(signatureBase.ToString()));
            return Convert.ToBase64String(hashBytes);
        }
    }
}