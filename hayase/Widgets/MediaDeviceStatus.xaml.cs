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
                    MediaDeviceStatusListItem item = new MediaDeviceStatusListItem(device.FriendlyName, device.PowerLevel + "%", device.PowerLevel / 100.0);
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
    }
}
