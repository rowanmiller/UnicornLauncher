using System;
using System.Threading.Tasks;
using UnicornLauncher.Model;
using Windows.Devices.Gpio;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnicornLauncher
{
    public sealed partial class LaunchControl : Page
    {
        private static GpioPin _pin;
        private DispatcherTimer _controlTimer;
        private int _secondsUntilLaunch;

        public LaunchControl()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pin == null)
                {
                    var gpio = GpioController.GetDefault();
                    _pin = gpio.OpenPin(5);
                    _pin.Write(GpioPinValue.High);
                    _pin.SetDriveMode(GpioPinDriveMode.Output);
                }
                Diagnostics.Text = "I/O initialized";
            }
            catch (Exception)
            {
                Diagnostics.Text = "Could not initialize I/O";
            }

            _controlTimer = new DispatcherTimer();
            _controlTimer.Interval = TimeSpan.FromSeconds(1);
            _controlTimer.Tick += ProcessTimerTick;

            await uiMediaElement.SpeakTextAsync("Good morning Build, welcome to launch control");
        }

        private async void InitiateLaunch_Click(object sender, RoutedEventArgs e)
        {
            await InitiateLaunchSequence();
        }

        private async Task InitiateLaunchSequence()
        {
            await ProvideStatusUpdate("Launch Initiated");
            _secondsUntilLaunch = 11;
            _controlTimer.Start();
        }

        private async void ProcessTimerTick(object sender, object e)
        {
            _secondsUntilLaunch--;

            if (_secondsUntilLaunch > 0)
            {
                await ProvideStatusUpdate(_secondsUntilLaunch.ToString());
            }
            else if (_secondsUntilLaunch == 0)
            {
                Status.Text = "Lift off";
                _pin?.Write(GpioPinValue.Low);
            }
            else if (_secondsUntilLaunch < 0)
            {
                _pin?.Write(GpioPinValue.High);
                _controlTimer.Stop();
                RecordLaunch();
            }
        }

        private async Task ProvideStatusUpdate(string status)
        {
            Status.Text = status;
            await uiMediaElement.SpeakTextAsync(status);
        }

        private void RecordLaunch()
        {
            try
            {
                Diagnostics.Text = "Saving launch information...";

                using (var db = new UnicornLauncherContext())
                {
                    db.Launches.Add(new Launch { Launched = DateTime.Now, Info = "Build demo launch" });
                    db.SaveChanges();
                }

                Diagnostics.Text = "Launch information saved";
            }
            catch (Exception ex)
            {
                Diagnostics.Text = "Failed to save launch: " + ex.ToString();
            }
        }
    }
}
