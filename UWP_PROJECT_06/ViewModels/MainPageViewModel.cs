using Microsoft.UI.Xaml.Controls;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Bookmarks;
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
            TestBookmarksService();
            TestNotesService();
            TestProblemsService();

            OpenCommand = new AsyncCommand<object>(OpenFile);

            OpenDictionaryPageCommand = new AsyncCommand<object>(OpenDictionaryPage);
            OpenVideosListPageCommand = new AsyncCommand<object>(OpenVideosListPage);
            OpenWordPageCommand = new AsyncCommand<object>(OpenWordPage);

            AddTabCommand = new AsyncCommand<object>(AddTab);
            OpenCloseExtraPaneCommand = new AsyncCommand<object>(OpenCloseExtraPane);
        }


        async void TestBookmarksService()
        {
            var t = 0;

            var bookmark = new Bookmark()
            { 
                Date = DateTime.Now,
                Content = "Some content to test this database."
            };

            BookmarksService.CreateBookmark(bookmark);

            t = 1;

            var bm = BookmarksService.ReadBookmark(1);
            var bms =  BookmarksService.ReadBookmarks();

            t = 2;

            bm.Date = DateTime.Now.AddDays(13);
            bm.Content = "Some another content to test this database.";

            BookmarksService.UpdateBookmark(bm);

            t = 3;

            bm.Date = DateTime.Now.AddDays(1);
            bm.Content = "Some another another content to test this database.";

            BookmarksService.CreateBookmark(bm);

            t = 4;

            BookmarksService.DeleteBookmark(1);

            t = 5;

            var task = new DailyTask() 
            { 
                BookmarkID = 2,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(1),
                Task = "Some task to test"
            };

            var task1 = new DailyTask()
            {
                BookmarkID = 2,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(4),
                Task = "Some task1 to test"
            };

            var task2 = new DailyTask()
            {
                BookmarkID = 1,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(18),
                Task = "Some task2 to test"
            };

            BookmarksService.CreateDailyTask(task);
            BookmarksService.CreateDailyTask(task1);
            BookmarksService.CreateDailyTask(task2);

            t = 6;

            var dt = BookmarksService.ReadDailyTask(2);
            var dts = BookmarksService.ReadDailyTasks(5);

            t = 7;

            dt.BookmarkID = 3;
            dt.TimeBegin = DateTime.Now.AddDays(-20);
            dt.TimeEnd = DateTime.Now.AddDays(-10);
            dt.Task = "Updated task";

            BookmarksService.UpdateDailyTask(dt);

            t = 8;

            BookmarksService.DeleteDailyTask(1);

            t = 9;

            BookmarksService.DeleteBookmark(2);

            t = 10;
        }

        void TestNotesService()
        {

        }

        void TestProblemsService()
        {

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
