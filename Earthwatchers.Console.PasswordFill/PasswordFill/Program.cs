using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace PasswordFill
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("tool to read users from earthwatchers database and create prefix and hash for them");
            var connectionString = "Data Source=dfrvf2t76i.database.windows.net;Initial Catalog=Earthwatchers2;Persist Security Info=True;User ID=greenpeace;Asynchronous Processing=True;Password=hqgdSHa1";
            
            // read the users
            var connection = new SqlConnection(connectionString);
            connection.Open();
            var earthwatchers = connection.Query<Earthwatcher>("select Id,EarthwatcherGuid as Guid, Name from Earthwatcher");

            // create the hash and prefix
            foreach (var earthwatcher in earthwatchers)
            {
                earthwatcher.PasswordPrefix = PasswordChecker.GeneratePrefix();
                earthwatcher.HashedPassword = PasswordChecker.GenerateHashedPassword("test123", earthwatcher.PasswordPrefix);
            }


            // store the new values
            foreach (var earthwatcher in earthwatchers)
            {
                //if (earthwatcher.Name == "bertt")
                //{
                    var sql = "update earthwatcher set passwordprefix = @passwordprefix, hashedpassword = @hashedpassword where id=@id";
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@passwordprefix", earthwatcher.PasswordPrefix));
                    cmd.Parameters.Add(new SqlParameter("@hashedpassword", earthwatcher.HashedPassword));
                    cmd.Parameters.Add(new SqlParameter("@id", earthwatcher.Id));
                    cmd.ExecuteNonQuery();
                //}
            }

            connection.Close();
            Console.WriteLine("finished");
            Console.ReadKey();
        }
    }
}
