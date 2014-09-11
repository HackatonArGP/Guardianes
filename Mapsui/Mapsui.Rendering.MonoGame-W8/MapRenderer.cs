﻿using Mapsui.Geometries;
using Mapsui.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BoundingBox = Mapsui.Geometries.BoundingBox;
using Color = Microsoft.Xna.Framework.Color;

namespace Mapsui.Rendering.MonoGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MapRenderer
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private readonly Game game;

        public MapRenderer(Game game)
        {
            this.game = game;
            graphics = new GraphicsDeviceManager(game);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void Draw(Map map, IViewport viewport, GameTime gameTime)
        {
            if (spriteBatch == null) spriteBatch = new SpriteBatch(game.GraphicsDevice);

            game.GraphicsDevice.Clear(Color.LightGray);
            spriteBatch.Begin();
            
            foreach (var layer in map.Layers)
            {
                foreach (var feature in layer.GetFeaturesInView(viewport.Extent, viewport.Resolution))
                {
                    if (feature.Geometry is IRaster)
                    {
                        var raster = (feature.Geometry as IRaster);
                        var destination = ToXna(RoundToPixel(WorldToScreen(viewport, raster.GetBoundingBox())));
                        var source = new Rectangle(0, 0, 256, 256);

                        raster.Data.Position = 0;
                        if (!feature.RenderedGeometry.Keys.Contains(new VectorStyle()))
                        {
                            feature.RenderedGeometry[new VectorStyle()] = Texture2D.FromStream(game.GraphicsDevice, raster.Data);
                        }
                        spriteBatch.Draw(feature.RenderedGeometry[new VectorStyle()] as Texture2D, destination, source, Color.White);
                    }
                }
            }
            spriteBatch.End();
        }

        private static BoundingBox WorldToScreen(IViewport viewport, BoundingBox boundingBox)
        {
            var box = new BoundingBox
                {
                    Min = viewport.WorldToScreen(boundingBox.Min),
                    Max = viewport.WorldToScreen(boundingBox.Max)
                };
            return box;
        }

        private static Rectangle ToXna(BoundingBox boundingBox)
        {
            return new Rectangle
            {
                X = (int)boundingBox.Left,
                Y = (int)boundingBox.Bottom,
                Width = (int)boundingBox.Width,
                Height = (int)boundingBox.Height
            };
        }

        public static BoundingBox RoundToPixel(BoundingBox dest)
        {
            // To get seamless aligning you need to round the 
            // corner coordinates to pixel. The new width and
            // height will be a result of that.
            return  new BoundingBox(
                Math.Round(dest.Left),
                Math.Round(dest.Top),
                Math.Round(dest.Right),
                Math.Round(dest.Bottom));
        }


    }
}
