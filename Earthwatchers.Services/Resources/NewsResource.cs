using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class NewsResource
    {
        private readonly INewsRepository newsRepository;

        public NewsResource(INewsRepository newsRepository)
        {
            this.newsRepository = newsRepository;
        }

        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<News>> GetNews(HttpRequestMessage request)
        {
            var newsCollection = newsRepository.GetNews();

            if (newsCollection != null)
                return new HttpResponseMessage<List<News>>(newsCollection) { StatusCode = HttpStatusCode.OK };
            return new HttpResponseMessage<List<News>>(null) { StatusCode = HttpStatusCode.PartialContent };
        }

        [BasicHttpAuthorization(Role.Moderator)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<News> PostNews(News news, HttpRequestMessage<News> request)
        {
            if (news.EarthwatcherId != 0 && news.NewsItem!= String.Empty)
                newsRepository.PostNews(news);

            return null;
        }

        [BasicHttpAuthorization(Role.Moderator)]
        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        public HttpResponseMessage<Comment> DeleteNews(int id, HttpRequestMessage request)
        {
            newsRepository.DeleteNews(id);
            return null;
        }
    }
}