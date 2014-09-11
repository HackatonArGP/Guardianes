using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Geometries;
using Earthwatchers.Models;
using Mapsui.Styles;

namespace Earthwatchers.UI.Layers
{
    public class TestLayer : Layer
    {
        private readonly MemoryProvider _source;

        public TestLayer(string name)
            : base(name)
        {
            _source = new MemoryProvider();
            DataSource = _source;

            List<Location> locations = new List<Location>();
            locations.Add(new Location(-63.307, -23.203));
            locations.Add(new Location(-63.200, -23.203));
            locations.Add(new Location(-63.200, -23.350));
            locations.Add(new Location(-63.307, -23.350));
            locations.Add(new Location(-63.307, -23.203));

            var polygon = new Polygon { ExteriorRing = new LinearRing(ConvertHexCoordinates(locations)) };
            
            var feature = new Feature { Geometry = polygon };

            var color = Color.Blue;

            if (name.Equals("08-may"))
            {
                color = Color.Green;
            }
            else if (name.Equals("03-jun"))
            {
                color = Color.Red;
            }

            var vStyle = new VectorStyle
            {
                Fill = new Brush { Color = color },
                Outline = new Pen { Color = Color.Black, Width = 0 },
            };

            feature.Styles.Add(vStyle);

            _source.Features.Add(feature);

            //Current.Instance.MapControl.OnViewChanged(true);
        }

        //Convert lon lat coordinates into spherical for use on map
        private static List<Point> ConvertHexCoordinates(IList<Location> locations)
        {
            if (locations == null || locations.Count == 0)
                return null;

            var newLocations = locations.Select(location => SphericalMercator.FromLonLat(location.Longitude, location.Latitude)).Select(spherical => new Point(spherical.x, spherical.y)).ToList();

            var sphericalClosing = SphericalMercator.FromLonLat(locations[0].Longitude, locations[0].Latitude);
            newLocations.Add(new Point(sphericalClosing.x, sphericalClosing.y));

            return newLocations;
        }
    }
}
