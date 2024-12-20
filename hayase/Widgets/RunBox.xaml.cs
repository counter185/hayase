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

namespace hayase.Widgets
{
    /// <summary>
    /// Logika interakcji dla klasy RunBox.xaml
    /// </summary>
    public partial class RunBox : UserControl
    {
        public RunBox()
        {
            InitializeComponent();
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            RunCommand(runBox.Text);
        }

        public void RunCommand(string command)
        {
            try
            {
                System.Diagnostics.Process.Start("cmd", "/c " + command);
                runBox.Text = "";
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void runBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunCommand(runBox.Text);
            }
        }
    }
}
