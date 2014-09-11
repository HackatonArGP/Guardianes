using System;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Windows.Media.Imaging;

namespace Earthwatchers.UI.Resources
{
    public class ResourceHelper
    {
        public static string ExecutingAssemblyName
        {
            get
            {
                var name = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                return name.Substring(0, name.IndexOf(','));
            }
        }

        public static Stream GetStream(string relativeUri, string assemblyName)
        {
            var res = Application.GetResourceStream(new Uri(assemblyName + ";component/" + relativeUri, UriKind.Relative)) ??
                      Application.GetResourceStream(new Uri(relativeUri, UriKind.Relative));
            
            return res != null ? res.Stream : null;
        }

        public static Stream GetStream(string relativeUri)
        {
            return GetStream(relativeUri, ExecutingAssemblyName);
        }

        public static BitmapImage GetBitmap(string relativeUri, string assemblyName)
        {
            var s = GetStream(relativeUri, assemblyName);

            if (s == null) 
                return null;

            using (s)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(s);
                return bmp;
            }
        }

        public static BitmapImage GetBitmap(string relativeUri)
        {
            return GetBitmap(relativeUri, ExecutingAssemblyName);
        }

        public static string GetString(string relativeUri, string assemblyName)
        {
            var s = GetStream(relativeUri, assemblyName);

            if (s == null) 
                return null;

            using (var reader = new StreamReader(s))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetString(string relativeUri)
        {
            return GetString(relativeUri, ExecutingAssemblyName);
        }

        public static FontSource GetFontSource(string relativeUri, string assemblyName)
        {
            var s = GetStream(relativeUri, assemblyName);

            if (s == null) 
                return null;

            using (s)
            {
                return new FontSource(s);
            }
        }

        public static FontSource GetFontSource(string relativeUri)
        {
            return GetFontSource(relativeUri, ExecutingAssemblyName);
        }

        public static object GetXamlObject(string relativeUri, string assemblyName)
        {
            var str = GetString(relativeUri, assemblyName);

            if (str == null) 
                return null;

            var obj = System.Windows.Markup.XamlReader.Load(str);
            return obj;
        }

        public static object GetXamlObject(string relativeUri)
        {
            return GetXamlObject(relativeUri, ExecutingAssemblyName);
        }
    }
}