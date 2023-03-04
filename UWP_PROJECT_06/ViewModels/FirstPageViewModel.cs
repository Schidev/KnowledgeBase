using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Popups;

namespace UWP_PROJECT_06.ViewModels
{
    public class FirstPageViewModel : ViewModelBase
    {
        public string OpenDictionaryHotkeyName { get => "OpenDictionary"; }
        public string OpenNotesHotkeyName { get => "OpenNotes"; }
        public string OpenSettingsHotkeyName { get => "OpenSettings"; }


        public AsyncCommand<string> OpenDictionaryCommand { get; set; }
        public FirstPageViewModel()
        {
            OpenDictionaryCommand = new AsyncCommand<string>(OpenDictionary);
        }

        private async Task OpenDictionary(string name)
        {
            try
            {
                InputInjector inputInjector = InputInjector.TryCreate();
                List<InjectedInputKeyboardInfo> keys = new List<InjectedInputKeyboardInfo>();

                string keyString = await SettingsService.ReadHotkey(name, "Key");
                var key = new InjectedInputKeyboardInfo();
                key.VirtualKey = (ushort)(VirtualKey)Enum.Parse(typeof(VirtualKey), keyString);
                key.KeyOptions = InjectedInputKeyOptions.KeyUp;
                keys.Add(key);

                string[] controlKeysArray = (await SettingsService.ReadHotkey(name, "Modifiers")).Split(",");

                foreach (string modifier in controlKeysArray)
                {
                    var controlKey = new InjectedInputKeyboardInfo();
                    controlKey.VirtualKey = (ushort)(VirtualKey)Enum.Parse(typeof(VirtualKey), modifier.Trim());
                    controlKey.KeyOptions = InjectedInputKeyOptions.KeyUp;
                    keys.Add(controlKey);
                }

                inputInjector.InjectKeyboardInput(keys.ToArray());
            }
            catch (Exception)
            {
                var msg = new MessageDialog("Action unavailiable (You need to change manifest app file).","Woops...");
                await msg.ShowAsync();
            }

        }
    }
}
