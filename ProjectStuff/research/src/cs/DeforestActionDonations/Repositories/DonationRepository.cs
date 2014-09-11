using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using Dapper;
using DeforestActionDonations.Models;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;

namespace DeforestActionDonations.Repositories
{
    public class DonationRepository
    {
        private IDbConnection connection = null;

        public DonationRepository(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public List<Adopter> GetAllAdopters()
        {
            connection.Open();
            var l = connection.Query<Adopter>("select gid, id, geom.STAsText() as geom, transaction_nr, name, username, amount, area from " + Constants.AdoptersTable);
            var adopters = l.ToList();
            connection.Close();
            return adopters;               
        }

        //TODO: Add request for adopter by username

        public Adopter AssignLand(Adopter adopter)
        {
            connection.Open();

            AssignPlotOfLand assigner = new AssignPlotOfLand();

            SqlGeometry startingSquareKm = GeometryExtensions.FromWkt("POLYGON((12390034 -32564, 12390034 -32565, 12390035 -32565, 12390035 -32564, 12390034 -32564))", Constants.SRID_INT);
            assigner.SetStartingPoint(startingSquareKm.STPointN(1));
            {
                foreach (var geom in connection.Query<string>("select top 1 geom.STAsText() from " + Constants.LandTable))
                {
                    assigner.SetOuterBoundary(SqlGeometry.STGeomFromText(new SqlChars(geom), Constants.SRID_INT));
                }
            }
            connection.Close();

            var adopters = GetAllAdopters();

            //Check if transaction does not exist, else return null
            foreach (var a in adopters)
            {
                if (a.transaction_nr == adopter.transaction_nr)
                    return null;
            }

            var assignedGeom = assigner.Assign(adopter.area, Constants.SRID_INT, adopters);
            if (assignedGeom == null) 
                return null;
            
            adopter.geom = new string(assignedGeom.STAsText().Value);
            
            return adopter;
        }

        public Adopter AddAdopter(Adopter adopter)
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "insert into " + Constants.AdoptersTable + "(cellnum, transaction_nr, name, username, amount, area, geom) values(@cellnum,@transaction_nr,@name,@username,@amount,@area,@geom)";
                cmd.Parameters.Add(new SqlParameter("@cellnum", 1));
                cmd.Parameters.Add(new SqlParameter("@transaction_nr", adopter.transaction_nr));
                cmd.Parameters.Add(new SqlParameter("@name", adopter.username));
                cmd.Parameters.Add(new SqlParameter("@username", adopter.username));
                cmd.Parameters.Add(new SqlParameter("@amount", adopter.amount));
                cmd.Parameters.Add(new SqlParameter("@area", adopter.area));
                cmd.Parameters.Add(new SqlParameter("@geom", SqlGeometry.STGeomFromText(new SqlChars(adopter.geom), Constants.SRID_INT)) { UdtTypeName = "Geometry" });

                cmd.ExecuteNonQuery();
                connection.Close();

                return adopter;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}