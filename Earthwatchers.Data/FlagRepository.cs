using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Dapper;
using Earthwatchers.Models;
using Microsoft.SqlServer.Types;

namespace Earthwatchers.Data
{
    public class FlagRepository : IFlagRepository
    {
        private readonly IDbConnection connection;

        public FlagRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void DeleteFlag(int flagId)
        {
            connection.Open();
           // const string sql = "delete from Flags where Id=@Id";
            //command.CommandText = sql;

            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Flag_DeleteFlag";
            command.Parameters.Add(new SqlParameter("@Id", flagId));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<Flag> GetFlags()
        {
            connection.Open();           
           //var flags = connection.Query<Flag>("select l.Id as Id, l.EarthwatcherId as EarthwatcherId, l.Location.Lat as Latitude, l.Location.Long as Longitude, l.Comment as Comment, Published, e.Name as UserName from Flags l left join Earthwatcher e on l.EarthwatcherId=e.Id");
           
            var flags = connection.Query<Flag>("EXEC Flag_GetFlags");
            
            connection.Close();
            return flags.ToList();
        }

        public Flag PostFlag(Flag flag)
        {
            connection.Open();
            //var cmd = connection.CreateCommand();
            //cmd.CommandText = "insert into Flags(EarthwatcherId, Location, Comment, Published) values(@userid, @location, @comment, @published) SET @ID = SCOPE_IDENTITY()";

            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Flag_PostFlag";

            cmd.Parameters.Add(new SqlParameter("@userid", flag.EarthwatcherId));
            cmd.Parameters.Add(new SqlParameter("@location", SqlGeography.Point(flag.Latitude, flag.Longitude, 4326)) { UdtTypeName = "Geography" });
            cmd.Parameters.Add(new SqlParameter("@comment", flag.Comment));
            cmd.Parameters.Add(new SqlParameter("@published", DateTime.Now.ToUniversalTime()));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            //var id = (int)idParameter.Value;
            //flag.Id = id;
            connection.Close();
            return flag;
        }
    }
}
