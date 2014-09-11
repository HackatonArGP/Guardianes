using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Earthwatchers.Services.admin
{
    //"C:\\Users\\Leonardo\\Desktop\\prueba"
    // ConfigurationManager.AppSettings.Get("kml.fincas.path");

    public class UploadFileDotNet : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
                string filename = context.Request.QueryString["InputFile"].ToString();
            
                using (FileStream fileStream =  File.Create(context.Server.MapPath(ConfigurationManager.AppSettings.Get("kml.fincas.path") + filename)))
                {
                    byte [] bufferData = new byte[4096];
                    int bytesToRead;
                    while((bytesToRead = context.Request.InputStream.Read(bufferData,0,bufferData.Length)) != 0)
                    {
                        fileStream.Write(bufferData, 0 , bytesToRead);
                    }
                    fileStream.Close();
                }
        }

            public bool IsReusable
            {
                get
                {
                    return false;
                }
            }
    }
}