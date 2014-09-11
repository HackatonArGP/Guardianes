using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

/*
 * GeoHex by @sa2da (http://geogames.net) is licensed under Creative Commons BY-SA 2.1 Japan License.
 * GeoHex V3 for c# ported to C# by @time
 */

namespace GeoHex
{
    public class GeoHex
    {
        public static String VERSION = "3.00";
        private static Regex INC15 = new Regex("[15]");
        private static Regex EXC125 = new Regex("[^125]");

        /// <summary>
        /// Encode a zone code:
        /// Creates a zone code based on longitude, latitude and zoomlevel
        /// </summary>
        /// <param name="lon">longitude</param>
        /// <param name="lat">latitude</param>
        /// <param name="level">zoomlevel</param>
        /// <returns>Zone code, returns null if input coordinates are out of range or level is lower than 0 or higher than 15</returns>
        public static String Encode(double lon, double lat, int level)
        {
            Zone zone = GetZoneByLocation(lon, lat, level);

            if (zone == null)
                return null;
            else
                return zone.code;
        }

        /// <summary>
        /// Decode a zone code:
        /// Creates a zone based on the zone code
        /// </summary>
        /// <param name="code">zone code</param>
        /// <returns>Zone</returns>
        public static Zone Decode(String code)
        {
            return GetZoneByCode(code);
        }
        
	    private static Zone GetZoneByLocation(double lon, double lat, int level) 
        {
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180 || level < 0 || level > 15)
                return null;

		    level += 2;
		    double h_size = Calculations.CalcHexSize(level);

		    XY z_xy = Calculations.Loc2Xy(lon, lat);
		    double lon_grid = z_xy.x;
		    double lat_grid = z_xy.y;
            double unit_x = 6 * h_size;
            double unit_y = 6 * h_size * Constants.h_k;
            double h_pos_x = (lon_grid + lat_grid / Constants.h_k) / unit_x;
            double h_pos_y = (lat_grid - Constants.h_k * lon_grid) / unit_y;
		    double h_x_0 = Math.Floor(h_pos_x);
		    double h_y_0 = Math.Floor(h_pos_y);
		    double h_x_q = h_pos_x - h_x_0;
		    double h_y_q = h_pos_y - h_y_0;
            
		    double h_x = Math.Round(h_pos_x);
            double h_y = Math.Round(h_pos_y);

		    if (h_y_q > -h_x_q + 1) 
            {
			    if((h_y_q < 2 * h_x_q) && (h_y_q > 0.5 * h_x_q))
                {
				    h_x = h_x_0 + 1;
				    h_y = h_y_0 + 1;
			    }
		    } 
            else if (h_y_q < -h_x_q + 1) 
            {
			    if ((h_y_q > (2 * h_x_q) - 1) && (h_y_q < (0.5 * h_x_q) + 0.5))
                {
				    h_x = h_x_0;
				    h_y = h_y_0;
			    }
		    }

            double h_lat = (Constants.h_k * h_x * unit_x + h_y * unit_y) / 2;
            double h_lon = (h_lat - h_y * unit_y) / Constants.h_k;

		    Loc z_loc = Calculations.XyToLoc(h_lon, h_lat);
		    double z_loc_x = z_loc.lon;
		    double z_loc_y = z_loc.lat;

            if (Constants.h_base - h_lon < h_size)
            {
			    z_loc_x = 180;
			    double h_xy = h_x;
			    h_x = h_y;
			    h_y = h_xy;
		    }

            StringBuilder h_code = new StringBuilder();
            List<Int32> code3_x = new List<Int32>();
            List<Int32> code3_y = new List<Int32>();
            StringBuilder code3 = new StringBuilder();
            StringBuilder code9 = new StringBuilder();
		    double mod_x = h_x;
		    double mod_y = h_y;

		    for(int i = 0;i <= level ; i++)
            {
		        double h_pow = Math.Pow(3,level-i);

		        if(mod_x >= Math.Ceiling(h_pow / 2))
                {
			        code3_x.Add(2);
		            mod_x -= h_pow;
		        }
                else if(mod_x <= -Math.Ceiling(h_pow/2))
                {
		            code3_x.Add(0);
                    mod_x += h_pow;
		        }
                else
                {
		            code3_x.Add(1);
		        }

		        if(mod_y >= Math.Ceiling(h_pow / 2))
                {
		            code3_y.Add(2);
                    mod_y -= h_pow;
		        }
                else if(mod_y <= -Math.Ceiling(h_pow / 2))
                {
		            code3_y.Add(0);
                    mod_y += h_pow;
		        }
                else
                {
		            code3_y.Add(1);
		        }
		    }

