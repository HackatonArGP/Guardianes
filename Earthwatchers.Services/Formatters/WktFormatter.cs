using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Earthwatchers.Models;

namespace Earthwatchers.Services.Formatters
{
    public class WktFormatter: MediaTypeFormatter     
    {
        private const string MediaType = "text/wkt";

        public WktFormatter()        
        {            
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MediaType));        
        }         
    
        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context)
        {
            var land = value as Land;
            if (null == land) return;
            // todo: create wkt from land... 
            // sample:
            const string wkt = "POINT (30 10)";
            var bytes = new UTF8Encoding().GetBytes(wkt);
            stream.Write(bytes, 0, bytes.Length);
            contentHeaders.ContentType = new MediaTypeHeaderValue(MediaType);
        }
    }
}