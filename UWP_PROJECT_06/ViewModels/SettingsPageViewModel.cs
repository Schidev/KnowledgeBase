using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Views.Settings;
using Windows.Security.Authentication.Identity.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP_PROJECT_06.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        bool isSettingsAppearanceOpen; public bool IsSettingsAppearanceOpen { get => isSettingsAppearanceOpen; set => SetProperty(ref isSettingsAppearanceOpen, value); }
        bool isSettingsFilesAndLinksOpen; public bool IsSettingsFilesAndLinksOpen { get => isSettingsFilesAndLinksOpen; set => SetProperty(ref isSettingsFilesAndLinksOpen, value); }
        bool isSettingsHotkeysOpen; public bool IsSettingsHotkeysOpen { get => isSettingsHotkeysOpen; set => SetProperty(ref isSettingsHotkeysOpen, value); }
        bool isSettingsHistoryOpen; public bool IsSettingsHistoryOpen { get => isSettingsHistoryOpen; set => SetProperty(ref isSettingsHistoryOpen, value); }
        bool isSettingsDictionaryOpen; public bool IsSettingsDictionaryOpen { get => isSettingsDictionaryOpen; set => SetProperty(ref isSettingsDictionaryOpen, value); }
        bool isSettingsSourcesOpen; public bool IsSettingsSourcesOpen { get => isSettingsSourcesOpen; set => SetProperty(ref isSettingsSourcesOpen, value); }
        bool isSettingsBookmarksOpen; public bool IsSettingsBookmarksOpen { get => isSettingsBookmarksOpen; set => SetProperty(ref isSettingsBookmarksOpen, value); }

        object currentContent; public object CurrentContent { get => currentContent; set => SetProperty(ref currentContent, value); }
        
        public AsyncCommand<object> SelectCommand { get; }

        public SettingsPageViewModel()
        {
            isSettingsAppearanceOpen = true;
            isSettingsFilesAndLinksOpen = false;
            isSettingsHotkeysOpen = false;
            isSettingsHistoryOpen = false;
            isSettingsDictionaryOpen = false;
            isSettingsSourcesOpen = false;
            isSettingsBookmarksOpen = false;

            CurrentContent = new SettingsAppearancePage();

            SelectCommand = new AsyncCommand<object>(Select);
        }

        async Task Select(object arg)
        {
            var button = arg as Button;

            if (button == null)
                return;

            if (button.Name == "appearanceBtn")
            {
                IsSettingsAppearanceOpen = true;
                CurrentContent = new SettingsAppearancePage();
            }
            else
            {
                IsSettingsAppearanceOpen = false;
            }

            if (button.Name == "filesAndLinksBtn")
            {
                IsSettingsFilesAndLinksOpen = true;
                CurrentContent = new SettingsFilesAndLinksPage();
            }
            else
            {
                IsSettingsFilesAndLinksOpen = false;
            }

            if (button.Name == "hotkeysBtn")
            {
                IsSettingsHotkeysOpen = true;
                CurrentContent = new SettingsHotkeysPage();
            }
            else
            {
                IsSettingsHotkeysOpen = false;
            }

            if (button.Name == "historyBtn")
            {
                IsSettingsHistoryOpen = true;
                CurrentContent = new SettingsHistoryPage();
            }
            else
            {
                IsSettingsHistoryOpen = false;
            }

            if (button.Name == "dictionaryBtn")
            {
                IsSettingsDictionaryOpen = true;
                CurrentContent = new SettingsDictionaryPage();
            }
            else
            {
                IsSettingsDictionaryOpen = false;
            }

            if (button.Name == "sourcesBtn")
            {
                IsSettingsSourcesOpen = true;
                CurrentContent = new SettingsSourcesPage();
            }
            else
            {
                IsSettingsSourcesOpen = false;
            }

            if (button.Name == "bookmarksBtn")
            {
                IsSettingsBookmarksOpen = true;
                CurrentContent = new SettingsBookmarksPage();
            }
            else
            {
                IsSettingsBookmarksOpen = false;
            }

        }

    }
}
