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
   public class ContestRepository : IContestRepository
    {
        private readonly IDbConnection connection;

        public ContestRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public Contest GetContest()
        {
            var all = GetAllContests();

            var contest = all.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).FirstOrDefault();
            return contest;
        }

        public Contest GetWinner()
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<Contest> contestInfo = connection.Query<Contest>("EXEC Contest_GetWinner").ToList();
            connection.Close();
            return contestInfo.FirstOrDefault();
        }

        public List<Contest> GetAllContests()
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<Contest> contestInfo = connection.Query<Contest>("EXEC Contest_GetAllContests").ToList();
            connection.Close();
            if (contestInfo != null)
                return contestInfo;
            else
                return null;
        }

        public Contest Insert(Contest contest)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Contest_Insert";
                cmd.Parameters.Add(new SqlParameter("@startDate", contest.StartDate));
                cmd.Parameters.Add(new SqlParameter("@endDate", contest.EndDate));
                cmd.Parameters.Add(new SqlParameter("@shortTitle", contest.ShortTitle));
                cmd.Parameters.Add(new SqlParameter("@title", contest.Title));
                cmd.Parameters.Add(new SqlParameter("@description", contest.Description));
                if (contest.ImageURL == null)
                    contest.ImageURL = "";
                cmd.Parameters.Add(new SqlParameter("@imageUrl", contest.ImageURL));
                var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParameter);
                cmd.ExecuteNonQuery();
                connection.Close();
                return contest;
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
                cmd.CommandText = "Contest_Delete";
                cmd.Parameters.Add(new SqlParameter("@id", id));
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
