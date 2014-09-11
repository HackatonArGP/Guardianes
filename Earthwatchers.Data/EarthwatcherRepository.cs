using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Earthwatchers.Models;
using System.Data.SqlClient;
using Dapper;

namespace Earthwatchers.Data
{
    public class EarthwatcherRepository : IEarthwatcherRepository
    {
        private readonly IDbConnection connection;

        public EarthwatcherRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        private int GetTotalScore(int Earthwatcher)
        {
            var scoreRepository = new ScoreRepository(connection.ConnectionString);
            return scoreRepository.GetScore(Earthwatcher);
        }

        public Earthwatcher GetEarthwatcher(int id)
        {
            connection.Open();
            var earthwatchers = connection.Query<Earthwatcher>(string.Format("EXEC Earthwatcher_GetEarthwatcher {0}", id));
            connection.Close();
            var earthwatcher = earthwatchers.FirstOrDefault();
            if (earthwatcher != null)
            {
                earthwatcher.Lands = GetEarthwatcherLands(earthwatcher.Name);
            }
            return earthwatcher;
        }

        public bool EarthwatcherExists(string name)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_EarthwatcherExists";
            cmd.Parameters.Add(new SqlParameter("@name", name));
            object obj = cmd.ExecuteScalar();
            connection.Close();
            if (obj == null)
            {
                return false;
            }
            return true;
        }
        
        public ApiEw GetApiEw(string api, string userId)  //Devuelve todos los campos de un ApiEW pasando el nombre de la api y el id del usuario
        {
            connection.Open();
            var earthwatchers = connection.Query<ApiEw>(string.Format("EXEC Earthwatcher_GetApiEw {0}, {1}", api, userId));
            connection.Close();
            var earthwatcher = earthwatchers.FirstOrDefault();
            
            return earthwatcher;
        }

        public ApiEw GetApiEwById(int apiEwId)  //Devuelve todos los campos de un ApiEW pasando el id del ApiEW
        {
            connection.Open();
            var earthwatchers = connection.Query<ApiEw>(string.Format("EXEC Earthwatcher_GetApiEwById {0}", apiEwId));
            connection.Close();
            var earthwatcher = earthwatchers.FirstOrDefault();

            return earthwatcher;
        }

