using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Earthwatchers.Models;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;

namespace Earthwatchers.Data
{
    public class SatelliteImageRepository : ISatelliteImageRepository
    {
        private readonly IDbConnection connection;

        public SatelliteImageRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public SatelliteImage Get(int id)
        {
            connection.Open();
            var satelliteImage = connection.Query<SatelliteImage>(string.Format("EXEC SatelliteImage_Get {0}", id)).FirstOrDefault();
            connection.Close();
            AddExtent(satelliteImage);
            return satelliteImage;
        }

        public List<SatelliteImage> GetAll()
        {
            connection.Open();
            var satelliteImages = connection.Query<SatelliteImage>("EXEC SatelliteImage_GetAll").ToList();
            connection.Close();
            UpdateList(satelliteImages);
            return satelliteImages;
        }

        public List<SatelliteImage> Intersects(string wkt)
        {
            connection.Open();
            var satelliteImages = connection.Query<SatelliteImage>(string.Format("EXEC SatelliteImage_Intersects {0}" , wkt)).ToList();
            connection.Close();
            UpdateList(satelliteImages);
            return satelliteImages.ToList();
        }

        private void AddExtent(SatelliteImage satelliteImage)
        {
            satelliteImage.Extent = new Extent(satelliteImage.xmin, satelliteImage.ymin, satelliteImage.xmax, satelliteImage.ymax, 4326);
        }

        public void UpdateList(List<SatelliteImage> images)
        {
            foreach (var satelliteImage in images)
            {
                AddExtent(satelliteImage);
            }
        }

        public void Update(int id, SatelliteImage satelliteImage)
        {
            throw new NotImplementedException();
        }

        public SatelliteImage Insert(SatelliteImage satelliteImage)
        {
            connection.Open();
            if(satelliteImage.AcquisitionDate == null) satelliteImage.AcquisitionDate = DateTime.Now;
            if (satelliteImage.Published == null) satelliteImage.Published = DateTime.Now.Date;
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SatelliteImage_Insert";
            
            cmd.Parameters.Add(new SqlParameter("@extent", satelliteImage.Wkt));
            cmd.Parameters.Add(new SqlParameter("@name", satelliteImage.Name));
            cmd.Parameters.Add(new SqlParameter("@provider", satelliteImage.Provider));
            cmd.Parameters.Add(new SqlParameter("@imagetype", satelliteImage.ImageType));
            cmd.Parameters.Add(new SqlParameter("@urltilecache", (object)satelliteImage.UrlTileCache?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@published", satelliteImage.Published));
            cmd.Parameters.Add(new SqlParameter("@acquisitiondate", satelliteImage.AcquisitionDate));
            cmd.Parameters.Add(new SqlParameter("@minlevel", satelliteImage.MinLevel));
            cmd.Parameters.Add(new SqlParameter("@maxlevel", satelliteImage.MaxLevel));
            cmd.Parameters.Add(new SqlParameter("@urlmetadata", (object)satelliteImage.UrlMetadata ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@iscloudy", satelliteImage.IsCloudy));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) {Direction = ParameterDirection.Output};
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            return satelliteImage;
        }

        public void Delete(int id)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SatelliteImage_Delete";
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
