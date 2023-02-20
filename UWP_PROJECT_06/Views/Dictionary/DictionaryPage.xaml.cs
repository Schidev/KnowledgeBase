using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_PROJECT_06.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var w = cardGrid.Width;
            var h = cardGrid.Height;

            cardGrid.Width = cardGrid.DesiredSize.Width;
            cardGrid.Height = cardGrid.DesiredSize.Height + 100;

            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(cardGrid);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            var displayInformation = DisplayInformation.GetForCurrentView();
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("testImage" + ".png", CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)rtb.PixelWidth,
                                     (uint)rtb.PixelHeight,
                                     displayInformation.RawDpiX,
                                     displayInformation.RawDpiX,
                                     pixels);
                await encoder.FlushAsync();
            }

            cardGrid.Width = w;
            cardGrid.Height = h;
        }
    }
}
