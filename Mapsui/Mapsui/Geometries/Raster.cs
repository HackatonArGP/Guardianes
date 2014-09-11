﻿using System;
using System.IO;

namespace Mapsui.Geometries
{
    public class Raster : IRaster
    {
        readonly BoundingBox boundingBox;
        public MemoryStream Data { get; private set; }
        public long TickFetched { get; private set; }

        public Raster(MemoryStream data, BoundingBox box)
        {
            Data = data;
            boundingBox = box;
            TickFetched = DateTime.Now.Ticks;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        #region IGeometry Members

        public int Dimension
        {
            get { return 2; }
        }

        public int SRID
        {
            get;
            set;
        }

        public Geometry Envelope()
        {
            throw new NotImplementedException();
        }

        public string AsText()
        {
            throw new NotImplementedException();
        }

        public byte[] AsBinary()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public Geometry Boundary()
        {
            throw new NotImplementedException();
        }

        public bool Relate(Geometry other, string intersectionPattern)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Disjoint(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Touches(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Crosses(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Within(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public double Distance(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public Geometry Intersection(Geometry geom)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
