using Earthwatchers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Earthwatchers.Data
{
    public class StatisticsQueryRepository : IStatisticsQueryRepository
    {
        private readonly IDbConnection connection;

        public StatisticsQueryRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public IEnumerable<StatisticsQuery> GetStats(string stat, DateTime startDate, DateTime endDate) 
        {
            connection.Open(); //TODOS LOS SP DEBEN TENER EL MISMO NOMBRE QE EL PARAMETRO
            var stats = connection.Query<StatisticsQuery>(string.Format("EXEC StatisticQuery_{0} {1} {2} ", stat, startDate, endDate));
            connection.Close();
            return stats;
        }
    }
}
