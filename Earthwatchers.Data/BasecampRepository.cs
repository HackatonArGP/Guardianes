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
    public class BasecampRepository : IBasecampRepository
    {
        private readonly IDbConnection connection;

        public BasecampRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<Basecamp> Get()
        {
            connection.Open();
            var basecamps = connection.Query<Basecamp>("EXEC Basecamp_Get");
            connection.Close();
            return basecamps.ToList();
        }

        public List<Basecamp> GetByBasecamp(string basecamp)
        {
            connection.Open();
            var basecamps = connection.Query<Basecamp>(string.Format("EXEC Basecamp_GetByBasecamp {0}",basecamp));
            connection.Close();
            return basecamps.ToList();
        }

        public List<Basecamp> GetBaseCamps()
        {
            connection.Open();
            var basecamps = connection.Query<Basecamp>("EXEC Basecamp_GetBaseCamps");
            connection.Close();
            return basecamps.ToList();
        }

        public Basecamp Insert(Basecamp basecamp)
        {
            try
            {
                connection.Open();

                var cmd = connection.CreateCommand() as SqlCommand;
                //cmd.CommandText = "INSERT INTO BasecampDetails (BasecampId, Location, Probability, Name) values(@basecampid, geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(20)) + ' ' + CAST(@latitude AS VARCHAR(20)) + ')', 4326), @probability, @name) SET @ID = SCOPE_IDENTITY()";
               
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Basecamp_Insert";
                
                cmd.Parameters.Add(new SqlParameter("@basecampid", basecamp.Id));
                cmd.Parameters.Add(new SqlParameter("@longitude", basecamp.Longitude));
                cmd.Parameters.Add(new SqlParameter("@latitude", basecamp.Latitude));
                cmd.Parameters.Add(new SqlParameter("@probability", basecamp.Probability));
                cmd.Parameters.Add(new SqlParameter("@name", basecamp.DetailName));
                cmd.Parameters.Add(new SqlParameter("@shortText", basecamp.ShortText));
                var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParameter);
                cmd.ExecuteNonQuery();

                //var id = (int)idParameter.Value;
                //basecamp.Id = id;
                connection.Close();
                return basecamp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Basecamp GetById(int id)
        {
            connection.Open();
            var basecamp = connection.Query<Basecamp>(string.Format("EXEC Basecamp_GetById {0}", id)).ToList();
            connection.Close();
            return basecamp.FirstOrDefault();
        }

        public Basecamp Edit(Basecamp basecamp)
        {
             try
            {
                connection.Open();

                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Basecamp_Edit";
                cmd.Parameters.Add(new SqlParameter("@basecampid", basecamp.Id));
                cmd.Parameters.Add(new SqlParameter("@longitude", basecamp.Longitude));
                cmd.Parameters.Add(new SqlParameter("@latitude", basecamp.Latitude));
                cmd.Parameters.Add(new SqlParameter("@probability", basecamp.Probability));
                cmd.Parameters.Add(new SqlParameter("@name", basecamp.DetailName));
                cmd.Parameters.Add(new SqlParameter("@shortText", basecamp.ShortText));
                cmd.Parameters.Add(new SqlParameter("@id", basecamp.IdDb));
                cmd.ExecuteNonQuery();

                connection.Close();
                return basecamp;
            }
             catch (Exception ex)
             {
                 throw ex;
             }
        }

        public void RecalculateDistance(int id)
        {
                try
            {
                if (id != 0)
                {
                    connection.Open();
                    var cmd = connection.CreateCommand() as SqlCommand;
                    cmd.CommandTimeout = 7200000;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "ReCalculate_LandDistanceFromBasecampId";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
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
                cmd.CommandText = "Basecamp_Delete";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
