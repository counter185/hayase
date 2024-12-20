using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace hayase.Config
{
    public class WidgetConfig
    {
        public List<string> widgetList = new List<string> {
            "hayase.headertext",
            "hayase.clock",
            "hayase.mediabuttons"
        };

        private static WidgetConfig _config = null;
        private static void TryLoadConfig()
        {
            string path = Environment.GetEnvironmentVariable("appdata") + "/cntrpl/hayase/conf.xml";
            _config = new WidgetConfig();
            if (File.Exists(path))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(path);
                XmlNode root = xdoc.SelectSingleNode("HayaseConfig");
                XmlNode widgetListRoot = root.SelectSingleNode("WidgetList");
                _config.widgetList.Clear();
                foreach (XmlNode widgetNode in widgetListRoot.ChildNodes)
                {
                    _config.widgetList.Add(widgetNode.InnerText);
                }
            }
        }
        public bool SaveConfig()
        {
            string path = Environment.GetEnvironmentVariable("appdata") + "/cntrpl/hayase/conf.xml";
            XmlDocument xdoc = new XmlDocument();
            XmlNode root = xdoc.CreateElement("HayaseConfig");
            XmlNode widgetListRoot = root.AppendChild(xdoc.CreateElement("WidgetList"));
            foreach (string widget in widgetList)
            {
                XmlNode widgetNode = widgetListRoot.AppendChild(xdoc.CreateElement("Widget"));
                widgetNode.InnerText = widget;
            }
            xdoc.AppendChild(root);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            xdoc.Save(path);

            return true;
        }
        public static WidgetConfig config
        {
            get { 
                if (_config == null)
                {
                    TryLoadConfig();
                }
                return _config;
            }
        }
    }
}
