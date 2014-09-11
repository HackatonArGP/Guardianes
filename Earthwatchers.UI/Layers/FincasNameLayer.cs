using System;
using System.Windows;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;
using Earthwatchers.Models;
using Mapsui.Styles;
using System.Globalization;
using Earthwatchers.UI.Requests;

namespace Earthwatchers.UI.Layers
{
    public class FincasNameLayer : Layer
    {
        private readonly MemoryProvider _source;
        private List<Basecamp> _fincas;
        LayerRequests layerRequest;

        public FincasNameLayer(string name)
            : base(name)
        {
            layerRequest = new LayerRequests(Constants.BaseApiUrl);
            layerRequest.GetFincas();
            layerRequest.FincasReceived += layerRequest_FincasReceived;
            _source = new MemoryProvider();
            DataSource = _source;

            Current.Instance.MapControl.zoomStarted += MapControlChanged;
            Current.Instance.MapControl.zoomFinished += MapControlChanged;
        }

        void layerRequest_FincasReceived(object sender, EventArgs e)
        {
            _fincas = sender as List<Basecamp>;
        }

        void MapControlChanged(object sender, EventArgs e)
        {
            UpdateNamesInMap();
        }

        public void UpdateNamesInMap()
        {
            if (Current.Instance.MapControl.Viewport.Resolution >= 2.4)
            {
                DrawNames();
            }
            else
            {
                ClearNames();
            }
        }

        public void ClearNames()
        {
            _source.Features.Clear();
        }

        double lon = -63.4405;
        double lat = -22.1193;

        private void DrawNames()
        {
            ClearNames();
            //Cargo los puntos de cada finca
            if (_fincas != null)
            {
                foreach (var f in _fincas)
                {
                    var sphericalMid = SphericalMercator.FromLonLat(f.Longitude, f.Latitude);
                    var feature = new Feature
                    {
                        Geometry = new Mapsui.Geometries.Point(sphericalMid.x, sphericalMid.y)
                    };

                    var res = Current.Instance.MapControl.Viewport.Resolution;
                    //Cargo las distintas imagenes a niveles de zoom
                    if (res > 2.79 && res <= 19.21)
                    {
                        var symbolStyle = new SymbolStyle { Symbol = GetSymbol("../Images/FincasName/"+f.Id.ToString()+"_1.png"), SymbolType = SymbolType.Rectangle };
                        feature.Styles.Add(symbolStyle);
                        _source.Features.Add(feature);
                    }
                    //else if (res > 19.21 && res <= 38.22)
                    else if (res > 19.21 && res <= 305)
                    {
                        var symbolStyle = new SymbolStyle { Symbol = GetSymbol("../Images/FincasName/" + f.Id.ToString() + "_2.png"), SymbolType = SymbolType.Rectangle };
                        feature.Styles.Add(symbolStyle);
                        _source.Features.Add(feature);
                    }
                    //else if (res > 38.22 && res <= 76.48)
                    //{
                    //    var symbolStyle = new SymbolStyle { Symbol = GetSymbol("../Images/FincasName/" + f.Id.ToString() + "_3.png"), SymbolType = SymbolType.Rectangle };
                    //    feature.Styles.Add(symbolStyle);
                    //    _source.Features.Add(feature);
                    //}
                    //else if (res > 76.48 && res <= 153)
                    //{
                    //    var symbolStyle = new SymbolStyle { Symbol = GetSymbol("../Images/FincasName/" + f.Id.ToString() + "_4.png"), SymbolType = SymbolType.Rectangle };
                    //    feature.Styles.Add(symbolStyle);
                    //    _source.Features.Add(feature);
                    //}
                    //else if (res > 153 && res <= 305)
                    //{
                    //    var symbolStyle = new SymbolStyle { Symbol = GetSymbol("../Images/FincasName/" + f.Id.ToString() + "_5.png"), SymbolType = SymbolType.Rectangle };
                    //    feature.Styles.Add(symbolStyle);
                    //    _source.Features.Add(feature);
                    //}
                }
            }
            Current.Instance.MapControl.OnViewChanged(true);

        }

        private static Bitmap GetSymbol(string bitmapname)
        {
            Bitmap bitmap = null;
            try
            {
                var filename = string.Format(CultureInfo.InvariantCulture, "Earthwatchers.UI;component/Resources/Images/{0}", bitmapname);
                var resourceInfo = Application.GetResourceStream(new Uri(filename, UriKind.Relative));
                bitmap = new Bitmap { Data = resourceInfo.Stream };
            }
            catch
            {
                //TODO: error handling
            }

            return bitmap;
        }


    }
}
