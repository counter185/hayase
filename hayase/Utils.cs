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
    }
}
