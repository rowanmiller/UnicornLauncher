using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnicornLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new UnicornLauncherContext())
            {
                HistoryGrid.ItemsSource = db.Launches
                    .OrderByDescending(l => l.Launched)
                    .Take(10)
                    .ToList();
            }
        }
    }
}
