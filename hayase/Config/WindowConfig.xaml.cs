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
using System.Windows.Shapes;

namespace hayase.Config
{
    /// <summary>
    /// Logika interakcji dla klasy WindowConfig.xaml
    /// </summary>
    public partial class WindowConfig : Window
    {
        MainWindow caller;
        bool exists = false;

        class WidgetDGridEntry
        {
            public bool Enabled { get; set; }
            public string Name { get; set; }
        }
        WidgetDGridEntry[] widgetEntries;

        public WindowConfig(MainWindow caller)
        {
            InitializeComponent();
            if (exists)
            {
                Close();
            }
            this.caller = caller;
            widgetEntries = (from x in caller.registeredWidgets
                             select new WidgetDGridEntry { Enabled = WidgetConfig.config.widgetList.Contains(x.Key), Name = x.Key }).ToArray();
            dgridWidgets.ItemsSource = widgetEntries;
            exists = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WidgetConfig.config.widgetList.Clear();
            foreach (WidgetDGridEntry w in widgetEntries)
            {
                if (w.Enabled)
                {
                    WidgetConfig.config.widgetList.Add(w.Name);
                }
            }
            WidgetConfig.config.SaveConfig();
            caller.RespawnWidgets();
            exists = false;
        }
    }
}