        public ApiEw CreateApiEwLogin(ApiEw ew)  //Inserta el registro en la tabla ApiEwLogin
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_CreateApiEwLogin";
            cmd.Parameters.Add(new SqlParameter("@Api", ew.Api));
            cmd.Parameters.Add(new SqlParameter("@UserId", ew.UserId));
            cmd.Parameters.Add(new SqlParameter("@NickName", ew.NickName));
            cmd.Parameters.Add(new SqlParameter("@SecretToken", ew.SecretToken));
            cmd.Parameters.Add(new SqlParameter("@AccessToken", ew.AccessToken));
            cmd.Parameters.Add(new SqlParameter("@Mail", ew.Mail));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            var id = (int)idParameter.Value;
            ew.Id = id;
            connection.Close();
            return ew;
        }
        public void LinkApiAndEarthwatcher(int apiEwId, int EwId) //Updatea la tabla ApiEwLogin con el EarthwatcherId y la tabla Earthwatcher con el UserId
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_LinkApiAndEarthwatcher";
            cmd.Parameters.Add(new SqlParameter("@ApiEwId", apiEwId));
            cmd.Parameters.Add(new SqlParameter("@EwId", EwId));
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdateAccessToken(int Id, string AccessToken)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_UpdateAccessToken";
            cmd.Parameters.Add(new SqlParameter("@ApiEwId", Id));
            cmd.Parameters.Add(new SqlParameter("@AccessToken", AccessToken));
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public Earthwatcher GetEarthwatcher(string name, bool getLands)
        {
            connection.Open();
            //var earthwatchers = connection.Query<Earthwatcher>(string.Format("EXEC Earthwatcher_GetEarthwatcherByName {0}", name));
            var earthwatchers = connection.Query<Earthwatcher>("select Id, EarthwatcherGuid as Guid, Name, Country, Role, IsPowerUser, Language, Region, NotifyMe, NickName, ApiEwId from Earthwatcher where Name = @Name", new { Name = name });
            connection.Close();
            var earthwatcher = earthwatchers.FirstOrDefault();
            if (earthwatcher != null && getLands)
            {
                earthwatcher.Lands = GetEarthwatcherLands(earthwatcher.Name);
            }
            return earthwatcher;
        }

        public Earthwatcher GetEarthwatcherByGuid(Guid guid)
        {
            connection.Open();
            var earthwatchers = connection.Query<Earthwatcher>(string.Format("EXEC Earthwatcher_GetEarthwatcherByGuid '{0}'", guid));
            connection.Close();
            var earthwatcher = earthwatchers.FirstOrDefault();
            if (earthwatcher != null)
            {
                earthwatcher.Lands = GetEarthwatcherLands(earthwatcher.Name);
            }
            return earthwatcher;
        }

        private List<Land> GetEarthwatcherLands(string name)
        {
            var landRepository = new LandRepository(connection.ConnectionString);
            return landRepository.GetLandByEarthwatcherName(name);
        }

        public Earthwatcher CreateEarthwatcher(Earthwatcher earthwatcher)
        {
            var passwordPrefix = "";
            var hashedPassword = "";
            if (earthwatcher.Password != null)
            {
               passwordPrefix = PasswordChecker.GeneratePrefix();
               hashedPassword = PasswordChecker.GenerateHashedPassword(earthwatcher.Password, passwordPrefix);
            }
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_CreateEarthwatcher";
            cmd.Parameters.Add(new SqlParameter("@guid", earthwatcher.Guid));
            cmd.Parameters.Add(new SqlParameter("@name", earthwatcher.Name));
            cmd.Parameters.Add(new SqlParameter("@role", earthwatcher.Role));
            cmd.Parameters.Add(new SqlParameter("@prefix", passwordPrefix));
            cmd.Parameters.Add(new SqlParameter("@hash", hashedPassword));
            cmd.Parameters.Add(new SqlParameter("@country", earthwatcher.Country));
            cmd.Parameters.Add(new SqlParameter("@language", earthwatcher.Language));
            if(!string.IsNullOrEmpty(earthwatcher.NickName))
            {
                cmd.Parameters.Add(new SqlParameter("@nick", earthwatcher.NickName));
            }
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            var id = (int)idParameter.Value;
            earthwatcher.Id = id;
            connection.Close();
            return earthwatcher;
        }

        public bool UpdatePassword(Earthwatcher earthwatcher, string passwordPrefix, string hashedPassword)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_UpdatePassword";
            cmd.Parameters.Add(new SqlParameter("@name", earthwatcher.Name));
            cmd.Parameters.Add(new SqlParameter("@prefix", passwordPrefix));
            cmd.Parameters.Add(new SqlParameter("@hash", hashedPassword));
            cmd.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        public void SetEarthwatcherAsPowerUser(int id, Earthwatcher earthwatcher)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_SetEarthwatcherAsPowerUser";
            cmd.Parameters.Add(new SqlParameter("@isPowerUser", true)); 
            cmd.Parameters.Add(new SqlParameter("@id", earthwatcher.Id));
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdateEarthwatcher(int id, Earthwatcher earthwatcher)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Earthwatcher_UpdateEarthwatcher";
                cmd.Parameters.Add(new SqlParameter("@role", earthwatcher.Role));
                cmd.Parameters.Add(new SqlParameter("@country", earthwatcher.Country));
                cmd.Parameters.Add(new SqlParameter("@name", earthwatcher.Name));  //PARA CAMBIAR EL MAIL
                if (!string.IsNullOrEmpty(earthwatcher.Region))
                {
                    cmd.Parameters.Add(new SqlParameter("@region", earthwatcher.Region));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@region", DBNull.Value));
                }
                cmd.Parameters.Add(new SqlParameter("@language", earthwatcher.Language));
                cmd.Parameters.Add(new SqlParameter("@notifyMe", earthwatcher.NotifyMe));
                cmd.Parameters.Add(new SqlParameter("@nickname", earthwatcher.NickName));
                cmd.Parameters.Add(new SqlParameter("@id", earthwatcher.Id));
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteEarthwatcher(int id)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Earthwatcher_DeleteEarthwatcher";
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public List<Earthwatcher> GetAllEarthwatchers()
        {
            connection.Open();
            var earthwatchers = connection.Query<Earthwatcher>("EXEC Earthwatcher_GetAllEarthwatchers");
            connection.Close();

            return earthwatchers.ToList();
        }

        public LandMini AssignLandToEarthwatcher(int earthwatcherId, string basecamp, string geohexKey) 
        {
            var land = GetFreeLand(basecamp, "'" + geohexKey + "'");
            if (land != null)
            {
                var landRepository = new LandRepository(connection.ConnectionString);
                landRepository.AssignLandToEarthwatcher(land.GeohexKey, earthwatcherId);
            }
            return land;
        }

        public LandMini GetFreeLand(string baseCamp, string geohexKey)
        {
            //Primero asigno un basecamp
            var basecampRepository = new BasecampRepository(connection.ConnectionString);
            List<Basecamp> basecamps = basecampRepository.GetByBasecamp(baseCamp);
            bool usedLand = false;
            string newgeohexKey = null;
            if (basecamps != null && basecamps.Count > 0)
            {
                int total = basecamps.Sum(x => x.Probability);
                int acumprob = 0;
                Random rand = new Random();
                int r = rand.Next(1, total + 1);
                int basecampid = 0;
                foreach (var basecamp in basecamps)
                {
                    acumprob += basecamp.Probability;
                    if (r < acumprob)
                    {
                        basecampid = basecamp.Id;
                        break;
                    }
                }

                if (basecampid != 0)
                {
                    var sql = "Earthwatcher_GetFreeLand";
                    connection.Open();
                    var cmd = connection.CreateCommand() as SqlCommand;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BasecampDetailId", basecampid);
                    cmd.Parameters.AddWithValue("@TutorLandId", Configuration.TutorLandId);
                    cmd.Parameters.AddWithValue("@GeoHexKey", geohexKey);

                    var reader = cmd.ExecuteReader();
                    var hasResult = reader.Read();
                    int landId = 0;
                    if (hasResult)
                    {
                        landId = reader.GetInt32(0);
                        newgeohexKey = reader.GetString(2);
                        usedLand = reader.GetBoolean(3);
                    }
                    connection.Close();
                    return new LandMini { GeohexKey = newgeohexKey, IsUsed = usedLand, Id = landId };
                }

            }

            return null;
        }


        public LandMini GetFreeLandByEarthwatcherId(string baseCamp, int earthwatcherId)
        {
            //Primero asigno un basecamp
            var basecampRepository = new BasecampRepository(connection.ConnectionString);
            List<Basecamp> basecamps = basecampRepository.GetByBasecamp(baseCamp);
            bool usedLand = false;
            string newgeohexKey = null;
            if (basecamps != null && basecamps.Count > 0)
            {
                int total = basecamps.Sum(x => x.Probability);
                int acumprob = 0;
                Random rand = new Random();
                int r = rand.Next(1, total + 1);
                int basecampid = 0;
                foreach (var basecamp in basecamps)
                {
                    acumprob += basecamp.Probability;
                    if (r < acumprob)
                    {
                        basecampid = basecamp.Id;
                        break;
                    }
                }

                if (basecampid != 0)
                {
                    try
                    {
                        var sql = "Earthwatcher_GetFreeLandByEarthwatcherId";
                        connection.Open();
                        var cmd = connection.CreateCommand() as SqlCommand;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BasecampDetailId", basecampid);
                        cmd.Parameters.AddWithValue("@TutorLandId", Configuration.TutorLandId);
                        cmd.Parameters.AddWithValue("@EarthwatcherId", earthwatcherId);

                        var reader = cmd.ExecuteReader();
                        var hasResult = reader.Read();
                        int landId = 0;
                        if (hasResult)
                        {
                            landId = reader.GetInt32(0);
                            newgeohexKey = reader.GetString(2);
                            usedLand = reader.GetBoolean(3);
                        }
                        connection.Close();
                        return new LandMini { GeohexKey = newgeohexKey, IsUsed = usedLand, Id = landId };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Ha ocurrido una excepcion :", ex); //TODO: HANDLER DE ESTA EXCEPTION
                    }
                }

            }

            return null;
        }


        public void SavePetitionSigned(PetitionsSigned petition) //No se usa
        {
            //if (petition.Signed != null && petition.PetitionId != 0 && !string.IsNullOrEmpty(petition.Earthwatcher))
            //{
            //    try
            //    {
            //        connection.Open();
            //        var cmd = connection.CreateCommand() as SqlCommand;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.CommandText = "Earthwatcher_SavePetition";
            //        cmd.Parameters.Add(new SqlParameter("@earthwatcherId", petition.Earthwatcher));
            //        cmd.Parameters.Add(new SqlParameter("@petitionId", petition.PetitionId));
            //        cmd.Parameters.Add(new SqlParameter("@signed", petition.Signed));
            //        cmd.ExecuteNonQuery();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //    finally
            //    {
            //        connection.Close();
            //    }
            //}
        }

        public PetitionsSigned HasSigned(PetitionsSigned petition) //no se usa
        {
            //try
            //{
            //    connection.Open();
            //    var cmd = connection.CreateCommand() as SqlCommand;
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.CommandText = "Earthwatcher_HasSigned";
            //    cmd.Parameters.Add(new SqlParameter("@earthwatcherId", petition.Earthwatcher));
            //    cmd.Parameters.Add(new SqlParameter("@petitionId", petition.PetitionId));
            //    var reader = cmd.ExecuteReader();
            //    var hasResult = reader.Read();
            //    if (hasResult)
            //    {
            //        petition.Signed = reader.GetBoolean(2); //Devuelve T/F si firmo o no la peticion
            //        return petition;
            //    }
            return null; //Devuelve null si no existe en la tabla para esa peticion
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    connection.Close();
            //}
        }



    }
}
