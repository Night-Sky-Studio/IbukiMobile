using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Esprima.Ast;
using IbukiBooruLibrary;
using IbukiMobile.Controls;
using IbukiMobile.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace IbukiMobile.Classes; 

public class Settings {
    [JsonIgnore]
    private readonly Random _rand = new Random();
    private static string SettingsFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "settings.json");

    [JsonProperty]
    private int _ActiveBooruID;

    [JsonIgnore]
    public int ActiveBooruID {
        get => _ActiveBooruID;
        set => _ActiveBooruID = Boorus.TryGetValue(value, out _) ? value : -1;
    }

    public Dictionary<int, Booru> Boorus { get; } = new Dictionary<int, Booru>();

    [JsonIgnore]
    public ObservableCollection<Booru> BindableBoorus => new ObservableCollection<Booru>(Boorus.Select(pair => pair.Value).ToList());

    [JsonIgnore]
    public Booru? ActiveBooru => Boorus.TryGetValue(ActiveBooruID, out Booru result) ? result : null;

    public void AddBooru(string Script, bool SetActive = false) {
        Boorus.Add(_rand.Next(100000, 999999), new Booru(Script));
        if (SetActive) ActiveBooruID = Boorus.Keys.ElementAt(Boorus.Count - 1);
    }
    
    public void InitSettings() {
        ActiveBooruID = -1;
        
        Boorus.Clear();
        
        // Add default booru
        Boorus.Add(_rand.Next(100000, 999999), new Booru(DependencyService.Get<IFileService>().ReadAsset("Extensions/Danbooru.js")));
        Boorus.Add(_rand.Next(100000, 999999), new Booru(DependencyService.Get<IFileService>().ReadAsset("Extensions/Safebooru.js")));
        
        // Set active booru
        ActiveBooruID = Boorus.Keys.ElementAt(1);

        if (File.Exists(SettingsFilePath)) {
            File.Delete(SettingsFilePath);
        }

        SaveSettingsFile();
    }
    
    // private static void CreateNewSettingsFile() {
    //     if (File.Exists(SettingsFilePath)) {
    //         File.Delete(SettingsFilePath);
    //     }
    //     using FileStream fileStream = File.OpenWrite(SettingsFilePath);
    // }
    
    public static Settings? LoadSettingsFromFile() {
        try {
            using FileStream fileStream = File.OpenRead(SettingsFilePath);
            using StreamReader streamReader = new StreamReader(fileStream);
            string json = streamReader.ReadToEnd();

            return json != "" ? JsonConvert.DeserializeObject<Settings>(json) : null;
        } catch (Exception e) {
            System.Diagnostics.Debug.WriteLine(e.ToString());
            return null;
        }
    }
    
    public void SaveSettingsFile() {
        using FileStream fileStream = File.OpenWrite(SettingsFilePath);
        using StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(JsonConvert.SerializeObject(this, Formatting.None));
    }

    public Settings() {
        /// Empty initializer
    }
}