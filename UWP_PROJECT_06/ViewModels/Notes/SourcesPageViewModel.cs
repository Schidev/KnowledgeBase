using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views.Notes;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourcesPageViewModel : ViewModelBase
    {
        string searchSourceText; public string SearchSourceText { get => searchSourceText; set => SetProperty(ref searchSourceText, value); }
        string searchUnknownSourceText; public string SearchUnknownSourceText { get => searchUnknownSourceText; set => SetProperty(ref searchUnknownSourceText, value); }
        int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value); }
        int selectedSourceTypeUnknown; public int SelectedSourceTypeUnknown { get => selectedSourceTypeUnknown; set => SetProperty(ref selectedSourceTypeUnknown, value); }

        Source selectedSource; public Source SelectedSource { get => selectedSource; set => SetProperty(ref selectedSource, value); }
        Source selectedUnknownSource; public Source SelectedUnknownSource { get => selectedUnknownSource; set => SetProperty(ref selectedUnknownSource, value); }
        bool isUnknownSourceSelected; public bool IsUnknownSourceSelected { get => isUnknownSourceSelected; set => SetProperty(ref isUnknownSourceSelected, value); }
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


        #region Hotkeys

        public string MenuBackHotkeyName { get => "MenuBack"; }
        public string MenuForwardHotkeyName { get => "MenuForward"; }
        public string MenuMoreHotkeyName { get => "MenuMore"; }

        public string FocusOnPrimarySearchHotkeyName { get => "FocusOnPrimarySearch"; }
        public string FocusOnSecondarySearchHotkeyName { get => "FocusOnSecondarySearch"; }
        public string FocusOnPrimaryLanguagePickerHotkeyName { get => "FocusOnPrimaryLanguagePicker"; }
        public string FocusOnSecondaryLanguagePickerHotkeyName { get => "FocusOnSecondaryLanguagePicker"; }

        public string ChangeModeHotkeyName { get => "ChangeMode"; }
        public string OpenInBrowserHotkeyName { get => "OpenInBrowser"; }
        public string AddNewCardHotkeyName { get => "AddNewCard"; }
        public string SaveCardHotkeyName { get => "SaveCard"; }

        public string CardBackHotkeyName { get => "CardBack"; }
        public string CardForwardHotkeyName { get => "CardForward"; }
        public string CardRefreshHotkeyName { get => "CardRefresh"; }
        public string CardDeleteHotkeyName { get => "CardDelete"; }
        public string CardClearHotkeyName { get => "CardClear"; }

        #endregion


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
            searchSourceText = "";
            searchUnknownSourceText = "";
            SelectedSourceType = 0;
            selectedSourceTypeUnknown = 0;

            SelectedSource = null;
            selectedUnknownSource = null;
            isUnknownSourceSelected = false;
            SelectedUnknownSourceId = 0;

            CurrentBrowserWord = "";

            FrameContent = null;

            IsReadingMode = false;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;

            LastOpenedSourceCard = new SourceCardPage();
            LastOpenedSourceEditCard = new SourceEditPage();
            LastOpenedSourceAddingCard = new SourceEditPage();
            LastWebSearchRequest = new WebView();

            Sources = new ObservableCollection<Grouping<string, Source>>();
            UnknownSources = new ObservableCollection<Grouping<string, UnknownSource>>();
            SearchSourceText = string.Empty;

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

        private async Task LoadSources()
        {
            List<Source> sources = new List<Source>();
            List<int> sourceTypes = new List<int>();

            List<string> sourceTypesDic = NotesService.ReadSourceTypes();
            List<Source> received_sources = NotesService.ReadSources();

            foreach (Source source in received_sources)
            {
                source.SourceName = MarkdownService.CheckSource(source.SourceName);

                if (!MarkdownService.CheckText(source.SourceName).Contains(MarkdownService.CheckText(SearchSourceText))) continue;
                if (selectedSourceType != 0 && sourceTypesDic[selectedSourceType - 1] != NotesService.ReadSourceType(source.SourceType)) continue;

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
        private async Task LoadUnknownSources()
        {
            List<UnknownSource> sources = new List<UnknownSource>();
            List<int> sourceTypes = new List<int>();

            List<string> sourceTypesDic = NotesService.ReadSourceTypes();
            List<UnknownSource> received_sources = HistoryService.ReadUnknownSources();

            foreach (UnknownSource source in received_sources)
            {
                if (!MarkdownService.CheckText(source.Source).Contains(MarkdownService.CheckText(SearchUnknownSourceText))) continue;
                if (SelectedSourceTypeUnknown != 0 && sourceTypesDic[SelectedSourceTypeUnknown - 1] != NotesService.ReadSourceType(source.SourceType)) continue;

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

        private async Task TextChanged(object arg)
        {
            AutoSuggestBox autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox == null)
                return;

            autoSuggestBox.Text = MarkdownService.CheckSource(autoSuggestBox.Text);

            LoadSources();
        }
        private async Task SourceTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            LoadSources();

            if (!IsOnlineDictionaryActive)
                return;

            IsOnlineDictionaryActive = false;
            SearchOnline();
        }
        private async Task UnknownSourceTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
                LoadUnknownSources();

        }
        private async Task UnknownSourceTextChanged(object arg)
        {
            AutoSuggestBox autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox == null)
                return;

            autoSuggestBox.Text = MarkdownService.CheckSource(autoSuggestBox.Text);

            LoadUnknownSources();
        }


        private async Task SourceSelected(object arg)
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
        private async Task UnknownSourceSelected(object arg)
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

        private async Task DeleteUnknownWord()
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

            FrameContent = new MarkdownTextBlock()
            {
                Text = await MarkdownService.ReadWebEmptyWord(),
                Padding = new Thickness(20, 0, 20, 0),
                Background = Application.Current.Resources["colorWhite"] as SolidColorBrush,
                Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }
        private async Task Clear()
        {
            MessageDialog message = new MessageDialog("Would you like to delete source permanently or clear all data?");

            message.Commands.Add(new UICommand { Label = "Delete", Id = 0 });
            message.Commands.Add(new UICommand { Label = "Clear", Id = 1 });
            message.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            var result = await message.ShowAsync();

            if ((int)result.Id == 0)
            {
                SourceEditPageViewModel viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;

                if (viewModel == null)
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

                Source source = NotesService.ReadSource(viewModel.Id);

                if (source == null)
                {
                    message = new MessageDialog("Source doesn't exist.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

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

            if ((int)result.Id == 1)
            {
                SourceEditPageViewModel viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;

                if (viewModel == null)
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

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

            if ((int)result.Id == 2)
                return;
        }
        private async Task Back()
        {
            WebView web = FrameContent as WebView;

            if (web == null)
                return;

            if (web.CanGoBack)
                web.GoBack();
        }
        private async Task Forward()
        {
            WebView web = FrameContent as WebView;

            if (web != null)
                return;

            if (web.CanGoForward)
                web.GoForward();
        }
        private async Task Refresh()
        {
            WebView web = FrameContent as WebView;

            if (web == null)
                return;

            web.Refresh();
        }
        private async Task ChangeMode()
        {
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;
            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

            if (IsReadingMode)
            {
                if (LastOpenedSourceCard.DataContext == null)
                    return;

                SourceCardPageViewModel _view = LastOpenedSourceCard.DataContext as SourceCardPageViewModel;
                Source _source = _view.CurrentSource;

                if (_source == null)
                    return;

                if (IsWritingMode)
                {
                    LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(_source.Id);
                    FrameContent = LastOpenedSourceCard;
                    IsWritingMode = false;

                    return;
                }

                FrameContent = LastOpenedSourceEditCard;
                IsWritingMode = true;

                IsReadingMode = true;

                return;
            }

            if (IsWritingMode)
            {
                if (LastOpenedSourceEditCard.DataContext != null)
                    FrameContent = LastOpenedSourceEditCard;

                IsReadingMode = true;

                return;
            }

            if (LastOpenedSourceCard.DataContext == null)
            {
                FrameContent = new MarkdownTextBlock()
                {
                    Text = await MarkdownService.ReadNoCardsOpen(),
                    Padding = new Thickness(20, 0, 20, 0),
                    Background = Application.Current.Resources["colorWhite"] as SolidColorBrush,
                    Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                IsReadingMode = false;
                IsWritingMode = false;

                return;
            }

            SourceCardPageViewModel view = LastOpenedSourceCard.DataContext as SourceCardPageViewModel;
            Source source = view.CurrentSource;

            if (source != null)
                LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(source.Id);

            FrameContent = LastOpenedSourceCard;
            IsReadingMode = true;
        }
        private async Task SearchOnline()
        {
            IsReadingMode = false;
            IsAddingMode = false;
            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

            if ((SearchUnknownSourceText == String.Empty && SearchSourceText == String.Empty && LastWebSearchRequest.Source == null) || (IsOnlineDictionaryActive && SearchSourceText == String.Empty))
            {
                FrameContent = new MarkdownTextBlock()
                {
                    Text = await MarkdownService.ReadWebEmptyWord(),
                    Padding = new Thickness(20, 0, 20, 0),
                    Background = Application.Current.Resources["colorWhite"] as SolidColorBrush,
                    Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                SelectedUnknownSourceId = 0;
                IsOnlineDictionaryActive = false;

                return;
            }

            if (SearchUnknownSourceText != String.Empty && SearchSourceText == String.Empty)
                SearchSourceText = SearchUnknownSourceText;

            if (SearchSourceText != String.Empty)
            {
                LastWebSearchRequest.Source = new Uri(@"https://www.google.com/search?q=" + SearchSourceText + "+это");
                CurrentBrowserWord = SearchSourceText;
                SearchSourceText = String.Empty;
                SearchUnknownSourceText = String.Empty;
                SelectedUnknownSourceId = 0;
            }

            FrameContent = LastWebSearchRequest;

            IsOnlineDictionaryActive = true;
        }
        private async Task AddWord()
        {
            IsOnlineDictionaryActive = false;
            IsReadingMode = false;
            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

            if (IsAddingMode)
            {
                FrameContent = new MarkdownTextBlock()
                {
                    Text = await MarkdownService.ReadNoCardsOpen(),
                    Padding = new Thickness(20, 0, 20, 0),
                    Background = Application.Current.Resources["colorWhite"] as SolidColorBrush,
                    Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                IsAddingMode = false;
                
                return;   
            }

            bool IsCanceled = true;

            if (LastOpenedSourceAddingCard.DataContext != null)
            {
                MessageDialog message = new MessageDialog("All changes wouln't be save. Would you like to continue?");

                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                var result = await message.ShowAsync();

                IsCanceled = (int)result.Id == 1;
            }

            if (LastOpenedSourceAddingCard.DataContext == null || !IsCanceled)
            {
                var viewModel = new SourceEditPageViewModel();
                LastOpenedSourceAddingCard.DataContext = viewModel;
            }

            FrameContent = LastOpenedSourceAddingCard;

            IsAddingMode = true;
        }
        private async Task Save()
        {
            if (!IsWritingMode && IsReadingMode)
                return;

            MessageDialog message;
            IsUnknownSourceSelected = IsOnlineDictionaryActive && (SelectedUnknownSourceId != 0);

            if (IsOnlineDictionaryActive)
            {
                message = new MessageDialog("Would you like to save this unknown word?");

                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                var result = await message.ShowAsync();

                if ((int)result.Id == 1)
                {
                    message = new MessageDialog("Unknown source wasn't saved.", "Woops...");
                    await message.ShowAsync();
                }

                UnknownSource source = new UnknownSource();

                source.Source = MarkdownService.CheckText(CurrentBrowserWord);
                source.SourceType = SelectedSourceType == 0 ? 1 : SelectedSourceType;
                source.LastModifiedOn = DateTime.Now;

                if (source.Source == String.Empty)
                {
                    message = new MessageDialog("Source must contain at least one character.", "Woops...");
                    await message.ShowAsync();
                }

                HistoryService.CreateUnknownSource(source);
                LoadUnknownSources();

                message = new MessageDialog("Unknown source was correctly saved.", "Congratulations!");
                await message.ShowAsync();

                return;
            }

            if (!IsAddingMode && !IsWritingMode)
                return;

            SourceEditPageViewModel viewModel = new SourceEditPageViewModel();

            viewModel = (IsReadingMode
                ? LastOpenedSourceEditCard.DataContext
                : LastOpenedSourceAddingCard.DataContext) as SourceEditPageViewModel;

            if (viewModel == null)
                return;

            if (viewModel.Source.SourceName == String.Empty)
            {
                message = new MessageDialog("Name must contain at least one character.", "Woops...");
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
                message = new MessageDialog("Link must contain at least one character.", "Woops...");
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

            var SourceTypesDictionary = NotesService.ReadSourceTypes();

            var tempSource = new Source()
            {
                Id = viewModel.Id,
                SourceName = String.Format("{0}_{1}", SourceTypesDictionary[viewModel.Source.SourceType - 1], MarkdownService.CheckSource(viewModel.Source.SourceName).Replace(" ", "_")),
                Duration = viewModel.Source.Duration,
                ActualTime = viewModel.Source.ActualTime,
                State = viewModel.Source.State,
                Theme = viewModel.Source.Theme,
                SourceType = (byte)NotesService.ReadSourceType(SourceTypesDictionary[viewModel.Source.SourceType - 1]),
                IsDownloaded = viewModel.IsDownloaded,
                Description = viewModel.Source.Description,
                SourceLink = viewModel.Source.SourceLink,
                CreatedOn = viewModel.Source.CreatedOn,
                LastModifiedOn = DateTime.UtcNow
            };

            
            if (IsAddingMode)
            {
                tempSource.CreatedOn = DateTime.UtcNow;
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

            foreach (Quote quote in viewModel.Quotes)
            {
                quote.QuoteBegin = quote.QuoteBegin.Trim();
                quote.QuoteEnd = quote.QuoteEnd.Trim();
                quote.OriginalQuote = quote.OriginalQuote.Trim();
                quote.TranslatedQuote = quote.TranslatedQuote.Trim();

                if (quote.QuoteBegin == String.Empty && quote.QuoteEnd == String.Empty && quote.OriginalQuote == String.Empty && quote.TranslatedQuote == String.Empty)
                    continue;

                if (quote.QuoteBegin == String.Empty && quote.QuoteEnd != String.Empty && quote.OriginalQuote != String.Empty && quote.TranslatedQuote != String.Empty)
                {
                    message = new MessageDialog("Quote begin must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (quote.QuoteBegin != String.Empty && quote.QuoteEnd == String.Empty && quote.OriginalQuote != String.Empty && quote.TranslatedQuote != String.Empty)
                {
                    message = new MessageDialog("Quote end must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (quote.QuoteBegin != String.Empty && quote.QuoteEnd != String.Empty && quote.OriginalQuote == String.Empty && quote.TranslatedQuote != String.Empty)
                {
                    message = new MessageDialog("Quote must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (quote.QuoteBegin != String.Empty && quote.QuoteEnd != String.Empty && quote.OriginalQuote != String.Empty && quote.TranslatedQuote == String.Empty)
                {
                    message = new MessageDialog("Translation must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }

                quote.QuoteBegin = MarkdownService.CheckQuoteStamp(quote.QuoteBegin);
                quote.QuoteEnd = MarkdownService.CheckQuoteStamp(quote.QuoteEnd);
                quote.OriginalQuote = quote.OriginalQuote.Replace("<br>", "\\r\\r");
                quote.TranslatedQuote = quote.TranslatedQuote.Replace("<br>", "\\r\\r");
                quote.SourceID = tempSource.Id;

                quote.LastModifiedOn = DateTime.UtcNow;

                if (!IsReadingMode || quote.Id == 0)
                {
                    quote.CreatedOn = DateTime.UtcNow;
                    NotesService.CreateQuote(quote);
                }
                else
                    NotesService.UpdateQuote(quote);

                quotesList.Add(quote);
            }

            #endregion
            #region Notes

            var notesList = new List<Note>();


            foreach (Note note in viewModel.Notes)
            {
                note.Stamp = note.Stamp.Trim();
                note.Title = note.Title.Trim();
                note.Note1 = note.Note1.Trim();

                if (note.Stamp == String.Empty && note.Title == String.Empty && note.Note1 == String.Empty)
                    continue;
                if (note.Stamp == String.Empty && note.Title != String.Empty && note.Note1 != String.Empty)
                {
                    message = new MessageDialog("Note stamp must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (note.Stamp != String.Empty && note.Title == String.Empty && note.Note1 != String.Empty)
                {
                    message = new MessageDialog("Note title must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (note.Stamp != String.Empty && note.Title != String.Empty && note.Note1 == String.Empty)
                {
                    message = new MessageDialog("Note content must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }

                note.Stamp = MarkdownService.CheckNoteStamp(note.Stamp);
                note.SourceID = tempSource.Id;

                
                note.LastModifiedOn = DateTime.UtcNow;

                if (!IsReadingMode || note.Id == 0)
                {
                    note.CreatedOn = DateTime.UtcNow;
                    NotesService.CreateNote(note);
                }
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
                
                if (extra.Key == String.Empty && extra.Value == String.Empty)
                    continue;
                
                if (extra.Key == String.Empty && extra.Value != String.Empty)
                {
                    message = new MessageDialog("Extra key must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }
                if (extra.Key != String.Empty && extra.Value == String.Empty)
                {
                    message = new MessageDialog("Extra value must contain at least one character.", "Woops..");
                    await message.ShowAsync();

                    return;
                }

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

        }
    }
}
