using Earthwatchers.Models;
using System.Net.Http;
using System;

namespace Earthwatchers.Services.Resources
{
    public class EarthwatcherLinks
    {
        public static void AddLinks(Earthwatcher earthwatcher, HttpRequestMessage request)
        {
            /*
            var uriBuilder = new UriBuilder(request.RequestUri) {Path = earthwatcher.LandUri};
            earthwatcher.LandUri=uriBuilder.Uri.ToString();

            uriBuilder.Path = earthwatcher.Uri;
            earthwatcher.Uri = uriBuilder.Uri.ToString();
             * */

        }
    }
}