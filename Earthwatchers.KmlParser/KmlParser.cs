using Earthwatchers.Models.KmlModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Earthwatcher.KmlParser
{
    class KmlParser
    {
        private double? ParseLatitude(string coordinate)
        {
            try
            {
                var lat = Convert.ToDouble(coordinate.Split(',')[0]);
                return lat;
            }
            catch
            {
                return null;
            }

        }

        private double? ParseLongitude(string coordinate)
        {
            try
            {
                var lon = Convert.ToDouble(coordinate.Split(',')[1]);
                return lon;
            }
            catch
            {
                return null;
            }
        }

        private string ParsePolygon(string[] pol)
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

        public Layer ReadKmlFile()
        {
            //TODO: Darle acceso a una carpeta con N archivos KML.
            var path = ConfigurationManager.AppSettings.Get("kmlparser.kmlfile");
            XDocument xDoc = XDocument.Load(path);
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
                                        PolygonGeom = this.ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                       .Replace("\n", " ")
                                                                       .Replace("\t", " ")
                                                                       .Replace(",0 ", "|")
                                                                       .Replace(",", " ")
                                                                       .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                    }).ToList()
                                )).ToList();
            return layer;
        }

        public List<string> ListErrors(Layer layer)
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
                            errors.Add(string.Format("Error Latitud {0} del poligono {1} en zona {2}", l.Index, p.Name, z.Name));
                        }
                        if (!l.Longitude.HasValue)
                        {
                            errors.Add(string.Format("Error Longitud {0} del poligono {1} en zona {2}", l.Index, p.Name, z.Name));
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

    class KmlParserV2
    {
        private double? ParseLatitude(string coordinate)
        {
            try
            {
                var lat = Convert.ToDouble(coordinate.Split(',')[0]);
                return lat;
            }
            catch
            {
                return null;
            }

        }

        private double? ParseLongitude(string coordinate)
        {
            try
            {
                var lon = Convert.ToDouble(coordinate.Split(',')[1]);
                return lon;
            }
            catch
            {
                return null;
            }
        }

        private string ParsePolygon(string[] pol)
        {
            try
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
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string GetPolygonName(XElement elem)
        {
             
            return "";
        }

        public Layer ReadKmlFile()
        {
            //TODO: Darle acceso a una carpeta con N archivos KML.
            XDocument xDoc = XDocument.Load(ConfigurationManager.AppSettings.Get("kmlparser.kmlfile"));
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            var doc = xDoc.Descendants(ns + "Document").ToList();
            var layerElement = doc.Elements(ns + "Folder").First();

            Layer layer = new Layer();
            layer.Name = "OTBN";

            //var pm = layerElement.Descendants(ns + "Placemark").First();
            //var name = pm.Element(ns + "name").Value;
            //var id = pm.Attribute("id").Value;

            var redZone = new Zone("Zona Roja", layerElement.Descendants(ns + "Placemark")
                                                            .Where(pm => pm.Element(ns + "name").Value == "proteccion")
                                                            .Select(pm => new Polygon()
                                                            {
                                                                Name = pm.Attribute("id") != null ? pm.Attribute("id").Value : null,
                                                                Locations = pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                                .Replace("\n", "")
                                                                                                .Replace("\t", "")
                                                                                                .Split(' ')
                                                                                                .Select(x => new Location(ParseLatitude(x), ParseLongitude(x)))
                                                                                                .ToList(),
                                                                PolygonGeom = this.ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                               .Replace("\n", " ")
                                                                                               .Replace("\t", " ")
                                                                                               .Replace(",0 ", "|")
                                                                                               .Replace(",", " ")
                                                                                               .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                                            }).ToList());

            var yellowZone = new Zone("Zona Amarilla", layerElement.Descendants(ns + "Placemark")
                                                            .Where(pm => pm.Element(ns + "name").Value == "mcb")
                                                            .Select(pm => new Polygon()
                                                            {
                                                                Name = pm.Attribute("id") != null ? pm.Attribute("id").Value : null,
                                                                Locations = pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                                .Replace("\n", "")
                                                                                                .Replace("\t", "")
                                                                                                .Split(' ')
                                                                                                .Select(x => new Location(ParseLatitude(x), ParseLongitude(x)))
                                                                                                .ToList(),
                                                                PolygonGeom = this.ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                               .Replace("\n", " ")
                                                                                               .Replace("\t", " ")
                                                                                               .Replace(",0 ", "|")
                                                                                               .Replace(",", " ")
                                                                                               .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                                            }).ToList());

            var greenZone = new Zone("Zona Verde", layerElement.Descendants(ns + "Placemark")
                                                            .Where(pm => pm.Element(ns + "name").Value == "potencial productivo")
                                                            .Select(pm => new Polygon()
                                                            {
                                                                Name = pm.Attribute("id") != null ? pm.Attribute("id").Value : null,
                                                                Locations = pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                                .Replace("\n", "")
                                                                                                .Replace("\t", "")
                                                                                                .Split(' ')
                                                                                                .Select(x => new Location(ParseLatitude(x), ParseLongitude(x)))
                                                                                                .ToList(),
                                                                PolygonGeom = this.ParsePolygon(pm.Descendants(ns + "coordinates").First().Value.Trim()
                                                                                               .Replace("\n", " ")
                                                                                               .Replace("\t", " ")
                                                                                               .Replace(",0 ", "|")
                                                                                               .Replace(",", " ")
                                                                                               .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                                                            }).ToList());
            layer.Zones = new List<Zone>();
            layer.Zones.Add(redZone);
            layer.Zones.Add(yellowZone);
            layer.Zones.Add(greenZone);


            return layer;
        }

        public List<string> ListErrors(Layer layer)
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
                            errors.Add(string.Format("Error Latitud {0} del poligono {1} en zona {2}", l.Index, p.Name, z.Name));
                        }
                        if (!l.Longitude.HasValue)
                        {
                            errors.Add(string.Format("Error Longitud {0} del poligono {1} en zona {2}", l.Index, p.Name, z.Name));
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
