// Copyright (c) 2011 Barend Gehrels, Amsterdam, the Netherlands

// License? Below is MIT

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Types;
using System.Data.SqlClient;
using Stormy;
using DeforestActionDonations.Models;

namespace DeforestActionDonations
{
    public class ExtentMapper : ISelectable<Extent>
    {
        public Extent ApplySelect(SqlDataReader reader, Connection c, IEnumerable<Extent> dummy)
        {
            Extent extent = new Extent();
            extent.MinX = (double)reader["min_x"];
            extent.MinY = (double)reader["min_y"];
            extent.MaxX = (double)reader["max_x"];
            extent.MaxY = (double)reader["max_y"];
            return extent;
        }
    }

    public class GeomMapper : ISelectable<Geom>
    {
        public Geom ApplySelect(SqlDataReader reader, Connection c, IEnumerable<Geom> dummy)
        {
            Geom geom = new Geom();
            geom.Id = (int)reader["gid"];
            geom.Geometry = (SqlGeometry)reader["geom"];
            return geom;
        }
    }
}
