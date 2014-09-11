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
    public class PopupMessageRepository : IPopupMessageRepository
    {
        private readonly IDbConnection connection;

        public PopupMessageRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<PopupMessage> GetMessage()
        {
            var all = GetAllMessages();

            var pms = all.Where(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).ToList();
            if (pms != null)
                return pms;
            else
                return null;
        }

        public List<PopupMessage> GetAllMessages()
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<PopupMessage> popupInfo = connection.Query<PopupMessage>("EXEC PopUpMessage_GetAllMessages").ToList();
            connection.Close();
            if (popupInfo != null)
                return popupInfo;
            else
                return null;
        }

        public PopupMessage Insert(PopupMessage message)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand() as SqlCommand;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PopUpMessage_Insert";
                cmd.Parameters.Add(new SqlParameter("@startDate", message.StartDate));
                cmd.Parameters.Add(new SqlParameter("@endDate", message.EndDate));
                cmd.Parameters.Add(new SqlParameter("@shortTitle", message.ShortTitle));
                cmd.Parameters.Add(new SqlParameter("@title", message.Title));
                cmd.Parameters.Add(new SqlParameter("@description", message.Description));
                if (message.ImageURL == null)
                    message.ImageURL = "";
                cmd.Parameters.Add(new SqlParameter("@imageUrl", message.ImageURL));
                var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParameter);
                cmd.ExecuteNonQuery();
                connection.Close();
                return message;
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
                cmd.CommandText = "PopUpMessage_Delete";
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
