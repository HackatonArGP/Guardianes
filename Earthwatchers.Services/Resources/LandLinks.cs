using Earthwatchers.Models;
using System.Net.Http;
using System;

namespace Earthwatchers.Services.Resources
{
    public class LandLinks
    {
        public static void AddLinks(Land land, HttpRequestMessage request)
        {
            //var uriBuilder = new UriBuilder(request.RequestUri) {Path = land.Uri};
            //land.Uri = uriBuilder.Uri.ToString();

        }
    }
}