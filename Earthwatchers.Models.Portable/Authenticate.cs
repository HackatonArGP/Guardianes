using System;

namespace Earthwatchers.Models
{
    public class Authenticate
    {
        public static Boolean IsAuthorized(string name, int start, int length, string authkey)
        {
            var nameAuthKey = GetAuthKey(name, start, length);
            return authkey.Equals(nameAuthKey);
        }

        public static string GetAuthKey(string name, int start, int length)
        {
            var md5 = MD5.GetMd5String(name);
            return GetPartialMd5(md5, start, length);
        }
        
        private static string GetPartialMd5(string md5, int start, int length)
        {
            // was: 2..6
            return md5.Substring(start, length);
        }
    }
}
