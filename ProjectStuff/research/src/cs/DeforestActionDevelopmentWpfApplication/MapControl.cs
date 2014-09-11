// For Visualization only
// Adapted from Wim Ligtendag's Blog
// To be replaced by either Paul's MapSui or a Web Mapping System

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;

using Microsoft.SqlServer.Types;
using Stormy;

namespace DeforestAction
{
    public class MapControl : FrameworkElement
    {
        private Pen outlinePen;
        private VisualCollection visualCollection;
        private IList<SqlGeometry> geometries;
        private IList<Brush> brushes;

        public MapControl()
        {
            visualCollection = new VisualCollection(this);
            geometries = new List<SqlGeometry>();
            CreateDrawingMedia();
        }

        public void Add(SqlGeometry g)
        {
            //geometries.Add(SqlGeometry.STGeomFromText(new System.Data.SqlTypes.SqlChars(g), 3857));
            geometries.Add(g);
        }

        public void ShowData(Connection connection, double ScaleValue)
        {
            visualCollection.Clear();
            {
                bool drawFill = true;
                DrawingVisual drawingVisual = new DrawingVisual();
                Rect clipRect = new Rect(new Size(this.ActualWidth, this.ActualHeight));
                drawingVisual.Clip = new RectangleGeometry(clipRect);

                Transform transform = Gateway.GetTransform(connection, ScaleValue, clipRect,
                                    "select gid, geom from htr_adopters");

                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    foreach (Geometry geometry in Gateway.GetTransformedGeometries(connection,
                                    "select gid,geom from " + Constants.LandTable, transform))
                    {
                        geometry.Freeze();
                        drawingContext.DrawGeometry(drawFill ? Brushes.Green : null, outlinePen, geometry);
                    }
                    foreach (Geometry geometry in Gateway.GetTransformedGeometries(connection,
                                    "select gid,geom from htr_adopters", transform))
                    {
                        geometry.Freeze();
                        drawingContext.DrawGeometry(drawFill ? Brushes.Red : null, outlinePen, geometry);
                    }
                    int index = 0;
                    foreach (Geometry geometry in Gateway.GetTransformedGeometries(geometries, transform))
                    {
                        geometry.Freeze();
                        drawingContext.DrawGeometry(brushes[index++], outlinePen, geometry);
                        if (index == brushes.Count)
                        {
                            index = 0;
                        }
                    }
                }
                visualCollection.Add(drawingVisual);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visualCollection.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        private void CreateDrawingMedia()
        {
            outlinePen = new Pen(Brushes.Black, 1.0);
            outlinePen.Freeze();
            brushes = new List<Brush>();
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(126, 96, 6)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(180, 55, 11)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(218, 227, 74)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(254, 134, 11)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(254, 224, 64)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(73, 64, 9)));
            brushes.Add(new System.Windows.Media.SolidColorBrush(Color.FromRgb(82, 52, 14)));
            foreach (var brush in brushes)
            {
                brush.Freeze();
            }
        }
    }
}
