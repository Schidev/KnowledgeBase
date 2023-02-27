using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsHistoryPageViewModel : ViewModelBase
    {
        HistoryItem selectedItem; public HistoryItem SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public ObservableRangeCollection<Grouping<string, HistoryItem>> History { get; set; }

        public AsyncCommand ReloadCommand { get; }
        public AsyncCommand<object> CopyPathCommand { get; }
        public AsyncCommand<object> HistoryItemSelectedCommand { get; }

        public SettingsHistoryPageViewModel()
        {
            SelectedItem = null;
            History = new ObservableRangeCollection<Grouping<string, HistoryItem>>();

            Load();

            ReloadCommand = new AsyncCommand(Load);
            CopyPathCommand = new AsyncCommand<object>(CopyPath);
            HistoryItemSelectedCommand = new AsyncCommand<object>(HistoryItemSelected);
        }

        async Task Load()
        {
            List<HistoryItem> items = new List<HistoryItem>();
            SortedSet<DateTime> dates = new SortedSet<DateTime>();

            List<HistoryItem> historyItems = await SettingsService.ReadHistory();

            historyItems.Reverse();

            foreach (HistoryItem item in historyItems)
            {
                items.Add(item);

                if (!dates.Contains(item.Date.Date))
                    dates.Add(item.Date.Date);
            }

            History.Clear();

            foreach (DateTime date in dates.Reverse())
            {
                History.Add(new Grouping<string, HistoryItem>(
                    date.ToString("D"),
                    items.Where(e => e.Date.Date == date.Date)));
            }

        }
        async Task HistoryItemSelected(object arg)
        {
            ListView historyItems = arg as ListView;

            if (historyItems.SelectedItem != null)
            {
                string fileName = historyItems.SelectedItem.ToString();
                string cardType = String.Empty;
                
                // ToDo: logic how to open card in dictionary (if it's word), in sources (if it source), ...
                // or in just card editor.
            }
        }
        async Task CopyPath(object arg)
        {
            TextBlock textBlock = arg as TextBlock;

            if (textBlock != null)
            {
                DataPackage dataPackage = new DataPackage();
                HistoryItem historyItem = textBlock.DataContext as HistoryItem;

                dataPackage.SetText(historyItem.FullPath);
                Clipboard.SetContent(dataPackage);

                Storyboard animation = textBlock.Resources["CopiedToClipboard"] as Storyboard;
                animation.Begin();
            }
        }
    }
}
