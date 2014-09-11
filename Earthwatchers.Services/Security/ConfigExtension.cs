using System.Linq;
using Microsoft.ApplicationServer.Http;

namespace Earthwatchers.Services.Security
{
    public static class ConfigExtension
    {
        public static void AppendAuthorizationRequestHandlers(this WebApiConfiguration config)
        {
            var requestHandlers = config.RequestHandlers;
            config.RequestHandlers = (c, e, od) =>
            {
                if (requestHandlers != null)
                {
                    requestHandlers(c, e, od); // Original request handler
                }
                var authorizeAttribute = od.Attributes.OfType<BasicHttpAuthorizationAttribute>()
                    .FirstOrDefault();
                if (authorizeAttribute != null)
                {
                    c.Add(new BasicHttpAuthorizationOperationHandler(authorizeAttribute));
                }
            };
        }
    }
}
