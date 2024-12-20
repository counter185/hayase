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
        List<Func<System.Windows.Controls.Control>> registeredWidgets = new List<Func<System.Windows.Controls.Control>>();
        List<System.Windows.Controls.Control> spawnedWidgets = new List<System.Windows.Controls.Control>();
        Timer animTimer = new Timer();

        HayaseTimer popupActivatedTimer = new HayaseTimer();

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

            popupActivatedTimer.Start();
        }

        
        public void RegisterWidget<T>(Func<T> widgetProvider) where T : System.Windows.Controls.Control
        {
            registeredWidgets.Add(widgetProvider);
        }

        public void InitWidgets()
        {
            registeredWidgets.Add(() => new Widgets.Text("Quick menu  (alt+Q)"));
            registeredWidgets.Add(() => new Widgets.Clock());
            registeredWidgets.Add(() => new Widgets.MediaButtons());
            registeredWidgets.Add(() => new Widgets.RunBox());
            registeredWidgets.Add(() => new Widgets.MediaDeviceStatus());

            double calcHeight = 0;
            foreach (var widgetProvider in registeredWidgets)
            {
                var widget = widgetProvider();
                calcHeight += widget.Height;
                spawnedWidgets.Add(widget);
                panelMain.Children.Add(widget);
            }
            Height = calcHeight + 20;
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
    }
}
