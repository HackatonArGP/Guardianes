
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Earthwatchers.Models;
using System.Linq;

namespace Earthwatchers.Data
{
    public class SettingsRepository : Earthwatchers.Data.ISettingsRepository
    {
        private readonly IDbConnection connection;

        public SettingsRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public Setting GetSetting(string name)
        {
            connection.Open();
            var setting = connection.Query<Setting>("select Name,Val from earthwatchers.Settings where Name=@name", new { name });
            connection.Close();
            return setting.FirstOrDefault();
        }

    }
}
