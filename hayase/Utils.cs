using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hayase
{
    public class Utils
    {
        public static string XAMLString(string s)
        {
            return s.Replace("_", "__");
        }

        public static System.Windows.Media.Color FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            value = value * 255;
            byte v = (byte)value;
            byte p = (byte)(value * (1 - saturation));
            byte q = (byte)(value * (1 - f * saturation));
            byte t = (byte)(value * (1 - (1 - f) * saturation));
            if (hi == 0)
                return System.Windows.Media.Color.FromRgb(v, t, p);
            else if (hi == 1)
                return System.Windows.Media.Color.FromRgb(q, v, p);
            else if (hi == 2)
                return System.Windows.Media.Color.FromRgb(p, v, t);
            else if (hi == 3)
                return System.Windows.Media.Color.FromRgb(p, q, v);
            else if (hi == 4)
                return System.Windows.Media.Color.FromRgb(t, p, v);
            else
                return System.Windows.Media.Color.FromRgb(v, p, q);
        }
    }
}
