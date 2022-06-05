using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IbukiBooruLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using IbukiMobile.Classes;
using static IbukiMobile.App;

namespace IbukiMobile {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell {
        public AppShell() {
            InitializeComponent();

            BoorusListView.ItemsSource = ApplicationSettings.BindableBoorus;
            BoorusListView.SelectedItem = ApplicationSettings.ActiveBooru;
        }

        private async void BoorusListView_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            ApplicationSettings.ActiveBooruID = ApplicationSettings.Boorus.GetID((Booru)BoorusListView.SelectedItem);
            _DashboardPage.Title = ApplicationSettings.ActiveBooru!.Name;
            ApplicationSettings.SaveSettingsFile();

            await _DashboardPage.UpdateCollection();
        }
    }
}