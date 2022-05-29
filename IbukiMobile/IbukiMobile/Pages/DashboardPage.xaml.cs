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

namespace IbukiMobile.Pages {
    public partial class DashboardPage : ContentPage {
        public Booru Danbooru { get; set; }
        public static ObservableCollection<BooruPost> ImagesCollection { get; set; } = new ObservableCollection<BooruPost>();
        
        private static int _Page = 1;
        private static bool _IsLoading = false;
        
        private static string _query;
        public delegate void SearchChangedEvent();
        public static event SearchChangedEvent searchChangedEvent;
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
            ImagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
        }

        private void ImagesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            BooruCollectionView.ItemsSource = ImagesCollection;
        }

        private async void DashboardPage_searchChangedEvent() {
            Title = SearchQuery == "" ? "Ibuki" : string.Join(" ", SearchQuery.Split(' ').Take(2));
            _Page = 1;
            await UpdateCollection();
        }

        private async void DashboardPage_OnAppearing(object sender, EventArgs e) {
            string script = DependencyService.Get<IFileService>().ReadAsset("Extensions/Danbooru.js");
            Danbooru = new Booru(script);

            await UpdateCollection();
        }
        private async Task UpdateCollection() {
            _Page = 1;
            ImagesCollection.Clear();

            LoadActivityIndicator.IsVisible = true;

            try {
                List<BooruPost> Posts = await Danbooru.GetPosts(page: _Page, search: SearchQuery);
                for (int i = 0; i < Posts.Count; i++) {
                    ImagesCollection.Add(Posts[i]);
                }
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
                List<BooruPost> Posts = await Danbooru.GetPosts(page: ++_Page, search: SearchQuery);
                for (int i = 0; i < Posts.Count; i++) {
                    ImagesCollection.Add(Posts[i]);
                }
            }
        }
    }
}
