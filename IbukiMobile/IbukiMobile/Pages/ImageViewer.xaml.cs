using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IbukiBooruLibrary.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IbukiMobile.Pages; 

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ImageViewer : ContentPage {
    public ImageViewer(BooruPost post) {
        InitializeComponent();
    }
}