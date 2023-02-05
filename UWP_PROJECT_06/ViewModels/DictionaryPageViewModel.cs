using ColorCode.Common;
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
using Windows.Globalization;
using Windows.UI.Xaml.Controls;

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

        Word selectedWordUnknownWord;
        public Word SelectedWordUnknownWord
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


        public ObservableRangeCollection<Grouping<string, Word>> WordsGroups { get; set; }
        public ObservableRangeCollection<Grouping<string, UnknownWord>> UnknownWordsGroups { get; set; }
        public ObservableRangeCollection<string> Languages { get; set; }
        
        public AsyncCommand<object> TextChangedCommand { get; }
        public AsyncCommand<object> LanguageSelectedCommand { get; }
        public AsyncCommand<object> WordSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordTextChangedCommand { get; }
        public AsyncCommand<object> UnknownWordLanguageSelectedCommand { get; }

        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand CardBackButtonPressedCommand { get; }
        public AsyncCommand CardForwardButtonPressedCommand { get; }
        public AsyncCommand SaveUnknownWordCommand { get; }



        public DictionaryPageViewModel()
        {
            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            UnknownWordsGroups = new ObservableRangeCollection<Grouping<string, UnknownWord>>();
            Languages = new ObservableRangeCollection<string>() { "All" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);
            
            AutoSuggestBoxText = "";
            AutoSuggestBoxTextUnknownWord = "";

            comboBoxSelectedIndex = 0;
            comboBoxSelectedIndexUnknownWord = 0;

            IsOnlineDictionaryActive = false;

            LoadWordsGroups();
            LoadUnknownWordsGroups();

            TextChangedCommand = new AsyncCommand<object>(TextChanged);
            UnknownWordTextChangedCommand = new AsyncCommand<object>(UnknownWordTextChanged);

            LanguageSelectedCommand = new AsyncCommand<object>(LanguageSelected);
            UnknownWordLanguageSelectedCommand = new AsyncCommand<object>(UnknownWordLanguageSelected);

            WordSelectedCommand = new AsyncCommand<object>(WordSelected);
            UnknownWordSelectedCommand = new AsyncCommand<object>(UnknownWordSelected);

            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            CardBackButtonPressedCommand = new AsyncCommand(CardBackButtonPressed);
            CardForwardButtonPressedCommand = new AsyncCommand(CardForwardButtonPressed);
            SaveUnknownWordCommand = new AsyncCommand(SaveUnknownWord);

        }

        string Input(string str)
        {
            str = str.ToLower();

            if (str.Contains("ß")) { str = str.Replace("ß", "ss"); }
            if (str.Contains("ä") || str.Contains("Ä")) { str = str.Replace("ä", "a"); str = str.Replace("Ä", "a"); }
            if (str.Contains("ö") || str.Contains("Ö")) { str = str.Replace("ö", "o"); str = str.Replace("Ö", "o"); }
            if (str.Contains("ü") || str.Contains("Ü")) { str = str.Replace("ü", "u"); str = str.Replace("Ü", "u"); }

            return str;
        }
        void LoadWordsGroups()
        {
            List<Word> words = new List<Word>();
            List<int> languages = new List<int>();

            List<Word> received_words = DictionaryService.ReadWords();

            foreach (Word word in received_words)
            {
                if (!Input(word.Word1).StartsWith(Input(AutoSuggestBoxText))) continue;
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

            if (WordsGroups.Count == 0)
                SearchOnline();
        }
        void LoadUnknownWordsGroups()
        {
            List<UnknownWord> words = new List<UnknownWord>();
            List<int> languages = new List<int>();

            List<UnknownWord> received_words = HistoryService.ReadUnknownWords();

            foreach (UnknownWord word in received_words)
            {
                if (!Input(word.Word).StartsWith(Input(AutoSuggestBoxTextUnknownWord))) continue;
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
                LoadWordsGroups();
        }
        async Task WordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList != null)
            {
                //Load();
            }
        }

        async Task SearchOnline()
        {
            if (!IsOnlineDictionaryActive)
            {
                IsOnlineDictionaryActive = true;

                List<string> Uris = new List<string>() {
                    @"https://dictionary.cambridge.org/dictionary/german-english/" + Input(AutoSuggestBoxText),
                    @"https://www.google.com/search?q=" + AutoSuggestBoxText + "+это",
                    @"https://dictionary.cambridge.org/dictionary/german-english/" + Input(AutoSuggestBoxText),
                    @"https://dictionary.cambridge.org/dictionary/english/" + Input(AutoSuggestBoxText)
                };

                var webBrowser = new WebView();
                webBrowser.Source = new Uri(Uris[comboBoxSelectedIndex]);

                FrameContent = webBrowser;
            }
            else
            {
                IsOnlineDictionaryActive = false;
                FrameContent = new Frame();
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

            word.Word = Input(AutoSuggestBoxText);
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

            if (wordsList != null)
            {
                //Load();
            }
        }

    }
}
