using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Earthwatchers.Models.KmlModels;
using System.Transactions;
using Dapper;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;

namespace Earthwatchers.Data
{
    public class LayerRepository : ILayerRepository
    {
        private readonly IDbConnection connection;

        public LayerRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void SaveLayer(Layer lay)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SaveLayer";
            cmd.Parameters.Add(new SqlParameter("@name", lay.Name));
            var desc = string.IsNullOrEmpty(lay.Description) ? DBNull.Value : (object)lay.Description;
            cmd.Parameters.Add(new SqlParameter("@description", desc));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            lay.Id = (int)idParameter.Value;

        }

        public void SaveZone(Zone zon, int layId)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SaveZone";

            cmd.Parameters.Add(new SqlParameter("@name", zon.Name));
            var desc = string.IsNullOrEmpty(zon.Description) ? DBNull.Value : (object)zon.Description;
            cmd.Parameters.Add(new SqlParameter("@description", desc));
            cmd.Parameters.Add(new SqlParameter("@layerId", layId));
            cmd.Parameters.Add(new SqlParameter("@param1", DBNull.Value));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            zon.Id = (int)idParameter.Value;

        }

        public void SaveFinca(Zone zon, int layId)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SaveZone";

            cmd.Parameters.Add(new SqlParameter("@name", zon.Name));
            var desc = string.IsNullOrEmpty(zon.Description) ? DBNull.Value : (object)zon.Description;
            cmd.Parameters.Add(new SqlParameter("@description", desc));
            cmd.Parameters.Add(new SqlParameter("@layerId", layId));
            cmd.Parameters.Add(new SqlParameter("@param1", zon.Param1));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            zon.Id = (int)idParameter.Value;

        }

        public void SavePolygon(Polygon pol, int zonId)
        {
            SqlGeometry polygon = SqlGeometry.Parse(pol.PolygonGeom);

            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SavePolygon";

            cmd.Parameters.Add(new SqlParameter("@name", pol.Name));
            cmd.Parameters.Add(new SqlParameter("@zoneId", zonId));
            cmd.Parameters.Add(new SqlParameter("@Polygon", System.Data.SqlDbType.Udt));
            cmd.Parameters["@Polygon"].UdtTypeName = "geometry";
            cmd.Parameters["@Polygon"].Value = polygon;

            SqlParameter idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            pol.Id = (int)idParameter.Value;
        }

        public void SaveFincaPolygon(Polygon pol, int zonId)
        {
            SqlGeometry polygon = SqlGeometry.Parse(pol.PolygonGeom);

            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SavePolygon";

            cmd.Parameters.Add(new SqlParameter("@name", pol.Name));
            cmd.Parameters.Add(new SqlParameter("@zoneId", zonId));
            cmd.Parameters.Add(new SqlParameter("@Polygon", System.Data.SqlDbType.Udt));
            cmd.Parameters["@Polygon"].UdtTypeName = "geometry";
            cmd.Parameters["@Polygon"].Value = polygon;

            SqlParameter idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
            pol.Id = (int)idParameter.Value;
        }

        public void SaveLocation(Location loc, int polId)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Layer_SaveLocation";

            cmd.Parameters.Add(new SqlParameter("@latitude", loc.Latitude));
            cmd.Parameters.Add(new SqlParameter("@longitude", loc.Longitude));
            cmd.Parameters.Add(new SqlParameter("@index", loc.Index));
            cmd.Parameters.Add(new SqlParameter("@polygonId", polId));
            var idParameter = new SqlParameter("@ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParameter);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void SaveLayerFull(Layer lay)
        {
            using (var scope = new TransactionScope())
            {
                SaveLayer(lay);
                foreach (Zone z in lay.Zones)
                {
                    SaveZone(z, lay.Id);
                    foreach (Polygon pol in z.Polygons)
                    {
                        SavePolygon(pol, z.Id);
                        foreach (Location loc in pol.Locations)
                        {
                            SaveLocation(loc, pol.Id);
                        }
                    }
                }

                scope.Complete();
            }
        }

        public void SaveFincaFull(Layer lay)
        {
            using (var scope = new TransactionScope())
            {
                var layerDb = this.GetLayerByName(lay.Name);
                lay.Id = layerDb.Id;

                foreach (Zone z in lay.Zones)
                {
                    DeleteZone(z.Id);
                    SaveFinca(z, lay.Id);
                    foreach (Polygon pol in z.Polygons)
                    {
                        SaveFincaPolygon(pol, z.Id);
                        foreach (Location loc in pol.Locations)
                        {
                            SaveLocation(loc, pol.Id);
                        }
                    }
                }

                scope.Complete();
            }
        }

        public void DeleteZone(int zoneId)
        {
            using (var cmd = connection.CreateCommand() as SqlCommand)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Zone_Delete";
                cmd.Parameters.Add(new SqlParameter("@ZoneId", zoneId));
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }


        public List<Zone> GetZones(int layerId)
        {
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<Zone> zones = connection.Query<Zone>(string.Format("EXEC Layer_GetZones {0}", layerId)).ToList();

            foreach (Zone zon in zones)
            {
                zon.Polygons = this.GetPolygons(zon.Id);
            }
            return zones.ToList();

        }

        public List<Polygon> GetPolygons(int zoneId)
        {
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<Polygon> polygons = connection.Query<Polygon>(string.Format("EXEC Layer_GetPolygons {0}", zoneId)).ToList();

            foreach (Polygon pol in polygons)
            {
                pol.Locations = this.GetLocations(pol.Id);
            }
            return polygons;
        }

        public List<Location> GetLocations(int polygonId)
        {
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            List<Location> locations = connection.Query<Location>(string.Format("EXEC Layer_GetLocations {0}", polygonId)).ToList();

            return locations;
        }

        public Layer GetLayer(int ID)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var layers = connection.Query<Layer>(string.Format("EXEC Layer_GetLayer {0}", ID)).ToList();

            var layer = layers.FirstOrDefault();
            layer.Zones = this.GetZones(layer.Id);
            connection.Close();
            return layer;
        }

        public Layer GetLayerByName(string name)
        {
            connection.Open();
            var cmd = connection.CreateCommand() as SqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var layers = connection.Query<Layer>(string.Format("EXEC Layer_GetLayerByName {0}", name)).ToList();

            var layer = layers.FirstOrDefault();
            if (layer != null)
            {
                layer.Zones = this.GetZones(layer.Id);
            }
            connection.Close();
            return layer;
        }
    }
}
