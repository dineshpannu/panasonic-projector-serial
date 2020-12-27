using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace PanasonicSerialCommon
{
    public class AspectRatio
    {
        /// <summary>
        /// Takes in a string like 16x9 or 1.78:1 and returns 1.78.
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public static double Parse(string aspect)
        {
            Log.Debug("Inspecting aspect ratio {Aspect}", aspect);

            double x = 0.0;
            if (!string.IsNullOrEmpty(aspect))
            {
                if (!double.TryParse(aspect, out x))
                {
                    double y = 0.0;

                    Match match = Regex.Match(aspect, @"^(\d+\.*\d*).(\d+\.*\d*)$");
                    if (match.Success && match.Groups.Count > 2)
                    {
                        double.TryParse(match.Groups[1].Value, out x);
                        double.TryParse(match.Groups[2].Value, out y);
                    }

                    if (y != 0)
                    {
                        x /= y;
                    }
                }

                Log.Debug("Parsed aspect: {Ratio}", x);
            }
            else
            {
                Log.Debug("Parsing {Aspect} failed", aspect);
            }


            return x;
        }
    }
}
