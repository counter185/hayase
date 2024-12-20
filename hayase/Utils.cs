using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static hayase.Interops;
using System.Windows.Interop;
using System.Windows;

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

        public static double DefaultAnimCurve(double x)
        {
            return Math.Pow(x - 1, 3) + 1;
        }

        public static void SetBlurBehind(Window target)
        {
            var bb = new DwmBlurbehind
            {
                dwFlags = DwmBlurBehindDwFlags.DwmBbEnable,
                Enabled = true,
            };

            var hwnd = new WindowInteropHelper(target).Handle;

            const int dwmwaNcrenderingPolicy = 2;
            var dwmncrpDisabled = 2;

            //MARGINS m = new MARGINS { Left = -1 };
            //DwmExtendFrameIntoClientArea(hwnd, ref m);

            //DwmSetWindowAttribute(hwnd, dwmwaNcrenderingPolicy, ref dwmncrpDisabled, sizeof(int));

            //HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;
            DwmEnableBlurBehindWindow(hwnd, ref bb);

            var accent = new AccentPolicy();
            accent.AccentState = 3;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = 19;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(accentPtr);


        }
    }
}
