using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    static public class Util
    {
        /// <summary>
        /// Convert numeric latitude to string format
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns>string</returns>
        static public string Latitude_Coordinates(double latitude)
        {
            string s = "";
            string gsLatNS = "N";
            string gsLatMin = "", gsLatSec = "";

            string gsLatDeg = Math.Floor(Math.Abs(latitude)).ToString();
            double min = (Math.Abs(latitude) - Math.Floor(Math.Abs(latitude))) * 60.0f;
            if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useDecimalMinutes)
            {
                gsLatMin = ((Math.Abs(latitude) - Math.Floor(Math.Abs(latitude))) * 60.0f).ToString("#0.0");
            }
            else
            {
                gsLatMin = Math.Floor(min).ToString();
                double sec = min - Math.Floor(min);
                gsLatSec = sec.ToString("#0.0");
            }

            s = gsLatDeg + "° " + gsLatMin + "' ";
            if (!HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useDecimalMinutes)
            {
                s += gsLatSec + "\"";
            }
            // need to figure N/S
            if (latitude < 0)
                gsLatNS = "S";
            s += " " + gsLatNS;
            return s;
        }

        /// <summary>
        /// Convert numeric logitude to string
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns>string</returns>
        static public string Longitude_Coordinates(double longitude)
        {
            string s = "";
            string gsLonEW = "E";
            string gsLonMin = "", gsLonSec = "";
            string gsLonDeg = Math.Floor(Math.Abs(longitude)).ToString();
            double min = (Math.Abs(longitude) - Math.Floor(Math.Abs(longitude))) * 60.0f;
            if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useDecimalMinutes)
            {
                gsLonMin = ((Math.Abs(longitude) - Math.Floor(Math.Abs(longitude))) * 60.0f).ToString("#0.0");
            }
            else
            {
                gsLonMin = Math.Floor(min).ToString();
                double sec = min - Math.Floor(min);
                gsLonSec = sec.ToString("#0.0");
            }

            s = gsLonDeg + "° " + gsLonMin + "' ";
            if (!HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useDecimalMinutes)
            {
                s += gsLonSec + "\"";
            }
            if (longitude < 0)
                gsLonEW = "W";
            s += " " + gsLonEW;
            // need to figure E/W 
            return s;
        }
    }
}
