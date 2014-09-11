using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Geometries;
using Earthwatchers.Models;
using Mapsui.Styles;
using Earthwatchers.UI.Requests;
using System.Windows;

namespace Earthwatchers.UI.Layers
{
    public class BasecampLayer : Layer
    {
        
        public delegate void LoadedEventHandler(object sender, EventArgs e);
        public event LoadedEventHandler Loaded;

        public delegate void LoadingEventHandler(object sender, EventArgs e);
        public event LoadingEventHandler Loading;

        private readonly MemoryProvider _source;
        LayerRequests layerRequest;

        public BasecampLayer(string name)
            : base(name)
        {
            _source = new MemoryProvider();
            DataSource = _source;

            layerRequest = new LayerRequests(Constants.BaseApiUrl);
            layerRequest.LayerRecived += layerRequest_LayerRecived;
        }

        public bool isFirstTime = true;
        public Earthwatchers.Models.KmlModels.Layer layersTest = null;
        public void LoadData()
        {
            //Loading(this, EventArgs.Empty);

            var layerName = Earthwatchers.Models.KmlModels.Layers.Basecamps.GetStringValue();
            layerRequest.GetLayersByName(layerName);

            Current.Instance.MapControl.OnViewChanged(true);
        }

        void layerRequest_LayerRecived(object sender, EventArgs e)
        {

            Polygon poligono;
            Earthwatchers.Models.KmlModels.Layer layer = sender as Earthwatchers.Models.KmlModels.Layer;

            if (layer != null)
            {
                isFirstTime = false;
                layersTest = layer;     
                foreach (Earthwatchers.Models.KmlModels.Zone zon in layer.Zones)
                {
                    foreach (var p in zon.Polygons)
                    {
                        poligono = new Polygon { ExteriorRing = new LinearRing(ConvertHexCoordinates(p.Locations.Select(l => new Location(l.Longitude.Value, l.Latitude.Value)).ToList())) };
                        var feature = new Feature { Geometry = poligono };
                        var vStyle = new VectorStyle
                        {
                            Fill = new Brush { Color = Color.FromArgb(48, 255, 255, 255) },
                            Outline = new Pen { Color = Color.FromArgb(100, 241, 251, 187), Width = 2 }, //#F1FBBB
                        };
                        feature.Styles.Add(vStyle);
                        _source.Features.Add(feature);
                    }
                }
                Current.Instance.MapControl.OnViewChanged(true);
            }
            Current.Instance.MapControl.OnViewChanged(true);
        }

        //TEST CALCULAR SI PERTENECE O NO A UN BASECAMP

         public Feature GetFeatureByHex(string hexcode)
         {
             if (_source == null)
                 return null;

             foreach (Feature feature in _source.Features)
             {
                 if (feature["bccode"] == null)
                     continue;

                 if (feature["bccode"].Equals(hexcode))
                     return feature;
             }

             return null;
         }

         public void AddBasecamp(Land land)
         {
             var zone = GeoHex.Decode(land.GeohexKey);

             var isOwn = Current.Instance.Lands.Any(x => x.GeohexKey == land.GeohexKey);
             var sphericalCoordinates = ConvertHexCoordinates(zone.getHexCoords());
             var polygon = new Polygon { ExteriorRing = new LinearRing(sphericalCoordinates) };
             var feature = new Feature { Geometry = polygon };
             feature["bccode"] = zone.code;

             _source.Features.Add(feature);
         }

        //FIN


        private static List<Mapsui.Geometries.Point> ConvertHexCoordinates(IList<Location> locations)
        {
            if (locations == null || locations.Count == 0)
                return null;

            var newLocations = locations.Select(location => SphericalMercator.FromLonLat(location.Longitude, location.Latitude)).Select(spherical => new Mapsui.Geometries.Point(spherical.x, spherical.y)).ToList();

            var sphericalClosing = SphericalMercator.FromLonLat(locations[0].Longitude, locations[0].Latitude);
            newLocations.Add(new Mapsui.Geometries.Point(sphericalClosing.x, sphericalClosing.y));

            return newLocations;
        }
    }
}
