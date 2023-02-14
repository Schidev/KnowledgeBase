using ColorCode;
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

        string CurrentBrowserWord { get; set; }

        
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


        int selectedUnknownWordId;
        public int SelectedUnknownWordId 
        {
            get => selectedUnknownWordId;
            set => SetProperty(ref selectedUnknownWordId, value);
        }
        bool isUnknownWordSelected;
        public bool IsUnknownWordSelected 
        { 
            get => isUnknownWordSelected;
            set => SetProperty(ref isUnknownWordSelected, value);
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

        public AsyncCommand DeleteUnknownWordCommand { get; }
        public AsyncCommand ClearCommand { get; }
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

            CurrentBrowserWord = "";
            AutoSuggestBoxText = "";
            AutoSuggestBoxTextUnknownWord = "";

            comboBoxSelectedIndex = 0;
            comboBoxSelectedIndexUnknownWord = 0;

            IsReadingMode = false;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;

            SelectedWord = null;
            SelectedWordUnknownWord = null;
            SelectedUnknownWordId = 0;

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

            DeleteUnknownWordCommand = new AsyncCommand(DeleteUnknownWord);
            ClearCommand = new AsyncCommand(Clear);
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
                LastOpenedWordCard.DataContext = new WordCardPageViewModel(SelectedWord.Id);
                LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(SelectedWord.Id);
                
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
                            LastOpenedWordCard.DataContext = new WordCardPageViewModel(word.Id);
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
                        var view = LastOpenedWordCard.DataContext as WordCardPageViewModel;
                        var word = view.CurrentWord;

                        if (word != null)
                            LastOpenedWordCard.DataContext = new WordCardPageViewModel(word.Id);
                        
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
            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
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

                SelectedUnknownWordId = 0;

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

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
                    CurrentBrowserWord = AutoSuggestBoxText;
                    AutoSuggestBoxText = "";
                    SelectedUnknownWordId = 0;
                }

                FrameContent = LastWebSearchRequest;

                IsOnlineDictionaryActive = true;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
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

                SelectedUnknownWordId = 0;

                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsAddingMode = false;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            }
        }
        async Task AddWord()
        {
            if (!IsAddingMode)
            {
                bool change = false;

                if (LastOpenedWordAddingCard.DataContext != null)
                {
                    MessageDialog message = new MessageDialog("All changes wouln't be save. Would you like to continue?");

                    message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                    message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                    var result = await message.ShowAsync();

                    change = (int)result.Id == 0;
                }

                if (LastOpenedWordAddingCard.DataContext == null || change)
                {
                    var viewModel = new WordEditPageViewModel();
                    LastOpenedWordAddingCard.DataContext = viewModel;

                    if (AutoSuggestBoxText.Trim() != String.Empty)
                        await viewModel.SetDefinition(AutoSuggestBoxText);
                    else if (CurrentBrowserWord.Trim() != String.Empty)
                        await viewModel.SetDefinition(CurrentBrowserWord);
                }

                FrameContent = LastOpenedWordAddingCard;

                IsAddingMode = true;
                IsOnlineDictionaryActive = false;
                IsReadingMode = false;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
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
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            }
        }


        async Task Clear()
        {
            MessageDialog message = new MessageDialog("Would you like to delete word permanently or clear all data?");

            message.Commands.Add(new UICommand { Label = "Delete", Id = 0 });
            message.Commands.Add(new UICommand { Label = "Clear", Id = 1 });
            message.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            var result = await message.ShowAsync();

            bool ForDeleting = (int)result.Id == 0;

            if ((int)result.Id == 2)
                return;

            if ((int)result.Id == 0)
            {
                var viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;

                if (viewModel != null)
                {
                    Word word = DictionaryService.ReadWord(viewModel.id);

                    if (word != null)
                    {
                        await MarkdownService.DeleteFile(word);
                        DictionaryService.DeleteWord(word.Id);

                        if (IsAddingMode)
                            LastOpenedWordAddingCard.DataContext = null;

                        LastOpenedWordCard.DataContext = null;
                        LastOpenedWordEditCard.DataContext = null;
                        
                        IsReadingMode = false;
                        IsWritingMode = false;
                        IsOnlineDictionaryActive = false;
                        IsAddingMode = false;
                        IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

                        ChangeMode();

                        message = new MessageDialog("Word was permanentely deleted.", "Congratulations!");
                        await message.ShowAsync();
                        return;
                    }
                    else
                    {
                        message = new MessageDialog("Word doesn't exist.", "Woops...");
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
                var viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;

                if (viewModel != null)
                {
                    viewModel.CurrentWord = "";

                    viewModel.LanguageSelectionComboBoxSelectedIndex = 0;
                    viewModel.StatusSelectionComboBoxSelectedIndex = 0;
                    viewModel.PartOfSpeechSelectionComboBoxSelectedIndex = 0;

                    viewModel.SelectedDate = DateTime.Now;

                    viewModel.MeaningString = new WordExtra() { LinkType = 5, ExtraText = "" };

                    for (int q = 0; q < viewModel.Extras.Count; q++)
                    {
                        int linkType = viewModel.Extras[q].FirstOrDefault().LinkType;

                        viewModel.Extras[q].Clear();
                        viewModel.Extras[q].Add(new WordExtra() { ExtraText = "", LinkType = linkType });
                    }

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

        async Task DeleteUnknownWord()
        {
            MessageDialog message = new MessageDialog("Are you sure you want to delete this unknown word permanentely?", "Deleting");

            message.Commands.Add(new UICommand { Label = "Yeah, delete it", Id = 0 });
            message.Commands.Add(new UICommand { Label = "No, cancel", Id = 1 });

            var result = await message.ShowAsync();

            if ((int)result.Id == 1)
                return;

            HistoryService.DeleteUnknownWord(SelectedUnknownWordId);
            SelectedUnknownWordId = 0;
            IsOnlineDictionaryActive = false;
            LastWebSearchRequest.DataContext = null;

            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            LoadUnknownWordsGroups();

            MarkdownTextBlock markdownTextBlock = new MarkdownTextBlock();
            FrameContent = markdownTextBlock;

            markdownTextBlock.Text = await MarkdownService.ReadWebEmptyWord();

            markdownTextBlock.Padding = new Thickness(20, 0, 20, 0);
            markdownTextBlock.Background = Application.Current.Resources["colorWhite"] as SolidColorBrush;
            markdownTextBlock.Foreground = Application.Current.Resources["colorDimGray"] as SolidColorBrush;
            markdownTextBlock.VerticalAlignment = VerticalAlignment.Center;
            markdownTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
        }

        async Task Save()
        {
            MessageDialog message;

            if (!IsWritingMode && IsReadingMode)
                return;

            if (IsAddingMode || (IsReadingMode && IsWritingMode))
            {
                WordEditPageViewModel viewModel = new WordEditPageViewModel();

                if (IsReadingMode)
                {
                    viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;
                }
                else 
                {
                    viewModel = LastOpenedWordAddingCard.DataContext as WordEditPageViewModel;
                }


                if (viewModel == null)
                    return;

                if (viewModel.CurrentWord == String.Empty)
                {
                    message = new MessageDialog("Word must contains at least one character.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.LanguageSelectionComboBoxSelectedIndex == 0)
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

                if (viewModel.SelectedDate.UtcDateTime.Date < DateTime.UtcNow.Date)
                {
                    message = new MessageDialog("The past is not your concern.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.PartOfSpeechSelectionComboBoxSelectedIndex == 0)
                {
                    message = new MessageDialog("You need to choose part of speech from the offered.", "Woops...");
                    await message.ShowAsync();

                    return;
                }

                if (viewModel.MeaningString.ExtraText == String.Empty)
                {
                    message = new MessageDialog("Meaning string cannot be empty.", "Woops...");
                    await message.ShowAsync();

                    return;
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
                var languagesDic = new Dictionary<int, string>()
                {
                    { 1, "rus" },
                    { 2, "deu" },
                    { 3, "eng" },
                    { 4, "fra" },
                    { 5, "ita" },
                    { 6, "spa" },
                };

                var tempWord = new Word()
                {
                    Id = viewModel.id,
                    Language = viewModel.LanguageSelectionComboBoxSelectedIndex,
                    Status = viewModel.StatusSelectionComboBoxSelectedIndex,
                    LastRepeatedOn = viewModel.SelectedDate.UtcDateTime,
                    CreatedOn = DateTime.UtcNow,
                    LastModifiedOn = DateTime.UtcNow,
                    PartOfSpeech = viewModel.PartOfSpeechSelectionComboBoxSelectedIndex,
                    Word1 = viewModel.CurrentWord.Contains("_rus") || viewModel.CurrentWord.Contains("_deu") || viewModel.CurrentWord.Contains("_eng") || viewModel.CurrentWord.Contains("_fra") || viewModel.CurrentWord.Contains("_ita") || viewModel.CurrentWord.Contains("_spa")
                            ? String.Format("{0}_{1}_{2}", MarkdownService.CheckWord(viewModel.CurrentWord).Replace(" ", "_"), partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex], languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex])
                            : String.Format("{0}_{1}_{2}", viewModel.CurrentWord.Replace(" ", "_"), partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex], languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex])
                };

                if (IsAddingMode)
                    DictionaryService.CreateWord(tempWord);
                else
                {
                    Word oldWord = DictionaryService.ReadWord(tempWord.Id);

                    if (oldWord.Word1 != tempWord.Word1)
                        await MarkdownService.RenameFile(oldWord, tempWord);
                    
                    DictionaryService.UpdateWord(tempWord);
                }

                tempWord = DictionaryService.ReadWord(tempWord.Word1);
                var extrasList = new List<WordExtra>();
                

                for (int q = 0; q < viewModel.Extras.Count; q++)
                {
                    if (q == 5)
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
                        if (extra.ExtraText.Trim() == String.Empty)
                            continue;

                        extra.ExtraText = extra.ExtraText.Trim();
                        
                        if ((q <= 4) || (q >= 8))
                        {
                            var extraText = extra.ExtraText.Contains("_rus") || extra.ExtraText.Contains("_deu") || extra.ExtraText.Contains("_eng") || extra.ExtraText.Contains("_fra") || extra.ExtraText.Contains("_ita") || extra.ExtraText.Contains("_spa")
                            ? extra.ExtraText
                            : String.Format("{0}_{1}_{2}", extra.ExtraText.Replace(" ", "_"), partsOfSpeechDic[tempWord.PartOfSpeech], languagesDic[tempWord.Language]);

                            extra.LinkedWordId = DictionaryService.ReadWord(extraText).Id;
                            extra.ExtraText = extra.LinkedWordId == 0 ? extra.ExtraText : "";
                        }
                        else if (q == 6 || q == 7)
                        {
                            extra.ExtraText = String.Format("{0}", extra.ExtraText);
                            extra.LinkedWordId = 0;
                        }

                        extra.WordId = tempWord.Id;

                        if ((extra.ExtraText != String.Empty && extra.LinkedWordId == 0) || (extra.ExtraText == String.Empty && extra.LinkedWordId != 0))
                        {
                            if (!IsReadingMode || extra.RowID == 0)
                            {
                                DictionaryService.CreateWordExtra(extra);
                                
                            }
                            else
                            {
                                DictionaryService.UpdateWordExtra(extra);
                            }

                            extrasList.Add(extra);
                        }
                    }
                }

                await MarkdownService.WriteWord(tempWord, extrasList);

                
                if (IsAddingMode)
                {
                    LastOpenedWordAddingCard.DataContext = null;
                }

                AutoSuggestBoxText = tempWord.Word1;
                SelectedWord = null;

                LastOpenedWordCard.DataContext = new WordCardPageViewModel(tempWord.Id);
                LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(tempWord.Id);

                FrameContent = LastOpenedWordCard;

                IsReadingMode = true;
                IsWritingMode = false;
                IsOnlineDictionaryActive = false;
                IsAddingMode = false;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

                message = new MessageDialog("Word was correctly saved.", "Congratulations!");
                await message.ShowAsync();
                return;
            }
            
            if (IsOnlineDictionaryActive)
            {
                message = new MessageDialog("Would you like to save this unknown word?");
                
                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0});
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1});

                var result = await message.ShowAsync();

                if ((int)result.Id == 0)
                {
                    UnknownWord word = new UnknownWord();

                    word.Word = MarkdownService.CheckText(CurrentBrowserWord);
                    word.Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex;
                    word.LastModifiedOn = DateTime.Now;

                    if (word.Word != String.Empty)
                    {
                        HistoryService.CreateUnknownWord(word);
                        LoadUnknownWordsGroups();

                        message = new MessageDialog("Unknown word was correctly saved.", "Congratulations!");
                        await message.ShowAsync();
                    }
                    else
                    {
                        message = new MessageDialog("Word must contains at least one character.", "Woops...");
                        await message.ShowAsync();
                    }
                }
                else
                {
                    message = new MessageDialog("Unknown word wasn't saved.", "Woops...");
                    await message.ShowAsync();
                }
            }

            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
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
                var unknownWord = wordsList.SelectedItem as UnknownWord;

                CurrentBrowserWord = unknownWord.Word;
                ComboBoxSelectedIndex = unknownWord.Language;

                List<string> Uris = new List<string>() {
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(CurrentBrowserWord),
                        @"https://www.google.com/search?q=" + CurrentBrowserWord + "+это",
                        @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(CurrentBrowserWord),
                        @"https://dictionary.cambridge.org/dictionary/english/" + MarkdownService.CheckText(CurrentBrowserWord),
                        @"https://dictionary.cambridge.org/dictionary/french-english/" + MarkdownService.CheckText(CurrentBrowserWord),
                        @"https://dictionary.cambridge.org/dictionary/italian-english/" + MarkdownService.CheckText(CurrentBrowserWord),
                        @"https://dictionary.cambridge.org/dictionary/spanish-english/" + MarkdownService.CheckText(CurrentBrowserWord)
                    };

                LastWebSearchRequest.Source = new Uri(Uris[SelectedWordUnknownWord.Language]);

                FrameContent = LastWebSearchRequest;
                SelectedUnknownWordId = SelectedWordUnknownWord.Id;
                SelectedWordUnknownWord = null;

                IsReadingMode = false;
                IsAddingMode= false;
                IsWritingMode = false;
                IsOnlineDictionaryActive = true;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            }
        }

    }
}
