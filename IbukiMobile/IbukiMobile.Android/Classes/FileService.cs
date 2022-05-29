using System.IO;
using Android.Content.Res;
using IbukiMobile.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(IbukiMobile.Droid.Classes.FileService))]
namespace IbukiMobile.Droid.Classes {
    public class FileService : IFileService {
        public string ReadAsset(string AssetName) {
            AssetManager Assets = Android.App.Application.Context.Assets;
            using StreamReader sr = new StreamReader(Assets.Open(AssetName));
            return sr.ReadToEnd();
        }
    }
}