using Earthwatchers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace Earthwatchers.Data
{
    public class JaguarRepository : IJaguarRepository
    {
        private readonly IDbConnection connection;

        public JaguarRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<JaguarGame> Get()
        {
            connection.Open();
            var jaguarPositions = connection.Query<JaguarGame>("EXEC Jaguar_Get");
            connection.Close();
            return jaguarPositions.ToList();
        }

        public JaguarGame Insert(JaguarGame jaguarPos)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Jaguar_Insert";
                cmd.Parameters.Add(new SqlParameter("@day", jaguarPos.Day));
                cmd.Parameters.Add(new SqlParameter("@hour", jaguarPos.Hour));
                cmd.Parameters.Add(new SqlParameter("@minutes", jaguarPos.Minutes));
                cmd.Parameters.Add(new SqlParameter("@longitude", jaguarPos.Longitude));
                cmd.Parameters.Add(new SqlParameter("@latitude", jaguarPos.Latitude));
                var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParameter);
                cmd.ExecuteNonQuery();
                connection.Close();
                return jaguarPos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Jaguar_Delete";
                cmd.Parameters.Add(new SqlParameter("@id", id));
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(int earthWatcherId, int posId)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Jaguar_Update";
                cmd.Parameters.Add(new SqlParameter("@earthWatcherId", earthWatcherId));
                cmd.Parameters.Add(new SqlParameter("@posId", posId));
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JaguarGame GetPos(int id)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var pos = connection.Query<JaguarGame>(string.Format("EXEC Jaguar_GetPos {0}", id));
            connection.Close();
            var jaguarPos = pos.FirstOrDefault() as JaguarGame;
            return jaguarPos;
        }

    }
}
