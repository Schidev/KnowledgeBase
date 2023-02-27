using ColorCode;
using MvvmHelpers.Commands;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using System.Security.AccessControl;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using System.Text.RegularExpressions;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourceEditPageViewModel : ViewModelBase
    {
        public int Id { get; set; }
        private int selectedState; public int SelectedState { get => selectedState; set => SetProperty(ref selectedState, value); }
        private int selectedTheme; public int SelectedTheme { get => selectedTheme; set => SetProperty(ref selectedTheme, value); }
        private int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value); }
        private bool isDownloaded; public bool IsDownloaded { get => isDownloaded; set => SetProperty(ref isDownloaded, value); }
        
        private Source source; public Source Source { get => source; set => SetProperty(ref source, value); }
        private Quote selectedQuote; public Quote SelectedQuote { get => selectedQuote; set => SetProperty(ref selectedQuote, value); }

        public ObservableRangeCollection<string> States { get; set; }
        public ObservableRangeCollection<string> Themes{ get; set; }
        public ObservableRangeCollection<string> SourceTypes { get; set; }

        public ObservableRangeCollection<Quote> Quotes { get; set; } 
        public ObservableRangeCollection<Note> Notes { get; set; } 
        public ObservableRangeCollection<SourceExtra> Extras { get; set; }


        public string TakeAScreenshotHotkeyName { get => "TakeAScreenshot"; }

        public AsyncCommand<object> ScreenshotCommand { get; set; }
        public AsyncCommand<object> DeleteCommand { get; set; }
        public AsyncCommand<object> StateSelectedCommand { get; }
        public AsyncCommand<object> ThemeSelectedCommand { get; }
        public AsyncCommand<object> SourceTypeSelectedCommand { get; }
        public AsyncCommand<object> LostFocusCommand { get; set; }

        public SourceEditPageViewModel() : this(0) { }
    
        public SourceEditPageViewModel(int id)
        {
            this.Id = id;

            States = new ObservableRangeCollection<string>() { "Select state" };
            Themes = new ObservableRangeCollection<string>() { "Select theme" };
            SourceTypes = new ObservableRangeCollection<string>() { "Select source type" };

            Quotes = new ObservableRangeCollection<Quote>();
            Notes = new ObservableRangeCollection<Note>();
            Extras = new ObservableRangeCollection<SourceExtra>();

            Load();

            ScreenshotCommand = new AsyncCommand<object>(Screenshot);
            DeleteCommand = new AsyncCommand<object>(Delete);

            StateSelectedCommand = new AsyncCommand<object>(StateSelected);
            ThemeSelectedCommand = new AsyncCommand<object>(ThemeSelected);
            SourceTypeSelectedCommand = new AsyncCommand<object>(SourceTypeSelected);

            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        private async Task Load()
        {
            #region Source

            source = NotesService.ReadSource(Id);
            source.SourceName = source.Id == 0 ? "" : source.SourceName;
            source.Duration = source.Id == 0 ? 0 : source.Duration;
            source.ActualTime = source.Id == 0 ? 0 : source.ActualTime;
            IsDownloaded = source.Id == 0 ? false : source.IsDownloaded;
            source.IsDownloaded = IsDownloaded;
            source.Description = source.Id == 0 ? "" : source.Description;
            source.SourceLink = source.Id == 0 ? "" : source.SourceLink;

            #endregion
            #region States

            foreach (string state in NotesService.ReadStates())
                States.Add(state);

            SelectedState = source.Id == 0 ? 0 : source.State;

            #endregion
            #region Themes

            foreach (string theme in NotesService.ReadThemes())
                Themes.Add(theme);

            SelectedTheme = source.Id == 0 ? 0 : source.Theme;

            #endregion
            #region Source types

            foreach (string sourceType in NotesService.ReadSourceTypes())
                SourceTypes.Add(sourceType);

            SelectedSourceType = source.Id == 0 ? 0 : source.SourceType;

            #endregion
            #region Quotes

            List<Quote> quotes = NotesService.ReadQuotes(source.Id);

            Quotes.Clear();

            foreach (Quote quote in quotes.OrderBy(n => n.QuoteBegin).ToList())
                Quotes.Add(quote);

            #endregion
            #region Notes

            List<Note> notes = NotesService.ReadNotes(source.Id);

            Notes.Clear();

            foreach (Note note in notes.OrderBy(n => n.Stamp).ToList())
                Notes.Add(note);

            #endregion
            #region Extras

            List<SourceExtra> extras = NotesService.ReadSourceExtras(source.Id);

            Extras.Clear();

            foreach (SourceExtra extra in extras.OrderBy(n => n.Key).ToList())
                Extras.Add(extra);

            #endregion

            LostFocus(Extras);
            LostFocus(Notes);
            LostFocus(Quotes);
        }

        private async Task StateSelected(object arg)
        {
            source.State = (byte)SelectedState;
        }

        private async Task ThemeSelected(object arg)
        {
            source.Theme = (byte)SelectedTheme;
        }

        private async Task SourceTypeSelected(object arg)
        {
            source.SourceType = (byte)SelectedSourceType;
        }

        private async Task Screenshot(object arg)
        {
            MessageDialog message = new MessageDialog("Would you like to take a screenshot of this card?");

            message.Commands.Add(new UICommand { Label = "Yeah", Id = 0 });
            message.Commands.Add(new UICommand { Label = "No", Id = 1 });

            var result = await message.ShowAsync();

            Grid cardGrid = arg as Grid;

            if (cardGrid == null || (int)result.Id == 1)
                return;

            RenderTargetBitmap rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(cardGrid);

            var pixelBuffer = await rtb.GetPixelsAsync();
            byte[] pixels = pixelBuffer.ToArray();
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();

            #region File System

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("images");

            folderName = System.IO.Path.Combine(vaultName, folderName);

            string fullPath = folderName;

            StorageFolder folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);

            StorageFile file = null;

            if (cardGrid.DataContext as Quote != null)
            {
                Quote quote = cardGrid.DataContext as Quote;

                file = await folder.CreateFileAsync(String.Format("IMAGE_QUOTE_{0}_STAMP_{1}.png",
                    MarkdownService.CheckSource(NotesService.ReadSource(quote.SourceID).SourceName),
                    quote.QuoteBegin.Replace(" ", "_").Replace(":", "_")), CreationCollisionOption.ReplaceExisting);
            }

            if (cardGrid.DataContext as Note != null)
            {
                Note note = cardGrid.DataContext as Note;
                file = await folder.CreateFileAsync(String.Format("IMAGE_NOTE_{0}_STAMP_{1}.png",
                    MarkdownService.CheckSource(NotesService.ReadSource(note.SourceID).SourceName),
                    note.Title.Replace(" ", "_").Replace(":", "_")), CreationCollisionOption.ReplaceExisting);
            }

            if (cardGrid.DataContext as SourceExtra != null)
            {
                SourceExtra extra = cardGrid.DataContext as SourceExtra;
                file = await folder.CreateFileAsync(String.Format("IMAGE_SOURCE_EXTRA_{0}_KEY_{1}.png",
                    MarkdownService.CheckSource(NotesService.ReadSource(extra.SourceID).SourceName),
                    extra.Key.ToUpper().Replace(" ","_").Replace(":", "_")), CreationCollisionOption.ReplaceExisting);
            }

            #endregion

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)rtb.PixelWidth,
                                     (uint)rtb.PixelHeight,
                                     displayInformation.RawDpiX,
                                     displayInformation.RawDpiX,
                                     pixels);
                await encoder.FlushAsync();
            }

            await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = file.Path, Date = DateTime.UtcNow });
        }
        private async Task Delete(object arg)
        {
            var grid = arg as Grid;

            foreach (object child in grid.Children)
            {
                var childGrid = child as Grid;

                if (childGrid == null)
                    continue;

                foreach (object innerChild in ((Grid)childGrid).Children)
                {
                    var textBox = innerChild as TextBox;

                    if (textBox == null)
                        continue;

                    textBox.Text = "";
                }

            }

            grid.Visibility = Visibility.Collapsed;
        }

        private async Task LostFocus(object arg)
        {
            var SourceExtrasCollection = arg as ObservableRangeCollection<SourceExtra>;

            #region Extras

            if (SourceExtrasCollection != null)
            {
                for (int q = Extras.Count - 1; q >= 0; q--)
                {
                    SourceExtra e = Extras[q];

                    e.Key = e.Key.Trim();
                    e.Value = e.Value.Trim();

                    if (e.Key == "" && e.Value == "")
                    {
                        if (e.Id != 0)
                        {
                            NotesService.DeleteSourceExtra(e.Id);

                            await MarkdownService.WriteSource(NotesService.ReadSource(e.SourceID),
                                NotesService.ReadQuotes(e.SourceID),
                                NotesService.ReadNotes(e.SourceID),
                                NotesService.ReadSourceExtras(e.SourceID));
                        }

                        Extras.RemoveAt(q);
                    }
                }

                Extras.Add(new SourceExtra()
                {
                    SourceID = Id,
                    Key = "",
                    Value = ""
                });

                return;
            }

            #endregion

            var NotesCollection = arg as ObservableRangeCollection<Note>;

            #region Notes

            if (NotesCollection != null)
            {
                for (int q = Notes.Count - 1; q >= 0; q--)
                {
                    Note n = Notes[q];
                    n.Stamp = n.Stamp.Trim();
                    n.Title = n.Title.Trim();
                    n.Note1 = n.Note1.Trim();

                    if (n.Stamp == "" && n.Title == "" && n.Note1 == "")
                    {
                        if (n.Id != 0)
                        {
                            NotesService.DeleteNote(n.Id);

                            await MarkdownService.WriteSource(NotesService.ReadSource(n.SourceID),
                                NotesService.ReadQuotes(n.SourceID),
                                NotesService.ReadNotes(n.SourceID),
                                NotesService.ReadSourceExtras(n.SourceID));
                        }

                        Notes.RemoveAt(q);
                    }
                }

                Notes.Add(new Note()
                {
                    SourceID = Id,
                    Stamp = "",
                    Title = "",
                    Note1 = ""
                });

                return;
            }

            #endregion

            var QuotesCollection = arg as ObservableRangeCollection<Quote>;

            #region Quotes

            if (QuotesCollection != null)
            {
                for (int q = Quotes.Count - 1; q >= 0; q--)
                {
                    Quote qu = Quotes[q];

                    qu.OriginalQuote = qu.OriginalQuote.Trim();
                    qu.TranslatedQuote = qu.TranslatedQuote.Trim();
                    qu.QuoteBegin = qu.QuoteBegin.Trim();
                    qu.QuoteEnd = qu.QuoteEnd.Trim();

                    if (qu.OriginalQuote == "" && qu.TranslatedQuote == "" &&
                        qu.QuoteBegin == "" && qu.QuoteEnd == "")
                    {
                        if (qu.Id != 0)
                        {
                            NotesService.DeleteNote(qu.Id);

                            await MarkdownService.WriteSource(NotesService.ReadSource(qu.SourceID),
                                NotesService.ReadQuotes(qu.SourceID),
                                NotesService.ReadNotes(qu.SourceID),
                                NotesService.ReadSourceExtras(qu.SourceID));
                        }

                        Quotes.RemoveAt(q);
                    }
                }

                Quotes.Add(new Quote()
                {
                    SourceID = Id,
                    QuoteBegin = "",
                    QuoteEnd = "",
                    OriginalQuote = "",
                    TranslatedQuote = ""
                });
            }

            #endregion
        }

    }

}
