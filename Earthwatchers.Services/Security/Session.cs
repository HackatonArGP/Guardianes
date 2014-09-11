using Earthwatchers.Data;
using Earthwatchers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;

namespace Earthwatchers.Services.Security
{
    public static class Session
    {
        public const string TOKENKEY = "authtoken";
        public const string secretKey = "clavesecreta";

        public static string GenerateCookie(Earthwatcher ew, bool keeplogued = true)
        {
            string compositeToken = Guid.NewGuid().ToString();
            string compositeEwName = ew.Name;
            string compositeEwRole = string.Join(",", ew.GetRoles());
            var expiresOn = keeplogued ? DateTime.UtcNow.AddDays(15) : DateTime.UtcNow.AddMinutes(30);
            var compositeExpiresOn = expiresOn.ToString(); 
            string composite = string.Format("{0}|{1}|{2}|{3}|{4}", compositeToken, compositeEwName, compositeEwRole, compositeExpiresOn, keeplogued);

            string strToken = TextEncrytion.EncryptString(composite, Session.secretKey);

            HttpCookie cookie = new HttpCookie(Session.TOKENKEY);
            cookie.Value = strToken;
            cookie.Expires = expiresOn;
            cookie.HttpOnly = false;
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

            return strToken;
        }
        
        public static string GenerateExpiredCookieValue()
        {
            var expiration = DateTime.Now.AddDays(-1);
            string compositeToken = Guid.Empty.ToString();
            string compositeEwName = "";
            string compositeEwRole = "";
            string compositeExpiresOn = expiration.Ticks.ToString();
            string composite = string.Format("{0}|{1}|{2}|{3}", compositeToken, compositeEwName, compositeEwRole, compositeExpiresOn);

            return TextEncrytion.EncryptString(composite, secretKey);
        }

        public static CookieInfo GetCookieInfo()
        {
            CookieInfo info = new CookieInfo();

            try
            {
                HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[Session.TOKENKEY];
                if (cookie == null) return info;

                string cookieCompositeValue = cookie.Value;
                string composite = TextEncrytion.DecryptString(cookieCompositeValue, Session.secretKey);
                string[] compositeValues = composite.Split('|');

                info.LoginToken = new Guid(compositeValues[0]);
                info.EarthwatcherName = compositeValues[1];
                info.Roles = compositeValues[2].Split(',');
                info.ExpiresOn = DateTime.Parse(compositeValues[3]);
                info.KeepLogged = Convert.ToBoolean(compositeValues[4]);

                if ((DateTime.UtcNow - info.ExpiresOn).Minutes > 1)
                {
                    //Borrar cookie 
                    DeleteCookie();
                    return new CookieInfo();
                }
                else
                {
                    RefreshCookie(info);
                }
            }
            catch
            {
            }

            return info;
        }
        
        public static bool HasLoggedUser()
        {
            var ci = GetCookieInfo();
            var isLogged = false;
            if (!string.IsNullOrEmpty(ci.EarthwatcherName) && ci.ExpiresOn >= DateTime.Now)
                isLogged = true;

            return isLogged;
        }

        public static void Logout()
        {
            DeleteCookie();
            
            //vaciar cache de usuario logueado
            HttpContext.Current.Session.Abandon();
        }


        private static void RefreshCookie(CookieInfo info)
        {
            var compositeToken = Guid.NewGuid().ToString();
            var compositeEwName = info.EarthwatcherName;
            var compositeEwRole = string.Join(",", info.Roles);
            var expiresOn = info.KeepLogged ? DateTime.UtcNow.AddDays(15) : DateTime.UtcNow.AddMinutes(30);
            var compositeExpiresOn = expiresOn.ToString();
            string composite = string.Format("{0}|{1}|{2}|{3}|{4}", compositeToken, compositeEwName, compositeEwRole, compositeExpiresOn, info.KeepLogged);

            string strToken = TextEncrytion.EncryptString(composite, Session.secretKey);

            HttpCookie cookie = new HttpCookie(Session.TOKENKEY);
            cookie.Value = strToken;
            cookie.HttpOnly = false;
            cookie.Expires = expiresOn;
            System.Web.HttpContext.Current.Response.Cookies.Set(cookie);

        }

        private static void DeleteCookie()
        {
            try
            {
                //eliminar cookie por expiracion
                HttpCookie cookie = new HttpCookie(Session.TOKENKEY);
                cookie.Value = "deleted";
                cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch
            {
            }
        }

        public class CookieInfo
        {
            public Guid LoginToken { get; set; }
            public DateTime ExpiresOn { get; set; }
            public string EarthwatcherName { get; set; }
            public string[] Roles { get; set; }
            public bool KeepLogged { get; set; }
        }
    }

    public static class TextEncrytion
    {
        public static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            System.Security.Cryptography.MD5CryptoServiceProvider HashProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            var ret = Convert.ToBase64String(Results);

            return HttpUtility.UrlEncode(Convert.ToBase64String(Results));
        }

        public static string DecryptString(string Message, string Passphrase)
        {
            Message = HttpUtility.UrlDecode(Message);

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            System.Security.Cryptography.MD5CryptoServiceProvider HashProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }
    }
}