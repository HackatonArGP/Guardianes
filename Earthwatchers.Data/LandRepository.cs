using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Data;
using Dapper;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public class LandRepository : ILandRepository
    {
        private readonly IDbConnection connection;

        public LandRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }


        public void CreateLand(List<Land> newLand) //Inserta los registros de la nueva land en la tabla lands
        {
            var nrland = newLand.Count.ToString();
            try
            {
                Debug.WriteLine("Start creating" + nrland + " land.");

                DataTable dt = new DataTable();
                dt.Columns.Add("GeohexKey", typeof(string));
                dt.Columns.Add("Confirmed", typeof(bool));
                dt.Columns.Add("LandStatus", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Observations", typeof(string));
                dt.Columns.Add("Lat", typeof(double));
                dt.Columns.Add("Long", typeof(double));
                dt.Columns.Add("BasecampId", typeof(int));
                dt.Columns.Add("LandThreat", typeof(int));

                foreach (var land in newLand)
                {
                    DataRow row = dt.NewRow();
                    row[0] = land.GeohexKey;
                    row[1] = false;
                    row[2] = 0;
                    row[3] = null;
                    row[4] = null;
                    row[5] = land.Latitude;
                    row[6] = land.Longitude;
                    row[7] = land.BasecampId ?? (object)DBNull.Value;
                    row[8] = land.LandThreat.GetHashCode();
                    dt.Rows.Add(row);

                    Debug.WriteLine(land.GeohexKey);
                }
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Lands_Create";
                command.Parameters.Add(new SqlParameter { ParameterName = "@lands", SqlDbType = System.Data.SqlDbType.Structured, Value = dt });
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<LandCSV> GetLandsCSV()
        {
            connection.Open();
            var l = connection.Query<LandCSV>("EXEC Land_GetLandsCSV");
            var lands = l.ToList();
            connection.Close();
            return lands;
        }

        public List<string> GetVerifiedLandsGeoHexCodes(int earthwatcherId, bool isPoll) //Trae los geoHexKey de las lands lockeadas o para demandar si es poll, sino trae los geoHexKey de las que estan confirmadas
        {
            List<string> values = new List<string>();
            try
            {
                connection.Open();
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                string sql = string.Empty;
                if (isPoll)
                {
                    sql = "EXEC Land_GetVerifiedLandsGeoHexCodes_1"; //Solo si es poll
                    if (earthwatcherId != 0)
                    {
                        sql = string.Format("EXEC Land_GetVerifiedLandsGeoHexCodes_2 {0}", earthwatcherId); //Solo si es poll y tiene EwId
                    }
                }
                else
                {
                    sql = "EXEC Land_GetVerifiedLandsGeoHexCodes_3";
                }

                command.CommandText = sql;
                values = connection.Query<string>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return values;
        }

        public List<Statistic> GetStats()  //Trae los datos de la tabla de estadisticas
        {
            try
            {
                connection.Open();
                var stats = connection.Query<Statistic>("EXEC Lands_Stats");
                connection.Close();
                return stats.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<LandCSV> GetLandsToConfirm(int page, bool showVerifieds = false) //Trae las parcelas a la pantalla de Land BE para confirmar o deconfirmar
        {
            connection.Open();

            var l = connection.Query<LandCSV>("EXEC Land_GetLandsToConfirm");
                connection.Close();

            //TODO: Llevar esta logica al StoredProcedure y resolver en BBDD.
            var pagesize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["backend.pagination.pagesize"]);
            IEnumerable<LandCSV> lands = null;
            if (showVerifieds)
            {
                lands = l.Where(x => x.DemandAuthorities).ToList(); //only ready to demand
            }
            else
            {
                lands = l.Where(x => !x.Confirmed.HasValue).ToList(); //only unverified
            }

            lands = lands.Skip((page - 1) * pagesize).Take(pagesize).ToList(); //pagination

            connection.Close();

            return lands.ToList();
        }

        public bool ResetLands() //Resetea el estado de las parcelas verdes (acceso desde BE)
        {
            bool result = true;
            try
            {
                connection.Open();
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Land_ResetLands";
                command.Parameters.Add(new SqlParameter("@TutorLandId", Configuration.TutorLandId));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public bool ForceLandsReset(List<Land> lands, int earthwatcherId = 0)  //Resetea las parcelas que fueron confirmadas como (sin desmonte) y las deja asignables nuevamente
        { 
            bool result = true;
            if (lands.Any(x => x.Reset.HasValue && x.Reset.Value))
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("GeohexKey", typeof(string));
                    dt.Columns.Add("Confirmed", typeof(bool));
                    dt.Columns.Add("LandStatus", typeof(int));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Observations", typeof(string));
                    dt.Columns.Add("Lat", typeof(double));
                    dt.Columns.Add("Long", typeof(double));

                    foreach (var l in lands.Where(x => x.Reset.HasValue && x.Reset.Value))
                    {
                        DataRow row = dt.NewRow();
                        row[0] = l.GeohexKey;
                        row[1] = l.Confirmed;
                        row[2] = l.LandStatus;
                        row[3] = string.Empty;
                        row[4] = l.LastUsersWithActivity;
                        dt.Rows.Add(row);
                    }

                    connection.Open();

                    var command = connection.CreateCommand() as SqlCommand;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Lands_ForceLandsReset";
                    command.Parameters.Add(new SqlParameter { ParameterName = "@lands", SqlDbType = System.Data.SqlDbType.Structured, Value = dt });
                    command.Parameters.Add(new SqlParameter("@EarthwatcherId", earthwatcherId));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = false;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool UpdateLandsDemand(List<Land> lands, int earthwatcherId = 0) // Deja rojas las parcelas que greenpeace dijo ser amarillas, y cambia el estado de las mal confirmadas.
        {
            bool result = true;
            if (lands.Any(x => x.Confirmed.HasValue))
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("GeohexKey", typeof(string));
                    dt.Columns.Add("Confirmed", typeof(bool));
                    dt.Columns.Add("LandStatus", typeof(int));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Observations", typeof(string));
                    dt.Columns.Add("Lat", typeof(double));
                    dt.Columns.Add("Long", typeof(double));

                    foreach (var l in lands.Where(x => x.Confirmed.HasValue))
                    {
                        DataRow row = dt.NewRow();
                        row[0] = l.GeohexKey;
                        row[1] = l.Confirmed;
                        row[2] = l.LandStatus;
                        row[3] = string.Empty;
                        row[4] = l.LastUsersWithActivity;
                        dt.Rows.Add(row);
                    }

                    connection.Open();

                    var command = connection.CreateCommand() as SqlCommand;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Lands_UpdateDemand";
                    command.Parameters.Add(new SqlParameter { ParameterName = "@lands", SqlDbType = System.Data.SqlDbType.Structured, Value = dt });
                    command.Parameters.Add(new SqlParameter("@EarthwatcherId", earthwatcherId));
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = false;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                result = false;
            }
            return result;
        } 

        
        public string GetImageBaseUrl(bool isBase)
        {
            string whereclause = isBase ? "Name = '2008'" : "Published = (Select MAX(Published) From SatelliteImage)";  //TODO: STORED PROCEDURE PARA ESTE QUERY
            string sql = string.Format("Select UrlTileCache From SatelliteImage Where {0}", whereclause);
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                var baseUrl = command.ExecuteScalar();
                if (baseUrl != null && baseUrl != DBNull.Value)
                {
                    return baseUrl.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Land> GetLands(int id = 0, bool getAll = false, int earthwatcherId = 0, string name = "", string geohexKey = "", string wkt = "POLYGON EMPTY", int status = 0)
        {
            connection.Open();
            var lands = connection.Query<Land>(string.Format("EXEC Lands_Get {0}, {1}, {2}, '{3}', '{4}', '{5}', {6}", id, getAll, earthwatcherId, name, geohexKey, wkt, status));
            connection.Close();
            return lands.ToList();
        }

        //REVISAR SP, USA UNA TABLA QUE NO EXISTE
        public void LoadThreatLevel() 
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Update_LandThreat";
                command.CommandTimeout = 7200000;
                command.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                connection.Close();
            }
        }

        public void LoadLandBasecamp()  //Le pone el BasecampId a la land si es que intersecta con alguno, sino pone null
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Update_LandBasecamp";
                command.CommandTimeout = 7200000;
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public List<Land> GetAllLands() //Hace un select * de land
        {
            connection.Open();
            var command = connection.CreateCommand() as SqlCommand;
            command.CommandType = CommandType.StoredProcedure;
            var lands = connection.Query<Land>("EXEC Land_GetAllLands");
            connection.Close();
            return lands.ToList();
        }

        public Land GetLand(int id)  
        {
            return GetLands(id).FirstOrDefault();
        }

        public List<Land> GetLandByEarthwatcherName(string name)
        {
            return GetLands(0, false, 0, name);
        }

        public List<Land> GetAll(int earthwatcherId)
        {
            return GetLands(0, true, earthwatcherId);
        }

        public List<Land> GetLandByIntersect(string wkt, int landId)
        {
            return GetLands(landId, false, 0, string.Empty, string.Empty, wkt);
        }

        public List<Land> GetLandByStatus(LandStatus status)
        {
            return GetLands(0, false, 0, string.Empty, string.Empty, "POLYGON EMPTY", (int)status);
        }

        public Land GetLandByGeoHexKey(string geoHexKey)
        {
            return GetLands(0, false, 0, string.Empty, geoHexKey).FirstOrDefault();
        }

        //NO SE USA
        public LandMini ReassignLand(Land land, string basecamp)
        {
            var earthwatcherRepository = new EarthwatcherRepository(connection.ConnectionString);
            var newLand = earthwatcherRepository.GetFreeLand(basecamp, "'" + land.GeohexKey + "'");
            if (newLand != null && !String.IsNullOrEmpty(newLand.GeohexKey))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Land_ReassignLand";
                command.Parameters.Add(new SqlParameter("@EarthwatcherId", land.EarthwatcherId));
                command.Parameters.Add(new SqlParameter("@currentLand", land.Id));
                command.Parameters.Add(new SqlParameter("@newGeoHexKey", newLand.GeohexKey));
                command.Parameters.Add(new SqlParameter("@status", Convert.ToInt32(land.LandStatus)));
                command.ExecuteNonQuery();
                connection.Close();

                return newLand;
            }
            else return null;
        }
       
        public LandMini ReassignLand(int earthwatcherId, string basecamp) //Si ya la habia reportado la pasa a greenpeace, sino la deja para asignar
        {
            var earthwatcherRepository = new EarthwatcherRepository(connection.ConnectionString);
            var newLand = earthwatcherRepository.GetFreeLandByEarthwatcherId(basecamp, earthwatcherId);
            if (newLand != null && !String.IsNullOrEmpty(newLand.GeohexKey))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Land_ReassignLandByEarthwatcherId";
                command.Parameters.Add(new SqlParameter("@EarthwatcherId", earthwatcherId));
                command.Parameters.Add(new SqlParameter("@newGeoHexKey", newLand.GeohexKey));
                command.ExecuteNonQuery();
                connection.Close();

                return newLand;
            }
            else return null;
        }

        public bool MassiveReassign(string basecamp)
        {
            Dictionary<int, string> updateLands = new Dictionary<int, string>();
            string newGeoHexKeys = string.Empty;
            bool success = true;

            try
            {
                //1. Traigo todos los lands asignados
                connection.Open();
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandType = CommandType.StoredProcedure;
                var lands = connection.Query<Land>("EXEC Land_MassiveReassign_sql").ToList();
                connection.Close();

                EarthwatcherRepository earthwatcherRepository = new EarthwatcherRepository(this.connection.ConnectionString);

                Random rand = new Random();
                //2. Genero un Random para tener 1/3 de posibilidades de asignar una tierra nueva y 2/3 de una tierra usada 
                int r = rand.Next(1, 4);

                for (int i = 0; i < lands.Count; i++)
                {
                    var land = lands[i];
                    if (updateLands.ContainsKey(land.EarthwatcherId.Value))
                    {
                        continue;
                    }

                    //3. El 1 y el 2 corresponden a asignar una parcela usada y el 
                    if (i > 0)
                    {
                        r++;
                        if (r > 3)
                        {
                            r = 1;
                        }
                    }

                    if (lands.Count > (updateLands.Count + 1))
                    {
                        if (r < 3)
                        {
                            //Parcela Usada
                            int ru = rand.Next(0, lands.Count - 1 - updateLands.Count);
                            //var l = lands.Where(x => x.Id != land.Id && !updateLands.ContainsKey(x.EarthwatcherId.Value) && !updateLands.ContainsValue(x.GeohexKey)).Skip(ru).FirstOrDefault();
                            var oldland = (from l in lands
                                           where (from u in updateLands where u.Key == l.EarthwatcherId.Value select u).Count() == 0
                                           select l).Skip(ru).FirstOrDefault();

                            if (oldland != null)
                            {
                                updateLands.Add(land.EarthwatcherId.Value, oldland.GeohexKey);
                                if (!updateLands.ContainsKey(oldland.EarthwatcherId.Value))
                                {
                                    updateLands.Add(oldland.EarthwatcherId.Value, land.GeohexKey);
                                }
                            }
                        }
                        else
                        {
                            var newFreeLand = earthwatcherRepository.GetFreeLand(basecamp, newGeoHexKeys);
                            if (newFreeLand != null)
                            {
                                //Parcela Nueva
                                if (newGeoHexKeys != string.Empty)
                                {
                                    newGeoHexKeys += ",";
                                }
                                newGeoHexKeys += "'" + land.GeohexKey + "'";
                                newGeoHexKeys += ",'" + newFreeLand.GeohexKey + "'";

                                updateLands.Add(land.EarthwatcherId.Value, newFreeLand.GeohexKey);
                            }
                        }
                    }
                    else
                    {
                        //Ultima parcela impar
                        //Parcela Nueva
                        var newland = (from l in lands
                                       where (from u in updateLands where u.Key == l.EarthwatcherId.Value select u).Count() == 0
                                       select l).FirstOrDefault();

                        if (newland != null)
                        {
                            var newFreeLand = earthwatcherRepository.GetFreeLand(basecamp, newGeoHexKeys);
                            if (newFreeLand != null)
                            {
                                if (newGeoHexKeys != string.Empty)
                                {
                                    newGeoHexKeys += ",";
                                }
                                newGeoHexKeys += "'" + land.GeohexKey + "'";
                                newGeoHexKeys += ",'" + newFreeLand.GeohexKey + "'";
                                updateLands.Add(newland.EarthwatcherId.Value, newFreeLand.GeohexKey);
                            }
                        }
                        break;
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("EarthwatcherId", typeof(int));
                dt.Columns.Add("GeohexKey", typeof(string));

                foreach (var land in updateLands)
                {
                    DataRow row = dt.NewRow();
                    row[0] = land.Key;
                    row[1] = land.Value;
                    dt.Rows.Add(row);
                }

                connection.Open();

                command.CommandText = "Lands_Reassign";
                command.Parameters.Add(new SqlParameter { ParameterName = "@lands", SqlDbType = System.Data.SqlDbType.Structured, Value = dt });
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return success;
        }

        public void UpdateLandStatus(int id, LandStatus landStatus) //Cambia el status de a parcela (Id) por el que fue pasado por parametro
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Land_UpdateLandStatus";
            command.Parameters.Add(new SqlParameter("@StatusChangedDateTime", DateTime.UtcNow));
            command.Parameters.Add(new SqlParameter("@LandStatus", landStatus));
            command.Parameters.Add(new SqlParameter("@Id", id));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AssignLandToEarthwatcher(string geohex, int earthwatcherid) //Desasigna la parcela de su dueño, se la asigna al nuevo(params) y le pone status 2(sin revisar)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Land_AssignLandToEarthwatcher";
            command.Parameters.Add(new SqlParameter("@GeohexKey", geohex));
            command.Parameters.Add(new SqlParameter("@EarthwatcherId", earthwatcherid));
            command.ExecuteNonQuery();
            connection.Close();
        }

        //NO SE USA
        public void UnassignLand(int id) //Elimina de EarthwatcherLands esa parcela y le cambia unicamente el status a 1(sin asignar)
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Land_UnassignLand";
            command.Parameters.Add(new SqlParameter("@Id", id));
            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<Score> GetLastUsersWithActivityScore(int landId) //Trae la lista de los usuarios que hicieron verificaciones en esa parcela
        {
            connection.Open();
            var scores = connection.Query<Score>(string.Format("EXEC Land_GetLastUsersWithActivityScore {0}", landId));
            connection.Close();
            return scores.ToList();
        }

        public LandMini AddVerification(LandMini land, bool isAlert)
        {
        //    connection.Open();
        //    var command = connection.CreateCommand();
        //    command.CommandType = CommandType.StoredProcedure;
        //    int verifications = connection.Query<int>(string.Format("EXEC Land_AddVerification {0}, {1}, {2} ", land.LandId, land.Id, isAlert ? 1 : 0)).First();
        //    connection.Close();
        //    return VerificationScoring(land, verifications);
            connection.Open();
            var sql = string.Format(@"IF NOT EXISTS (select Earthwatcher from Verifications where land = {0} and earthwatcher = {1} and IsAlert = {2})
	                                    BEGIN
		                                    Delete From Verifications where land = {0} and earthwatcher = {1}
		                                    INSERT INTO Verifications (Land, Earthwatcher, IsAlert)
		                                    VALUES ({0}, {1}, {2})
		                                    Select count(Land) From Verifications Where land = {0}
	                                    END
                                    ELSE
                                        BEGIN
	                                        SELECT 0
                                    END

                                    Declare @Alert int = (select COUNT(IsAlert) as Alert from Verifications where IsDeleted = 0 and IsAlert = 1 and Land = {0})
                                    Declare @Ok int = (select COUNT(IsAlert) as Ok from Verifications where IsDeleted = 0 and IsAlert = 0 and Land = {0})
                                    Declare @ActualOwner int = (select top 1 Earthwatcher from EarthwatcherLands where Land = {0})
                                    Declare @OwnerVerification int = (select isAlert from Verifications where IsDeleted = 0 and Earthwatcher = @ActualOwner)
		
                                    update Land 
                                    set LandStatus = case when @Alert > @Ok then 4
					                                      when @Ok > @Alert then 3
					                                      when @ActualOwner != 17 and @OwnerVerification = 1 then 4 
					                                      when @ActualOwner != 17 and @OwnerVerification = 0 then 3 end
                                    where Id = {0}
                                    ", land.LandId, land.Id, isAlert ? 1 : 0);
            int verifications = connection.Query<int>(sql).First();
            connection.Close();
            return VerificationScoring(land, verifications);
        }

        public void AddPoll(LandMini land)  //Guarda en la tabla PollResults el resultado de la poll
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Land_AddPoll";
            command.Parameters.Add(new SqlParameter("@GeohexKey", land.GeohexKey));
            command.Parameters.Add(new SqlParameter("@EarthwatcherId", land.EarthwatcherId));
            command.Parameters.Add(new SqlParameter("@IsUsed", land.IsUsed ? 1 : 0));
            command.ExecuteNonQuery();
            connection.Close();
        }

        private LandMini VerificationScoring(LandMini land, int verifications) //Busca al primer usuario que reporto esa parcela con 30 verif, le asigna los puntos, lockea la parcela y la pasa a greenpeace.
        {
            if (verifications >= 30) //cambiar Verifications
            {
                //Si es el usuario de Greenpeace debería buscar el owner original
                if (land.EarthwatcherId == Configuration.GreenpeaceId)
                {
                    connection.Open();

                    int? ewId = connection.Query<int>(string.Format("EXEC Land_VerificationScoring {0}, {1}", land.LandId, Configuration.GreenpeaceId)).SingleOrDefault();
                    if (ewId != null)
                    {
                        land.EarthwatcherId = Convert.ToInt32(ewId);
                    }
                    connection.Close();
                }

                //Obtengo el mail del owner original para mandarle la notificacion por mail
                connection.Open();
                string idobjMail = connection.Query<string>(string.Format("EXEC Land_VerificationScoring_2 {0}, {1}", land.LandId, Configuration.GreenpeaceId)).SingleOrDefault();
                if (idobjMail != null)
                {
                    land.Email = idobjMail;
                }
                connection.Close();

                //Le asigno los 2000 puntos
                var scoreRepository = new ScoreRepository(connection.ConnectionString);
                scoreRepository.PostScore(new Score { Action = ActionPoints.Action.LandVerified.ToString(), LandId = land.LandId, EarthwatcherId = land.EarthwatcherId, Points = ActionPoints.Points(ActionPoints.Action.LandVerified), Published = DateTime.UtcNow });

                //El owner de la parcela pasa a ser Greenpeace y la parcela se lockea
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Land_VerificationScoring_3";
                cmd.Parameters.Add(new SqlParameter("@LandId", land.LandId));
                cmd.ExecuteNonQuery();
                connection.Close();
                return land;
            }
            return null;
        }
    }
}
