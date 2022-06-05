using System;
using System.IO;
using System.Linq;
using IbukiBooruLibrary;
using IbukiMobile.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IbukiMobile {
    public partial class App : Application {
        public static Settings ApplicationSettings { get; set; }
    
        public App() {
            InitializeComponent();

            ApplicationSettings = new Settings();

            if (Settings.LoadSettingsFromFile() is { } settings) {
                ApplicationSettings = settings;
                for (int i = 0; i < ApplicationSettings.Boorus.Count; i++) {
                    ApplicationSettings.Boorus.Values.ElementAt(i).Initialize();
                }
            } else
                ApplicationSettings.InitSettings();

            MainPage = new AppShell();
        }

        protected override void OnStart() {

        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
