using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EarthWatchers.SL.GUI.Controls;
using EarthWatchers.SL.GUI.Controls.Comments;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;
using Earthwatchers.Models;
using Mapsui.Styles;
using System.Globalization;
using EarthWatchers.SL.Requests;
using Flag = Earthwatchers.Models.Flag;

namespace EarthWatchers.SL.Layers
{
    public class FlagLayer : Layer
    {
        private readonly MemoryProvider _source;
        private readonly FlagRequests   _flagRequest;

        public FlagLayer(string name)
            : base(name)
        {
            _source = new MemoryProvider();
            DataSource = _source;
            
            _flagRequest = new FlagRequests(Constants.BaseApiUrl);
            _flagRequest.FlagsReceived += FlagsReceived;
            Current.Instance.MapControl.MouseInfoDown += MapControlMouseInfoDown;
            Current.Instance.MapControl.MouseInfoDownLayers.Add(this);

            RequestFlags();
        }


        private static void MapControlMouseInfoDown(object sender, Mapsui.Windows.MouseInfoEventArgs e)
        {
            if (!e.LayerName.Equals(Constants.flagLayerName) || e.Feature == null) return;
            //if (e.Feature == null) return;

            var username = e.Feature["username"].ToString();
            var flagid = e.Feature["flagid"].ToString();
            var comment = e.Feature["comment"].ToString();
            var longitude = e.Feature["longitude"];
            var latitude = e.Feature["latitude"];
            var fiw = new FlagInfoWindow(username, flagid, comment, (double)longitude, (double)latitude);

            fiw.Show();
        }

        public void DeleteFlag(string flagId)
        {
            if (Current.Instance.Username == null || Current.Instance.Password == null)
                return;

            var fr = new FlagRequests(Constants.BaseApiUrl);
            fr.FlagRemoved += FlagRemoved;
            fr.Delete(flagId, Current.Instance.Username, Current.Instance.Password);
        }

        private void FlagRemoved(object sender, EventArgs e)
        {
            RequestFlags();
        }

        public void RequestFlags()
        {
            _flagRequest.GetFlags();
        }

        private void FlagsReceived(object sender, EventArgs e)
        {
            var flag = sender as List<Flag>;
            if(flag != null)
                DrawFlags(flag);
        }

        private void DrawFlags(IEnumerable<Flag> flags)
        {
            if (flags == null) return;

            ClearGraphics();

            foreach (var flag in flags)
            {
                var bmpName = "flag_shadow.png";
                
                if(Current.Instance.Earthwatcher != null)
                    if(Current.Instance.Earthwatcher.Name.Equals(flag.UserName))
                        bmpName = "flag_shadow_red.png";

                var sphericalMid = SphericalMercator.FromLonLat(flag.Longitude, flag.Latitude);
                var feature = new Feature
                {
                    Geometry = new Mapsui.Geometries.Point(sphericalMid.x, sphericalMid.y)
                };
                var symbolStyle = new SymbolStyle { Symbol = GetSymbol(bmpName) ,SymbolType = SymbolType.Rectangle, SymbolOffset = new Offset{X = 9f, Y = 0f}};

                feature.Styles.Add(symbolStyle);
                feature["longitude"] = flag.Longitude;
                feature["latitude"] = flag.Latitude;
                feature["comment"] = flag.Comment;
                feature["flagid"] = flag.Id;
                feature["username"] = flag.UserName;
                _source.Features.Add(feature);
            }

            Current.Instance.MapControl.OnViewChanged(true);
        }

        private static Bitmap GetSymbol(string bmpName)
        {
            Bitmap bitmap = null;

            try
            {
                var filename = string.Format(CultureInfo.InvariantCulture, "EarthWatchers.SL;component/Resources/Images/{0}", bmpName);
                var resourceInfo = Application.GetResourceStream(new Uri(filename, UriKind.Relative));
                bitmap = new Bitmap { Data = resourceInfo.Stream };                
            }
            catch
            {
                //TODO: error handling
            }

            return bitmap;
        }

        private void ClearGraphics()
        {
            _source.Features.Clear();
        }


    }
}
