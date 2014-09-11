using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Models.Portable;
using Earthwatchers.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class CollectionsResource
    {
        private readonly ICollectionsRepository collectionsRepository;

        public CollectionsResource(ICollectionsRepository collectionsRepository)
        {
            this.collectionsRepository = collectionsRepository;
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "{earthwatcherId}")]
        public HttpResponseMessage<List<CollectionItem>> GetCollectionItemsByEarthwatcher(int earthwatcherId, HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<CollectionItem>>(collectionsRepository.GetCollectionItemsByEarthwatcher(earthwatcherId)) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<CollectionItem>>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/newitem={id}")]
        public HttpResponseMessage<CollectionItem> GetNewCollectionItem(int id, HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<CollectionItem>(collectionsRepository.GetNewCollectionItem(id)) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<CollectionItem>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/totalitems={ewid}")]
        public HttpResponseMessage<int> GetTotalItems(int ewid, HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<int>(collectionsRepository.GetTotalItems(ewid)) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<int>(-1) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }
    }
}