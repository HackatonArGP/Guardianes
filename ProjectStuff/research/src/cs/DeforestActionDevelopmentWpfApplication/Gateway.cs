// For Visualization only
// Adapted from Wim Ligtendag's Blog
// To be replaced by either Paul's MapSui or a Web Mapping System


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows;

using Microsoft.SqlServer.Types;

using Stormy;


namespace DeforestAction
{
    public class Gateway
    {
        public static IEnumerable<Geometry> GetTransformedGeometries(IEnumerable<SqlGeometry> collection, Transform transform)
        {
            foreach (var item in collection)
            {
                Geometry wpf = item.ToWpfGeometry();
                wpf.Transform = transform;
                 
                yield return wpf;
            }
        }

        public static IEnumerable<Geometry> GetTransformedGeometries(Connection connection, string query, Transform transform)
        {
            var collection = connection.Select<Geom>(query);
            foreach (var item in collection)
            {
                Geometry wpf = item.Geometry.ToWpfGeometry();
                wpf.Transform = transform;
                yield return wpf;
            }
        }

        public static Transform GetTransform(Connection connection, double ScaleValue, Rect viewPortExtent, string query)
        {
            return CalculateResizeTransform(ScaleValue, GetExtent(connection, query, "geom"), viewPortExtent);
        }


        private static Rect GetExtent(Connection connection, string query, string geometryFieldName)
        {
            var coll = connection.Select<Extent>(String.Format(
                @"with viewy as (select {0}.STEnvelope() as envelope from ({1}) t),
                      corny as (select envelope.STPointN(1) as min_corner, envelope.STPointN(3) as max_corner from viewy),
                      boxy as (select min_corner.STX as minx , min_corner.STY as miny, max_corner.STX as maxx, max_corner.STY as maxy from corny)
                    select min(minx) as min_x,min(miny) as min_y,max(maxx) as max_x,max(maxy) as max_y from boxy"
                , geometryFieldName, query));

            foreach(var ext in coll)
            {
                return new Rect(ext.MinX, ext.MinY, ext.MaxX - ext.MinX, ext.MaxY - ext.MinY);
            }
            return Rect.Empty;
        }

        private static TransformGroup CalculateResizeTransform(double ScaleValue, Rect source, Rect target)
        {
            double originWidth = Math.Abs(source.Right - source.Left);
            double originHeight = Math.Abs(source.Top - source.Bottom);

            double scaleFactor = CalculateScaleFactor(ScaleValue, originWidth, originHeight, target.Width, target.Height);

            ScaleTransform scaleTransform = new ScaleTransform(scaleFactor, -scaleFactor);

            TranslateTransform translateTransform = new TranslateTransform
            {
                X = (target.Width - (source.Left + source.Right) * scaleFactor) / 2,
                Y = (target.Height + (source.Bottom + source.Top) * scaleFactor) / 2
            };

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            return transformGroup;
        }

        private static double CalculateScaleFactor(double ScaleValue, double originWidth, double originHeight, double destinationWidth, double destinationHeight)
        {
            double originAspectRatio = originWidth / originHeight;
            double destinationAspectRatio = destinationWidth / destinationHeight;
            double scaleFactor = originAspectRatio < destinationAspectRatio
                ? destinationHeight / originHeight
                : destinationWidth / originWidth;
            return scaleFactor * ScaleValue;
        }
    }
}
