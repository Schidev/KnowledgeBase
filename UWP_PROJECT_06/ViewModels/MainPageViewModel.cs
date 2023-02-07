using Microsoft.UI.Xaml.Controls;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Visibility leftExtendedPanelVisibility;
        public Visibility LeftExtendedPanelVisibility
        {
            get => leftExtendedPanelVisibility;
            set => SetProperty(ref leftExtendedPanelVisibility, value);
        }

        Visibility rightExtendedPanelVisibility;
        public Visibility RightExtendedPanelVisibility
        {
            get => rightExtendedPanelVisibility;
            set => SetProperty(ref rightExtendedPanelVisibility, value);
        }

        public AsyncCommand<object> OpenCommand { get; }

        public AsyncCommand<object> OpenDictionaryPageCommand { get; }
        public AsyncCommand<object> OpenVideosListPageCommand { get; }
        public AsyncCommand<object> OpenWordPageCommand { get; }

        public AsyncCommand<object> AddTabCommand { get; }
        public AsyncCommand<object> OpenCloseExtraPaneCommand { get; }

        public MainPageViewModel()
        {
            rightExtendedPanelVisibility= Visibility.Collapsed;
            leftExtendedPanelVisibility= Visibility.Visible;
            
            InitializeServices();

            OpenCommand = new AsyncCommand<object>(OpenFile);

            OpenDictionaryPageCommand = new AsyncCommand<object>(OpenDictionaryPage);
            OpenVideosListPageCommand = new AsyncCommand<object>(OpenVideosListPage);
            OpenWordPageCommand = new AsyncCommand<object>(OpenWordPage);

            AddTabCommand = new AsyncCommand<object>(AddTab);
            OpenCloseExtraPaneCommand = new AsyncCommand<object>(OpenCloseExtraPane);
        }

        async Task InitializeServices()
        {
            await SettingsService.Initialize();
            DictionaryService.InitializeDatabase();
            NotesService.InitializeDatabase();
            ProblemsService.InitializeDatabase();
            BookmarksService.InitializeDatabase();
            HistoryService.InitializeDatabase();
        }


        private async Task OpenDictionaryPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Dictionary";
                currentTab.Name = "dictionaryPage";

                var frame = new Frame();
                frame.Navigate(typeof(DictionaryPage));

                currentTab.Content = frame;

                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }
        }

        private async Task OpenVideosListPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Videos";
                currentTab.Name = "videoListPage";
                var frame = new Frame();
                frame.Navigate(typeof(VideoListPage));

                currentTab.Content = frame;


                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }

        }

        private async Task OpenWordPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Word";
                currentTab.Name = "wordPage";
                var frame = new Frame();
                frame.Navigate(typeof(WordPage));

                currentTab.Content = frame;


                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }

        }


        private async Task OpenFile(object arg)
        {
            var webBrowser = arg as WebView;

            if (webBrowser != null)
                webBrowser.Source = new Uri(@"https://www.google.com/search?q=hello+world");
        }

        private async Task AddTab(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                // New tab
                var newTab = new TabViewItem();
                // Name the header "Settings"
                newTab.Header = "New tab";
                // name the tab
                newTab.Name = "newTab";

                // Change the tab icon.
                //newTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting };

                // Create a frame instance
                Frame frame = new Frame();

                // Add the frame to the tab
                newTab.Content = frame;

                // Navigate the frame to the settings page.
                frame.Navigate(typeof(DictionaryPage));

                // Add the tab to the tab control.
                tabControl.TabItems.Add(newTab);

                // Set the newly created tab as the selected tab.
                tabControl.SelectedItem = newTab;
            }
        }

        private async Task OpenCloseExtraPane(object arg)
        {
            var pane = arg as Grid;
            if (pane == null) return;

            var parent = pane.Parent as Grid;
            if (parent == null) return;

            int columnIndex = pane.Name == "leftPane" ? 2 : 6;

            pane.Visibility = pane.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

            parent.ColumnDefinitions[columnIndex].Width = pane.Visibility == Visibility.Collapsed
                                                ? new GridLength(0)
                                                : new GridLength(1, GridUnitType.Auto);
        }
    }
}
