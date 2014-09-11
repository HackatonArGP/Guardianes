using System;
using System.Net;
using Microsoft.ApplicationServer.Http.Dispatcher;
using System.Net.Http;
using NLog;

namespace Earthwatchers.Services
{
    public class GlobalErrorHandler : HttpErrorHandler {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override bool OnTryProvideResponse(Exception exception, ref HttpResponseMessage message) {
            logger.ErrorException("Unhandled Exception.", exception);
            
            if (exception != null)
            {
                if (!(exception is HttpResponseException))
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    message = new HttpResponseMessage {StatusCode = HttpStatusCode.InternalServerError};
                }
                else
                {
                    message = ((HttpResponseException)exception).Response;
                }

            }
            return true;
        }
    }
}