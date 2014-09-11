﻿using System.IO;
using System.Linq;

namespace Mapsui.Styles
{
    public class Bitmap
    {
        private MemoryStream data;

        public Stream Data
        {
            get { return data; }
            set
            {
                if (value == null)
                {
                    data = null;
                    return;
                }
                data = CopyStreamToMemoryStream(value);
                //value.Close(); // not possible in PCL, not sure what effect this will have
            }
        }

        private static MemoryStream CopyStreamToMemoryStream(Stream input)
        {
            var output = new MemoryStream();
            input.Position = 0;
            var buffer = new byte[4096];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            output.Position = 0;
            return output;
        }

        #region Equals operator

        public override bool Equals(object obj)
        {
            if (!(obj is Bitmap))
            {
                return false;
            }
            return Equals((Bitmap)obj);
        }

        public bool Equals(Bitmap bitmap)
        {
            if (!CompareMemoryStreams(data, bitmap.data)) return false;
            return true;
        }

        private static bool CompareMemoryStreams(MemoryStream ms1, MemoryStream ms2)
        {
            if (ms1.Length != ms2.Length)
                return false;
            ms1.Position = 0;
            ms2.Position = 0;

            var msArray1 = ms1.ToArray();
            var msArray2 = ms2.ToArray();

            return msArray1.SequenceEqual(msArray2);
        }

        public override int GetHashCode()
        {
            // Since Data.GetHashCode reads the full stream it is more efficient
            // to return the stream length. 
            return Data.CanSeek ? (int) Data.Length : 0;
        }

        public static bool operator ==(Bitmap bitmap1, Bitmap bitmap2)
        {
            return Equals(bitmap1, bitmap2);
        }

        public static bool operator !=(Bitmap bitmap1, Bitmap bitmap2)
        {
            return !Equals(bitmap1, bitmap2);
        }

        #endregion

    }
}
