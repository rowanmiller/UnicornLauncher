﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnicornLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void LaunchControl_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(LaunchControl));
        }

        private void LaunchHistory_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(History));
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            ScenarioFrame.Navigate(typeof(Admin));
        }
    }

    public class UnicornLauncherContext : DbContext
    {
        public DbSet<Launch> Launches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=launchdata.db");
        }
    }

    public class Launch
    {
        public int LaunchId { get; set; }
        public DateTime Launched { get; set; }
        public string Info { get; set; }
    }
}