		    for(int i = 0; i < code3_x.Count; i++)
            {                                   
		        code3.Append("" + code3_x[i] + code3_y[i]);
                //Convert to terneral (Base 3) not standard supported in .Net
                code9.Append(BaseConverter.BaseStringToValue(code3.ToString(), 3));
                h_code.Append(code9);
                code9.Length = 0;
                code3.Length = 0;
		    }

            String h_2 = h_code.ToString(3, (h_code.Length) - 3); 
            int h_1 = Convert.ToInt32(h_code.ToString(0, 3));
		    int h_a1 = (int)Math.Floor((double)h_1 / 30);
		    int h_a2 = h_1 % 30;

            StringBuilder h_code_r = new StringBuilder();

            h_code_r.Append(Constants.h_key[h_a1]).Append(Constants.h_key[h_a2]).Append(h_2.ToString());

		    return new Zone(z_loc_y, z_loc_x, h_x, h_y, h_code_r.ToString());
	    }

	    private static Zone GetZoneByCode(String code) 
        {
		    int level = code.Length;
		    double h_size =  Calculations.CalcHexSize(level);
            double unit_x = 6 * h_size;
            double unit_y = 6 * h_size * Constants.h_k;
		    double h_x = 0;
		    double h_y = 0;
            
            string h_dec9 = "" + (Constants.h_key.IndexOf(code[0]) * 30 + Constants.h_key.IndexOf(code[1].ToString()) + "") + code.Substring(2);

            if(h_dec9.Length >= 3)
            {
                if (RegMatch(h_dec9[0], INC15) && RegMatch(h_dec9[1], EXC125) && RegMatch(h_dec9[2], EXC125))
                {
		            if(h_dec9[0] == '5')
                    {
                        h_dec9 = "7" + h_dec9.Substring(1, h_dec9.Length -1);
		            }
                    else if(h_dec9[0] == '1')
                    {
                        h_dec9 = "3" + h_dec9.Substring(1, h_dec9.Length - 1);
		            }
		        }
            }

            int d9xlen = h_dec9.Length;
            
		    for(int i=0; i < level + 1 - d9xlen; i++)
            {
		        h_dec9 = "0" + h_dec9;
		        d9xlen++;
		    }

            StringBuilder h_dec3 = new StringBuilder();

		    for(int i=0; i < d9xlen; i++)
            {
                String h_dec0 = BaseConverter.NumberToBaseString(h_dec9[i].ToString(), 3).ToString();
		        if(h_dec0.Length == 1)
                {
    		        h_dec3.Append("0");
	    	    }
		        h_dec3.Append(h_dec0);
		    }

            List<char> h_decx = new List<char>();
            List<char> h_decy = new List<char>();

		    for(int i = 0; i < h_dec3.Length / 2; i++)
            {
		        h_decx.Add(h_dec3[i * 2]);
		        h_decy.Add(h_dec3[(i * 2 + 1)]);
		    }

		    for(int i = 0; i <= level; i++)
            {
		        double h_pow = Math.Pow(3,level -i);
		        if(h_decx[i] == '0')
                {
		            h_x -= h_pow;
		        }
                else if(h_decx[i] == '2')
                {
                    h_x += h_pow;
		        }
		        if(h_decy[i] == '0')
                {
                    h_y -= h_pow;
		        }
                else if(h_decy[i] == '2')
                {
                    h_y += h_pow;
		        }
		    }

		    double h_lat_y = (Constants.h_k * h_x * unit_x + h_y * unit_y) / 2;
		    double h_lon_x = (h_lat_y - h_y * unit_y) / Constants.h_k;

		    Loc h_loc = Calculations.XyToLoc(h_lon_x, h_lat_y);

		    if (h_loc.lon > 180) 
            {
			    h_loc.lon -= 360;
		    } 
            else if (h_loc.lon < -180) 
            {
			    h_loc.lon += 360;
		    }

		    return new Zone(h_loc.lat, h_loc.lon, h_x, h_y, code);
	    }

	    private static bool RegMatch(String cs, Regex pat) 
        {
            return Regex.IsMatch(cs, pat.ToString());		    
	    }

	    private static bool RegMatch(char ch, Regex pat) 
        {
		    return RegMatch("" + ch, pat);
	    }
    }
}
