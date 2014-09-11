using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Earthwatchers.Models.Portable;
using Earthwatchers.Data;
using Earthwatchers.Models;


namespace Earthwatchers.Services.Security.MembershipProviders
{
    public class EarthwatchersMembershipProvider : IMembershipProvider
    {
        private readonly string connectionString;

        public EarthwatchersMembershipProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ValidateUser(string username, string password, int apiEwId = 0)
        {
            var isValid = false;
            ApiEw apiEw = null;
            EarthwatcherRepository earthwatcherRepo = new EarthwatcherRepository(connectionString);
            if(apiEwId != 0)
            {
                apiEw = earthwatcherRepo.GetApiEwById(apiEwId);
            }
            if (apiEw == null)
            {
                var credentials = CredentialsRepository.GetCredentials(connectionString, username);
                isValid = PasswordChecker.CheckPassword(password, credentials.Prefix, credentials.Hash);
            }
            else
            {
                //TODO: Validacion de AccessToken, por ahora si entro con una api lo manda derecho
                isValid = true;
            }
            return isValid;
        }
    }
}