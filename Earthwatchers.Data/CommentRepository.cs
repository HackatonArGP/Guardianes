using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbConnection connection;

        public CommentRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<Comment> GetCommentsByLand(int id)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            var comments = connection.Query<Comment>(string.Format("EXEC Comments_GetCommentsByLand {0}", id));
            connection.Close();
            return comments.ToList();
        }

        public List<Comment> GetCommentsByUserId(int id)
        {
            connection.Open();
            var comments = connection.Query<Comment>(string.Format("EXEC Comments_GetCommentsByUserId {0}", id));
            connection.Close();
            return comments.ToList();
        }

        public Comment PostComment(Comment comment)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Comments_PostComment";

            cmd.Parameters.Add(new SqlParameter("@userid", comment.EarthwatcherId));
            cmd.Parameters.Add(new SqlParameter("@landid", comment.LandId));
            cmd.Parameters.Add(new SqlParameter("@Comment", comment.UserComment));
            cmd.Parameters.Add(new SqlParameter("@Published", DateTime.Now.ToUniversalTime()));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) {Direction = ParameterDirection.Output};
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            return comment;
        }

        public void DeleteComment(int commentId)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Comments_DeleteComment";
            cmd.Parameters.Add(new SqlParameter("@commentId", commentId));
            cmd.ExecuteNonQuery();

            connection.Close();
        }
    }
}
