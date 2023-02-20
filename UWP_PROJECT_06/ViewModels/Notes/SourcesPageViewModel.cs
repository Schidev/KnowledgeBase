using ColorCode;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using UWP_PROJECT_06.Views.Notes;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static SQLite.SQLite3;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourcesPageViewModel : ViewModelBase
    {
        string searchSourceText; public string SearchSourceText { get => searchSourceText; set => SetProperty(ref searchSourceText, value); }
        string searchUnknownSourceText; public string SearchUnknownSourceText { get => searchUnknownSourceText; set => SetProperty(ref searchUnknownSourceText, value); }
        int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value);}
        int selectedSourceTypeUnknown; public int SelectedSourceTypeUnknown { get => selectedSourceTypeUnknown; set => SetProperty(ref selectedSourceTypeUnknown, value);}
        Source selectedSource; public Source SelectedSource { get => selectedSource; set => SetProperty(ref selectedSource, value); }
        Source selectedUnknownSource; public Source SelectedUnknownSource { get => selectedUnknownSource; set => SetProperty(ref selectedUnknownSource, value); }
        bool isUnknownSourceSelected; public bool IsUnknownSourceSelected { get => isUnknownSourceSelected; set => SetProperty(ref isUnknownSourceSelected, value);}
        int selectedUnknownSourceId; public int SelectedUnknownSourceId { get => selectedUnknownSourceId; set => SetProperty(ref selectedUnknownSourceId, value); }

        string CurrentBrowserWord { get; set; }


        object frameContent; public object FrameContent { get => frameContent; set => SetProperty(ref frameContent, value); }
        bool isOnlineDictionaryActive; public bool IsOnlineDictionaryActive { get => isOnlineDictionaryActive; set => SetProperty(ref isOnlineDictionaryActive, value); }
        bool isReadingMode; public bool IsReadingMode { get => isReadingMode; set => SetProperty(ref isReadingMode, value); }
        bool isWritingMode; public bool IsWritingMode { get => isWritingMode; set => SetProperty(ref isWritingMode, value); }
        bool isAddingMode; public bool IsAddingMode { get => isAddingMode; set => SetProperty(ref isAddingMode, value); }


        SourceEditPage LastOpenedSourceEditCard { get; set; }
        SourceEditPage LastOpenedSourceAddingCard { get; set; }
        SourceCardPage LastOpenedSourceCard { get; set; }
        WebView LastWebSearchRequest { get; set; }


        public ObservableCollection<Grouping<string, Source>> Sources { get; set; }
        public ObservableCollection<Grouping<string, UnknownSource>> UnknownSources { get; set; }
        public ObservableRangeCollection<string> SourceTypes { get; set; }

        
        public AsyncCommand DeleteUnknownWordCommand { get; }
        public AsyncCommand ClearCommand { get; }
        public AsyncCommand BackCommand { get; }
        public AsyncCommand ForwardCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SaveCommand { get; }
        public AsyncCommand<object> TextChangedCommand { get; }
        public AsyncCommand<object> UnknownSourceTextChangedCommand { get; }
        public AsyncCommand<object> SourceTypeSelectedCommand { get; }
        public AsyncCommand<object> UnknownSourceTypeSelectedCommand { get; }


        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand AddWordCommand { get; }


        public AsyncCommand<object> SourceSelectedCommand { get; }
        public AsyncCommand<object> UnknownSourceSelectedCommand { get; }

        public SourcesPageViewModel()
        {
            LastOpenedSourceCard = new SourceCardPage();
            LastOpenedSourceEditCard = new SourceEditPage();
            LastOpenedSourceAddingCard = new SourceEditPage();
            LastWebSearchRequest = new WebView();

            Sources = new ObservableCollection<Grouping<string, Source>>();
            UnknownSources = new ObservableCollection<Grouping<string, UnknownSource>>();
            SearchSourceText = string.Empty;

            FrameContent = null;

            CurrentBrowserWord = "";
            searchSourceText = "";
            searchUnknownSourceText = "";
            
            SelectedSourceType = 0;
            
            IsReadingMode = false;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;
            isUnknownSourceSelected = false;

            SelectedSource = null;
            SelectedUnknownSourceId = 0;
            

            ChangeMode();

            SourceTypes = new ObservableRangeCollection<string>() { "All" };
            foreach (string sourceType in NotesService.ReadSourceTypes())
                SourceTypes.Add(sourceType);


            LoadSources();
            LoadUnknownSources();

            DeleteUnknownWordCommand = new AsyncCommand(DeleteUnknownWord);
            ClearCommand = new AsyncCommand(Clear);
            BackCommand = new AsyncCommand(Back);
            ForwardCommand = new AsyncCommand(Forward);
            RefreshCommand = new AsyncCommand(Refresh);
            SaveCommand = new AsyncCommand(Save);
            TextChangedCommand = new AsyncCommand<object>(TextChanged);
            UnknownSourceTextChangedCommand = new AsyncCommand<object>(UnknownSourceTextChanged);
            SourceTypeSelectedCommand = new AsyncCommand<object>(SourceTypeSelected);
            UnknownSourceTypeSelectedCommand = new AsyncCommand<object>(UnknownSourceTypeSelected);

            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            AddWordCommand = new AsyncCommand(AddWord);

            SourceSelectedCommand = new AsyncCommand<object>(SourceSelected);
            UnknownSourceSelectedCommand = new AsyncCommand<object>(UnknownSourceSelected);
        }

        async Task LoadSources()
        {
            List<Source> sources = new List<Source>();
            List<int> sourceTypes = new List<int>();

            List<Source> received_sources = NotesService.ReadSources();

            foreach (Source source in received_sources)
            {
                source.SourceName = source.SourceName.Replace("VIDEO_", "").Replace("SOUND_", "").Replace("IMAGE_", "").Replace("DOCUMENT_", "").Replace("_", " ");

                if (!MarkdownService.CheckText(source.SourceName).StartsWith(MarkdownService.CheckText(SearchSourceText))) continue;
                if (selectedSourceType != 0 && source.SourceType != selectedSourceType) continue;


                sources.Add(source);

                if (!sourceTypes.Contains(source.SourceType))
                    sourceTypes.Add(source.SourceType);
            }

            Sources.Clear();

            foreach (int sourceType in sourceTypes)
            {
                string source_type = NotesService.ReadSourceType(sourceType);

                Sources.Add(new Grouping<string, Source>(
                    source_type,
                    sources.Where(e => e.SourceType == sourceType)));
            }
        }
        async Task LoadUnknownSources()
        {
            List<UnknownSource> sources = new List<UnknownSource>();
            List<int> sourceTypes = new List<int>();

            List<UnknownSource> received_sources = HistoryService.ReadUnknownSources();

            foreach (UnknownSource source in received_sources)
            {
                if (!MarkdownService.CheckText(source.Source).StartsWith(MarkdownService.CheckText(SearchUnknownSourceText))) continue;
                if (SelectedSourceTypeUnknown != 0 && source.SourceType != SelectedSourceTypeUnknown) continue;

                sources.Add(source);

                if (!sourceTypes.Contains(source.SourceType))
                    sourceTypes.Add(source.SourceType);
            }

            UnknownSources.Clear();

            foreach (int sourceTypeId in sourceTypes)
            {
                string sourceType = NotesService.ReadSourceType(sourceTypeId);

                UnknownSources.Add(new Grouping<string, UnknownSource>(
                    sourceType,
                    sources.Where(e => e.SourceType == sourceTypeId)));
            }

        }
        
        async Task UnknownSourceTextChanged(object arg)
        {
            var autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
            {
                autoSuggestBox.Text = autoSuggestBox.Text.Replace("VIDEO_", "").Replace("SOUND_", "").Replace("IMAGE_", "").Replace("DOCUMENT_", "").Replace("_", " ");

                LoadUnknownSources();
            }
        }
        async Task TextChanged(object arg)
        {
            var autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
            {
                autoSuggestBox.Text = autoSuggestBox.Text.Replace("VIDEO_", "").Replace("SOUND_", "").Replace("IMAGE_", "").Replace("DOCUMENT_", "").Replace("_", " ");

                LoadSources();
            }
        }
        
        async Task UnknownSourceTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
                LoadUnknownSources();
            
        }
        async Task SourceTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
            {
                LoadSources();
                if (IsOnlineDictionaryActive)
                {
                    IsOnlineDictionaryActive = false;
                    SearchOnline();
                }

            }
        }

        async Task Save()
        {
            MessageDialog message;

            if (!IsWritingMode && IsReadingMode)
                return;

            if (IsAddingMode || (IsReadingMode && IsWritingMode))
            {
                SourceEditPageViewModel viewModel = new SourceEditPageViewModel();

                if (IsReadingMode)
                {
                    viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;
                }
                else
                {
                    viewModel = LastOpenedSourceAddingCard.DataContext as SourceEditPageViewModel;
                }

                if (viewModel == null)
                    return;
                
                if (viewModel.Source.SourceName == String.Empty)
                {
                    message = new MessageDialog("Name must contains at least one character.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.Source.State == 0)
                {
                    message = new MessageDialog("You need to choose state from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.Source.Theme == 0)
                {
                    message = new MessageDialog("You need to choose theme from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.Source.SourceType == 0)
                {
                    message = new MessageDialog("You need to choose source type from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.Source.SourceLink == "")
                {
                    message = new MessageDialog("Link must contains at least one character.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                Regex reg = new Regex(@"[0-9]+");

                if (!reg.IsMatch(viewModel.Source.Duration.ToString()))
                {
                    message = new MessageDialog("Duration is not valid.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (!reg.IsMatch(viewModel.Source.Duration.ToString()))
                {
                    message = new MessageDialog("Duration is not valid.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                var SourceTypesDictionary = new Dictionary<int, string>()
                    {
                        { 1, "VIDEO" },
                        { 2, "SOUND" },
                        { 3, "IMAGE" },
                        { 4, "DOCUMENT" }
                    };

                var tempSource = new Source()
                {
                    Id = viewModel.Id,
                    SourceName = viewModel.Source.SourceName.Contains("VIDEO_") || viewModel.Source.SourceName.Contains("SOUND_") || viewModel.Source.SourceName.Contains("IMAGE_") || viewModel.Source.SourceName.Contains("DOCUMENT_")
                                ? String.Format("{0}_{1}", SourceTypesDictionary[viewModel.Source.SourceType], viewModel.Source.SourceName.Replace(" ", "_").Replace("VIDEO_", "").Replace("SOUND_", "").Replace("IMAGE_", "").Replace("DOCUMENT_", ""))
                                : String.Format("{0}_{1}", SourceTypesDictionary[viewModel.Source.SourceType], viewModel.Source.SourceName.Replace(" ", "_")),
                    Duration = viewModel.Source.Duration,
                    ActualTime = viewModel.Source.ActualTime,
                    State = viewModel.Source.State,
                    Theme = viewModel.Source.Theme,
                    SourceType = viewModel.Source.SourceType,
                    IsDownloaded = viewModel.IsDownloaded,
                    Description = viewModel.Source.Description,
                    SourceLink = viewModel.Source.SourceLink
                };

                if (IsAddingMode)
                {
                    NotesService.CreateSource(tempSource);
                }
                else
                {
                    Source oldSource = NotesService.ReadSource(tempSource.Id);

                    if (oldSource.SourceName != tempSource.SourceName || oldSource.SourceType != tempSource.SourceType)
                        await MarkdownService.RenameFile(oldSource, tempSource);

                    NotesService.UpdateSource(tempSource);
                }

                tempSource = NotesService.ReadSource(tempSource.SourceName);

                #region Quotes

                var quotesList = new List<Quote>();

                Regex quoteString1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)");
                Regex quoteString2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
                Regex quoteString3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

                Regex quoteString4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)");
                Regex quoteString5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");


                foreach (Quote quote in viewModel.Quotes)
                {
                    quote.QuoteBegin = quote.QuoteBegin.Trim();
                    quote.QuoteEnd = quote.QuoteEnd.Trim();
                    quote.OriginalQuote = quote.OriginalQuote.Trim();
                    quote.TranslatedQuote = quote.TranslatedQuote.Trim();

                    if (quote.QuoteBegin == String.Empty || quote.QuoteEnd == String.Empty || quote.OriginalQuote == String.Empty || quote.TranslatedQuote == String.Empty)
                        continue;

                    #region Quote begin

                    if (quoteString4.IsMatch(quote.QuoteBegin) || quoteString5.IsMatch(quote.QuoteBegin))
                    {
                        var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                        quote.QuoteBegin = String.Format("page {0} line {1}", temp[0], temp[1]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteBegin) && !quoteString2.IsMatch(quote.QuoteBegin) && !quoteString3.IsMatch(quote.QuoteBegin))
                    {
                        var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteBegin = String.Format("00:00:{0}", temp[0]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteBegin) && quoteString2.IsMatch(quote.QuoteBegin) && !quoteString3.IsMatch(quote.QuoteBegin))
                    {
                        var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteBegin = String.Format("00:{0}:{1}", temp[0], temp[1]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteBegin) && quoteString2.IsMatch(quote.QuoteBegin) && quoteString3.IsMatch(quote.QuoteBegin))
                    {
                        var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteBegin = String.Format("{0}:{1}:{2}", temp[0], temp[1], temp[2]);
                    }

                    #endregion
                    #region Quote end

                    if (quoteString4.IsMatch(quote.QuoteEnd) || quoteString5.IsMatch(quote.QuoteEnd))
                    {
                        var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                        quote.QuoteEnd = String.Format("page {0} line {1}", temp[0], temp[1]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteEnd) && !quoteString2.IsMatch(quote.QuoteEnd) && !quoteString3.IsMatch(quote.QuoteEnd))
                    {
                        var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteEnd = String.Format("00:00:{0}", temp[0]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteEnd) && quoteString2.IsMatch(quote.QuoteEnd) && !quoteString3.IsMatch(quote.QuoteEnd))
                    {
                        var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteEnd = String.Format("00:{0}:{1}", temp[0], temp[1]);
                    }
                    else if (quoteString1.IsMatch(quote.QuoteEnd) && quoteString2.IsMatch(quote.QuoteEnd) && quoteString3.IsMatch(quote.QuoteEnd))
                    {
                        var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                        quote.QuoteEnd = String.Format("{0}:{1}:{2}", temp[0], temp[1], temp[2]);
                    }

                    #endregion
                    #region Original quote

                    quote.OriginalQuote = quote.OriginalQuote.Replace("<br>", "\\r\\r");

                    #endregion
                    #region Original quote

                    quote.TranslatedQuote = quote.TranslatedQuote.Replace("<br>", "\\r\\r");

                    #endregion

                    quote.SourceID = tempSource.Id;

                    if (!IsReadingMode || quote.Id == 0)
                        NotesService.CreateQuote(quote);
                    else
                        NotesService.UpdateQuote(quote);

                    quotesList.Add(quote);
                }

                #endregion
                #region Notes

                var notesList = new List<Note>();

                Regex noteRegex1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)");
                Regex noteRegex2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
                Regex noteRegex3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

                Regex noteRegex4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)([0-9]+)([\s]*)([lL]*)([\s]+)([0-9]+)([\s]*)");
                Regex noteRegex5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");


                foreach (Note note in viewModel.Notes)
                {
                    note.Stamp = note.Stamp.Trim();
                    note.Title = note.Title.Trim();
                    note.Note1 = note.Note1.Trim();

                    if (note.Stamp == String.Empty || note.Title == String.Empty || note.Note1 == String.Empty)
                        continue;

                    if (noteRegex4.IsMatch(note.Stamp) || noteRegex5.IsMatch(note.Stamp))
                    {
                        var temp = Regex.Replace(note.Stamp, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                        note.Stamp = String.Format("page {0} line {1} - page {2} line {3}", temp[0], temp[1], temp[2], temp[3]);
                    }
                    else if (noteRegex1.IsMatch(note.Stamp) && !noteRegex2.IsMatch(note.Stamp) && !noteRegex3.IsMatch(note.Stamp))
                    {
                        var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                        note.Stamp = String.Format("00:00:{0}-00:00:{1}", temp[0], temp[1]);
                    }
                    else if (noteRegex1.IsMatch(note.Stamp) && noteRegex2.IsMatch(note.Stamp) && !noteRegex3.IsMatch(note.Stamp))
                    {
                        var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                        note.Stamp = String.Format("00:{0}:{1}-00:{2}:{3}", temp[0], temp[1], temp[2], temp[3]);
                    }
                    else if (noteRegex1.IsMatch(note.Stamp) && noteRegex2.IsMatch(note.Stamp) && noteRegex3.IsMatch(note.Stamp))
                    {
                        var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                        note.Stamp = String.Format("{0}:{1}:{2}-{3}:{4}:{5}", temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                    }

                    note.SourceID = tempSource.Id;

                    if (!IsReadingMode || note.Id == 0)
                        NotesService.CreateNote(note);
                    else
                        NotesService.UpdateNote(note);

                    notesList.Add(note);
                }

                #endregion
                #region Extras

                var extrasList = new List<SourceExtra>();

                foreach (SourceExtra extra in viewModel.Extras)
                {
                    extra.Key = extra.Key.Trim();
                    extra.Value = extra.Value.Trim();

                    if (extra.Key == String.Empty || extra.Value == String.Empty)
                        continue;

                    extra.SourceID = tempSource.Id;

                    if (!IsReadingMode || extra.Id == 0)
                        NotesService.CreateSourceExtra(extra);
                    else
                        NotesService.UpdateSourceExtra(extra);
                    
                    extrasList.Add(extra);
                }

                #endregion

                LoadSources();

                await MarkdownService.WriteSource(tempSource, quotesList, notesList, extrasList);

                if (IsAddingMode)
                {
                    LastOpenedSourceAddingCard.DataContext = null;
                }

                SearchSourceText = tempSource.SourceName;
                SelectedSource = null;

                LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(tempSource.Id);
                LastOpenedSourceEditCard.DataContext = new SourceEditPageViewModel(tempSource.Id);
                
                FrameContent = LastOpenedSourceCard;

                IsReadingMode = true;
                IsWritingMode = false;
                IsOnlineDictionaryActive = false;
                IsAddingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

                message = new MessageDialog("Card was correctly saved.", "Congratulations!");
                await message.ShowAsync();
                return;
            }

            if (IsOnlineDictionaryActive)
            {
                message = new MessageDialog("Would you like to save this unknown word?");

                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                var result = await message.ShowAsync();

                if ((int)result.Id == 0)
                {
                    UnknownSource source = new UnknownSource();

                    source.Source = MarkdownService.CheckText(CurrentBrowserWord);
                    source.SourceType = SelectedSourceType == 0 ? 1 : SelectedSourceType;
                    source.LastModifiedOn = DateTime.Now;

                    if (source.Source != String.Empty)
                    {
                        HistoryService.CreateUnknownSource(source);
                        LoadUnknownSources();

                        message = new MessageDialog("Unknown source was correctly saved.", "Congratulations!");
                        await message.ShowAsync();
                    }
                    else
                    {
                        message = new MessageDialog("Source must contains at least one character.", "Woops...");
                        await message.ShowAsync();
                    }
                }
                else
                {
                    message = new MessageDialog("Unknown source wasn't saved.", "Woops...");
                    await message.ShowAsync();
                }
            }

            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

        }
        async Task Back()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent != null)
            {
                if (frameContent.CanGoBack)
                    frameContent.GoBack();

                return;
            }
        }
        async Task Forward()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent != null)
            {
                if (frameContent.CanGoForward)
                    frameContent.GoForward();

                return;
            }
        }
        async Task Refresh()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent != null)
                frameContent.Refresh();

            return;
        }

        async Task Clear() 
        {
            MessageDialog message = new MessageDialog("Would you like to delete source permanently or clear all data?");

            message.Commands.Add(new UICommand { Label = "Delete", Id = 0 });
            message.Commands.Add(new UICommand { Label = "Clear", Id = 1 });
            message.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            var result = await message.ShowAsync();

            bool ForDeleting = (int)result.Id == 0;

            if ((int)result.Id == 2)
                return;

            if ((int)result.Id == 0)
            {
                var viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;

                if (viewModel != null)
                {
                    Source source = NotesService.ReadSource(viewModel.Id);

                    if (source != null)
                    {
                        await MarkdownService.DeleteFile(source);
                        NotesService.DeleteSource(source.Id);

                        if (IsAddingMode)
                            LastOpenedSourceAddingCard.DataContext = null;

                        LastOpenedSourceCard.DataContext = null;
                        LastOpenedSourceEditCard.DataContext = null;

                        IsReadingMode = false;
                        IsWritingMode = false;
                        IsOnlineDictionaryActive = false;
                        IsAddingMode = false;
                        IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

                        ChangeMode();
                        LoadSources();

                        message = new MessageDialog("Source was permanentely deleted.", "Congratulations!");
                        await message.ShowAsync();
                        return;
                    }
                    else
                    {
                        message = new MessageDialog("Source doesn't exist.", "Woops...");
                        await message.ShowAsync();
                        return;
                    }
                }
                else
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }
            }
            else
            {
                var viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;

                if (viewModel != null)
                {
                    viewModel.Source.SourceName = "";
                    viewModel.SelectedState = 0;
                    viewModel.SelectedTheme = 0;
                    viewModel.SelectedSourceType = 0;
                    viewModel.IsDownloaded = false;

                    viewModel.Source.Duration = 0;
                    viewModel.Source.ActualTime = 0;
                    viewModel.Source.Description = "";
                    viewModel.Source.SourceLink = "";

                    viewModel.Quotes.Clear();
                    viewModel.Notes.Clear();
                    viewModel.Extras.Clear();

                    message = new MessageDialog("Your edit page is now blank.", "Congratulations!");
                    await message.ShowAsync();
                    return;
                }
                else
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }
            }
        }
        async Task ChangeMode()
        {
            MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();

            if (IsReadingMode)
            {
                if (LastOpenedSourceCard.DataContext != null)
                {
                    var view = LastOpenedSourceCard.DataContext as SourceCardPageViewModel;
                    var word = view.CurrentSource;

                    if (word != null)
                    {
                        if (IsWritingMode)
                        {
                            LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(word.Id);
                            FrameContent = LastOpenedSourceCard;
                            IsWritingMode = false;
                        }
                        else
                        {
                            FrameContent = LastOpenedSourceEditCard;
                            IsWritingMode = true;
                        }

                        IsReadingMode = true;
                    }
                }
            }
            else
            {
                if (IsWritingMode)
                {
                    if (LastOpenedSourceEditCard.DataContext != null)
                        FrameContent = LastOpenedSourceEditCard;

                    IsReadingMode = true;
                }
                else
                {
                    if (LastOpenedSourceCard.DataContext == null)
                    {
                        FrameContent = markdownTextBlock;
                        IsReadingMode = false;
                        IsWritingMode = false;
                    }
                    else
                    {
                        var view = LastOpenedSourceCard.DataContext as SourceCardPageViewModel;
                        var word = view.CurrentSource;

                        if (word != null)
                            LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(word.Id);

                        FrameContent = LastOpenedSourceCard;
                        IsReadingMode = true;
                    }
                }
            }

            markdownTextBlock.Text = await MarkdownService.ReadNoCardsOpen();
            markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
            markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
            markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
            markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
            markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

            IsOnlineDictionaryActive = false;
            IsAddingMode = false;
            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
        }
        async Task SearchOnline()
        {
            if (SearchSourceText == String.Empty && LastWebSearchRequest.Source == null)
            {
                MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
                FrameContent = markdownTextBlock;

                markdownTextBlock.Text = await MarkdownService.ReadWebEmptyWord();

                markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
                markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
                markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
                markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
                markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                SelectedUnknownSourceId = 0;

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

                return;
            }

            if (!IsOnlineDictionaryActive || SearchSourceText != String.Empty)
            {
                if (SearchSourceText != String.Empty)
                {
                    LastWebSearchRequest.Source = new Uri(@"https://www.google.com/search?q=" + SearchSourceText + "+это");
                    CurrentBrowserWord = SearchSourceText;
                    SearchSourceText = "";
                    SelectedUnknownSourceId = 0;
                }

                FrameContent = LastWebSearchRequest;

                IsOnlineDictionaryActive = true;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            }
            else
            {
                MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
                FrameContent = markdownTextBlock;

                markdownTextBlock.Text = await MarkdownService.ReadNoCardsOpen();
                markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
                markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
                markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
                markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
                markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                SelectedUnknownSourceId = 0;

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            }
        }
        async Task AddWord()
        {
            if (!IsAddingMode)
            {
                bool change = false;

                if (LastOpenedSourceAddingCard.DataContext != null)
                {
                    MessageDialog message = new MessageDialog("All changes wouln't be save. Would you like to continue?");

                    message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                    message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                    var result = await message.ShowAsync();

                    change = (int)result.Id == 0;
                }

                if (LastOpenedSourceAddingCard.DataContext == null || change)
                {
                    var viewModel = new SourceEditPageViewModel();
                    LastOpenedSourceAddingCard.DataContext = viewModel;
                }

                FrameContent = LastOpenedSourceAddingCard;

                IsAddingMode = true;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            }
            else
            {
                MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
                FrameContent = markdownTextBlock;

                markdownTextBlock.Text = await MarkdownService.ReadNoCardsOpen();
                markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
                markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
                markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
                markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
                markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                IsAddingMode = false;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            }
        }


        async Task SourceSelected(object arg)
        {
            ListView sourcesList = arg as ListView;

            if (sourcesList.SelectedItem != null)
            {
                LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(SelectedSource.Id);
                LastOpenedSourceEditCard.DataContext = new SourceEditPageViewModel(SelectedSource.Id);

                SelectedSource = null;

                FrameContent = LastOpenedSourceCard;

                IsReadingMode = true;
                IsWritingMode = false;
                IsOnlineDictionaryActive = false;
                IsAddingMode = false;
            }
        }
        async Task UnknownSourceSelected(object arg)
        {
            ListView sourcesList = arg as ListView;

            if (sourcesList.SelectedItem != null)
            {
                var unknownSource = sourcesList.SelectedItem as UnknownSource;

                CurrentBrowserWord = unknownSource.Source;
                SelectedUnknownSourceId = unknownSource.SourceType;

                LastWebSearchRequest.Source = new Uri(@"https://www.google.com/search?q=" + CurrentBrowserWord + "+это");

                FrameContent = LastWebSearchRequest;
                SelectedUnknownSourceId = unknownSource.Id;
                SelectedUnknownSource = null;

                IsReadingMode = false;
                IsAddingMode = false;
                IsWritingMode = false;
                IsOnlineDictionaryActive = true;
                IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            }
        }


      



        async Task DeleteUnknownWord() 
        {
            MessageDialog message = new MessageDialog("Are you sure you want to delete this unknown source permanentely?", "Deleting");

            message.Commands.Add(new UICommand { Label = "Yeah, delete it", Id = 0 });
            message.Commands.Add(new UICommand { Label = "No, cancel", Id = 1 });

            var result = await message.ShowAsync();

            if ((int)result.Id == 1)
                return;

            HistoryService.DeleteUnknownSource(SelectedUnknownSourceId);
            SelectedUnknownSourceId = 0;
            IsOnlineDictionaryActive = false;
            LastWebSearchRequest.DataContext = null;

            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);
            LoadUnknownSources();

            MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
            FrameContent = markdownTextBlock;

            markdownTextBlock.Text = await MarkdownService.ReadWebEmptyWord();

            markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
            markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
            markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
            markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
            markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
        }


    }
}
