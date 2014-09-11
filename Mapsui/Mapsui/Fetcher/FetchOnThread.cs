﻿// Copyright 2010 - Paul den Dulk (Geodan)
//
// This file is part of Mapsui.
// Mapsui is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// Mapsui is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Mapsui; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using BruTile;

namespace Mapsui.Fetcher
{
    class FetchOnThread
        //This class is needed because in CF one can only pass arguments to a thread using a class constructor.
        //Once support for CF is dropped (replaced by SL on WinMo?) this class should be removed.
    {
        readonly ITileProvider tileProvider;
        readonly TileInfo tileInfo;
        readonly FetchTileCompletedEventHandler fetchTileCompleted;

        public FetchOnThread(ITileProvider tileProvider, TileInfo tileInfo, FetchTileCompletedEventHandler fetchTileCompleted)
        {
            this.tileProvider = tileProvider;
            this.tileInfo = tileInfo;
            this.fetchTileCompleted = fetchTileCompleted;
        }

        public void FetchTile(object state)
        {
            Exception error = null;
            byte[] image = null;

            try
            {
                if (tileProvider != null) image = tileProvider.GetTile(tileInfo);
            }
            catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
            {
                error = ex;
            }
            fetchTileCompleted(this, new FetchTileCompletedEventArgs(error, false, tileInfo, image));
        }
    }

    public delegate void FetchTileCompletedEventHandler(object sender, FetchTileCompletedEventArgs e);

    public class FetchTileCompletedEventArgs
    {
        public FetchTileCompletedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
        {
            Error = error;
            Cancelled = cancelled;
            TileInfo = tileInfo;
            Image = image;
        }

        public Exception Error;
        public readonly bool Cancelled;
        public readonly TileInfo TileInfo;
        public readonly byte[] Image;
    }
}
