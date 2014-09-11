using System;

namespace Earthwatchers.Models
{
	public class Zone 
    {
		public double lat;
		public double lon;
        public double x;
        public double y;
		public string code;
		public int level;

        /// <summary>
        /// Represents the hexagon including a size, center position, tag and polygon coordinates
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="code">zone tag</param>
		public Zone(double lat, double lon, double x, double y, String code) 
        {
			this.lat = lat;
			this.lon = lon;
			this.x = x;
			this.y = y;
			this.code = code;
			this.level = getLevel();
		}

		private int getLevel() 
        {
			return this.code.Length - 2;
		}

		public double getHexSize() 
        {
			return Calculations.CalcHexSize(this.getLevel() + 2);
		}

		public Location[] getHexCoords() 
        {
			double h_lat = this.lat;
			double h_lon = this.lon;
			XY h_xy = Calculations.Loc2Xy(h_lon, h_lat);
			double h_x = h_xy.x;
			double h_y = h_xy.y;
			double h_deg = Math.Tan(Math.PI * (60.0 / 180.0));
			double h_size = this.getHexSize();
            double h_top = Calculations.XyToLoc(h_x, h_y + h_deg * h_size).Latitude;
            double h_btm = Calculations.XyToLoc(h_x, h_y - h_deg * h_size).Latitude;

            double h_l = Calculations.XyToLoc(h_x - 2 * h_size, h_y).Longitude;
            double h_r = Calculations.XyToLoc(h_x + 2 * h_size, h_y).Longitude;
            double h_cl = Calculations.XyToLoc(h_x - 1 * h_size, h_y).Longitude;
            double h_cr = Calculations.XyToLoc(h_x + 1 * h_size, h_y).Longitude;
			
            return new Location[]
            {
                new Location(h_l, h_lat),
				new Location(h_cl, h_top),
				new Location(h_cr, h_top),
				new Location(h_r, h_lat),
				new Location(h_cr, h_btm),
				new Location(h_cl, h_btm)
			};
		}
	}
}
