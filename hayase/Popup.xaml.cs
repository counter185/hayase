using hayase.Widgets;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static hayase.Interops;

namespace hayase
{
    /// <summary>
    /// Logika interakcji dla klasy Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        Timer popupTimer = new Timer();
        HayaseTimer closeTimer = new HayaseTimer();
        double originalHeight;

        public Popup()
        {
            InitializeComponent();
            originalHeight = Height;
            Height = 0;
            popupTimer.Interval = 8;
            popupTimer.Tick += (a, b) =>
            {
                AnimationUpdate();
                if (closeTimer.Elapsed() > 5000)
                {
                    popupTimer.Stop();
                    Close();
                }
            };
            popupTimer.Start();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            closeTimer.Start();
        }

        public void AnimationUpdate()
        {
            //Console.WriteLine("*** anim update");
            double animPercent = Math.Min(Utils.DefaultAnimCurve(closeTimer.PercentElapsed(1200)), 1.0 - Utils.DefaultAnimCurve(closeTimer.PercentElapsed(1200, 3800)));
            this.Height = originalHeight * animPercent;
            /*double borderAnimPercent = popupActivatedTimer.PercentElapsed(800);
            if (borderAnimPercent < 1)
            {
                byte borderAlpha = (byte)(255 * (0.1 + (1.0 - DefaultAnimCurve(borderAnimPercent)) * 0.9));
                borderTemp.A = borderAlpha;
                borderBrush.Color = borderTemp;
            }*/
        }

        public static void SpawnNotificationPopup(string message)
        {
            Popup popup = new Popup();

            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            Screen s = Screen.FromPoint(new System.Drawing.Point(w32Mouse.X, w32Mouse.Y));
            System.Drawing.Rectangle screenBounds = s.Bounds;
            popup.Left = screenBounds.Right - popup.Width - 5;
            popup.Top = screenBounds.Bottom - popup.originalHeight - 100;

            popup.popupText.Content = message;
            popup.Show();
            Utils.SetBlurBehind(popup);
        }
    }
}
