using System;
using System.Linq;
using Windows.Devices.Gpio;
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

        private void InitiateLaunch_Click(object sender, RoutedEventArgs e)
        {
            InitiateLaunchSequence();
        }

        private void InitiateLaunchSequence()
        {
            Status.Text = "Launch Initiated";
            _secondsUntilLaunch = 6;
            _controlTimer.Start();
        }

        private void ProcessTimerTick(object sender, object e)
        {
            _secondsUntilLaunch--;

            if (_secondsUntilLaunch > 0)
            {
                Status.Text = _secondsUntilLaunch.ToString();
            }
            else if (_secondsUntilLaunch == 0)
            {
                Status.Text = "0";
                _pin.Write(GpioPinValue.Low);
            }
            else if (_secondsUntilLaunch < 0)
            {
                _pin.Write(GpioPinValue.High);
                _controlTimer.Stop();
                RecordLaunch();
            }
        }

        private void RecordLaunch()
        {
            try
            {
                using (var db = new UnicornLauncherContext())
                {
                    db.Database.EnsureCreated();
                    db.Launches.Add(new Launch { Launched = DateTime.Now });
                    db.SaveChanges();

                    Diagnostics.Text = $"{db.Launches.Count()} launches in db";
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Text = ex.ToString();
            }
        }
    }
}
