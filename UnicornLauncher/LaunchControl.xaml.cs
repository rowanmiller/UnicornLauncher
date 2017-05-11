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
    public sealed partial class LaunchControl : Page
    {
        private GpioPin _pin;
        private DispatcherTimer _controlTimer;
        private int _secondsUntilLaunch;
        private IRandomAccessStream _speechStream;

        public LaunchControl()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var gpio = GpioController.GetDefault();
                _pin = gpio.OpenPin(5);
                _pin.Write(GpioPinValue.High);
                _pin.SetDriveMode(GpioPinDriveMode.Output);
            }
            catch (Exception)
            {
                Diagnostics.Text = "Could not initialize I/O";
            }

            _controlTimer = new DispatcherTimer();
            _controlTimer.Interval = TimeSpan.FromSeconds(1);
            _controlTimer.Tick += ProcessTimerTick;
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
            var t = SpeakTextAsync(status);
            Status.Text = status;
            await t;
        }

        async Task SpeakTextAsync(string text)
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                var stream = await synthesizer.SynthesizeTextToStreamAsync(text);
                await uiMediaElement.PlayStreamAsync(stream, true);
            }
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
