using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace hayase.Widgets
{
    /// <summary>
    /// Logika interakcji dla klasy Clock.xaml
    /// </summary>
    public partial class Clock : UserControl, IHayaseWidget
    {
        public Clock()
        {
            InitializeComponent();
            UpdateTime();
        }

        public void OnHayaseActivated()
        {
        }

        public void Tick(ulong timeSinceActivation)
        {
            UpdateTime();
        }

        void UpdateTime()
        {
            clockMainTime.Content = DateTime.Now.ToString("HH:mm:ss");
            clockMainDate.Content = DateTime.Now.ToString("D");
        }
    }
}
