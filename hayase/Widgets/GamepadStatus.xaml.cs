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
using System.IO;
using HidSharp;
using System.Runtime.InteropServices.WindowsRuntime;
using hayase.Widgets.Sub;
using Windows.Devices.Power;

namespace hayase.Widgets
{
    /// <summary>
    /// Logika interakcji dla klasy GamepadStatus.xaml
    /// </summary>
    public partial class GamepadStatus : UserControl, IHayaseWidget
    {
        public GamepadStatus()
        {
            InitializeComponent();
        }

        public void OnHayaseActivated()
        {
            UpdateDevices();
        }

        public void Tick(ulong timeSinceActivation)
        {
            
        }

        public void UpdateDevices()
        {
            panel_devices.Children.Clear();
            foreach (HidDevice device in DeviceList.Local.GetHidDevices())
            {
                try
                {
                    //Console.WriteLine(device.VendorID + " - " + device.ProductID);
                    Control c = TryGetInfoFromDevice(device, device.VendorID, device.ProductID);
                    if (c != null)
                    {
                        panel_devices.Children.Add(c);
                    }
                    //Console.WriteLine(device.GetFriendlyName());
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //Console.WriteLine($"{device.GetManufacturer()} - {device.GetProductName()} - {device.GetSerialNumber()} - {device.GetFriendlyName()}");
            }
        }

        public Control TryGetInfoFromDevice(HidDevice device, int vid, int pid)
        {
            string vidpidstring = vid.ToString("X4") + pid.ToString("X4");
            //Console.WriteLine(vidpidstring);
            switch (vidpidstring)
            {
                case "054C0CE6":
                    // dualsense
                    //connect and read packet
                    if (device.TryOpen(out var stream))
                    {
                        byte[] inputReport = new byte[64];
                        stream.Read(inputReport);
                        stream.Close();
                        //dump packet to file
                        byte battery0 = inputReport[53];
                        byte battery1 = inputReport[54];
                        int batteryPercent = (battery0 & 0x0f) * 100 / 8;
                        bool batteryCharging = (battery1 & 0x08) != 0;
                        return new MediaDeviceStatusListItem("DualSense", batteryPercent + "%" + (batteryCharging ? "+" : ""), batteryPercent/100.0);
                        //Console.WriteLine($"dualsense battery level: {batteryPercent} {batteryCharging} ({battery0.ToString("x")} {battery1.ToString("x")})");
                        //File.WriteAllBytes("dualsense.bin", inputReport);
                        //Console.WriteLine("dumped dualsense.bin");

                    }
                    break;
                case "054C09CC":
                    //dualshock4 v2
                    if (device.TryOpen(out var stream2))
                    {
                        byte[] inputReport = new byte[64];
                        stream2.Read(inputReport);
                        stream2.Close();
                        byte battery = inputReport[30];
                        int batteryPercent = (int)((battery & 0x0f) * 100 / 11);
                        bool charging = (battery & 0x10) != 0;
                        //File.WriteAllBytes("dualshock4.bin", inputReport);
                        Console.WriteLine(device.GetProductName());
                        return new MediaDeviceStatusListItem("DualShock 4 v2", batteryPercent + "%" + (charging ? "+" : ""), batteryPercent / 100.0);
                        //Console.WriteLine($"dualshock4 battery level: {batteryPercent}");
                        
                        Console.WriteLine("dumped dualshock4.bin");
                    }
                    break;
                case "057E2009":
                    //sp pro controller
                    if (device.TryOpen(out var stream3))
                    {
                        byte[] inputReport = new byte[16];
                        stream3.Read(inputReport);
                        stream3.Close();
                        byte battery = inputReport[2];
                        int batteryPercent = (int)(((battery & 0xE0) >> 4) * 100 / 8);
                        bool charging = false;// (battery & 0x10) != 0;
                        //File.WriteAllBytes("procontroller.bin", inputReport);
                        Console.WriteLine(device.GetProductName());
                        return new MediaDeviceStatusListItem("Switch Pro Controller", batteryPercent + "%" + (charging ? "+" : ""), batteryPercent / 100.0);
                        //Console.WriteLine($"pro controller battery level: {batteryPercent}");
                        //Console.WriteLine("dumped procontroller.bin");
                    }
                    break;
                default:
                    Console.WriteLine($"unknown hid device {vidpidstring} {device.GetFriendlyName()}");
                    break;
            }
            return null;
        }
    }
}
