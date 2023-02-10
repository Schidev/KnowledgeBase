using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_PROJECT_06.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DictionaryPage : Page
    {
        public DictionaryPage()
        {
            this.InitializeComponent();
        }

        private void Autosuggest_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine(e.Key);
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var t = (AutoSuggestBox)sender;
                var data = t.DataContext as DictionaryPageViewModel;
                data.SearchOnlineCommand.ExecuteAsync();
            }
        }

        void FocusOnSearch(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            Autosuggest.Focus(FocusState.Programmatic);
        }

        void FocusOnLanguages(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            LanguagesComboBox.Focus(FocusState.Programmatic);
        }
        

    }
}
