using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace Earthwatchers.WindowsPhone.Helpers
{
    /// <summary>
    /// Gets the Land object from a service
    /// </summary>
    public class GetLandHelper
    {
        public delegate void OnLandLoaded(Land land);
        public event OnLandLoaded onLandLoaded;

        private string _webRequestString;

        public void Start(string user)
        {
            string webServiceMethodUri = string.Empty;
            try
            {
                string restUrl = "http://e3ecfa40aaa04d918d187ecb17d406a6.cloudapp.net/earthwatchers/ShowLand?userName={0}";
                webServiceMethodUri = string.Format(restUrl, user);
                _webRequestString = webServiceMethodUri;
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(webServiceMethodUri);
                httpWebRequest.Method = "GET";

                //new thread environment
                httpWebRequest.BeginGetResponse(responseResultMethod, httpWebRequest);
            }
            catch (Exception ex)
            {

            }
        }

        private void responseResultMethod(IAsyncResult ar)
        {
            if (!ar.IsCompleted) return;
            Land land = null;
            Stream stream = null;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                response = (HttpWebResponse)request.EndGetResponse(ar);

                stream = response.GetResponseStream();
                XmlReader reader = XmlReader.Create(stream);
                land = new Land();
                
                XmlSerializer xmls = new XmlSerializer(typeof(Land));
                land = xmls.Deserialize(reader) as Land;
                reader.Close();

                //close objects
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
                land = null;
            }
            finally
            {
                if (onLandLoaded != null)
                {
                    onLandLoaded(land);
                }
            }
        }
    }
}
