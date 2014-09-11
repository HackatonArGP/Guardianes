using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Models.KmlModels;

namespace Earthwatchers.Landgenerator
{

    //var currentLon = topLeft.X;
    //var currentLat = topLeft.Y;

    class Program
    {
        private static ComputableLayer _forestlaw = null;
        private static ComputableLayer _basecamps = null;

        static void Main(string[] args)
        {
            try
            {
                var st = DateTime.Now;

                //var topLeft = new PointD(-65.742, -21.988);     //UBICACIONES DEL CUADRADO GRANDE
                //var bottomRight = new PointD(-62.057556, -26.558016); 

                var topLeft = new PointD(-63.655230, -22.055296);
                var bottomRight = new PointD(-63.098436, -22.508956);

                var repo = new LayerRepository("Server=.;Database=Earthwatchers;Trusted_Connection=yes;");


                Console.WriteLine("Cargando Fincas en memoria...");
                Layer basecampsLayer = repo.GetLayerByName("FincasLayer");//TODO: reemplazar por el nobmre final q va a tener el layer de Basecamps/Fincas

                Console.WriteLine("Cargando Ley de bosques en memoria...");
                Layer forestLawLayer = repo.GetLayerByName("OTBN");

                var bclist = basecampsLayer.Zones.Select(z => new ComputableZone(z)).ToList();
                _basecamps = new ComputableLayer(bclist);

                var lawlist = forestLawLayer.Zones.Select(z => new ComputableZone(z)).ToList();
                _forestlaw = new ComputableLayer(lawlist);



                Console.WriteLine("Intersectando Lands con Fincas y ley de bosques...");
                var newLands = GenerateLands(topLeft, bottomRight, 7);


                // write land to database...
                //var landRepository = new LandRepository("Data Source=dfrvf2t76i.database.windows.net;Initial Catalog=Earthwatchers;Persist Security Info=True;User ID=Editor;Asynchronous Processing=True;Password=8p3k00l!!!!");
                var conbase = newLands.Where(x => x.BasecampId != null);
                Console.WriteLine("Guardando las " + newLands.Count + " lands generadas...");
                var landRepository = new LandRepository("Server=.;Database=Earthwatchers;Trusted_Connection=yes;");
                landRepository.CreateLand(newLands);

                Console.WriteLine("Cargando Threat levels...");
                landRepository.LoadThreatLevel();

                Console.WriteLine("Asignando BasecampId a lands...");
                landRepository.LoadLandBasecamp();

                Console.WriteLine("Carga Completa");
                var horaFin = DateTime.Now;
                Console.WriteLine("Hora Fin CreateLand: " + horaFin); 

                Console.WriteLine("Klaar"); //Listo Holandés
                Console.ReadKey();

                Console.WriteLine("Por las dudas hay mas");//BORRAR
                Console.ReadKey();                        //BORRAR
                Console.WriteLine("sisi, son varios");   //BORRAR
                Console.ReadKey();                      //BORRAR
                Console.WriteLine("el ultimo");        //BORRAR
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<Land> GenerateLands(PointD topLeft, PointD bottomRight, int level)
        {
            var horaInicio = DateTime.Now;
            Console.WriteLine("Hora de inicio GenerateLands: " + horaInicio); 
            var newLand = new List<Land>();
            const double increase = 0.0075;

            int assignables = 0;
            int unassignables = 0;

            for (var i = topLeft.X; i <= bottomRight.X; i += increase)
            {
                for (var j = topLeft.Y; j >= bottomRight.Y; j -= increase)
                {
                    var hexKey = GeoHex.Encode(i, j, level);
                    var land = new Land();
                    land.Longitude = i;
                    land.Latitude = j;
                    land.GeohexKey = hexKey;

                    if ((!newLand.Any(l => l.GeohexKey == hexKey)))
                    {
                        //if (ComputeLandThreat(land))
                        //{
                         //ComputeBasecampIntersection(land);
                         newLand.Add(land);
                         assignables++;
                    }
                        else unassignables++;
                    //}
                    //else unassignables++;
                }
            }
            var horaFin = DateTime.Now;
            Console.WriteLine("Hora Fin GenerateLands: " + horaFin);
            Console.WriteLine("ASIGNABLES: " + assignables);
            Console.WriteLine("NO ASIGNABLES: " + unassignables); 

            return newLand;
        }

        /// <summary>
        /// Realiza todos los calculos para evaluar el nivel de amenaza de una parcela. Y asigna el nivel de amenaza a la zona.
        /// </summary>
        /// <param name="land"></param>
        public static bool ComputeLandThreat(Land land)
        {
            bool assignable = false;
            var zone = _forestlaw.GetConainerZone(land.Latitude, land.Longitude);
            if (zone != null)
            {
                if (zone.Name == "Zona Roja")
                {
                    land.LandThreat = LandThreat.High;
                    assignable = true;

                }
                else if (zone.Name == "Zona Amarilla")
                {
                    land.LandThreat = LandThreat.Intermediate;
                    assignable = true;
                }
                else
                {
                    land.LandThreat = LandThreat.Zero;
                }
            }
                return assignable;
        }

        /// <summary>
        /// Realiza los calculos de interseccion de la land contra las fincas existentes, obteniendo y asignando el basecampId. 
        /// </summary>
        /// <param name="land"></param>
        public static void ComputeBasecampIntersection(Land land) 
        {
            int bcId = 0;
            var zone = _basecamps.GetConainerZone(land.Latitude, land.Longitude);
            if (zone != null)
            {
                int.TryParse(zone.Param1, out bcId);
                if(bcId != 0)
                {
                    land.BasecampId = bcId;
                }
            }
        }

        public class ComputableZone
        {
            private Models.KmlModels.Zone _zone;
            private List<PolygonMath> _polygonsMath;

            public ComputableZone(Models.KmlModels.Zone zone)
            {
                _zone = zone;
                _polygonsMath = new List<PolygonMath>();

                foreach (var p in _zone.Polygons)
                {
                    var pm = new PolygonMath();
                    foreach (var l in p.Locations)
                    {
                        if (l.Latitude.HasValue && l.Longitude.HasValue)
                            pm.Points.Add(new Vector(l.Latitude.Value, l.Longitude.Value));
                    }
                    _polygonsMath.Add(pm);
                }
            }

            public bool Contains(double lat, double lon)
            {
                var contains = false;

                var landCenter = new PolygonMath();
                landCenter.Points.Add(new Vector(lat, lon));

                var vel = new Vector(0, 0);
                foreach (var p in _polygonsMath)
                {
                    if (p.PolygonCollision(landCenter, vel).Intersect)
                    {
                        contains = true;
                        break;
                    }
                }

                return contains;
            }

            public Models.KmlModels.Zone GetZone()
            {
                return _zone;
            }
        }

        public class ComputableLayer : List<ComputableZone>
        {
            public ComputableLayer(List<ComputableZone> initialdata)
            {
                this.AddRange(initialdata);
            }

            public Models.KmlModels.Zone GetConainerZone(double lat, double lon)
            {
                Models.KmlModels.Zone zone = null;
                foreach (var f in this)
                {
                    if (f.Contains(lat, lon))
                    {
                        zone = f.GetZone();
                        break;
                    }
                }
                return zone;
            }
        }


        //public class PointD
        //{
        //    public PointD(double x, double y)
        //    {
        //        this.X = x;
        //        this.Y = y;
        //    }

        //    public double X { get; set; }
        //    public double Y { get; set; }

        //}

    }
}