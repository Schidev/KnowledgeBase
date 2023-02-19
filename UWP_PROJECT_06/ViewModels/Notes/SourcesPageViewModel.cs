using ColorCode;
using Microsoft.Toolkit.Uwp.UI.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
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

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourcesPageViewModel : ViewModelBase
    {
        string searchSourceText; public string SearchSourceText { get => searchSourceText; set => SetProperty(ref searchSourceText, value); }
        int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value);}
        Source selectedSource; public Source SelectedSource { get => selectedSource; set => SetProperty(ref selectedSource, value); }
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
        public ObservableRangeCollection<string> SourceTypes { get; set; }


        public AsyncCommand DeleteUnknownWordCommand { get; }
        public AsyncCommand ClearCommand { get; }
        public AsyncCommand BackCommand { get; }
        public AsyncCommand ForwardCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SaveCommand { get; }


        public AsyncCommand ChangeModeCommand { get; }
        public AsyncCommand SearchOnlineCommand { get; }
        public AsyncCommand AddWordCommand { get; }


        public AsyncCommand<object> SourceSelectedCommand { get; }

        public SourcesPageViewModel()
        {
            LastOpenedSourceCard = new SourceCardPage();
            LastOpenedSourceEditCard = new SourceEditPage();
            LastOpenedSourceAddingCard = new SourceEditPage();
            LastWebSearchRequest = new WebView();

            Sources = new ObservableCollection<Grouping<string, Source>>();
            searchSourceText = string.Empty;

            FrameContent = null;

            CurrentBrowserWord = "";
            searchSourceText = "";
            
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

            DeleteUnknownWordCommand = new AsyncCommand(DeleteUnknownWord);
            ClearCommand = new AsyncCommand(Clear);
            BackCommand = new AsyncCommand(Back);
            ForwardCommand = new AsyncCommand(Forward);
            RefreshCommand = new AsyncCommand(Refresh);
            SaveCommand = new AsyncCommand(Save);

            ChangeModeCommand = new AsyncCommand(ChangeMode);
            SearchOnlineCommand = new AsyncCommand(SearchOnline);
            AddWordCommand = new AsyncCommand(AddWord);

            SourceSelectedCommand = new AsyncCommand<object>(SourceSelected);
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

                // ToDo: то, что внизу)

                //    var partsOfSpeechDic = new Dictionary<int, string>()
                //    {
                //        { 1, "noun" },
                //        { 2, "noun" },
                //        { 3, "noun" },
                //        { 4, "noun" },
                //        { 5, "noun" },
                //        { 6, "verb" },
                //        { 7, "adj" },
                //        { 8, "adv" },
                //        { 9, "prep" },
                //        { 10, "num" },
                //        { 11, "pron" },
                //        { 12, "conj" },
                //        { 13, "part" },
                //        { 14, "interj" },
                //        { 15, "posspron" },
                //        { 16, "det" },
                //        { 17, "pref" },
                //    };
                //    var languagesDic = new Dictionary<int, string>()
                //    {
                //        { 1, "rus" },
                //        { 2, "deu" },
                //        { 3, "eng" },
                //        { 4, "fra" },
                //        { 5, "ita" },
                //        { 6, "spa" },
                //    };

                //    var tempWord = new Word()
                //    {
                //        Id = viewModel.id,
                //        Language = viewModel.LanguageSelectionComboBoxSelectedIndex,
                //        Status = viewModel.StatusSelectionComboBoxSelectedIndex,
                //        LastRepeatedOn = viewModel.SelectedDate.UtcDateTime,
                //        CreatedOn = DateTime.UtcNow,
                //        LastModifiedOn = DateTime.UtcNow,
                //        PartOfSpeech = viewModel.PartOfSpeechSelectionComboBoxSelectedIndex,
                //        Word1 = viewModel.CurrentWord.Contains("_rus") || viewModel.CurrentWord.Contains("_deu") || viewModel.CurrentWord.Contains("_eng") || viewModel.CurrentWord.Contains("_fra") || viewModel.CurrentWord.Contains("_ita") || viewModel.CurrentWord.Contains("_spa")
                //                ? String.Format("{0}_{1}_{2}", MarkdownService.CheckWord(viewModel.CurrentWord).Replace(" ", "_"), partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex], languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex])
                //                : String.Format("{0}_{1}_{2}", viewModel.CurrentWord.Replace(" ", "_"), partsOfSpeechDic[viewModel.PartOfSpeechSelectionComboBoxSelectedIndex], languagesDic[viewModel.LanguageSelectionComboBoxSelectedIndex])
                //    };

                //    if (IsAddingMode)
                //    {
                //        DictionaryService.CreateWord(tempWord);
                //    }
                //    else
                //    {
                //        Word oldWord = DictionaryService.ReadWord(tempWord.Id);

                //        if (oldWord.Word1 != tempWord.Word1)
                //            await MarkdownService.RenameFile(oldWord, tempWord);

                //        DictionaryService.UpdateWord(tempWord);
                //    }

                //    tempWord = DictionaryService.ReadWord(tempWord.Word1);
                //    var extrasList = new List<WordExtra>();


                //    for (int q = 0; q < viewModel.Extras.Count; q++)
                //    {
                //        if (q == 5 && tempWord.PartOfSpeech != 5)
                //        {
                //            viewModel.MeaningString.WordId = tempWord.Id;
                //            viewModel.MeaningString.LinkedWordId = 0;

                //            if (IsReadingMode)
                //                DictionaryService.UpdateWordExtra(viewModel.MeaningString);
                //            else
                //                DictionaryService.CreateWordExtra(viewModel.MeaningString);

                //            extrasList.Add(viewModel.MeaningString);
                //            continue;
                //        }

                //        foreach (WordExtra extra in viewModel.Extras[q])
                //        {
                //            if (extra.ExtraText.Trim() == String.Empty)
                //                continue;

                //            extra.ExtraText = extra.ExtraText.Trim();

                //            if ((q <= 4) || (q >= 8))
                //            {
                //                var extraText = extra.ExtraText.Contains("_rus") || extra.ExtraText.Contains("_deu") || extra.ExtraText.Contains("_eng") || extra.ExtraText.Contains("_fra") || extra.ExtraText.Contains("_ita") || extra.ExtraText.Contains("_spa")
                //                ? extra.ExtraText
                //                : String.Format("{0}_{1}_{2}", extra.ExtraText.Replace(" ", "_"), partsOfSpeechDic[tempWord.PartOfSpeech], languagesDic[tempWord.Language]);

                //                extra.LinkedWordId = DictionaryService.ReadWord(extraText).Id;
                //                extra.ExtraText = extra.LinkedWordId == 0 ? extra.ExtraText : "";
                //            }
                //            else if (q == 6 || q == 7)
                //            {
                //                extra.ExtraText = String.Format("{0}", extra.ExtraText);
                //                extra.LinkedWordId = 0;
                //            }

                //            extra.WordId = tempWord.Id;

                //            if ((extra.ExtraText != String.Empty && extra.LinkedWordId == 0) || (extra.ExtraText == String.Empty && extra.LinkedWordId != 0))
                //            {
                //                if (!IsReadingMode || extra.RowID == 0)
                //                {
                //                    DictionaryService.CreateWordExtra(extra);

                //                }
                //                else
                //                {
                //                    DictionaryService.UpdateWordExtra(extra);
                //                }

                //                extrasList.Add(extra);
                //            }
                //        }
                //    }

                //    await MarkdownService.WriteWord(tempWord, extrasList);


                //    if (IsAddingMode)
                //    {
                //        LastOpenedWordAddingCard.DataContext = null;
                //    }

                //    AutoSuggestBoxText = tempWord.Word1;
                //    SelectedWord = null;

                //    LastOpenedWordCard.DataContext = new WordCardPageViewModel(tempWord.Id);
                //    LastOpenedWordEditCard.DataContext = new WordEditPageViewModel(tempWord.Id);

                //    FrameContent = LastOpenedWordCard;

                //    IsReadingMode = true;
                //    IsWritingMode = false;
                //    IsOnlineDictionaryActive = false;
                //    IsAddingMode = false;
                //    IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);

                //    message = new MessageDialog("Word was correctly saved.", "Congratulations!");
                //    await message.ShowAsync();
                //    return;
                //}

                //if (IsOnlineDictionaryActive)
                //{
                //    message = new MessageDialog("Would you like to save this unknown word?");

                //    message.Commands.Add(new UICommand { Label = "Yes, I would", Id = 0 });
                //    message.Commands.Add(new UICommand { Label = "No, I wouldn't", Id = 1 });

                //    var result = await message.ShowAsync();

                //    if ((int)result.Id == 0)
                //    {
                //        UnknownWord word = new UnknownWord();

                //        word.Word = MarkdownService.CheckText(CurrentBrowserWord);
                //        word.Language = comboBoxSelectedIndex == 0 ? 2 : comboBoxSelectedIndex;
                //        word.LastModifiedOn = DateTime.Now;

                //        if (word.Word != String.Empty)
                //        {
                //            HistoryService.CreateUnknownWord(word);
                //            LoadUnknownWordsGroups();

                //            message = new MessageDialog("Unknown word was correctly saved.", "Congratulations!");
                //            await message.ShowAsync();
                //        }
                //        else
                //        {
                //            message = new MessageDialog("Word must contains at least one character.", "Woops...");
                //            await message.ShowAsync();
                //        }
                //    }
                //    else
                //    {
                //        message = new MessageDialog("Unknown word wasn't saved.", "Woops...");
                //        await message.ShowAsync();
                //    }
                //}

                //IsUnknownWordSelected = IsOnlineDictionaryActive && (SelectedUnknownWordId != 0);
            }
        }

        async Task Refresh() {}

        async Task Forward() {}

        async Task Back() {}

        async Task Clear() {}
        async Task DeleteUnknownWord() { }

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
        async Task LoadSources()
        {
            List<Source> sources = new List<Source>();
            List<int> sourceTypes = new List<int>();

            List<Source> received_sources = NotesService.ReadSources();

            foreach (Source source in received_sources)
            {
                if (!MarkdownService.CheckText(source.SourceName).StartsWith(MarkdownService.CheckText(SearchSourceText))) continue;
                if (selectedSourceType != 0 && source.SourceType != selectedSourceType) continue;

                sources.Add(source);

                if (!sourceTypes.Contains(source.SourceType))
                    sourceTypes.Add(source.SourceType);
            }

            Sources.Clear();

            foreach (int sourceType in sourceTypes)
            {
                string source_type = DictionaryService.ReadLanguage(sourceType);

                Sources.Add(new Grouping<string, Source>(
                    source_type,
                    sources.Where(e => e.SourceType == sourceType)));
            }
        }
    }
}
