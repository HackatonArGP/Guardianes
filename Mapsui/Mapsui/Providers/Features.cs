// Copyright 2010 - Paul den Dulk (Geodan)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Providers
{
    public class Features : IFeatures
    {
        private readonly List<IFeature> features = new List<IFeature>();
        
        public string PrimaryKey { get; private set; }

        public int Count
        {
            get { return features.Count; }
        }

        public IFeature this[int index]
        {
            get { return features[index]; }
        }

        public Features()
        {
            // Perhaps this constructor should get a dictionary parameter
            // to specify the name and type of the columns
        }

        public Features(string primaryKey)
        {
            PrimaryKey = primaryKey;
        }

        public IFeature New()
        {
            // At this point it is possible to initialize an improved version of
            // Feature with a specifed set of columns.
            return new Feature();
        }

        public void Add(IFeature feature)
        {
            features.Add(feature);
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return features.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return features.GetEnumerator();
        }

        public void Delete(object id)
        {
            if (string.IsNullOrEmpty(PrimaryKey)) throw new Exception("Primary key of Features was not set");
            features.Remove(features.First(f => f[PrimaryKey].Equals(id)));
        }

        public void Clear()
        {
            features.Clear();
        }
    }        
}

