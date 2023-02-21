using Microsoft.UI.Xaml.Controls;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using UWP_PROJECT_06.Views.Notes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private Visibility leftExtendedPanelVisibility; public Visibility LeftExtendedPanelVisibility { get => leftExtendedPanelVisibility; set => SetProperty(ref leftExtendedPanelVisibility, value); }
        private Visibility rightExtendedPanelVisibility; public Visibility RightExtendedPanelVisibility { get => rightExtendedPanelVisibility; set => SetProperty(ref rightExtendedPanelVisibility, value); }

        #region TODO: Hotkeys in settings
        // Maybe in settings hotkeys
        public String OpenDictionaryModifiers { get => "Menu"; }
        public String OpenDictionaryKey { get => "Number1"; }
        #endregion

        public AsyncCommand<object> OpenNotesPageCommand { get; }
        public AsyncCommand<object> OpenSettingsPageCommand { get; }
        public AsyncCommand<object> OpenDictionaryPageCommand { get; }
        
        public AsyncCommand<object> AddTabCommand { get; }
        public AsyncCommand<object> OpenCloseExtraPaneCommand { get; }

        public MainPageViewModel()
        {
            rightExtendedPanelVisibility = Visibility.Collapsed; // Into settings
            leftExtendedPanelVisibility = Visibility.Collapsed; // Ito settings

            InitializeServices();

            OpenNotesPageCommand = new AsyncCommand<object>(OpenNotesPage);
            OpenSettingsPageCommand = new AsyncCommand<object>(OpenSettingsPage);
            OpenDictionaryPageCommand = new AsyncCommand<object>(OpenDictionaryPage);

            AddTabCommand = new AsyncCommand<object>(AddTab);
            OpenCloseExtraPaneCommand = new AsyncCommand<object>(OpenCloseExtraPane);
        }

        private async Task InitializeServices()
        {
            await SettingsService.Initialize();
            DictionaryService.InitializeDatabase();
            NotesService.InitializeDatabase();
            ProblemsService.InitializeDatabase();
            BookmarksService.InitializeDatabase();
            HistoryService.InitializeDatabase();
        }

        private async Task OpenNotesPage(object arg)
        {
            TabView tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(SourcesPage));

            TabViewItem currentTab = new TabViewItem()
            { 
                Header = "Notes",
                Name = "NotesPage",
                Content = frame
            };

            tabControl.TabItems.Add(currentTab);
            tabControl.SelectedItem = currentTab;
        }
        private async Task OpenSettingsPage(object arg)
        {
            TabView tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(SettingsPage));

            TabViewItem currentTab = new TabViewItem()
            {
                Header = "Settings",
                Name = "settingsPage",
                Content = frame
            };

            tabControl.TabItems.Add(currentTab);
            tabControl.SelectedItem = currentTab;
        }
        private async Task OpenDictionaryPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(DictionaryPage));

            TabViewItem currentTab = new TabViewItem()
            {
                Header = "Dictionary",
                Name = "dictionaryPage",
                Content = frame
            };

            tabControl.TabItems.Add(currentTab);
            tabControl.SelectedItem = currentTab;
        }
        
        private async Task AddTab(object arg)
        {
            TabView tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(DictionaryPage));

            TabViewItem newTab = new TabViewItem()
            {
                Header = "Dictionary Page",
                Name = "newTab",
                Content = frame
            };
            
            tabControl.TabItems.Add(newTab);
            tabControl.SelectedItem = newTab;
        }
        private async Task OpenCloseExtraPane(object arg)
        {
            Grid pane = arg as Grid;
            if (pane == null) return;

            Grid parent = pane.Parent as Grid;
            if (parent == null) return;

            int columnIndex = pane.Name == "leftPane" ? 2 : 6;

            pane.Visibility = pane.Visibility == Visibility.Collapsed 
                ? Visibility.Visible 
                : Visibility.Collapsed;

            parent.ColumnDefinitions[columnIndex].Width = pane.Visibility == Visibility.Collapsed
                ? new GridLength(0)
                : new GridLength(1, GridUnitType.Auto);
        }
    }
}
