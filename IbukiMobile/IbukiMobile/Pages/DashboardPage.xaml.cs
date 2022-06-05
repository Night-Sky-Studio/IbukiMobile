using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IbukiBooruLibrary;
using IbukiBooruLibrary.Classes;
using IbukiMobile.Interfaces;
using Xamarin.Forms;
using static IbukiMobile.App;

namespace IbukiMobile.Pages;

public partial class DashboardPage : ContentPage {
    //public Booru Danbooru { get; set; }
    public ObservableCollection<BooruPost> ImagesCollection { get; } = new ObservableCollection<BooruPost>();
    
    private static int _Page = 1;
    private static bool _IsLoading = false;
    
    private static string _query = "";
    public delegate void SearchChangedEvent();
    public static event SearchChangedEvent? searchChangedEvent;
    public static string SearchQuery {
        get => _query;
        set {
            _query = value.Trim();
            searchChangedEvent?.Invoke();
        }
    }
    
    public DashboardPage() {
        InitializeComponent();
        searchChangedEvent += DashboardPage_searchChangedEvent;
        //ImagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
    }
    private void UpdateCollectionViewSource() {
        BooruCollectionView.ItemsSource = ImagesCollection;
    }

    private async void DashboardPage_searchChangedEvent() {
        Title = SearchQuery == "" ? "Ibuki" : string.Join(" ", SearchQuery.Split(' ').Take(2));
        _Page = 1;
        await UpdateCollection();
    }

    private async void DashboardPage_OnAppearing(object sender, EventArgs e) {
        // string script = DependencyService.Get<IFileService>().ReadAsset("Extensions/Danbooru.js");
        // Danbooru = new Booru(script);

        System.Diagnostics.Debug.WriteLine($"ActiveBooruID: {ApplicationSettings.ActiveBooruID}");
        
        await UpdateCollection();
    }
    public async Task UpdateCollection() {
        _Page = 1;
        ImagesCollection.Clear();

        LoadActivityIndicator.IsVisible = true;

        try {
            List<BooruPost> Posts = await ApplicationSettings.ActiveBooru!.GetPosts(page: _Page, search: SearchQuery);
            for (int i = 0; i < Posts.Count; i++) {
                ImagesCollection.Add(Posts[i]);
            }
            UpdateCollectionViewSource();
        } catch (BooruException e) {
            ErrorLabel.Text = e.Message;
            ErrorLabel.IsVisible = true;
        }
        LoadActivityIndicator.IsVisible = false;
    }
    private async void RefreshView_Refreshing(object sender, EventArgs e) {
        await UpdateCollection();
        CollectionRefreshView.IsRefreshing = false;
    }

    private async void BooruCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e) {
        if (e.LastVisibleItemIndex - 3 == ImagesCollection.Count - 4 && !_IsLoading) {
            List<BooruPost> Posts = await ApplicationSettings.ActiveBooru!.GetPosts(page: ++_Page, search: SearchQuery);
            for (int i = 0; i < Posts.Count; i++) {
                ImagesCollection.Add(Posts[i]);
            }
            UpdateCollectionViewSource();
        }
    }
}