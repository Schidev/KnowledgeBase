using ColorCode.Common;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Services.Converters;
using UWP_PROJECT_06.Views;
using Windows.Globalization;
using Windows.UI.Popups;
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
        
        bool isWritingMode;
        public bool IsWritingMode
        { 
            get => isWritingMode;
            set => SetProperty(ref isWritingMode, value);
        }
        
        bool isAddingMode;
        public bool IsAddingMode
        {
            get => isAddingMode;
            set => SetProperty(ref isAddingMode, value);
        }

        WordEditPage LastOpenedWordEditCard { get; set; }
        WordEditPage LastOpenedWordAddingCard { get; set; }
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

        public AsyncCommand BackCommand { get; }
        public AsyncCommand ForwardCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SaveCommand { get; }


        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand AddWordCommand { get; }
        

        public DictionaryPageViewModel()
        {
            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            UnknownWordsGroups = new ObservableRangeCollection<Grouping<string, UnknownWord>>();

            LastOpenedWordEditCard = new WordEditPage() { DataContext = null };
            LastOpenedWordAddingCard = new WordEditPage() { DataContext = null };
            LastOpenedWordCard = new WordCardPage() { DataContext = null };
            LastWebSearchRequest = new WebView();

            AutoSuggestBoxText = "";
            AutoSuggestBoxTextUnknownWord = "";

            comboBoxSelectedIndex = 0;
            comboBoxSelectedIndexUnknownWord = 0;

            IsReadingMode = false;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;

            SelectedWord = null;

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

            BackCommand = new AsyncCommand(Back);
            ForwardCommand = new AsyncCommand(Forward);
            RefreshCommand = new AsyncCommand(Refresh);
            SaveCommand = new AsyncCommand(Save);

            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            AddWordCommand = new AsyncCommand(AddWord);

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
                LastOpenedWordCard.DataContext = new WordCardPageViewModel(SelectedWord.Id); ;
                LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(SelectedWord.Id); ;
                
                SelectedWord = null;

                FrameContent = LastOpenedWordCard;

                IsReadingMode = true;
                IsWritingMode = false;  
                IsOnlineDictionaryActive = false;
                IsAddingMode = false;
            }
        }

        async Task ChangeMode()
        {
            MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();

            if (IsReadingMode)
            {
                if (LastOpenedWordCard.DataContext != null)
                {
                    var view = LastOpenedWordCard.DataContext as WordCardPageViewModel;
                    var word = view.CurrentWord;

                    if (word != null)
                    {
                        if (IsWritingMode)
                        {
                            FrameContent = LastOpenedWordCard;
                            IsWritingMode = false;
                        }
                        else
                        {
                            FrameContent = LastOpenedWordEditCard;
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
                    if (LastOpenedWordEditCard.DataContext != null)
                        FrameContent = LastOpenedWordEditCard;

                    IsReadingMode = true;
                }
                else 
                {
                    if (LastOpenedWordCard.DataContext == null)
                    {
                        FrameContent = markdownTextBlock;
                        IsReadingMode = false;
                        IsWritingMode = false;
                    }
                    else
                    {
                        FrameContent = LastOpenedWordCard;
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
        }
        async Task SearchOnline()
        {
            if (AutoSuggestBoxText == String.Empty && LastWebSearchRequest.Source == null)
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
                IsAddingMode = false;

                return;
            }

            if (!IsOnlineDictionaryActive || AutoSuggestBoxText != String.Empty)
            {
                List<string> Uris = new List<string>() {
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://www.google.com/search?q=" + AutoSuggestBoxText + "+это",
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://dictionary.cambridge.org/dictionary/english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://dictionary.cambridge.org/dictionary/french-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://dictionary.cambridge.org/dictionary/italian-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                        @"https://dictionary.cambridge.org/dictionary/spanish-english/" + MarkdownService.CheckText(AutoSuggestBoxText)
                    };

                if (AutoSuggestBoxText != String.Empty)
                {
                    LastWebSearchRequest.Source = new Uri(Uris[comboBoxSelectedIndex]);
                    AutoSuggestBoxText = "";
                }
                FrameContent = LastWebSearchRequest;

                IsOnlineDictionaryActive = true;
                IsReadingMode = false;
                IsAddingMode = false;
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
                IsAddingMode = false;
            }
        }
        async Task AddWord()
        {
            if (!IsAddingMode)
            {
                if (LastOpenedWordAddingCard.DataContext == null)
                {
                    var viewModel = new WordEditPageViewModel();
                    LastOpenedWordAddingCard.DataContext = viewModel;

                    if (AutoSuggestBoxText != String.Empty)
                        await viewModel.SetDefinition(AutoSuggestBoxText);
                }

                FrameContent = LastOpenedWordAddingCard;

                IsAddingMode = true;
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

                IsAddingMode = false;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
            }
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
        async Task Save()
        {
            if (IsReadingMode)
            {
                MessageDialog message = new MessageDialog("There are no actions connected to this page.", "Woops...");
                await message.ShowAsync();
            }
            else if (IsAddingMode)
            {
                MessageDialog message = new MessageDialog("There are no actions connected to this page.", "Woops...");
                await message.ShowAsync();
            }
            else if (IsOnlineDictionaryActive)
            {
                MessageDialog message = new MessageDialog("Would you like to save this unknown word?");
                
                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0});
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1});

                var result = await message.ShowAsync();

                if ((int)result.Id == 0)
                {
                    UnknownWord word = new UnknownWord();

                    word.Word = MarkdownService.CheckText(AutoSuggestBoxText);
                    word.Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex;
                    word.LastModifiedOn = DateTime.Now;

                    HistoryService.CreateUnknownWord(word);
                    LoadUnknownWordsGroups();

                    message = new MessageDialog("Congratulations!", "Unknown word WAS correctly saved.");
                    await message.ShowAsync();
                }
                else
                {
                    message = new MessageDialog("Woops...", "Unknown word WAS NOT saved.");
                    await message.ShowAsync();
                }
            }
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
