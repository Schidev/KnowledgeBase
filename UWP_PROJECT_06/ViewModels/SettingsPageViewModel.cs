using MvvmHelpers.Commands;
using System.Threading.Tasks;
using UWP_PROJECT_06.Views.Settings;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private bool isSettingsAppearanceOpen; public bool IsSettingsAppearanceOpen { get => isSettingsAppearanceOpen; set => SetProperty(ref isSettingsAppearanceOpen, value); }
        private bool isSettingsFilesAndLinksOpen; public bool IsSettingsFilesAndLinksOpen { get => isSettingsFilesAndLinksOpen; set => SetProperty(ref isSettingsFilesAndLinksOpen, value); }
        private bool isSettingsHotkeysOpen; public bool IsSettingsHotkeysOpen { get => isSettingsHotkeysOpen; set => SetProperty(ref isSettingsHotkeysOpen, value); }
        private bool isSettingsHistoryOpen; public bool IsSettingsHistoryOpen { get => isSettingsHistoryOpen; set => SetProperty(ref isSettingsHistoryOpen, value); }
        private bool isSettingsDictionaryOpen; public bool IsSettingsDictionaryOpen { get => isSettingsDictionaryOpen; set => SetProperty(ref isSettingsDictionaryOpen, value); }
        private bool isSettingsSourcesOpen; public bool IsSettingsSourcesOpen { get => isSettingsSourcesOpen; set => SetProperty(ref isSettingsSourcesOpen, value); }
        private bool isSettingsBookmarksOpen; public bool IsSettingsBookmarksOpen { get => isSettingsBookmarksOpen; set => SetProperty(ref isSettingsBookmarksOpen, value); }
        private bool isSettingsRecoveryOpen; public bool IsSettingsRecoveryOpen { get => isSettingsRecoveryOpen; set => SetProperty(ref isSettingsRecoveryOpen, value); }

        private object currentContent; public object CurrentContent { get => currentContent; set => SetProperty(ref currentContent, value); }
        
        public AsyncCommand<object> SelectCommand { get; }

        public SettingsPageViewModel()
        {
            IsSettingsAppearanceOpen = true;
            IsSettingsFilesAndLinksOpen = false;
            IsSettingsHotkeysOpen = false;
            IsSettingsHistoryOpen = false;
            IsSettingsDictionaryOpen = false;
            IsSettingsSourcesOpen = false;
            IsSettingsBookmarksOpen = false;
            IsSettingsRecoveryOpen = false;

            CurrentContent = new SettingsAppearancePage();

            SelectCommand = new AsyncCommand<object>(Select);
        }

        private async Task Select(object arg)
        {
            Button button = arg as Button;

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

            if (button.Name == "recoveryBtn")
            {
                IsSettingsRecoveryOpen = true;
                CurrentContent = new SettingsRecoveryPage();
            }
            else
            {
                IsSettingsRecoveryOpen = false;
            }
        }

    }
}
