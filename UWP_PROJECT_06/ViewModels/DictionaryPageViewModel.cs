using ColorCode.Common;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Services.Converters;
using UWP_PROJECT_06.Views;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP_PROJECT_06.ViewModels
{
    public class DictionaryPageViewModel : ViewModelBase
    {
        string autoSuggestBoxText;
        public string AutoSuggestBoxText
        {
            get => autoSuggestBoxText;
            set => SetProperty(ref autoSuggestBoxText, value);
        }

        int comboBoxSelectedIndex;
        public int ComboBoxSelectedIndex
        { 
            get => comboBoxSelectedIndex;
            set => SetProperty(ref comboBoxSelectedIndex, value);
        }

        Word selectedWord;
        public Word SelectedWord
        {
            get => selectedWord;
            set => SetProperty(ref selectedWord, value);
        }


        string autoSuggestBoxTextUnknownWord;
        public string AutoSuggestBoxTextUnknownWord
        {
            get => autoSuggestBoxTextUnknownWord;
            set => SetProperty(ref autoSuggestBoxTextUnknownWord, value);
        }

        int comboBoxSelectedIndexUnknownWord;
        public int ComboBoxSelectedIndexUnknownWord
        {
            get => comboBoxSelectedIndexUnknownWord;
            set => SetProperty(ref comboBoxSelectedIndexUnknownWord, value);
        }

        UnknownWord selectedWordUnknownWord;
        public UnknownWord SelectedWordUnknownWord
        {
            get => selectedWordUnknownWord;
            set => SetProperty(ref selectedWordUnknownWord, value);
        }

        object frameContent;
        public object FrameContent
        { 
            get => frameContent;
            set => SetProperty(ref frameContent, value);
        }

        bool isOnlineDictionaryActive;
        public bool IsOnlineDictionaryActive
        {
            get => isOnlineDictionaryActive;
            set => SetProperty(ref isOnlineDictionaryActive, value);
        }

        bool isReadingMode;
        public bool IsReadingMode
        {
            get => isReadingMode;
            set => SetProperty(ref isReadingMode, value);
        }
        
        bool isEditingMode;
        public bool IsEditingMode
        {
            get => isEditingMode;
            set => SetProperty(ref isEditingMode, value);
        }



        WordEditPage LastOpenedWordEditCard { get; set; }
        WordCardPage LastOpenedWordCard { get; set; }
        WebView LastWebSearchRequest { get; set; }


        public ObservableRangeCollection<Grouping<string, Word>> WordsGroups { get; set; }
        public ObservableRangeCollection<Grouping<string, UnknownWord>> UnknownWordsGroups { get; set; }
        public ObservableRangeCollection<string> Languages { get; set; }
        
        public AsyncCommand<object> TextChangedCommand { get; }
        public AsyncCommand<object> LanguageSelectedCommand { get; }
        public AsyncCommand<object> WordSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordTextChangedCommand { get; }
        public AsyncCommand<object> UnknownWordLanguageSelectedCommand { get; }

        public AsyncCommand CardBackButtonPressedCommand { get; }
        public AsyncCommand CardForwardButtonPressedCommand { get; }
        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand AddWordCommand { get; }
        public AsyncCommand SaveUnknownWordCommand { get; }


        public DictionaryPageViewModel()
        {
            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            UnknownWordsGroups = new ObservableRangeCollection<Grouping<string, UnknownWord>>();

            LastOpenedWordEditCard = new WordEditPage();
            LastOpenedWordCard = new WordCardPage();
            LastWebSearchRequest = new WebView();

            AutoSuggestBoxText = "";
            AutoSuggestBoxTextUnknownWord = "";

            comboBoxSelectedIndex = 0;
            comboBoxSelectedIndexUnknownWord = 0;

            IsReadingMode = true;
            IsOnlineDictionaryActive = false;
            IsEditingMode = false;

            ChangeMode();

            Languages = new ObservableRangeCollection<string>() { "All" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);
            
            LoadWordsGroups();
            LoadUnknownWordsGroups();

            TextChangedCommand = new AsyncCommand<object>(TextChanged);
            UnknownWordTextChangedCommand = new AsyncCommand<object>(UnknownWordTextChanged);

            LanguageSelectedCommand = new AsyncCommand<object>(LanguageSelected);
            UnknownWordLanguageSelectedCommand = new AsyncCommand<object>(UnknownWordLanguageSelected);

            WordSelectedCommand = new AsyncCommand<object>(WordSelected);
            UnknownWordSelectedCommand = new AsyncCommand<object>(UnknownWordSelected);

            CardBackButtonPressedCommand = new AsyncCommand(CardBackButtonPressed);
            CardForwardButtonPressedCommand = new AsyncCommand(CardForwardButtonPressed);
            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            AddWordCommand = new AsyncCommand(AddWord);
            SaveUnknownWordCommand = new AsyncCommand(SaveUnknownWord);

        }


        void LoadWordsGroups()
        {
            List<Word> words = new List<Word>();
            List<int> languages = new List<int>();

            List<Word> received_words = DictionaryService.ReadWords();

            foreach (Word word in received_words)
            {
                if (!MarkdownService.CheckText(word.Word1).StartsWith(MarkdownService.CheckText(AutoSuggestBoxText))) continue;
                if (comboBoxSelectedIndex != 0 && word.Language != comboBoxSelectedIndex) continue;

                words.Add(word);
                
                if (!languages.Contains(word.Language))
                    languages.Add(word.Language);
            }

            WordsGroups.Clear();

            foreach (int languageId in languages)
            {
                string language = DictionaryService.ReadLanguage(languageId);

                WordsGroups.Add(new Grouping<string, Word>(
                    language,
                    words.Where(e => e.Language == languageId)));
            }

            //if (WordsGroups.Count == 0)
            //    SearchOnline();
        }
        void LoadUnknownWordsGroups()
        {
            List<UnknownWord> words = new List<UnknownWord>();
            List<int> languages = new List<int>();

            List<UnknownWord> received_words = HistoryService.ReadUnknownWords();

            foreach (UnknownWord word in received_words)
            {
                if (!MarkdownService.CheckText(word.Word).StartsWith(MarkdownService.CheckText(AutoSuggestBoxTextUnknownWord))) continue;
                if (comboBoxSelectedIndexUnknownWord != 0 && word.Language != comboBoxSelectedIndexUnknownWord) continue;

                words.Add(word);

                if (!languages.Contains(word.Language))
                    languages.Add(word.Language);
            }

            UnknownWordsGroups.Clear();

            foreach (int languageId in languages)
            {
                string language = DictionaryService.ReadLanguage(languageId);

                UnknownWordsGroups.Add(new Grouping<string, UnknownWord>(
                    language,
                    words.Where(e => e.Language == languageId)));
            }
        }


        async Task TextChanged(object arg)
        {
            var autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
                LoadWordsGroups();
        }
        async Task LanguageSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
            { 
                LoadWordsGroups();
                if (IsOnlineDictionaryActive)
                {
                    IsOnlineDictionaryActive = false;
                    SearchOnline();
                }

            }
        }
        async Task WordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList.SelectedItem != null)
            {
                var viewModel = new WordCardPageViewModel(SelectedWord.Id);
                LastOpenedWordCard.DataContext = viewModel;

                SelectedWord = null;

                FrameContent = LastOpenedWordCard;

                IsReadingMode = true;
                IsOnlineDictionaryActive = false;
            }
        }


        async Task ChangeMode()
        {
            if (!IsReadingMode)
            {
                if (SelectedWord != null)
                {
                    var viewModel = new WordCardPageViewModel(SelectedWord.Id);
                    LastOpenedWordCard.DataContext = viewModel;

                    SelectedWord = null;
                }

                FrameContent = LastOpenedWordCard;

                IsReadingMode = true;
                IsOnlineDictionaryActive = false;
                IsEditingMode = false;
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
                
                IsReadingMode = false;
                IsOnlineDictionaryActive = false;
                IsEditingMode = false;
            }
        }
        async Task SearchOnline()
        {
            if (AutoSuggestBoxText == String.Empty)
            {
                MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
                FrameContent = markdownTextBlock;

                markdownTextBlock.Text = await MarkdownService.ReadWebEmptyWord();

                markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
                markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
                markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
                markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
                markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsEditingMode = false;

                return;
            }

            if (!IsOnlineDictionaryActive)
            {
                List<string> Uris = new List<string>() {
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://www.google.com/search?q=" + AutoSuggestBoxText + "+это",
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://dictionary.cambridge.org/dictionary/english/" + MarkdownService.CheckText(AutoSuggestBoxText)
                    };

                LastWebSearchRequest.Source = new Uri(Uris[comboBoxSelectedIndex]);

                FrameContent = LastWebSearchRequest;

                IsOnlineDictionaryActive = true;
                IsReadingMode = false;
                IsEditingMode = false;
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

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsEditingMode = false;
            }
        }
        async Task AddWord()
        {
            if (!IsEditingMode)
            {
                if (LastOpenedWordCard.DataContext != null)
                {
                    var view = LastOpenedWordCard.DataContext as WordCardPageViewModel;
                    var word = view.CurrentWord;
                    
                    if (word == null)
                        LastOpenedWordEditCard.DataContext = new WordEditPageViewModel();
                    else
                        LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(word.Id);
                }
                else 
                {
                    LastOpenedWordEditCard.DataContext = new WordEditPageViewModel();
                }

                FrameContent = LastOpenedWordEditCard;

                IsEditingMode = true;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
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
                
                IsEditingMode = false;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
            }
        }


        async Task CardBackButtonPressed()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent != null)
            {
                if (frameContent.CanGoBack)
                    frameContent.GoBack();
                
                return;
            }
        }
        async Task CardForwardButtonPressed()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent != null)
            {
                if (frameContent.CanGoForward)
                    frameContent.GoForward();

                return;
            }
        }
        



        async Task SaveUnknownWord()
        {
            UnknownWord word = new UnknownWord();

            word.Word = MarkdownService.CheckText(AutoSuggestBoxText);
            word.Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex;
            word.LastModifiedOn = DateTime.Now;

            HistoryService.CreateUnknownWord(word);
            LoadUnknownWordsGroups();
        }
        async Task UnknownWordTextChanged(object arg)
        {
            var autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
                LoadUnknownWordsGroups();
        }
        async Task UnknownWordLanguageSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
                LoadUnknownWordsGroups();
        }
        async Task UnknownWordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList.SelectedItem != null)
            {
                List<string> Uris = new List<string>() {
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(SelectedWordUnknownWord.Word),
                        @"https://www.google.com/search?q=" + SelectedWordUnknownWord.Word + "+это",
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(SelectedWordUnknownWord.Word),
                        @"https://dictionary.cambridge.org/dictionary/english/" + MarkdownService.CheckText(SelectedWordUnknownWord.Word)
                    };

                LastWebSearchRequest.Source = new Uri(Uris[SelectedWordUnknownWord.Language]);

                FrameContent = LastWebSearchRequest;
                SelectedWordUnknownWord = null;

                IsReadingMode = false;
                IsOnlineDictionaryActive = true;
            }
        }

    }
}
