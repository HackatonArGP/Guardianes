using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using Dapper;
using DeforestActionDonations.Models;

namespace DeforestActionDonations.Repositories
{
    public class LandRepository
    {
        private IDbConnection connection = null;

        public LandRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<DonationsLand> GetAll()
        {
            connection.Open();
            var l = connection.Query<DonationsLand>("select top 1 gid, geom.STAsText() as geom from " + Constants.LandTable);
            var lands = l.ToList();
            connection.Close();
            return lands;               
        }
    }
}