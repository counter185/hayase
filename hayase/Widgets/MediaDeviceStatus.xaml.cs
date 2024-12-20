using hayase.Widgets.Sub;
using MediaDevices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hayase.Widgets
{
    /// <summary>
    /// Logika interakcji dla klasy MediaDeviceStatus.xaml
    /// </summary>
    public partial class MediaDeviceStatus : UserControl, IHayaseWidget
    {
        public MediaDeviceStatus()
        {
            InitializeComponent();
            
        }

        public void OnHayaseActivated()
        {
            UpdateDevices();
        }

        public void UpdateDevices()
        {
            panel_devices.Children.Clear();
            MediaDevice.GetDevices().ToList().ForEach(device =>
            {
                try
                {
                    device.Connect();
                    MediaDeviceStatusListItem item = new MediaDeviceStatusListItem();
                    item.deviceName.Content = device.FriendlyName;
                    item.batteryBar.Value = device.PowerLevel/100.0;
                    item.batteryBar.Foreground = new SolidColorBrush( GetBatteryColor(device.PowerLevel / 100.0));
                    item.batteryPercentage.Content = device.PowerLevel + "%";
                    panel_devices.Children.Add(item);
                    device.Disconnect();
                    
                }
                catch (Exception e)
                {

                }
            });
        }

        public void Tick(ulong timeSinceActivation)
        {
            
        }

        System.Windows.Media.Color GetBatteryColor(double batteryLevel)
        {
            double hue = 180 * batteryLevel;
            double saturation = 1.0;
            double value = 0.56;

            return FromHSV(hue, saturation, value);
        }

        static System.Windows.Media.Color FromHSV(double hue, double saturation, double value)
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
