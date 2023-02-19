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

namespace UWP_PROJECT_06.Views.Notes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SourcesPage : Page
    {
        public SourcesPage()
        {
            this.InitializeComponent();
        }

        private void Autosuggest_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }

        void FocusOnSearch(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            
        }

        void FocusOnLanguages(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            
        }

    }
}
