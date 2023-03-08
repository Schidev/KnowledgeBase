using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_PROJECT_06
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<TabViewItem> RecentlyClosedTabs { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            RecentlyClosedTabs = new List<TabViewItem>();
        }

        private async void tabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            RecentlyClosedTabs.Add(args.Tab);
            sender.TabItems.Remove(args.Tab);

            if (sender.TabItems.Count == 0)
            {
                var viewModel = sender.DataContext as MainPageViewModel;
                await viewModel.AddNewTabCommand.ExecuteAsync(sender);
            }
        }
        private async void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabView = args.Element as TabView;

            if (tabView == null)
                return;

            if (((TabViewItem)tabView.SelectedItem).IsClosable)
            {
                RecentlyClosedTabs.Add(tabView.SelectedItem as TabViewItem);
                tabView.TabItems.Remove(tabView.SelectedItem);
            }

            if (tabView.TabItems.Count == 0)
            {
                var viewModel = tabView.DataContext as MainPageViewModel;
                await viewModel.AddNewTabCommand.ExecuteAsync(tabView);
            }
        }
        private async void AddNewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabView = args.Element as TabView;
            
            if (tabView == null)
                return;

            var viewModel = tabView.DataContext as MainPageViewModel;
            await viewModel.AddNewTabCommand.ExecuteAsync(tabView);
        }
        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabView = args.Element as TabView;

            if (tabView == null)
                return;

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = tabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < tabView.TabItems.Count)
                tabView.SelectedIndex = tabToSelect;
        }
        private void NextTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) 
        {
            var tabView = args.Element as TabView;

            if (tabView == null)
                return;

            if (tabView.SelectedIndex + 1 == tabView.TabItems.Count)
                tabView.SelectedIndex = 0;
            
            tabView.SelectedIndex += 1;
        }
        private void PreviousTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabView = args.Element as TabView;

            if (tabView == null)
                return;

            if (tabView.SelectedIndex == 0)
                tabView.SelectedIndex = tabView.TabItems.Count - 1;

            tabView.SelectedIndex -= 1;
        }
        private void OpenRecentlyClosedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var tabView = args.Element as TabView;

            if (tabView == null)
                return;

            tabView.TabItems.Add(RecentlyClosedTabs.Last());
            RecentlyClosedTabs.Remove(RecentlyClosedTabs.Last());

            tabView.SelectedIndex = tabView.TabItems.Count - 1;
        }

        private async void tabView_Loaded(object sender, RoutedEventArgs e)
        {
            TabView tabView = sender as TabView;
            var viewModel = tabView.DataContext as MainPageViewModel;
            await viewModel.AddNewTabCommand.ExecuteAsync(sender);
        }
    }
}
