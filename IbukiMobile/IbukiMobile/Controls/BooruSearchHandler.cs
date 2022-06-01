using IbukiMobile.Pages;
using Xamarin.Forms;

namespace IbukiMobile.Controls {
    class BooruSearchHandler : SearchHandler {
        protected override void OnQueryConfirmed() {
            //base.OnQueryConfirmed();
            DashboardPage.SearchQuery = Query;
        }
        protected override void OnQueryChanged(string oldValue, string newValue) {
            base.OnQueryChanged(oldValue, newValue);
            if(oldValue == DashboardPage.SearchQuery && newValue == "") {
                DashboardPage.SearchQuery = "";
                SearchBoxVisibility = SearchBoxVisibility.Collapsible;
            }
        }
    }
}