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
    public class CollectionsRepository : ICollectionsRepository
    {
        private readonly IDbConnection connection;

        public CollectionsRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<CollectionItem> GetCollectionItemsByEarthwatcher(int earthwatcherId)
        {
            connection.Open();
            var items = connection.Query<CollectionItem>(string.Format("EXEC Collections_GetCollectionItemsByEarthwatcher {0}", earthwatcherId));
            connection.Close();
            return items.ToList();
        }

        public CollectionItem GetNewCollectionItem(int earthwatcherId)
        {
            connection.Open();
            var items = connection.Query<CollectionItem>(string.Format("EXEC Collections_GetNewCollectionItem {0}", earthwatcherId));
            connection.Close();
            return items.FirstOrDefault();
        }

        public int GetTotalItems(int earthwatcherId)
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Collections_GetTotalItems";
            cmd.Parameters.Add(new SqlParameter("@earthwatcherId", earthwatcherId));
            object obj = cmd.ExecuteScalar();
            connection.Close();
            if (obj == null)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }
    }
}
