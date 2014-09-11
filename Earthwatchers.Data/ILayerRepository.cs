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

namespace Earthwatchers.Data
{
    public interface ILayerRepository
    {
        void SaveLayer(Layer lay);
        void SaveZone(Zone zon, int layId);
        void SavePolygon(Polygon pol, int zonId);
        void SaveLocation(Location loc, int polId);
        void SaveLayerFull(Layer lay);
        void SaveFincaFull(Layer lay);
        Layer GetLayer(int ID);
        Layer GetLayerByName(string name);
        void DeleteZone(int zoneId);
    }
}
