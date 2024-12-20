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
using Windows.Media.Control;
using static System.Net.Mime.MediaTypeNames;

namespace hayase.Widgets
{
    /// <summary>
    /// Logika interakcji dla klasy MediaButtons.xaml
    /// </summary>
    public partial class MediaButtons : UserControl, IHayaseWidget
    {
        const int VK_MEDIA_NEXT_TRACK =	0xB0;
        const int VK_MEDIA_PLAY_PAUSE =	0xB3;
        const int VK_MEDIA_PREV_TRACK =	0xB1;

        ulong lastMediaUpdate = 0;

        public MediaButtons()
        {
            InitializeComponent();
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            Interops.keybd_event(VK_MEDIA_PREV_TRACK, 0, 0, 0);
            UpdateMedia(600);
        }

        private void playpause_Click(object sender, RoutedEventArgs e)
        {
            Interops.keybd_event(VK_MEDIA_PLAY_PAUSE, 0, 0, 0);
            UpdateMedia(600);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Interops.keybd_event(VK_MEDIA_NEXT_TRACK, 0, 0, 0);
            UpdateMedia(600);
        }

        public void Tick(ulong timeSinceActivation)
        {
            if (lastMediaUpdate == 0 || lastMediaUpdate < timeSinceActivation - 2000)
            {
                lastMediaUpdate = timeSinceActivation;
                UpdateMedia();
            } 
        }

        public void OnHayaseActivated()
        {
            lastMediaUpdate = 0;
            UpdateMedia();
        }

        public void UpdateMedia(long wait = 0)
        {
            Task.Run(async () =>
            {
                await Task.Delay((int)wait);
                var gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
                var session = gsmtcsm.GetCurrentSession();
                if (session != null) { 
                    var mediaProperties = await GetMediaProperties(session);
                    Dispatcher.Invoke(() =>
                    {
                        songTitle.Content = Utils.XAMLString(mediaProperties.Title);
                        songArtist.Content = $"{Utils.XAMLString(mediaProperties.Artist)} :   {Utils.XAMLString(mediaProperties.AlbumTitle)}";
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        songTitle.Content = "<no media>";
                        songArtist.Content = "---";
                    });
                }
            });
        }

        private static async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
            await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

        private static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session) =>
            await session.TryGetMediaPropertiesAsync();
    }
}
