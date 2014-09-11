using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Earthwatchers.Models.Portable;

namespace Earthwatchers.Data
{
    public class CredentialsRepository
    {
        public static Credentials GetCredentials(string connectionString, string username)
        {
            //TODO: VER PORQUE NO FUNCIONA CON CONNECTION.QUERY
            var connection = new SqlConnection(connectionString);
            var sql = "select passwordprefix as prefix, hashedpassword as hash from Earthwatcher where name =  @username";
            connection.Open();
            var credentialList = connection.Query<Credentials>(sql, new { Username = username });
            //var credentialList = connection.Query<Credentials>(string.Format("Credentials_GetCredentials {0}", username));
            connection.Close();
            return credentialList.FirstOrDefault();
        }
    }
}
