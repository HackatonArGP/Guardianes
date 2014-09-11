using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Earthwatchers.Models;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;

namespace Earthwatchers.Data
{
    public class NewsRepository : INewsRepository
    {
        private readonly IDbConnection connection;

        public NewsRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<News> GetNews()
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var comments = connection.Query<News>("EXEC News_GetNews");
            connection.Close();
            return comments.ToList();
        }

        public News PostNews(News news)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "News_PostNews";
            
            cmd.Parameters.Add(new SqlParameter("@userid", news.EarthwatcherId));
            cmd.Parameters.Add(new SqlParameter("@NewsItem", news.NewsItem));
            cmd.Parameters.Add(new SqlParameter("@Published", DateTime.Now.ToUniversalTime()));
            cmd.Parameters.Add(new SqlParameter("@shape", SqlGeography.STGeomFromText(new SqlChars(news.Wkt), 4326)) { UdtTypeName = "Geography" });
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) {Direction = ParameterDirection.Output};
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            return news;
        }

        public void DeleteNews(int commentId)
        {
            connection.Open();
            var command = connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText ="News_DeleteNews";
            command.Parameters.Add(new SqlParameter("@Id", commentId));
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
