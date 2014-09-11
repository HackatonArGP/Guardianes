using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BruTile;
using BruTile.PreDefined;
using BruTile.Web;
using Earthwatchers.Models;
using Mapsui.Geometries;
using Mapsui.Layers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;
using Earthwatchers.Data;
using System.ComponentModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Earthwatchers.Services.Resources
{
    public static class ImagesGeneratorTool
    {
        private static ILandRepository landRepository; //TODO: borrame!
        private static bool includeHexagon;
        private static string _geoHexKey;

        public static void Run(ILandRepository _landRepository, bool _includeHexagon, string geoHexKey)
        {
            landRepository = _landRepository;
            includeHexagon = _includeHexagon;
            _geoHexKey = geoHexKey;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            // Calling the DoWork Method Asynchronously
            worker.RunWorkerAsync(); //we can also pass parameters to the async method....
        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO: Log completeness
        }

        private static CloudStorageAccount storageAccount = null;
        private static CloudBlobClient blobClient = null;
        private static CloudBlobContainer container = null;

        private static ImageCodecInfo jgpEncoder = null;
        private static EncoderParameters myEncoderParameters = null;

        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            string baseurl = landRepository.GetImageBaseUrl(true);
            string newurl = landRepository.GetImageBaseUrl(false);

            //Esto quedo obsoleto si no vamos a usar Azure
            /*
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=guardianes;AccountKey=vlG2nCfujtarq9++4+Qh21vZvD6c9+PUfNqR/9o+yc7AXifypGBVeEYgSRBMRx9AhLGoIcGJkgSqypduaaBnxw==");
            blobClient = storageAccount.CreateCloudBlobClient();
            if (includeHexagon)
            {
                container = blobClient.GetContainerReference("demand");
            }
            else
            {
                container = blobClient.GetContainerReference("minigame");
            }
             * */

            //Encoder
            jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            List<string> hexCodes = new List<string>();
            if (string.IsNullOrEmpty(_geoHexKey))
            {
                hexCodes = landRepository.GetVerifiedLandsGeoHexCodes(0, includeHexagon);
            }
            else
            {
                hexCodes.Add(_geoHexKey);
            }

            foreach (var geohex in hexCodes)
            {
                var zone = GeoHex.Decode(geohex);

                var sphericalCoordinates = ConvertHexCoordinates(zone.getHexCoords());

                var top = sphericalCoordinates.Max(s => s.Y);
                var bottom = sphericalCoordinates.Min(s => s.Y);
                var left = sphericalCoordinates.Min(s => s.X);
                var right = sphericalCoordinates.Max(s => s.X);

                var extent = new BruTile.Extent(left, bottom, right, top);

                var schema = new SphericalMercatorWorldSchema();
                schema.Extent = extent;

                var tiles = schema.GetTilesInView(extent, 13);

                var newTop = tiles.Max(s => s.Extent.MaxY);
                var newBottom = tiles.Min(s => s.Extent.MinY);
                var newLeft = tiles.Min(s => s.Extent.MinX);
                var newRight = tiles.Max(s => s.Extent.MaxX);

                int[] cols = tiles.OrderBy(t => t.Index.Col).ThenByDescending(t => t.Index.Row).Select(t => t.Index.Col).Distinct().ToArray();
                int[] rows = tiles.OrderBy(t => t.Index.Col).ThenByDescending(t => t.Index.Row).Select(t => t.Index.Row).Distinct().ToArray();
                int width = 256 * cols.Length;
                int height = 256 * rows.Length;
                float hexLeft = (float)(((left - newLeft) * width) / (newRight - newLeft));
                float hexTop = (float)(((top - newTop) * height) / (newBottom - newTop));

                CreateCanvas(baseurl, geohex, tiles, width, height, hexLeft, hexTop, cols, rows, true);
                CreateCanvas(newurl, geohex, tiles, width, height, hexLeft, hexTop, cols, rows, false);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static void CreateCanvas(string baseurl, string geohex, IEnumerable<TileInfo> tiles, int width, int height, float hexLeft, float hexTop, int[] cols, int[] rows, bool isBase)
        {
            //create an object that will do the drawing operations
            int x = 0;
            int y = 0;
            int count = 0;
            int col = 0;
            int row = 0;
            string tile = string.Empty;

            using (var canvas = new Bitmap(width, height))
            {
                using (var artist = Graphics.FromImage(canvas))
                {
                    artist.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    foreach (TileInfo tileInfo in tiles.OrderBy(t => t.Index.Col).ThenByDescending(t => t.Index.Row))
                    {
                        if (count > 0)
                        {
                            x = Array.IndexOf(cols, tileInfo.Index.Col) * 256;
                            y = Array.IndexOf(rows, tileInfo.Index.Row) * 256;
                        }

                        tile = string.Format("{0}/{1}/{2}/{3}.png", baseurl, tileInfo.Index.Level, tileInfo.Index.Col, tileInfo.Index.Row);
                        //System.Diagnostics.Debug.WriteLine(tile);

                        var request = WebRequest.Create(tile);
                        using (var response = request.GetResponse())
                        {
                            using (var stream = response.GetResponseStream())
                            {
                                var imageTile = Bitmap.FromStream(stream);
                                artist.DrawImage(imageTile, new PointF(x, y));
                            }
                        }

                        col = tileInfo.Index.Col;
                        row = tileInfo.Index.Row;
                        count++;
                    }

                    //Dibujo el Hexágono
                    if (includeHexagon)
                    {
                        PointF[] points = new PointF[7];
                        points.SetValue(new PointF(hexLeft + 0, hexTop + 32f), 0);
                        points.SetValue(new PointF(hexLeft + 19f, hexTop + 0), 1);
                        points.SetValue(new PointF(hexLeft + 54f, hexTop + 0), 2);
                        points.SetValue(new PointF(hexLeft + 73f, hexTop + 32f), 3);
                        points.SetValue(new PointF(hexLeft + 54f, hexTop + 64f), 4);
                        points.SetValue(new PointF(hexLeft + 19f, hexTop + 64f), 5);
                        points.SetValue(new PointF(hexLeft + 0, hexTop + 32f), 6);

                        SolidBrush fillbrush = new SolidBrush(Color.FromArgb(0, 255, 165, 0));
                        SolidBrush borderbrush = new SolidBrush(Color.FromArgb(255, 0, 128, 0));
                        artist.FillPolygon(fillbrush, points);
                        artist.DrawPolygon(new Pen(borderbrush, 5), points);
                        artist.Save();
                    }
                }
                try
                {
                    //Crop y Márgenes
                    int rectx = 0;
                    if (width > 256)
                    {
                        rectx = (int)hexLeft - 74;
                    }

                    if (rectx < 0)
                        rectx = 0;

                    int recty = 0;
                    if (height > 256)
                    {
                        recty = (int)hexTop - 80;
                    }

                    if (recty < 0)
                        recty = 0;

                    Rectangle cropArea = new Rectangle(rectx, recty, 256, 256);


                    string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SatelliteImages", includeHexagon ? "demand" : "game1", string.Format("{0}{1}.jpg", geohex, isBase ? "-a" : "-d"));
                    using (Bitmap canvasCropped = canvas.Clone(cropArea, canvas.PixelFormat))
                    {
                        //canvasCropped.Save(filePath, ImageFormat.Jpeg);
                        canvasCropped.Save(filePath, jgpEncoder, myEncoderParameters);
                    }

                    //Quedo obsoleto al no tener que usar Azure
                    /*
                    //Azure Blob
                    string filePath = string.Format("{0}{1}.jpg", geohex, isBase ? "-a" : "-d");
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);
                    // Create or overwrite the "myblob" blob with contents from a local file.
                    using (var fileStream = new System.IO.MemoryStream())
                    {
                        using (Bitmap canvasCropped = canvas.Clone(cropArea, canvas.PixelFormat))
                        {
                            //canvasCropped.Save(fileStream, ImageFormat.Jpeg);
                            canvasCropped.Save(fileStream, jgpEncoder, myEncoderParameters);
                        }
                        fileStream.Position = 0;
                        blockBlob.UploadFromStream(fileStream);
                    }
                     * */

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

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