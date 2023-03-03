using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsSourcesPageViewModel : ViewModelBase
    {
        private bool isRenaming; public bool IsRenaming { get => isRenaming; set => SetProperty(ref isRenaming, value); }
        private string SourceType { get; set; }

        public AsyncCommand<object> RenameCommand { get; }
        public AsyncCommand<object> SaveCommand { get; }
        public AsyncCommand<object> DeleteCommand { get; }
        public AsyncCommand ReloadCommand { get; set; }

        
        public ObservableRangeCollection<Pair> SourceTypes { get; set; }

        public SettingsSourcesPageViewModel()
        {
            SourceType = "";
            IsRenaming = false;

            SourceTypes = new ObservableRangeCollection<Pair>();

            Load();

            RenameCommand = new AsyncCommand<object>(Rename);
            ReloadCommand = new AsyncCommand(Load);
            SaveCommand = new AsyncCommand<object>(Save);
            DeleteCommand = new AsyncCommand<object>(Delete);
        }

        private async Task Load()
        {
            SourceTypes.Clear();

            foreach (string sourceType in NotesService.ReadSourceTypes())
            {
                if (sourceType != "UNKNOWN" && sourceType != "VIDEO" && sourceType != "SOUND" && sourceType != "IMAGE" && sourceType != "DOCUMENT")
                {
                    SourceTypes.Add(new Pair()
                    {
                        Key = sourceType,
                        Value = await SettingsService.ReadPath(sourceType.ToLower())
                    });
                }
            }

            SourceTypes.Add(new Pair() { Key = "", Value = ""});
        }
        async Task Save(object arg)
        {
            TextBox textBox = arg as TextBox;
            if (textBox == null) return;

            Pair pair = textBox.DataContext as Pair;
            if (pair == null) return;

            if (IsRenaming)
            {
                MessageDialog msg = new MessageDialog("Finish renaming source type name and try again.", "Woops...");
                await msg.ShowAsync();
    
                return;
            }

            if (MarkdownService.CheckSourceType(pair.Key) == null)
            {
                MessageDialog msg = new MessageDialog("This name unavailable.", "Woops...");
                await msg.ShowAsync();

                return;
            }

            if (MarkdownService.CheckSourceType(pair.Value) == null)
            {
                MessageDialog msg = new MessageDialog("This folder path unavailable.", "Woops...");
                await msg.ShowAsync();

                pair.Value = await SettingsService.ReadPath(pair.Key.ToLower());
                textBox.Text = pair.Value;

                return;
            }

            int id = NotesService.ReadSourceType(pair.Key);

            if (id == 0)
            {
                await SettingsService.ClearVault();

                await SettingsService.CreatePath(pair.Key.ToLower(), pair.Value);
                NotesService.CreateSourceType(new SourceType() { SourceType1 = pair.Key.ToUpper() });

                MessageDialog msg = new MessageDialog("New source type was correctly added.", "Congratulations!");
                await msg.ShowAsync();

                return;
            }
            else
            {
                await SettingsService.ClearVault();

                await SettingsService.WritePath(pair.Key.ToLower(), pair.Value);
                NotesService.UpdateSourceType(new SourceType() {Id = (byte)id, SourceType1 = pair.Key.ToUpper() });

                await SettingsService.RecreateSourcesVoult();

                MessageDialog msg = new MessageDialog("Source type was correctly updated.", "Congratulations!");
                await msg.ShowAsync();
            }
        }
        async Task Delete(object arg)
        {
            TextBox textBox = arg as TextBox;
            if (textBox == null) return;

            Pair pair = textBox.DataContext as Pair;
            if (pair == null) return;

            if (IsRenaming)
                pair.Key = SourceType;
            
            int id = NotesService.ReadSourceType(pair.Key);
            MessageDialog msg;
            if (id == 0)
            {
                msg = new MessageDialog("This source type does not exist.", "Woops...");
                await msg.ShowAsync();
                return;
            }

            if (id < 6)
            {
                msg = new MessageDialog("This action unavailiable.", "Woops...");
                await msg.ShowAsync();
                return;
            }


            await SettingsService.ClearVault();
            
            await SettingsService.DeletePath(pair.Key.ToLower());
            NotesService.DeleteSourceType(id);

            await SettingsService.RecreateSourcesVoult();

            msg = new MessageDialog("Source type was successfully deleted.", "Congratulations!");
            await msg.ShowAsync();
        }
        private async Task Rename(object arg)
        {            
            TextBox textBox = arg as TextBox;

            if (textBox == null) return;

            if (IsRenaming)
            {
                textBox.IsEnabled = false;

                Pair pair = textBox.DataContext as Pair;
                if (pair == null) return;

                if (SourceType == pair.Key)
                {
                    IsRenaming = false;
                    return;
                }

                if (MarkdownService.CheckSourceType(textBox.Text) == null)
                {
                    pair.Key = SourceType;
                    textBox.Text = SourceType;
                    SourceType = "";

                    IsRenaming = false;
                    
                    MessageDialog msg = new MessageDialog("This name unavailable.", "Woops...");
                    await msg.ShowAsync();

                    return;
                }

                foreach (string name in NotesService.ReadSourceTypes())
                {
                    if (name.ToLower() == pair.Key.ToLower())
                    {
                        MessageDialog msg = new MessageDialog("This name is already exist.", "Woops...");
                        await msg.ShowAsync();

                        return;
                    }
                }

                if (SourceType != "" && SourceType != null)
                {
                    await SettingsService.ClearVault();

                    int id = NotesService.ReadSourceType(SourceType);
                    NotesService.UpdateSourceType(new SourceType() { Id = (byte)id, SourceType1 = textBox.Text });
                    await SettingsService.UpdatePath(SourceType.ToLower(), textBox.Text.ToLower());

                    await SettingsService.RecreateSourcesVoult();

                    MessageDialog msg = new MessageDialog("Name was correctly updated.", "Congratulations!");
                    await msg.ShowAsync();
                }
                else
                {
                    await SettingsService.ClearVault();
                    await SettingsService.CreatePath(pair.Key.ToLower(), pair.Value == "" ? pair.Key + "S" : pair.Value);
                    NotesService.CreateSourceType(new SourceType() { SourceType1 = pair.Key.ToUpper() });

                    await SettingsService.RecreateSourcesVoult();

                    MessageDialog msg = new MessageDialog("Name was correctly created.", "Congratulations!");
                    await msg.ShowAsync();
                }

                SourceType = "";
                IsRenaming = false;
            }
            else
            {
                IsRenaming = true;
                textBox.IsEnabled = true;
                SourceType = textBox.Text;
            }

        }

    }
}
