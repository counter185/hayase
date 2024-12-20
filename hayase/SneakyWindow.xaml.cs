using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace hayase
{
    /// <summary>
    /// Logika interakcji dla klasy SneakyWindow.xaml
    /// </summary>
    public partial class SneakyWindow : Window
    {
        private MainWindow caller;

        public SneakyWindow(MainWindow caller)
        {
            InitializeComponent();
            this.caller = caller;
            if (Process.GetProcessesByName("hayase").Length > 1)
            {
                System.Windows.MessageBox.Show("Hayase is already running", "hayase", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            RebindHotkey();
        }

        void RebindHotkey()
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            Interops.RegisterHotKey(new WindowInteropHelper(this).Handle, 39, 1, (int)Keys.Q);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312 && wParam.ToInt32() == 39)
            {
                Console.WriteLine("Hotkey pressed");
                ToggleVisibility();
            }

            return IntPtr.Zero;
        }

        void ToggleVisibility()
        {
            if (caller.Visibility == Visibility.Visible)
            {
                caller.HayaseDeactivated();
                caller.Hide();
            }
            else
            {
                caller.HayaseActivated();
                caller.Show();
                caller.HayaseLateActivated();
            }
        }

    }
}
