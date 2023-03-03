using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Services;
using Windows.UI.Popups;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsRecoveryPageViewModel
    {
        public AsyncCommand SynchronizeCommand { get; }
        public SettingsRecoveryPageViewModel()
        {
            SynchronizeCommand = new AsyncCommand(Synchronize);
        }
        private async Task Synchronize()
        {
            MessageDialog message = new MessageDialog("Be patient, it may take time.", "We've started!");
            await message.ShowAsync();

            await SettingsService.RecreateDictionaryVoult();
            
            await SettingsService.ClearVault();
            await SettingsService.RecreateSourcesVoult();
            
            message = new MessageDialog("Thanks for your patience. Data synchronization has successfully finished.", "We've done!");
            await message.ShowAsync();
        }
    }
}
