using hayase.Config;
using hayase.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static hayase.Interops;

namespace hayase
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, Func<System.Windows.Controls.Control>> registeredWidgets = new Dictionary<string, Func<System.Windows.Controls.Control>>();
        List<System.Windows.Controls.Control> spawnedWidgets = new List<System.Windows.Controls.Control>();
        Timer animTimer = new Timer();

        HayaseTimer popupActivatedTimer = new HayaseTimer();

        SolidColorBrush borderBrush = new SolidColorBrush(Colors.White);
        Color borderTemp = Colors.White;

        double originalWidth;

        public MainWindow()
        {
            InitializeComponent();
            originalWidth = Width;
            this.Hide();
            new SneakyWindow(this).Show();
            InitWidgets();
            animTimer.Interval = 8;
            animTimer.Tick += (a,b) => AnimationUpdate();
            BorderBrush = borderBrush;

            popupActivatedTimer.Start();
        }

        
        public void RegisterWidget<T>(string key, Func<T> widgetProvider) where T : System.Windows.Controls.Control
        {
            registeredWidgets.Add(key, widgetProvider);
        }

        public void InitWidgets()
        {
            registeredWidgets.Add("hayase.headertext", () => new Widgets.Text("Quick menu  (alt+Q)"));
            registeredWidgets.Add("hayase.clock", () => new Widgets.Clock());
            registeredWidgets.Add("hayase.mediabuttons", () => new Widgets.MediaButtons());
            registeredWidgets.Add("hayase.runbox", () => new Widgets.RunBox());
            registeredWidgets.Add("hayase.mediadevicestatus", () => new Widgets.MediaDeviceStatus());
            registeredWidgets.Add("hayase.gamepadstatus", () => new Widgets.GamepadStatus());

            RespawnWidgets();
        }

        public void RespawnWidgets()
        {
            panelMain.Children.Clear();
            double calcHeight = 0;
            foreach (string widgetID in WidgetConfig.config.widgetList)
            {
                if (!registeredWidgets.ContainsKey(widgetID))
                {
                    Console.WriteLine($"Widget {widgetID} not found");
                    continue;
                }
                var widget = registeredWidgets[widgetID]();
                calcHeight += widget.Height;
                spawnedWidgets.Add(widget);
                panelMain.Children.Add(widget);
            }
            Height = calcHeight + 20;
            if (Visibility == Visibility.Visible)
            {
                HayaseActivated();
            }
        }

        public void HayaseDeactivated()
        {
            animTimer.Stop();
        }

        public void HayaseActivated()
        {
            animTimer.Start();

            //move the window to the current screen
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            Screen s = Screen.FromPoint(new System.Drawing.Point(w32Mouse.X, w32Mouse.Y));
            Console.WriteLine($"{w32Mouse.X} {w32Mouse.Y}  -> {s.DeviceName}");
            System.Drawing.Rectangle screenBounds = s.Bounds;
            this.Left = screenBounds.Left + 5;
            this.Top = screenBounds.Top + screenBounds.Height - this.Height - 50;

            foreach (var widget in spawnedWidgets)
            {
                if (widget is IHayaseWidget)
                {
                    (widget as IHayaseWidget).OnHayaseActivated();
                }
            }

            //anims
            popupActivatedTimer.Start();
            AnimationUpdate();
        }
        public void HayaseLateActivated()
        {
            this.BringIntoView();
            SetBlurBehind();
        }

        public double DefaultAnimCurve(double x)
        {
            return Math.Pow(x-1, 3) + 1;
        }

        public void AnimationUpdate()
        {
            //Console.WriteLine("*** anim update");
            this.Width = originalWidth * DefaultAnimCurve(popupActivatedTimer.PercentElapsed(350));
            double borderAnimPercent = popupActivatedTimer.PercentElapsed(800);
            if (borderAnimPercent < 1)
            {
                byte borderAlpha = (byte)(255 * (0.1 + (1.0 - DefaultAnimCurve(borderAnimPercent)) * 0.9));
                borderTemp.A = borderAlpha;
                borderBrush.Color = borderTemp;
            }

            foreach (var widget in spawnedWidgets)
            {
                if (widget is IHayaseWidget)
                {
                    (widget as IHayaseWidget).Tick(popupActivatedTimer.Elapsed());
                }
            }
        }

        void SetBlurBehind()
        {
            var bb = new DwmBlurbehind
            {
                dwFlags = DwmBlurBehindDwFlags.DwmBbEnable,
                Enabled = true,
            };

            var hwnd = new WindowInteropHelper(this).Handle;

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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Close hayase?", "hayase", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            new WindowConfig(this).Show();
        }
    }
}
