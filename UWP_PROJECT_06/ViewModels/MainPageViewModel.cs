using Microsoft.UI.Xaml.Controls;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using UWP_PROJECT_06.Views.Notes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWP_PROJECT_06.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private Visibility leftExtendedPanelVisibility; public Visibility LeftExtendedPanelVisibility { get => leftExtendedPanelVisibility; set => SetProperty(ref leftExtendedPanelVisibility, value); }
        private Visibility rightExtendedPanelVisibility; public Visibility RightExtendedPanelVisibility { get => rightExtendedPanelVisibility; set => SetProperty(ref rightExtendedPanelVisibility, value); }

        #region Hotkeys

        public string ExpandRightPanelHotkeyName { get => "ExpandRightPanel"; }
        public string ExpandLeftPanelHotkeyName { get => "ExpandLeftPanel"; }
       
        public string OpenDictionaryHotkeyName { get => "OpenDictionary"; }
        public string OpenNotesHotkeyName { get => "OpenNotes"; }
        public string OpenSettingsHotkeyName { get => "OpenSettings"; }
        public string OpenHistoryHotkeyName { get => "OpenHistory"; }

        public string AddNewTabHotkeyName { get => "AddNewTab"; }
        public string CloseCurrentTabHotkeyName { get => "CloseCurrentTab"; }
        public string NextTabHotkeyName { get => "NextTab"; }
        public string PreviousTabHotkeyName { get => "PreviousTab"; }
        public string OpenRecentlyClosedTabHotkeyName { get => "OpenRecentlyClosedTab"; }

        public string OpenFirstTabHotkeyName { get => "OpenFirstTab"; }
        public string OpenSecondTabHotkeyName { get => "OpenSecondTab"; }
        public string OpenThirdTabHotkeyName { get => "OpenThirdTab"; }
        public string OpenFourthTabHotkeyName { get => "OpenFourthTab"; }
        public string OpenFifthTabHotkeyName { get => "OpenFifthTab"; }
        public string OpenSixthTabHotkeyName { get => "OpenSixthTab"; }
        public string OpenSeventhTabHotkeyName { get => "OpenSeventhTab"; }
        public string OpenEighthTabHotkeyName { get => "OpenEighthTab"; }
        public string OpenLastTabHotkeyName { get => "OpenLastTab"; }

        #endregion
    
        public AsyncCommand<object> OpenFirstPageCommand { get; }
        public AsyncCommand<object> OpenNotesPageCommand { get; }
        public AsyncCommand<object> OpenSettingsPageCommand { get; }
        public AsyncCommand<object> OpenDictionaryPageCommand { get; }

        public AsyncCommand<object> AddNewTabCommand { get; }
        public AsyncCommand<object> OpenCloseExtraPaneCommand { get; }
        public AsyncCommand<object> RestoreCommand { get; }

        public MainPageViewModel()
        {
            rightExtendedPanelVisibility = Visibility.Visible; // Into settings
            leftExtendedPanelVisibility = Visibility.Collapsed; // Ito settings

            OpenFirstPageCommand = new AsyncCommand<object>(OpenFirstPage);
            OpenNotesPageCommand = new AsyncCommand<object>(OpenNotesPage);
            OpenSettingsPageCommand = new AsyncCommand<object>(OpenSettingsPage);
            OpenDictionaryPageCommand = new AsyncCommand<object>(OpenDictionaryPage);

            AddNewTabCommand = new AsyncCommand<object>(AddNewTab);
            OpenCloseExtraPaneCommand = new AsyncCommand<object>(OpenCloseExtraPane);
        }

        private async Task OpenFirstPage(object arg)
        {
            TabView tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(FirstPage));

            TabViewItem currentTab = new TabViewItem()
            {
                Header = "Blank page",
                Name = "BlankPage",
                Content = frame
            };

            tabControl.TabItems.Add(currentTab);
            tabControl.SelectedItem = currentTab;
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

            foreach (TabViewItem item in tabControl.TabItems)
            {
                if (item.Name == "settingsPage")
                {
                    tabControl.SelectedItem = item;
                    return;
                }
            }

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

        private async Task AddNewTab(object arg)
        {
            TabView tabControl = arg as TabView;

            if (tabControl == null)
                return;

            Frame frame = new Frame();
            frame.Navigate(typeof(FirstPage));

            TabViewItem newTab = new TabViewItem()
            {
                Header = "New tab",
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
