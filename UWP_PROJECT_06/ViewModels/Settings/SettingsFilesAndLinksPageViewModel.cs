using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsFilesAndLinksPageViewModel : ViewModelBase
    {
        string vaultPath; public string VaultPath { get => vaultPath; set => SetProperty(ref vaultPath, value); }
        string dictionaryPath; public string DictionaryPath { get => dictionaryPath; set => SetProperty(ref dictionaryPath, value); }
        string bookmarksPath; public string BookmarksPath { get => bookmarksPath; set => SetProperty(ref bookmarksPath, value); }
        string videosPath; public string VideosPath { get => videosPath; set => SetProperty(ref videosPath, value); }
        string soundsPath; public string SoundsPath { get => soundsPath; set => SetProperty(ref soundsPath, value); }
        string imagesPath; public string ImagesPath { get => imagesPath; set => SetProperty(ref imagesPath, value); }
        string documentsPath; public string DocumentsPath { get => documentsPath; set => SetProperty(ref documentsPath, value); }


        public AsyncCommand LostFocusCommand { get; }

        public SettingsFilesAndLinksPageViewModel()
        {
            Load();
            LostFocusCommand = new AsyncCommand(LostFocus);
        }

        async Task Load()
        {
            VaultPath = await SettingsService.ReadPath("vault");
            DictionaryPath = await SettingsService.ReadPath("dictionary");
            BookmarksPath = await SettingsService.ReadPath("bookmarks");
            VideosPath = await SettingsService.ReadPath("videos");
            SoundsPath = await SettingsService.ReadPath("sounds");
            ImagesPath = await SettingsService.ReadPath("images");
            DocumentsPath = await SettingsService.ReadPath("documents");
        }

        async Task LostFocus()
        {
            await SettingsService.WritePath("vault", VaultPath);
            await SettingsService.WritePath("dictionary", DictionaryPath);
            await SettingsService.WritePath("bookmarks", BookmarksPath);
            await SettingsService.WritePath("videos", VideosPath);
            await SettingsService.WritePath("sounds", SoundsPath);
            await SettingsService.WritePath("images", ImagesPath);
            await SettingsService.WritePath("documents", DocumentsPath);
        }

    }
}
