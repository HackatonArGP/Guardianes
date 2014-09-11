using Earthwatchers.Models.KmlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using Earthwatchers.Data;

namespace Earthwatchers.Services.admin
{
    public static class KmlParserBasecamp
    {
        private static double? ParseLatitude(string coordinate)
        {
            try
            {
                var lat = Convert.ToDouble((coordinate.Split(',')[0]),System.Globalization.CultureInfo.InvariantCulture);
                return lat;
            }
            catch
            {
                return null;
            }

        }

        private static double? ParseLongitude(string coordinate)
        {
            try
            {
                var lon = Convert.ToDouble((coordinate.Split(',')[1]), System.Globalization.CultureInfo.InvariantCulture);
                return lon;
            }
            catch
            {
                return null;
            }
        }

        private static string ParsePolygon(string[] pol)
        {
            string polygon = string.Empty;
            foreach (var item in pol)
            {
                if (polygon == string.Empty)
                {
                    polygon = "POLYGON ((";
                }
                else
                {
                    polygon += ",";
                }
                polygon += item;
            }
            polygon += "))";

            return polygon;
        }

        public static Layer ReadKmlFiles()
        {

            var path = ConfigurationManager.AppSettings.Get("kml.fincas.path");
            var filename = Directory.GetFiles(path).FirstOrDefault();
            XDocument xDoc = XDocument.Load(filename);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            var doc = xDoc.Descendants(ns + "Document").ToList();
            var layerElement = doc.Elements(ns + "Folder").First();

            Layer layer = new Layer();
            layer.Name = layerElement.Element(ns + "name").Value;
            layer.Zones = layerElement.Elements(ns + "Folder").Select(ze =>
                   new Zone(ze.Element(ns + "name").Value,
                              ze.Descendants(ns + "Placemark").Select(pm =>
                                    new Polygon()
                                    {
                                        Name = pm.Element(ns + "name").Value,
                                        Locations = pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                        .Replace("\n", "")
                                                                        .Replace("\t", "")
                                                                        .Split(' ')
                                                                        .Select(x => new Location(ParseLatitude(x), ParseLongitude(x)))
                                                                        .ToList(),
                                        PolygonGeom = ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                       .Replace("\n", " ")
                                                                       .Replace("\t", " ")
                                                                       .Replace(",0 ", "|")
                                                                       .Replace(",", " ")
                                                                       .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                    }).ToList(),
                                ze.Element(ns + "description").Value,
                             "" //Agrega el basecampId a la zona (TODOS NULL PORQUE ES CARGA MASIVA)
                                )).ToList();
            //Una vez finalizado mover a la carpeta Achieved
            return layer;
        }

        public static List<string> ReadKmlFile(int id)
        {
            var errors = new List<string>();
            var path = ConfigurationManager.AppSettings.Get("kml.fincas.path");
            if (Directory.GetFiles(path).Count() > 1)
            {
                errors.Add("Se encontró mas de un archivo kml a procesar en la carpeta, estos archivos ya fueron eliminados. Intente nuevamente la operación");

                for (var i = 0; i <= Directory.GetFiles(path).Count(); i++ )
                {
                    var filename = Directory.GetFiles(path).FirstOrDefault();
                    var archivePath = ConfigurationManager.AppSettings.Get("kml.fincas.archive.path");
                    var archiveName = new FileInfo(filename).Name.Replace(".kml", string.Format("{0}.{1}.{2}.kml", "archive", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss")));
                    if (!Directory.Exists(archivePath))
                        Directory.CreateDirectory(archivePath);
                    File.Move(filename, Path.Combine(archivePath, archiveName));
                }
            }
            else
            {
                var filename = Directory.GetFiles(path).FirstOrDefault();
                XDocument xDoc = XDocument.Load(filename);
                XNamespace ns = "http://www.opengis.net/kml/2.2";

                var doc = xDoc.Descendants(ns + "Document").ToList();
                Layer layer = new Layer();
                layer.Name = "FincasLayer";
                layer.Zones = doc.Elements(ns + "Folder").Select(ze =>
                       new Zone(ze.Element(ns + "name").Value,
                                  ze.Descendants(ns + "Placemark").Select(pm =>
                                        new Polygon()
                                        {
                                            Name = pm.Element(ns + "name").Value,
                                            Locations = pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                            .Replace("\n", "")
                                                                            .Replace("\t", "")
                                                                            .Split(' ')
                                                                            .Select(x => new Location(ParseLatitude(x), ParseLongitude(x)))
                                                                            .ToList(),
                                            PolygonGeom = ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                           .Replace("\n", " ")
                                                                           .Replace("\t", " ")
                                                                           .Replace(",0 ", "|")
                                                                           .Replace(",", " ")
                                                                           .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                        }).ToList(),
                                    ze.Element(ns + "description").Value,
                                 id.ToString() //Agrega el basecampId a la zona
                                    )).ToList();

                errors = KmlParserBasecamp.ListErrors(layer);

                if (errors.Count == 0)
                {
                    //Save finca
                    LayerRepository la = new LayerRepository(ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
                    la.SaveFincaFull(layer);
                    LandRepository landRepo = new LandRepository(ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
                    landRepo.LoadLandBasecamp();
                }

                //Una vez finalizado mover a la carpeta Achieved
                var archivePath = ConfigurationManager.AppSettings.Get("kml.fincas.archive.path");
                var archiveName = new FileInfo(filename).Name.Replace(".kml", string.Format("{0}.{1}.{2}.kml", "archive", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss")));
                if (!Directory.Exists(archivePath))
                    Directory.CreateDirectory(archivePath);
                File.Move(filename, Path.Combine(archivePath, archiveName));
            }

            return errors;
        }

        public static List<string> ListErrors(Layer layer)
        {
            List<string> errors = new List<string>();
            int polygonsWithoutID = 0;
            foreach (var z in layer.Zones)
            {
                foreach (var p in z.Polygons)
                {

                    if (p.Name == null)
                        polygonsWithoutID++;

                    for (var i = 0; i < p.Locations.Count; i++)
                    {
                        var l = p.Locations[i];
                        l.Index = i;
                        if (!l.Latitude.HasValue)
                        {
                            errors.Add(string.Format("Error Latitud {0} del poligono {1} en finca {2}", l.Index, p.Name, z.Name));
                        }
                        if (!l.Longitude.HasValue)
                        {
                            errors.Add(string.Format("Error Longitud {0} del poligono {1} en finca {2}", l.Index, p.Name, z.Name));
                        }
                    }
                }
            }

            if (polygonsWithoutID > 0)
            {
                errors.Add(string.Format("{0} Polygons without ID", polygonsWithoutID));
            }

            return errors;
        }
    }
}