using System.Linq;
using UnicornLauncher.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnicornLauncher
{
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
