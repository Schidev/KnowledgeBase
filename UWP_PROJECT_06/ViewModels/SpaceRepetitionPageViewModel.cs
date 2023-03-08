using ColorCode;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.ViewModels.Notes;
using UWP_PROJECT_06.Views;
using UWP_PROJECT_06.Views.Notes;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP_PROJECT_06.ViewModels
{
    public class SpaceRepetitionPageViewModel : ViewModelBase
    {
        private bool isDictionarySelected; public bool IsDictionarySelected { get => isDictionarySelected; set => SetProperty(ref isDictionarySelected, value); }
        private bool isSourcesSelected; public bool IsSourcesSelected { get => isSourcesSelected; set => SetProperty(ref isSourcesSelected, value); }
        private bool isNotesSelected; public bool IsNotesSelected { get => isNotesSelected; set => SetProperty(ref isNotesSelected, value); }
        private bool isQuotesSelected; public bool IsQuotesSelected { get => isQuotesSelected; set => SetProperty(ref isQuotesSelected, value); }

        private string searchText; public string SearchText { get => searchText; set => SetProperty(ref searchText, value); }
        private int selectedLanguage; public int SelectedLanguage { get => selectedLanguage; set => SetProperty(ref selectedLanguage, value); }
        private int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value); }
        private int selectedNoteSource; public int SelectedNoteSource { get => selectedNoteSource; set => SetProperty(ref selectedNoteSource, value); }
        private int selectedQuoteSource; public int SelectedQuoteSource { get => selectedQuoteSource; set => SetProperty(ref selectedQuoteSource, value); }
        private int selectedCardType; public int SelectedCardType { get => selectedCardType; set => SetProperty(ref selectedCardType, value); }
        private DateTimeOffset dateBegin; public DateTimeOffset DateBegin { get => dateBegin; set => SetProperty(ref dateBegin, value); }
        private DateTimeOffset dateEnd; public DateTimeOffset DateEnd { get => dateEnd; set => SetProperty(ref dateEnd, value); }

        private object frameContent; public object FrameContent { get => frameContent; set => SetProperty(ref frameContent, value); }
        private bool isReadingMode; public bool IsReadingMode { get => isReadingMode; set => SetProperty(ref isReadingMode, value); }
        private bool isWritingMode; public bool IsWritingMode { get => isWritingMode; set => SetProperty(ref isWritingMode, value); }
        private string selectedCard; public string SelectedCard { get => selectedCard; set => SetProperty(ref selectedCard, value); }


        #region Hotkeys

        public string MenuBackHotkeyName { get => "MenuBack"; }
        public string MenuForwardHotkeyName { get => "MenuForward"; }
        public string MenuMoreHotkeyName { get => "MenuMore"; }

        public string ChangeModeHotkeyName { get => "ChangeMode"; }
        public string SaveCardHotkeyName { get => "SaveCard"; }

        public string StartRepeatingHotkeyName { get => "StartRepeating"; }
        public string RepeatAfterOneDayHotkeyName { get => "RepeatAfterOneDay"; }
        public string RepeatAfterThreeDayHotkeyName { get => "RepeatAfterThreeDay"; }
        public string RepeatAfterSevenDayHotkeyName { get => "RepeatAfterSevenDay"; }
        public string RepeatAfterThirdteenDayHotkeyName { get => "RepeatAfterThirdteenDay"; }
        public string RepeatAfterTwentyNineDayHotkeyName { get => "RepeatAfterTwentyNineDay"; }
        public string RepeatAfterFiftyNineDayHotkeyName { get => "RepeatAfterFiftyNineDay"; }
        public string RepeatAfterSixtyOneDayHotkeyName { get => "RepeatAfterSixtyOneDay"; }

        #endregion


        private WordEditPage LastOpenedWordEditCard { get; set; }
        private WordCardPage LastOpenedWordCard { get; set; }
        private SourceEditPage LastOpenedSourceEditCard { get; set; }
        private SourceCardPage LastOpenedSourceCard { get; set; }
        private NoteEditPage LastOpenedNoteEditCard { get; set; }
        private QuoteEditPage LastOpenedQuoteEditCard { get; set; }

        public ObservableRangeCollection<Grouping<string, Word>> Words { get; private set; }
        public ObservableRangeCollection<Grouping<string, Source>> Sources { get; private set; }
        public ObservableRangeCollection<Grouping<string, Note>> Notes { get; private set; }
        public ObservableRangeCollection<Grouping<string, Quote>> Quotes { get; private set; }

        public ObservableRangeCollection<string> Languages { get; private set; }
        public ObservableRangeCollection<string> SourceTypes { get; private set; }
        public ObservableRangeCollection<string> NotesSourcesNames { get; private set; }
        public ObservableRangeCollection<string> QuotesSourcesNames { get; private set; }
        public ObservableRangeCollection<string> CardTypes { get; private set; }

        public AsyncCommand<object> WordSelectedCommand { get; }
        public AsyncCommand<object> SourceSelectedCommand { get; }
        public AsyncCommand<object> NoteSelectedCommand { get; }
        public AsyncCommand<object> QuoteSelectedCommand { get; }

        public AsyncCommand ReloadCommand { get; }
        public AsyncCommand<object> SearchTextChangedCommand { get; }
        public AsyncCommand<object> LanguageSelectedCommand { get; }
        public AsyncCommand<object> SourceTypeSelectedCommand { get; }
        public AsyncCommand<object> NoteSourceNameSelectedCommand { get; }
        public AsyncCommand<object> QuoteSourceNameSelectedCommand { get; }
        public AsyncCommand<object> CardTypeSelectedCommand { get; }
        
        
        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SaveCommand { get; }
        public AsyncCommand<object> OpenNextCommand { get; }

        public SpaceRepetitionPageViewModel()
        {
            IsDictionarySelected = false;
            IsSourcesSelected = false;
            IsNotesSelected = false;
            IsQuotesSelected = false;

            SearchText = "";
            SelectedLanguage = 0;
            SelectedSourceType = 0;
            SelectedNoteSource = 0;
            SelectedQuoteSource = 0;
            SelectedCardType = 0;

            FrameContent = null;
            IsReadingMode = false;
            IsWritingMode = false;
            SelectedCard = "";

            LastOpenedWordCard = new WordCardPage() { DataContext = null };
            LastOpenedWordEditCard = new WordEditPage() { DataContext = null };
            LastOpenedSourceCard = new SourceCardPage() { DataContext = null };
            LastOpenedSourceEditCard = new SourceEditPage() { DataContext = null };
            LastOpenedNoteEditCard = new NoteEditPage() { DataContext = null };
            LastOpenedQuoteEditCard = new QuoteEditPage() { DataContext = null };

            ChangeMode();

            DateBegin = new DateTimeOffset(DateTime.UtcNow.AddDays(-1).Date);
            DateEnd = new DateTimeOffset(DateTime.UtcNow.Date);

            Languages = new ObservableRangeCollection<string>() { "All" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);

            Languages.Add("None");

            SourceTypes = new ObservableRangeCollection<string>() { "All" };

            foreach (string sourceType in NotesService.ReadSourceTypes())
                SourceTypes.Add(sourceType);

            SourceTypes.Add("None");

            CardTypes = new ObservableRangeCollection<string>() { "All", "Dictionary", "Sources", "Notes", "Quotes" };

            NotesSourcesNames = new ObservableRangeCollection<string>();
            QuotesSourcesNames = new ObservableRangeCollection<string>();

            Words = new ObservableRangeCollection<Grouping<string, Word>>();
            Sources = new ObservableRangeCollection<Grouping<string, Source>>();
            Notes = new ObservableRangeCollection<Grouping<string, Note>>();
            Quotes = new ObservableRangeCollection<Grouping<string, Quote>>();

            Reload();

            ReloadCommand = new AsyncCommand(Reload);
            SearchTextChangedCommand = new AsyncCommand<object>(SearchTextChanged);
            LanguageSelectedCommand = new AsyncCommand<object>(LanguageSelected);
            SourceTypeSelectedCommand = new AsyncCommand<object>(SourceTypeSelected);
            NoteSourceNameSelectedCommand = new AsyncCommand<object>(NoteSourceNameSelected);
            QuoteSourceNameSelectedCommand = new AsyncCommand<object>(QuoteSourceNameSelected);
            CardTypeSelectedCommand = new AsyncCommand<object>(CardTypeSelected);

            WordSelectedCommand = new AsyncCommand<object>(WordSelected);
            SourceSelectedCommand = new AsyncCommand<object>(SourceSelected);
            NoteSelectedCommand = new AsyncCommand<object>(NoteSelected);
            QuoteSelectedCommand = new AsyncCommand<object>(QuoteSelected);

            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SaveCommand = new AsyncCommand(Save);
            OpenNextCommand = new AsyncCommand<object>(OpenNext);
        }

        private async Task WordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList.SelectedItem == null)
                return;

            LastOpenedSourceCard.DataContext = null;
            LastOpenedSourceEditCard.DataContext = null;
            LastOpenedNoteEditCard.DataContext = null;
            LastOpenedQuoteEditCard.DataContext = null;

            Word selectedWord = wordsList.SelectedItem as Word;

            OpenWordCard(selectedWord);
            wordsList.SelectedItem = null;
        }
        private async Task SourceSelected(object arg)
        {
            ListView sourcesList = arg as ListView;

            if (sourcesList.SelectedItem == null)
                return;

            LastOpenedWordCard.DataContext = null;
            LastOpenedWordEditCard.DataContext = null;
            LastOpenedNoteEditCard.DataContext = null;
            LastOpenedQuoteEditCard.DataContext = null;

            Source selectedSource = sourcesList.SelectedItem as Source;

            OpenSourceCard(selectedSource);

            sourcesList.SelectedItem = null;
        }
        private async Task NoteSelected(object arg)
        {
            ListView notesList = arg as ListView;

            if (notesList.SelectedItem == null)
                return;

            LastOpenedWordCard.DataContext = null;
            LastOpenedWordEditCard.DataContext = null;
            LastOpenedSourceCard.DataContext = null;
            LastOpenedSourceEditCard.DataContext = null;
            LastOpenedQuoteEditCard.DataContext = null;

            Note selectedNote = notesList.SelectedItem as Note;

            OpenNoteCard(selectedNote);

            notesList.SelectedItem = null;
        }
        private async Task QuoteSelected(object arg)
        {
            ListView quotesList = arg as ListView;

            if (quotesList.SelectedItem == null)
                return;

            LastOpenedWordCard.DataContext = null;
            LastOpenedWordEditCard.DataContext = null;
            LastOpenedSourceCard.DataContext = null;
            LastOpenedSourceEditCard.DataContext = null;
            LastOpenedNoteEditCard.DataContext = null;
            
            Quote selectedQuote = quotesList.SelectedItem as Quote;

            OpenQuoteCard(selectedQuote);

            quotesList.SelectedItem = null;
        }

        private void OpenWordCard(Word selectedWord)
        {
            SelectedCard = DictionaryService.ReadWord(selectedWord.Id).Word1;

            LastOpenedWordCard.DataContext = new WordCardPageViewModel(selectedWord.Id);
            LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(selectedWord.Id);

            FrameContent = LastOpenedWordCard;

            IsReadingMode = true;
            IsWritingMode = false;
        }
        private void OpenSourceCard(Source selectedSource)
        {
            SelectedCard = NotesService.ReadSource(selectedSource.Id).SourceName;

            LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(selectedSource.Id);
            LastOpenedSourceEditCard.DataContext = new SourceEditPageViewModel(selectedSource.Id);

            FrameContent = LastOpenedSourceCard;

            IsReadingMode = true;
            IsWritingMode = false;
        }
        private void OpenNoteCard(Note selectedNote)
        {
            SelectedCard = NotesService.ReadSource(selectedNote.SourceID).SourceName;
            LastOpenedNoteEditCard.DataContext = new NoteEditPageViewModel(selectedNote.Id);

            FrameContent = LastOpenedNoteEditCard;

            IsReadingMode = true;
            IsWritingMode = true;

        }
        private void OpenQuoteCard(Quote selectedQuote)
        {
            SelectedCard = NotesService.ReadSource(selectedQuote.SourceID).SourceName;
            LastOpenedQuoteEditCard.DataContext = new QuoteEditPageViewModel(selectedQuote.Id);

            FrameContent = LastOpenedQuoteEditCard;

            IsReadingMode = true;
            IsWritingMode = true;

        }

        private async Task Reload()
        {
            await LoadWords();
            await LoadSources();
            await LoadNotes();
            await LoadQuotes();
        }
        private async Task LoadWords()
        {
            if (IsSourcesSelected || IsNotesSelected || IsQuotesSelected)
                return;

            List<Word> words = new List<Word>();
            List<int> languages = new List<int>();

            List<Word> received_words = DictionaryService.ReadWords();

            foreach (Word word in received_words)
            {
                if (SelectedLanguage == Languages.Count - 1) continue;
                if (!MarkdownService.CheckText(word.Word1).StartsWith(MarkdownService.CheckText(SearchText))) continue;
                if (SelectedLanguage != 0 && word.Language != SelectedLanguage) continue;
                if (word.LastModifiedOn < DateBegin || word.LastModifiedOn > DateEnd) continue;

                words.Add(word);

                if (!languages.Contains(word.Language))
                    languages.Add(word.Language);
            }

            Words.Clear();

            foreach (int languageId in languages)
            {
                string language = DictionaryService.ReadLanguage(languageId);

                Words.Add(new Grouping<string, Word>(
                    language,
                    words.Where(e => e.Language == languageId)));
            }
        }
        private async Task LoadSources()
        {
            if (IsDictionarySelected || IsNotesSelected || IsQuotesSelected)
                return;

            List<Source> sources = new List<Source>();
            List<int> sourceTypes = new List<int>();

            List<string> sourceTypesDic = NotesService.ReadSourceTypes();
            List<Source> received_sources = NotesService.ReadSources();

            foreach (Source source in received_sources)
            {
                source.SourceName = MarkdownService.CheckSource(source.SourceName);

                if (SelectedSourceType == SourceTypes.Count - 1) continue;
                if (!MarkdownService.CheckText(source.SourceName).Contains(MarkdownService.CheckText(SearchText))) continue;
                if (SelectedSourceType != 0 && sourceTypesDic[SelectedSourceType - 1] != NotesService.ReadSourceType(source.SourceType)) continue;
                if (source.LastModifiedOn < DateBegin || source.LastModifiedOn > DateEnd) continue;

                sources.Add(source);

                if (!sourceTypes.Contains(source.SourceType))
                    sourceTypes.Add(source.SourceType);
            }

            Sources.Clear();

            foreach (int sourceType in sourceTypes)
            {
                if (sourceType == 1)
                    continue;

                string source_type = NotesService.ReadSourceType(sourceType);

                Sources.Add(new Grouping<string, Source>(
                    source_type,
                    sources.Where(e => e.SourceType == sourceType)));
            }
        }
        private async Task LoadNotes()
        {
            if (IsDictionarySelected || IsSourcesSelected || IsQuotesSelected)
                return;

            List<Note> notes = new List<Note>();
            List<int> sourceIds = new List<int>();

            List<Note> received_notes = NotesService.ReadNotes();

            foreach (Note note in received_notes)
            {
                if (!MarkdownService.CheckText(note.Title).Contains(MarkdownService.CheckText(SearchText))) continue;
                if (SelectedNoteSource != 0 && note.SourceID != SelectedNoteSource) continue;
                if (note.LastModifiedOn < DateBegin || note.LastModifiedOn > DateEnd) continue;
                notes.Add(note);

                if (!sourceIds.Contains(note.SourceID))
                    sourceIds.Add(note.SourceID);
            }

            Notes.Clear();
            NotesSourcesNames.Clear();
            NotesSourcesNames.Add("All");

            foreach (int sourceId in sourceIds)
            {
                string source_name = NotesService.ReadSource(sourceId).SourceName;
                
                NotesSourcesNames.Add(source_name);
                
                Notes.Add(new Grouping<string, Note>(
                    source_name,
                    notes.Where(e => e.SourceID == sourceId)));
            }
        }
        private async Task LoadQuotes()
        {
            if (IsDictionarySelected || IsSourcesSelected || IsNotesSelected)
                return;

            List<Quote> quotes = new List<Quote>();
            List<int> sourceIds = new List<int>();

            List<Quote> received_quotes = NotesService.ReadQuotes();

            foreach (Quote quote in received_quotes)
            {
                if (!MarkdownService.CheckText(quote.OriginalQuote).Contains(MarkdownService.CheckText(SearchText))) continue;
                if (SelectedQuoteSource != 0 && quote.SourceID != SelectedQuoteSource) continue;
                if (quote.LastModifiedOn < DateBegin || quote.LastModifiedOn > DateEnd) continue;

                quotes.Add(quote);

                if (!sourceIds.Contains(quote.SourceID))
                    sourceIds.Add(quote.SourceID);
            }

            Quotes.Clear();
            QuotesSourcesNames.Clear();
            QuotesSourcesNames.Add("All");

            foreach (int sourceId in sourceIds)
            {
                string source_name = NotesService.ReadSource(sourceId).SourceName;

                QuotesSourcesNames.Add(source_name);

                Quotes.Add(new Grouping<string, Quote>(
                    source_name,
                    quotes.Where(e => e.SourceID == sourceId)));
            }
        }


        private async Task SearchTextChanged(object arg)
        {
            AutoSuggestBox autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox == null)
                return;

            Reload();
        }
        private async Task LanguageSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            LoadWords();
        }
        

        private async Task SourceTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            LoadSources();
        }
        private async Task NoteSourceNameSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            LoadNotes();
        }
        private async Task QuoteSourceNameSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            LoadQuotes();
        }
        private async Task CardTypeSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox == null)
                return;

            IsDictionarySelected = false;
            IsSourcesSelected = false;
            IsNotesSelected = false;
            IsQuotesSelected = false;

            if (SelectedCardType == 0)
            {
                SelectedLanguage = 0;
                SelectedSourceType = 0;
                SelectedNoteSource = 0;
                SelectedQuoteSource = 0;
                Reload();
            }
            if (SelectedCardType == 1)
            { 
                IsDictionarySelected = true;
                
                SelectedLanguage = 0;
                SelectedSourceType = SourceTypes.Count - 1;
                SelectedNoteSource = NotesSourcesNames.Count - 1;
                SelectedQuoteSource = QuotesSourcesNames.Count - 1;

                LoadWords();
                Sources.Clear();
                Notes.Clear();
                Quotes.Clear();

            }
            if (SelectedCardType == 2)
            {
                IsSourcesSelected = true;

                SelectedLanguage = Languages.Count - 1; 
                SelectedSourceType = 0;
                SelectedNoteSource = NotesSourcesNames.Count - 1;
                SelectedQuoteSource = QuotesSourcesNames.Count - 1;

                Words.Clear();
                LoadSources();
                Notes.Clear();
                Quotes.Clear();
            }
            if (SelectedCardType == 3)
            {
                IsNotesSelected = true;

                SelectedLanguage = Languages.Count - 1;
                SelectedSourceType = SourceTypes.Count - 1;
                SelectedNoteSource = 0;
                SelectedQuoteSource = QuotesSourcesNames.Count - 1;

                Words.Clear();
                Sources.Clear();
                LoadNotes();
                Quotes.Clear();
            }
            if (SelectedCardType == 4)
            {
                IsQuotesSelected = true;

                SelectedLanguage = Languages.Count - 1;
                SelectedSourceType = SourceTypes.Count - 1;
                SelectedNoteSource = NotesSourcesNames.Count - 1;
                SelectedQuoteSource = 0;

                Words.Clear();
                Sources.Clear();
                Notes.Clear();
                LoadQuotes();
            }

            
        }


        private async Task ChangeMode()
        {
            if (LastOpenedNoteEditCard.DataContext != null || LastOpenedQuoteEditCard.DataContext != null)
                return;
            
            if (LastOpenedWordEditCard.DataContext != null)
            {
                if (IsReadingMode)
                {
                    if (LastOpenedWordCard.DataContext == null)
                        return;

                    WordCardPageViewModel _view = LastOpenedWordCard.DataContext as WordCardPageViewModel;
                    Word _word = _view.CurrentWord;

                    if (_word == null)
                        return;

                    if (IsWritingMode)
                    {
                        LastOpenedWordCard.DataContext = new WordCardPageViewModel(_word.Id);
                        FrameContent = LastOpenedWordCard;
                        IsWritingMode = false;
                        IsReadingMode = true;
                        return;
                    }

                    FrameContent = LastOpenedWordEditCard;
                    IsWritingMode = true;
                    IsReadingMode = true;

                    return;
                }

                if (IsWritingMode)
                {
                    if (LastOpenedWordEditCard.DataContext != null)
                        FrameContent = LastOpenedWordEditCard;

                    IsReadingMode = true;
                    return;
                }

                if (LastOpenedWordCard.DataContext == null)
                {
                    MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();

                    FrameContent = markdownTextBlock;

                    markdownTextBlock.Text = await MarkdownService.ReadNoCardsOpen();
                    markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
                    markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
                    markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
                    markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
                    markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                    IsReadingMode = false;
                    IsWritingMode = false;
                    return;
                }

                WordCardPageViewModel view = LastOpenedWordCard.DataContext as WordCardPageViewModel;
                Word word = view.CurrentWord;

                if (word != null)
                    LastOpenedWordCard.DataContext = new WordCardPageViewModel(word.Id);

                FrameContent = LastOpenedWordCard;
                IsReadingMode = true;
                return;
            }

            if (LastOpenedSourceEditCard.DataContext != null)
            {
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
        }
        private async Task OpenNext(object arg)
        {
            MessageDialog msg = new MessageDialog("That's all for today. Good work.", "Congratulations!");
            
            if (LastOpenedWordCard.DataContext == null && LastOpenedWordEditCard.DataContext == null &&
                LastOpenedSourceCard.DataContext == null && LastOpenedSourceEditCard.DataContext == null &&
                LastOpenedNoteEditCard.DataContext == null && LastOpenedQuoteEditCard.DataContext == null)
            {
                await Reload();

                if (Words.Count != 0)
                {
                    OpenWordCard(Words.FirstOrDefault().FirstOrDefault());
                    return;
                }
                if (Sources.Count != 0)
                {
                    OpenSourceCard(Sources.FirstOrDefault().FirstOrDefault());
                    return;
                }
                if (Notes.Count != 0)
                {
                    OpenNoteCard(Notes.FirstOrDefault().FirstOrDefault());
                    return;
                }
                if (Quotes.Count != 0)
                {
                    OpenQuoteCard(Quotes.FirstOrDefault().FirstOrDefault());
                    return;
                }

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
                
                await msg.ShowAsync();
                return;
            }
            
            Button btn = arg as Button;
            if (btn == null) return;

            int n = 0;

            if (!Int32.TryParse(btn.Content.ToString(), out n))
                return;
            
            IsWritingMode = true;

            await Save(n);
            
            await Reload();

            if (Words.Count != 0)
            {
                OpenWordCard(Words.FirstOrDefault().FirstOrDefault());
                return;
            }
            if (Sources.Count != 0)
            {
                if (!Sources.FirstOrDefault().FirstOrDefault().SourceName.Contains("UNKNOWN_SOURCE_UNKNOWN"))
                {
                    OpenSourceCard(Sources.FirstOrDefault().FirstOrDefault());
                    return;
                }
            }
            if (Notes.Count != 0)
            {
                OpenNoteCard(Notes.FirstOrDefault().FirstOrDefault());
                return;
            }
            if (Quotes.Count != 0)
            {
                OpenQuoteCard(Quotes.FirstOrDefault().FirstOrDefault());
                return;
            }

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
            
            await msg.ShowAsync();
        }
        private async Task Save()
        {
            Save(0);
        }
        private async Task Save(int n)
        {
            if (!IsWritingMode && IsReadingMode)
                return;

            MessageDialog message;

            if (LastOpenedWordEditCard.DataContext != null)
            {
                WordEditPageViewModel viewModel = new WordEditPageViewModel();

                viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;

                if (viewModel == null)
                    return;

                if (viewModel.CurrentWord == String.Empty)
                {
                    message = new MessageDialog("Word must contains at least one character.", "Woops...");
                    await message.ShowAsync();

                    return;
                }
                if (viewModel.LanguageSelectionComboBoxSelectedIndex == 0 || viewModel.LanguageSelectionComboBoxSelectedIndex > 6)
                {
                    message = new MessageDialog("You need to choose language from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }
                if (viewModel.StatusSelectionComboBoxSelectedIndex == 0)
                {
                    message = new MessageDialog("You need to choose status from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }
                if (viewModel.PartOfSpeechSelectionComboBoxSelectedIndex == 0)
                {
                    message = new MessageDialog("You need to choose part of speech from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }
                if (viewModel.PartOfSpeechSelectionComboBoxSelectedIndex != 5)
                {
                    if (viewModel.MeaningString.ExtraText == String.Empty)
                    {
                        message = new MessageDialog("Meaning string cannot be empty.", "Woops...");
                        await message.ShowAsync();

                        return;
                    }
                }

                var partsOfSpeechDic = new Dictionary<int, string>()
                {
                    { 1, "noun" },
                    { 2, "noun" },
                    { 3, "noun" },
                    { 4, "noun" },
                    { 5, "noun" },
                    { 6, "verb" },
                    { 7, "adj" },
                    { 8, "adv" },
                    { 9, "prep" },
                    { 10, "num" },
                    { 11, "pron" },
                    { 12, "conj" },
                    { 13, "part" },
                    { 14, "interj" },
                    { 15, "posspron" },
                    { 16, "det" },
                    { 17, "pref" },
                };
                var languagesDic = DictionaryService.ReadLanguagesCodes();


                var tempWord = new Word()
                {
                    Id = viewModel.id,
                    Language = viewModel.LanguageSelectionComboBoxSelectedIndex,
                    Status = viewModel.StatusSelectionComboBoxSelectedIndex,
                    LastRepeatedOn = DateTime.UtcNow.AddDays(n),
                    CreatedOn = viewModel.id == 0 ? DateTime.UtcNow : DictionaryService.ReadWord(viewModel.id).CreatedOn,
                    LastModifiedOn = DateTime.UtcNow.AddDays(n),
                    PartOfSpeech = viewModel.PartOfSpeechSelectionComboBoxSelectedIndex,
                    Word1 = String.Format("{0}_{1}_{2}", MarkdownService.CheckWord(viewModel.CurrentWord),
                        partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex],
                        languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex - 1])
                };

                Word oldWord = DictionaryService.ReadWord(tempWord.Id);

                if (oldWord.Word1 != tempWord.Word1)
                    await MarkdownService.RenameFile(oldWord, tempWord);

                DictionaryService.UpdateWord(tempWord);

                tempWord = DictionaryService.ReadWord(tempWord.Word1);
                var extrasList = new List<WordExtra>();

                for (int q = 0; q < viewModel.Extras.Count; q++)
                {
                    if (q == 5 && tempWord.PartOfSpeech != 5)
                    {
                        viewModel.MeaningString.WordId = tempWord.Id;
                        viewModel.MeaningString.LinkedWordId = 0;

                        if (IsReadingMode)
                            DictionaryService.UpdateWordExtra(viewModel.MeaningString);
                        else
                            DictionaryService.CreateWordExtra(viewModel.MeaningString);

                        extrasList.Add(viewModel.MeaningString);
                        continue;
                    }

                    foreach (WordExtra extra in viewModel.Extras[q])
                    {
                        extra.ExtraText = extra.ExtraText.Trim();
                        extra.WordId = tempWord.Id;

                        if (extra.ExtraText == String.Empty)
                            continue;

                        if ((q <= 4) || (q >= 8))
                        {
                            string extraText = "";

                            if (extra.LinkedWordId == 0)
                            {
                                bool contains = false;

                                foreach (string code in languagesDic)
                                    contains = contains || extra.ExtraText.Contains("_" + code);

                                extraText = contains
                                    ? extra.ExtraText
                                    : String.Format("{0}_{1}_{2}", MarkdownService.CheckWord(extra.ExtraText), partsOfSpeechDic[tempWord.PartOfSpeech], languagesDic[tempWord.Language]);

                                extra.LinkedWordId = DictionaryService.ReadWord(extraText).Id;
                            }

                            extra.ExtraText = extra.LinkedWordId == 0 ? extraText : "";
                        }
                        else if (q == 6 || q == 7)
                        {
                            extra.LinkedWordId = 0;
                        }

                        if ((extra.ExtraText != String.Empty && extra.LinkedWordId == 0) || (extra.ExtraText == String.Empty && extra.LinkedWordId != 0))
                        {
                            if (!IsReadingMode || extra.RowID == 0)
                                DictionaryService.CreateWordExtra(extra);
                            else
                                DictionaryService.UpdateWordExtra(extra);

                            extrasList.Add(extra);
                        }
                    }
                }

                await MarkdownService.WriteWord(tempWord, extrasList);

                LastOpenedWordCard.DataContext = new WordCardPageViewModel(tempWord.Id);
                LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(tempWord.Id);

                FrameContent = LastOpenedWordCard;

                IsReadingMode = true;
                IsWritingMode = false;

                message = new MessageDialog("Word was correctly saved.", "Congratulations!");
                await message.ShowAsync();

                return;
            }
            if (LastOpenedSourceEditCard.DataContext != null)
            {
                SourceEditPageViewModel viewModel = new SourceEditPageViewModel();

                viewModel = LastOpenedSourceEditCard.DataContext as SourceEditPageViewModel;

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
                    LastModifiedOn = DateTime.UtcNow.AddDays(n)
                };


                Source oldSource = NotesService.ReadSource(tempSource.Id);

                if (oldSource.SourceName != tempSource.SourceName || oldSource.SourceType != tempSource.SourceType)
                    await MarkdownService.RenameFile(oldSource, tempSource);

                NotesService.UpdateSource(tempSource);

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

                    quote.LastModifiedOn = DateTime.UtcNow.AddDays(n);

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


                    note.LastModifiedOn = DateTime.UtcNow.AddDays(n);

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

                LastOpenedSourceCard.DataContext = new SourceCardPageViewModel(tempSource.Id);
                LastOpenedSourceEditCard.DataContext = new SourceEditPageViewModel(tempSource.Id);

                FrameContent = LastOpenedSourceCard;

                IsReadingMode = true;
                IsWritingMode = false;

                message = new MessageDialog("Card was correctly saved.", "Congratulations!");
                await message.ShowAsync();
            }
            if (LastOpenedNoteEditCard.DataContext != null)
            {
                NoteEditPageViewModel viewModel = LastOpenedNoteEditCard.DataContext as NoteEditPageViewModel;
                Note note = viewModel.SelectedNote;

                note.Stamp = note.Stamp.Trim();
                note.Title = note.Title.Trim();
                note.Note1 = note.Note1.Trim();

                if (note.Stamp == String.Empty && note.Title == String.Empty && note.Note1 == String.Empty)
                    return;

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
                
                note.LastModifiedOn = DateTime.UtcNow.AddDays(n);

                NotesService.UpdateNote(note);

                await MarkdownService.WriteSource(NotesService.ReadSource(note.SourceID),
                    NotesService.ReadQuotes(note.SourceID),
                    NotesService.ReadNotes(note.SourceID),
                    NotesService.ReadSourceExtras(note.SourceID));

                LastOpenedNoteEditCard.DataContext = new NoteEditPageViewModel(note.Id);

                FrameContent = LastOpenedNoteEditCard;

                IsReadingMode = true;
                IsWritingMode = true;

                message = new MessageDialog("Note was correctly saved.", "Congratulations!");
                await message.ShowAsync();

                return;
            }
            if (LastOpenedQuoteEditCard.DataContext != null)
            {
                QuoteEditPageViewModel viewModel = LastOpenedQuoteEditCard.DataContext as QuoteEditPageViewModel;
                Quote quote = viewModel.SelectedQuote;

                quote.QuoteBegin = quote.QuoteBegin.Trim();
                quote.QuoteEnd = quote.QuoteEnd.Trim();
                quote.OriginalQuote = quote.OriginalQuote.Trim();
                quote.TranslatedQuote = quote.TranslatedQuote.Trim();

                if (quote.QuoteBegin == String.Empty && quote.QuoteEnd == String.Empty && quote.OriginalQuote == String.Empty && quote.TranslatedQuote == String.Empty)
                    return;

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
                
                quote.LastModifiedOn = DateTime.UtcNow.AddDays(n);

                NotesService.UpdateQuote(quote);

                await MarkdownService.WriteSource(NotesService.ReadSource(quote.SourceID),
                    NotesService.ReadQuotes(quote.SourceID),
                    NotesService.ReadNotes(quote.SourceID),
                    NotesService.ReadSourceExtras(quote.SourceID));

                LastOpenedQuoteEditCard.DataContext = new QuoteEditPageViewModel(quote.Id);

                FrameContent = LastOpenedQuoteEditCard;

                IsReadingMode = true;
                IsWritingMode = true;

                message = new MessageDialog("Quote was correctly saved.", "Congratulations!");
                await message.ShowAsync();

                return;
            }
        }
    }
}
