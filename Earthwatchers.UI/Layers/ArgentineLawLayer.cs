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

namespace Earthwatchers.UI.Layers
{
    public class ArgentineLawLayer : Layer
    {
        public delegate void LoadedEventHandler(object sender, EventArgs e);
        public event LoadedEventHandler Loaded;

        public delegate void LoadingEventHandler(object sender, EventArgs e);
        public event LoadingEventHandler Loading;

        private readonly MemoryProvider _source;
        LayerRequests layerRequest;

        public ArgentineLawLayer(string name)
            : base(name)
        {
            _source = new MemoryProvider();
            DataSource = _source;

            //ArgentineLawLayer.HardcodedLocations(_source);

            layerRequest = new LayerRequests(Constants.BaseApiUrl);
            layerRequest.LayerRecived += layerRequest_LayerRecived;

            //DrawImage();
        }

        public bool isFirstTime = true;
        public void LoadData()
        {
            Loading(this, EventArgs.Empty);

            var layerName = Earthwatchers.Models.KmlModels.Layers.LeyBosquesSalta.GetStringValue();
            layerRequest.GetLayersByName(layerName);

            Current.Instance.MapControl.OnViewChanged(true);
        }

        void layerRequest_LayerRecived(object sender, EventArgs e)
        {
            isFirstTime = false;

            Polygon poligono;
            Earthwatchers.Models.KmlModels.Layer layer = sender as Earthwatchers.Models.KmlModels.Layer;
            if (layer != null)
            {
                foreach (Earthwatchers.Models.KmlModels.Zone zon in layer.Zones)
                {
                    if (zon.Name == "Zona Amarilla")
                    {
                        #region YellowArea

                        foreach (var p in zon.Polygons)
                        {
                            poligono = new Polygon { ExteriorRing = new LinearRing(ConvertHexCoordinates(p.Locations.Select(l => new Location(l.Longitude.Value, l.Latitude.Value)).ToList())) };
                            var feature = new Feature { Geometry = poligono };
                            var vStyle = new VectorStyle
                            {
                                Fill = new Brush { Color = Color.FromArgb(58, 254, 238, 0) },
                                Outline = new Pen { Color = Color.FromArgb(0, 255, 255, 0), Width = 2 },
                            };
                            feature.Styles.Add(vStyle);
                            _source.Features.Add(feature);
                        }
                        #endregion
                    }
                    else if (zon.Name == "Zona Roja")
                    {
                        #region RedArea
                        foreach (var p in zon.Polygons)
                        {
                            poligono = new Polygon { ExteriorRing = new LinearRing(ConvertHexCoordinates(p.Locations.Select(l => new Location(l.Longitude.Value, l.Latitude.Value)).ToList())) };
                            var feature = new Feature { Geometry = poligono };
                            var vStyle = new VectorStyle
                            {
                                Fill = new Brush { Color = Color.FromArgb(58, 255, 0, 0) },
                                Outline = new Pen { Color = Color.FromArgb(0, 255, 0, 0), Width = 2 },
                            };
                            feature.Styles.Add(vStyle);
                            _source.Features.Add(feature);
                        }
                        #endregion
                    }
                    else if (zon.Name == "Zona Verde")
                    {
                        #region GreenArea
                        foreach (var p in zon.Polygons)
                        {
                            poligono = new Polygon { ExteriorRing = new LinearRing(ConvertHexCoordinates(p.Locations.Select(l => new Location(l.Longitude.Value, l.Latitude.Value)).ToList())) };
                            var feature = new Feature { Geometry = poligono };
                            var vStyle = new VectorStyle
                            {
                                Fill = new Brush { Color = Color.FromArgb(110, 0, 144, 69) },
                                Outline = new Pen { Color = Color.FromArgb(0, 3, 255, 3), Width = 2 },
                            };
                            feature.Styles.Add(vStyle);
                            _source.Features.Add(feature);
                        }
                        #endregion
                    }

                }
                Current.Instance.MapControl.OnViewChanged(true);
            }

            Loaded(this, EventArgs.Empty);
        }

        //private void DrawImage()
        //{
        //    //var img = System.Drawing
        //    ////var raster = new Raster();

        //    var feature = new Feature { Geometry = null };
        //    var vStyle = new VectorStyle
        //    {
        //        Fill = new Brush { Color = Color.FromArgb(50, 255, 255, 0) }
        //    };
        //    feature.Styles.Add(vStyle);
        //    _source.Features.Add(feature);

        //    Current.Instance.MapControl.OnViewChanged(true);
        //}

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
