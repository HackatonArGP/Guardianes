using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Earthwatchers.Models;
using Point = Mapsui.Geometries.Point;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class NewsControl
    {
        private readonly News news;

        public NewsControl(News news)
        {
            InitializeComponent();
            this.news = news;
            Init();
        }

        private void Init()
        {
           // if(_news.Wkt == null)
           //     btnZoomTo.Visibility = Visibility.Collapsed;

            txtDate.Text = news.Published.ToShortDateString();
            var words = news.NewsItem.Split(new[] { ' ' });
            var linkIndexes = new List<int>();

            //find the elements in the array that match what we are looking for
            //and add them to our List<>
            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("http://") || words[i].Contains("https://") || words[i].Contains("@"))
                {
                    linkIndexes.Add(i);
                }
            }

            //if i (the index) is not in the List then it is a normal string
            //otherwise it is a hyperlink
            for (var i = 0; i < words.Length; i++)
            {
                if (!linkIndexes.Contains(i))
                {
                    var txt = new TextBlock {Text = words[i] + " "};

                    textWrap.Children.Add(txt);
                }
                else
                {
                    var lnk = new HyperlinkButton
                    {
                        NavigateUri = new Uri(words[i]),
                        TargetName = "_blank",
                        Content = words[i]
                    };

                    textWrap.Children.Add(lnk);
                }
            }
        }

        private void BtnZoomToClick(object sender, RoutedEventArgs e)
        {
            var polyCoords = news.Wkt.Replace("POLYGON ((", "").Replace("))", "");
            var splitCoords = polyCoords.Split(',');

            var firstCoord = splitCoords[0].TrimStart().Split(' ');
            var secondCoord = splitCoords[2].TrimStart().Split(' ');
            var leftSpherical = SphericalMercator.FromLonLat(Double.Parse(firstCoord[0], CultureInfo.InvariantCulture), Double.Parse(firstCoord[1], CultureInfo.InvariantCulture));
            var rightSpherical = SphericalMercator.FromLonLat(Double.Parse(secondCoord[0], CultureInfo.InvariantCulture), Double.Parse(secondCoord[1], CultureInfo.InvariantCulture));

            Current.Instance.MapControl.ZoomToBox(new Point(leftSpherical.x, leftSpherical.y), new Point(rightSpherical.x, rightSpherical.y));
        }
    }
}
