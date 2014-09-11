using System;
using System.Net;
using System.Text;

namespace Earthwatchers.Models.Portable
{
    public class Base64Helper
    {
        public static string ToBase64String(NetworkCredential credentials)
        {
            var str = credentials.UserName + ":" + credentials.Password;
            var encoding = new UTF8Encoding();
            return Convert.ToBase64String(encoding.GetBytes(str));
        }

        public static string ToBase64String(string data)
        {
            var encoding = new UTF8Encoding();
            return Convert.ToBase64String(encoding.GetBytes(data));
        }

        public static string FromBase64String(string data)
        {
            var encoder = new UTF8Encoding();
            var utf8Decode = encoder.GetDecoder();

            var todecodeByte = Convert.FromBase64String(data);
            var charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
            var decodedChar = new char[charCount];
            utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
            var result = new String(decodedChar);
            return result;
        }
    }
}


