using System;
using System.Threading.Tasks;
using UnicornLauncher.Model;
using Windows.ApplicationModel.Core;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnicornLauncher
{
    public sealed partial class Admin : Page
    {
        public Admin()
        {
            this.InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new UnicornLauncherContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        private async void Talk_Click(object sender, RoutedEventArgs e)
        {
            await uiMediaElement.SpeakTextAsync("Welcome to Unicorn Launcher");
        }
    }
}
