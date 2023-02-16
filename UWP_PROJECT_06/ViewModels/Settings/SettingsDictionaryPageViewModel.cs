using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using Windows.UI;
using Windows.UI.Popups;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsDictionaryPageViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Pair> Colors { get; set; }

        public AsyncCommand<object> LostFocusCommand { get; set; }

        public SettingsDictionaryPageViewModel()
        {
            Colors = new ObservableRangeCollection<Pair>();
            Load();

            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        async Task Load()
        {
            List<string> partsOfSpeech = DictionaryService.ReadPartsOfSpeech();
            var pairs = new Dictionary<string, string>();

            Colors.Clear();

            for (int q = 0; q < partsOfSpeech.Count; q++)
                Colors.Add(new Pair()
                {
                    Key = partsOfSpeech[q],
                    Value = await SettingsService.ReadColor(q+1)
                });
        }

        async Task LostFocus(object arg)
        {
            Regex reg = new Regex(@"#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]");

            foreach (Pair pair in Colors)
            {
                if (!reg.IsMatch(pair.Value))
                {
                    MessageDialog message = new MessageDialog("Color was written in incorrect format.", "Woops...");
                    await message.ShowAsync();
                    
                    return;
                }

                await SettingsService.WriteColor(pair.Key.Replace(" ", "_"), pair.Value.ToUpper());
            }
            Load();
        }
    }
}
