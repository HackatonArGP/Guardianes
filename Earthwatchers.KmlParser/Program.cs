using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Earthwatchers.Models.KmlModels;
using Earthwatchers.Data;
using System.Globalization;
using System.Threading;
using System.Configuration;

namespace Earthwatcher.KmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;

            //var parser = new KmlParserV2();
            var parser = new KmlParser();
            var layer = parser.ReadKmlFile();
            List<string> errors = parser.ListErrors(layer);

            if (!errors.Any())
            {
                Console.Write("\n Archivo leido correctamente, no contiene erorres" + "\n");
                Console.WriteLine(" Importando archivos a la base de datos...");

                LayerRepository la = new LayerRepository(ConfigurationManager.ConnectionStrings["Earthwatchers_DSN"].ConnectionString);
                la.SaveLayerFull(layer);

                Console.WriteLine(" Archivos importados correctamente");
                Console.WriteLine(" Presione Enter para salir");

                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\n Archivo leido correctamente, contiene erorres en los siguientes campos: " + "\n");

                foreach (string er in errors)
                {
                    Console.WriteLine(" " + er.ToString());
                }
                Console.WriteLine("\n Solucione los problemas pendientes y vuelva a cargar el archivo");
                Console.ReadLine();
            }
        }
    }
}