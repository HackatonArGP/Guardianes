using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface INewsRepository
    {
        void DeleteNews(int commentId);
        List<News> GetNews();
        News PostNews(News news);
    }
}
