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
        public ObservableRangeCollection<Grouping<string, Word>> UnknownWordsGroups { get; set; }
        public ObservableRangeCollection<string> Languages { get; set; }
        
        public AsyncCommand<object> TextChangedCommand { get; }
        public AsyncCommand<object> LanguageSelectedCommand { get; }
        public AsyncCommand<object> WordSelectedCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand CardBackButtonPressedCommand { get; }
        public AsyncCommand CardForwardButtonPressedCommand { get; }
        public AsyncCommand SaveUnknownWordCommand { get; }



        public DictionaryPageViewModel()
        {
            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            UnknownWordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            Languages = new ObservableRangeCollection<string>() { "All" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);
            
            AutoSuggestBoxText = "";
            comboBoxSelectedIndex = 0;
            IsOnlineDictionaryActive = false;

            Load();

            TextChangedCommand = new AsyncCommand<object>(TextChanged);
            LanguageSelectedCommand = new AsyncCommand<object>(LanguageSelected);
            WordSelectedCommand = new AsyncCommand<object>(WordSelected);
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

        void Load()
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

        async Task TextChanged(object arg)
        {
            var autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
                Load();
        }
        async Task LanguageSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
                Load();
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
            bool isWordAdded = false;

            Word word = new Word();

            word.Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex;
            word.Word1 = Input(AutoSuggestBoxText);
            word.CreatedOn = DateTime.Now;

            string language = DictionaryService.ReadLanguage(word.Language);

            foreach (Grouping<string, Word> group in UnknownWordsGroups)
            {
                if (language == group.Key)
                {
                    group.Add(word);
                    isWordAdded = true;
                }
            }

            if (!isWordAdded)
            {
                UnknownWordsGroups.Add(new Grouping<string, Word>(
                    language, 
                    new Collection<Word>() { word }));
            }

        }

    }
}
