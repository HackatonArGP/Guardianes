using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Earthwatchers.Services.Security;
using Microsoft.ApplicationServer.Http;

namespace Earthwatchers.Services
{
    public class ApiConfiguration:WebApiConfiguration
    {
        public ApiConfiguration()
        {
            EnableTestClient = true;

            RequestHandlers = (c, e, od) =>
                                  {
                                      // TODO: Configure request operation handlers
                                  };

            this.AppendAuthorizationRequestHandlers();
        }
    }
}