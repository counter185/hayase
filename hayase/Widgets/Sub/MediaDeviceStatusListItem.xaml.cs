using System;
using System.Collections.Generic;
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

namespace hayase.Widgets.Sub
{
    /// <summary>
    /// Logika interakcji dla klasy MediaDeviceStatusListItem.xaml
    /// </summary>
    public partial class MediaDeviceStatusListItem : UserControl
    {
        public MediaDeviceStatusListItem()
        {
            InitializeComponent();
        }
        public MediaDeviceStatusListItem(string name, string batteryPercentageText, double batteryPercent)
        {
            InitializeComponent();
            deviceName.Content = name;
            batteryPercentage.Content = batteryPercentageText;
            batteryBar.Value = batteryPercent;
            batteryBar.Foreground = new SolidColorBrush(GetBatteryColor(batteryPercent));
        }

        System.Windows.Media.Color GetBatteryColor(double batteryLevel)
        {
            double hue = 180 * batteryLevel;
            double saturation = 1.0;
            double value = 0.56;

            return Utils.FromHSV(hue, saturation, value);
        }
    }
}
