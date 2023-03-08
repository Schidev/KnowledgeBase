using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_PROJECT_06.ViewModels
{
    public class DictionaryPageViewModel : ViewModelBase
    {   
        private string autoSuggestBoxText; public string AutoSuggestBoxText { get => autoSuggestBoxText; set => SetProperty(ref autoSuggestBoxText, value); }
        private int comboBoxSelectedIndex; public int ComboBoxSelectedIndex { get => comboBoxSelectedIndex; set => SetProperty(ref comboBoxSelectedIndex, value); }
        private string autoSuggestBoxTextUnknownWord; public string AutoSuggestBoxTextUnknownWord { get => autoSuggestBoxTextUnknownWord; set => SetProperty(ref autoSuggestBoxTextUnknownWord, value); }
        private int comboBoxSelectedIndexUnknownWord; public int ComboBoxSelectedIndexUnknownWord { get => comboBoxSelectedIndexUnknownWord; set => SetProperty(ref comboBoxSelectedIndexUnknownWord, value); }

        string CurrentBrowserWord { get; set; }
        private string selectedWordString; public string SelectedWordString { get => selectedWordString; set => SetProperty(ref selectedWordString, value); }
        private Word selectedWord; public Word SelectedWord { get => selectedWord; set => SetProperty(ref selectedWord, value); }
        
        
        private bool isUnknownWordSelected; public bool IsUnknownWordSelected { get => isUnknownWordSelected; set => SetProperty(ref isUnknownWordSelected, value); }
        private int selectedUnknownWordId; public int SelectedUnknownWordId { get => selectedUnknownWordId; set => SetProperty(ref selectedUnknownWordId, value); }
        private UnknownWord selectedWordUnknownWord; public UnknownWord SelectedWordUnknownWord { get => selectedWordUnknownWord; set => SetProperty(ref selectedWordUnknownWord, value); }


        private object frameContent; public object FrameContent { get => frameContent; set => SetProperty(ref frameContent, value); }
        private bool isOnlineDictionaryActive; public bool IsOnlineDictionaryActive { get => isOnlineDictionaryActive; set => SetProperty(ref isOnlineDictionaryActive, value); }
        private bool isReadingMode; public bool IsReadingMode { get => isReadingMode; set => SetProperty(ref isReadingMode, value); }
        private bool isWritingMode; public bool IsWritingMode { get => isWritingMode; set => SetProperty(ref isWritingMode, value); }
        private bool isAddingMode; public bool IsAddingMode { get => isAddingMode; set => SetProperty(ref isAddingMode, value); }

        private WordEditPage LastOpenedWordEditCard { get; set; }
        private WordEditPage LastOpenedWordAddingCard { get; set; }
        private WordCardPage LastOpenedWordCard { get; set; }
        private WebView LastWebSearchRequest { get; set; }

        public ObservableRangeCollection<Grouping<string, Word>> WordsGroups { get; set; }
        public ObservableRangeCollection<Grouping<string, UnknownWord>> UnknownWordsGroups { get; set; }
        public ObservableRangeCollection<string> Languages { get; set; }

        #region Hotkeys

        public string MenuBackHotkeyName { get => "MenuBack"; }
        public string MenuForwardHotkeyName { get => "MenuForward"; }
        public string MenuMoreHotkeyName { get => "MenuMore"; }
        
        public string FocusOnPrimarySearchHotkeyName { get => "FocusOnPrimarySearch"; }
        public string FocusOnSecondarySearchHotkeyName { get => "FocusOnSecondarySearch"; }
        public string FocusOnPrimaryLanguagePickerHotkeyName { get => "FocusOnPrimaryLanguagePicker"; }
        public string FocusOnSecondaryLanguagePickerHotkeyName { get => "FocusOnSecondaryLanguagePicker"; }
        
        public string TakeAScreenshotHotkeyName { get => "TakeAScreenshot"; }
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

        #region Panel commands

        public AsyncCommand<object> ScreenshotCommand { get; }

        #endregion
        #region Page commands
        
        public AsyncCommand<object> TextChangedCommand { get; }
        public AsyncCommand<object> LanguageSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordTextChangedCommand { get; }
        public AsyncCommand<object> UnknownWordLanguageSelectedCommand { get; }

        public AsyncCommand<object> WordSelectedCommand { get; }
        public AsyncCommand<object> UnknownWordSelectedCommand { get; }

        #endregion
        #region Word card commands

        public AsyncCommand DeleteUnknownWordCommand { get; }
        public AsyncCommand ClearCommand { get; }
        public AsyncCommand BackCommand { get; }
        public AsyncCommand ForwardCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand AddWordCommand { get; }
        public AsyncCommand SaveCommand { get; }

        #endregion

        public DictionaryPageViewModel()
        {
            AutoSuggestBoxText = "";
            ComboBoxSelectedIndex = 0; 
            AutoSuggestBoxTextUnknownWord = "";
            ComboBoxSelectedIndexUnknownWord = 0;

            CurrentBrowserWord = "";
            SelectedWordString = "";
            SelectedWord = null;

            IsUnknownWordSelected = false;
            SelectedUnknownWordId = 0;
            SelectedWordUnknownWord = null;

            IsReadingMode = false;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;

            LastOpenedWordEditCard = new WordEditPage() { DataContext = null };
            LastOpenedWordAddingCard = new WordEditPage() { DataContext = null };
            LastOpenedWordCard = new WordCardPage() { DataContext = null };
            LastWebSearchRequest = new WebView() { DataContext = null };

            ChangeMode();

            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            UnknownWordsGroups = new ObservableRangeCollection<Grouping<string, UnknownWord>>();
            Languages = new ObservableRangeCollection<string>() { "All" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);
            
            LoadWordsGroups();
            LoadUnknownWordsGroups();
            
            ScreenshotCommand = new AsyncCommand<object>(Screenshot);

            TextChangedCommand = new AsyncCommand<object>(TextChanged);
            LanguageSelectedCommand = new AsyncCommand<object>(LanguageSelected);
            UnknownWordTextChangedCommand = new AsyncCommand<object>(UnknownWordTextChanged);
            UnknownWordLanguageSelectedCommand = new AsyncCommand<object>(UnknownWordLanguageSelected);

            WordSelectedCommand = new AsyncCommand<object>(WordSelected);
            UnknownWordSelectedCommand = new AsyncCommand<object>(UnknownWordSelected);

            DeleteUnknownWordCommand = new AsyncCommand(DeleteUnknownWord);
            ClearCommand = new AsyncCommand(Clear);
            BackCommand = new AsyncCommand(Back);
            ForwardCommand = new AsyncCommand(Forward);
            RefreshCommand = new AsyncCommand(Refresh);
            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            AddWordCommand = new AsyncCommand(AddWord);
            SaveCommand = new AsyncCommand(Save);
        }

        private void LoadWordsGroups()
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
        }
        private void LoadUnknownWordsGroups()
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

        private async Task Screenshot(object arg)
        {
            MessageDialog message = new MessageDialog("Would you like to take a screenshot of this word card?");

            message.Commands.Add(new UICommand { Label = "Yeah", Id = 0 });
            message.Commands.Add(new UICommand { Label = "No", Id = 1 });

            var result = await message.ShowAsync();

            Grid cardGrid = arg as Grid;

            if (cardGrid == null || (int)result.Id == 1)
                return;

            double w = cardGrid.Width;
            double h = cardGrid.Height;

            cardGrid.Width = cardGrid.DesiredSize.Width;
            cardGrid.Height = cardGrid.DesiredSize.Height + 100;

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

            StorageFile file = await folder.CreateFileAsync(String.Format("IMAGE_{0}.png", SelectedWordString), CreationCollisionOption.ReplaceExisting);

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
            cardGrid.Width = w;
            cardGrid.Height = h;
        }

        private async Task TextChanged(object arg)
        {
            AutoSuggestBox autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
                LoadWordsGroups();
        }
        private async Task LanguageSelected(object arg)
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
        private async Task UnknownWordTextChanged(object arg)
        {
            AutoSuggestBox autoSuggestBox = arg as AutoSuggestBox;

            if (autoSuggestBox != null)
                LoadUnknownWordsGroups();
        }
        private async Task UnknownWordLanguageSelected(object arg)
        {
            ComboBox comboBox = arg as ComboBox;

            if (comboBox != null)
                LoadUnknownWordsGroups();
        }

        private async Task WordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList.SelectedItem == null)
                return;
         
            LastOpenedWordCard.DataContext = new WordCardPageViewModel(SelectedWord.Id);
            LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(SelectedWord.Id);

            SelectedWordString = DictionaryService.ReadWord(SelectedWord.Id).Word1;

            SelectedWord = null;
            FrameContent = LastOpenedWordCard;

            IsReadingMode = true;
            IsWritingMode = false;
            IsOnlineDictionaryActive = false;
            IsAddingMode = false;

        }
        private async Task UnknownWordSelected(object arg)
        {
            ListView wordsList = arg as ListView;

            if (wordsList.SelectedItem != null)
            {
                UnknownWord unknownWord = wordsList.SelectedItem as UnknownWord;

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
                IsAddingMode = false;
                IsWritingMode = false;
                IsOnlineDictionaryActive = true;
                IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            }
        }


        private async Task DeleteUnknownWord()
        {
            MessageDialog message = new MessageDialog("Are you sure you want to delete this unknown word permanentely?", "Deleting");

            message.Commands.Add(new UICommand { Label = "Yeah, delete it", Id = 0 });
            message.Commands.Add(new UICommand { Label = "No, cancel", Id = 1 });

            IUICommand result = await message.ShowAsync();

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
        private async Task Clear()
        {
            MessageDialog message = new MessageDialog("Would you like to delete word permanently or clear all data?");

            message.Commands.Add(new UICommand { Label = "Delete", Id = 0 });
            message.Commands.Add(new UICommand { Label = "Clear", Id = 1 });
            message.Commands.Add(new UICommand { Label = "Cancel", Id = 2 });

            IUICommand result = await message.ShowAsync();

            if ((int)result.Id == 0)
            {
                WordEditPageViewModel viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;

                if (viewModel == null)
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

                Word word = DictionaryService.ReadWord(viewModel.id);

                if (word == null)
                {
                    message = new MessageDialog("Word doesn't exist.", "Woops...");
                    await message.ShowAsync();
                    return;
                }
                
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

            if ((int)result.Id == 1)
            {
                WordEditPageViewModel viewModel = LastOpenedWordEditCard.DataContext as WordEditPageViewModel;

                if (viewModel == null)
                {
                    message = new MessageDialog("Action is not availiable.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

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

            if ((int)result.Id == 2)
                return;
        }
        private async Task Back()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent == null)
                return;

            if (frameContent.CanGoBack)
                frameContent.GoBack();            
        }
        private async Task Forward()
        {
            WebView frameContent = FrameContent as WebView;

            if (frameContent == null)
                return;
            
            if (frameContent.CanGoForward)
                frameContent.GoForward(); 
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
            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

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
        }
        private async Task SearchOnline()
        {
            IsReadingMode = false;
            IsAddingMode = false;
            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

            if ((AutoSuggestBoxTextUnknownWord == String.Empty && AutoSuggestBoxText == String.Empty && LastWebSearchRequest.Source == null) || (IsOnlineDictionaryActive && AutoSuggestBoxText == String.Empty))
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

                SelectedUnknownWordId = 0;
                IsOnlineDictionaryActive = false;

                return;
            }

            if (AutoSuggestBoxText == String.Empty && AutoSuggestBoxTextUnknownWord != String.Empty)
                AutoSuggestBoxText = AutoSuggestBoxTextUnknownWord;

            FrameContent = LastWebSearchRequest;
            IsOnlineDictionaryActive = true;

            if (AutoSuggestBoxText == String.Empty)
                return;

            List<string> Uris = new List<string>()
            {
                @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                @"https://www.google.com/search?q=" + AutoSuggestBoxText + "+это",
                @"https://dictionary.cambridge.org/dictionary/german-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                @"https://dictionary.cambridge.org/dictionary/english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                @"https://dictionary.cambridge.org/dictionary/french-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                @"https://dictionary.cambridge.org/dictionary/italian-english/" + MarkdownService.CheckText(AutoSuggestBoxText),
                @"https://dictionary.cambridge.org/dictionary/spanish-english/" + MarkdownService.CheckText(AutoSuggestBoxText)
            };

            int searchIndex = ComboBoxSelectedIndex;

            if (ComboBoxSelectedIndex == 0 && ComboBoxSelectedIndexUnknownWord != 0)
                searchIndex = ComboBoxSelectedIndexUnknownWord;

            LastWebSearchRequest.Source = new Uri(Uris[searchIndex]);
            CurrentBrowserWord = AutoSuggestBoxText;
            AutoSuggestBoxText = "";
            AutoSuggestBoxTextUnknownWord = "";
            SelectedUnknownWordId = 0;
        }
        private async Task AddWord()
        {
            IsOnlineDictionaryActive = false;
            IsReadingMode = false;
            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

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
            }
            
            bool IsCanceled = true;

            if (LastOpenedWordAddingCard.DataContext != null)
            {
                MessageDialog message = new MessageDialog("All changes wouln't be save. Would you like to continue?");

                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                var result = await message.ShowAsync();

                IsCanceled = (int)result.Id == 1;
            }

            if (LastOpenedWordAddingCard.DataContext == null || !IsCanceled)
            {
                WordEditPageViewModel viewModel = new WordEditPageViewModel();
                LastOpenedWordAddingCard.DataContext = viewModel;

                if (AutoSuggestBoxText.Trim() != String.Empty)
                    await viewModel.SetDefinition(AutoSuggestBoxText);
                else if (CurrentBrowserWord.Trim() != String.Empty)
                    await viewModel.SetDefinition(CurrentBrowserWord);
            }

            FrameContent = LastOpenedWordAddingCard;
            IsAddingMode = true;
        }
        private async Task Save()
        {
            if (!IsWritingMode && IsReadingMode)
                return;

            MessageDialog message;

            IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

            if (IsOnlineDictionaryActive)
            {
                message = new MessageDialog("Would you like to save this unknown word?");

                message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                var result = await message.ShowAsync();

                if ((int)result.Id == 1)
                {
                    message = new MessageDialog("Unknown word wasn't saved.", "Woops...");
                    await message.ShowAsync();
                    return;
                }

                UnknownWord word = new UnknownWord()
                {
                    Word = MarkdownService.CheckText(CurrentBrowserWord),
                    Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex,
                    LastModifiedOn = DateTime.Now
                };

                if (word.Word == String.Empty)
                {
                    message = new MessageDialog("Word must contains at least one character.", "Woops...");
                    await message.ShowAsync();
                }

                HistoryService.CreateUnknownWord(word);
                LoadUnknownWordsGroups();

                message = new MessageDialog("Unknown word was correctly saved.", "Congratulations!");
                await message.ShowAsync();

                return;
            }

            if (!IsAddingMode && !IsWritingMode)
                return;

            WordEditPageViewModel viewModel = new WordEditPageViewModel();

            viewModel = (IsReadingMode ? LastOpenedWordEditCard.DataContext : LastOpenedWordAddingCard.DataContext) as WordEditPageViewModel;
            
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
            //if (viewModel.SelectedDate.UtcDateTime.Date < DateTime.UtcNow.Date)
            //{
            //    message = new MessageDialog("The past is not your concern.", "Woops...");
            //    await message.ShowAsync();

            //    return;
            //}
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
                LastRepeatedOn = viewModel.SelectedDate.UtcDateTime,
                CreatedOn = viewModel.id == 0 ? DateTime.UtcNow : DictionaryService.ReadWord(viewModel.id).CreatedOn,
                LastModifiedOn = viewModel.SelectedDate.UtcDateTime,
                PartOfSpeech = viewModel.PartOfSpeechSelectionComboBoxSelectedIndex,
                Word1 = String.Format("{0}_{1}_{2}", MarkdownService.CheckWord(viewModel.CurrentWord), 
                    partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex], 
                    languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex - 1])
            };

            if (IsAddingMode)
            {
                DictionaryService.CreateWord(tempWord);
            }
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

            if (IsAddingMode)
                LastOpenedWordAddingCard.DataContext = null;
            
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
    }
}
