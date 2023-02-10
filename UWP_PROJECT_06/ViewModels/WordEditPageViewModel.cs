using ColorCode;
using Microsoft.Toolkit;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Dictionary.OnlineDictionary;
using UWP_PROJECT_06.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class WordEditPageViewModel : ViewModelBase
    {
        int id { get; set; }
        Definition CurrentDefinition { get; set; }


        string currentWord;
        public string CurrentWord
        {
            get => currentWord;
            set => SetProperty(ref currentWord, value);
        }

        int languageSelectionComboBoxSelectedIndex;
        public int LanguageSelectionComboBoxSelectedIndex
        {
            get => languageSelectionComboBoxSelectedIndex;
            set => SetProperty(ref languageSelectionComboBoxSelectedIndex, value);
        }

        int statusSelectionComboBoxSelectedIndex;
        public int StatusSelectionComboBoxSelectedIndex
        {
            get => statusSelectionComboBoxSelectedIndex;
            set => SetProperty(ref statusSelectionComboBoxSelectedIndex, value);
        }
        
        int partOfSpeechSelectionComboBoxSelectedIndex;
        public int PartOfSpeechSelectionComboBoxSelectedIndex
        {
            get => partOfSpeechSelectionComboBoxSelectedIndex;
            set => SetProperty(ref partOfSpeechSelectionComboBoxSelectedIndex, value);
        }

        DateTimeOffset selectedDate;
        public DateTimeOffset SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }

        WordExtra meaningString;
        public WordExtra MeaningString
        {
            get => meaningString;
            set => SetProperty(ref meaningString, value);
        }

        Visibility isRussian; public Visibility IsRussian { get => isRussian; set => SetProperty(ref isRussian, value); }
        Visibility isGerman; public Visibility IsGerman { get => isGerman; set => SetProperty(ref isGerman, value); }
        Visibility isEnglish; public Visibility IsEnglish { get => isEnglish; set => SetProperty(ref isEnglish, value); }
        Visibility isSpanish; public Visibility IsSpanish { get => isSpanish; set => SetProperty(ref isSpanish, value); }
        Visibility isItalian; public Visibility IsItalian { get => isItalian; set => SetProperty(ref isItalian, value); }
        Visibility isFrench; public Visibility IsFrench { get => isFrench; set => SetProperty(ref isFrench, value); }

        public ObservableRangeCollection<string> Languages { get; set; }
        public ObservableRangeCollection<string> Statuses { get; set; }
        public ObservableRangeCollection<string> PartsOfSpeech { get; set; }

        public List<ObservableRangeCollection<WordExtra>> extras;
        public List<ObservableRangeCollection<WordExtra>> Extras
        {
            get => extras;
            set => SetProperty(ref extras, value);
        }

        public AsyncCommand LanguageSelectedCommand { get; }
        public AsyncCommand ClearCommand { get; }
        public AsyncCommand<object> DeleteCommand { get; }
        public AsyncCommand<object> LostFocusCommand { get; }

        public WordEditPageViewModel() : this(0) {}
        public WordEditPageViewModel(int id)
        {
            this.id = id;

            Languages = new ObservableRangeCollection<string>() { "Select language" };
            Statuses = new ObservableRangeCollection<string>() { "Select status" };
            PartsOfSpeech = new ObservableRangeCollection<string>() { "Select part of speech" };
            
            MeaningString = new WordExtra() { LinkType = 5, ExtraText = "" };
            Extras = new List<ObservableRangeCollection<WordExtra>>();

            Load();

            LanguageSelectedCommand = new AsyncCommand(LanguageSelected);
            ClearCommand = new AsyncCommand(Clear);
            DeleteCommand = new AsyncCommand<object>(Delete);
            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        async Task Load()
        {
            #region Word

            var word = DictionaryService.ReadWord(id);
            
            currentWord = word.Word1;
            LanguageSelectionComboBoxSelectedIndex = word.Language;
            StatusSelectionComboBoxSelectedIndex = word.Status;
            PartOfSpeechSelectionComboBoxSelectedIndex = word.PartOfSpeech;
            SelectedDate = id == 0 ? DateTime.UtcNow : word.LastRepeatedOn;

            #endregion
            #region Languages

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);

            IsRussian = LanguageSelectionComboBoxSelectedIndex == 1 ? Visibility.Collapsed : Visibility.Visible;
            IsGerman = LanguageSelectionComboBoxSelectedIndex == 2 ? Visibility.Collapsed : Visibility.Visible;
            IsEnglish = LanguageSelectionComboBoxSelectedIndex == 3 ? Visibility.Collapsed : Visibility.Visible;
            IsSpanish = LanguageSelectionComboBoxSelectedIndex == 4 ? Visibility.Collapsed : Visibility.Visible;
            IsItalian = LanguageSelectionComboBoxSelectedIndex == 5 ? Visibility.Collapsed : Visibility.Visible;
            IsFrench = LanguageSelectionComboBoxSelectedIndex == 6 ? Visibility.Collapsed : Visibility.Visible;

            #endregion
            #region Statuses

            foreach (string status in DictionaryService.ReadStatuses())
                Statuses.Add(status);

            #endregion
            #region Parts of speech

            foreach (string partOfSpeech in DictionaryService.ReadPartsOfSpeech())
                PartsOfSpeech.Add(partOfSpeech);

            #endregion
            #region Extras

            for (int q = 0; q < DictionaryService.ReadLinkTypes().Count + 2; q++)
                extras.Add(new ObservableRangeCollection<WordExtra>());

            if (id != 0)
            {
                List<WordExtra> currentWordExtras = DictionaryService.ReadWordExtras(id);

                for (int q = 0; q < currentWordExtras.Count; q++)
                {
                    int index = currentWordExtras[q].LinkType;
                    int linkedWordId = currentWordExtras[q].LinkedWordId;

                    if (linkedWordId != 0)
                    {
                        string text = DictionaryService.ReadWord(linkedWordId).Word1;

                        int underscoresAmount = text.Split("_").Length - 1;
                        string[] splittedText = text.Split("_", underscoresAmount);

                        foreach (string str in splittedText)
                            if (str != splittedText.Last())
                                currentWordExtras[q].ExtraText += str;
                    }

                    extras[index].Add(currentWordExtras[q]);
                }

                MeaningString = extras[5].FirstOrDefault();
            }

            for (int q = 0; q < DictionaryService.ReadLinkTypes().Count + 2; q++)
                extras[q].Add(new WordExtra() { ExtraText = "", LinkType = q });

            #endregion
        }

        async Task LanguageSelected()
        {
            IsRussian = LanguageSelectionComboBoxSelectedIndex == 1 ? Visibility.Collapsed : Visibility.Visible;
            IsGerman = LanguageSelectionComboBoxSelectedIndex == 2 ? Visibility.Collapsed : Visibility.Visible;
            IsEnglish = LanguageSelectionComboBoxSelectedIndex == 3 ? Visibility.Collapsed : Visibility.Visible;
            IsSpanish = LanguageSelectionComboBoxSelectedIndex == 4 ? Visibility.Collapsed : Visibility.Visible;
            IsItalian = LanguageSelectionComboBoxSelectedIndex == 5 ? Visibility.Collapsed : Visibility.Visible;
            IsFrench = LanguageSelectionComboBoxSelectedIndex == 6 ? Visibility.Collapsed : Visibility.Visible;
        }

        async Task LostFocus(object arg)
        {
            var extras = arg as ObservableRangeCollection<WordExtra>;
            if (extras == null) return;

            int linkType = extras.FirstOrDefault().LinkType;

            for (int q = extras.Count - 1; q >= 0; q--)
            {
                extras[q].ExtraText = extras[q].ExtraText.Trim();

                if (extras[q].ExtraText == "")
                    extras.RemoveAt(q);
            }

            extras.Add(new WordExtra()
            {
                LinkType = linkType,
                ExtraText = ""
            });
        }

        async Task Delete(object arg)
        {
            TextBox textBox = arg as TextBox;

            if (textBox == null) return;

            textBox.Text = "";

            Grid parent = textBox.Parent as Grid;
            parent.Visibility = Visibility.Collapsed;
        }

        async Task Clear()
        {
            CurrentWord = "";

            LanguageSelectionComboBoxSelectedIndex = 0;
            StatusSelectionComboBoxSelectedIndex = 0;
            PartOfSpeechSelectionComboBoxSelectedIndex = 0;
            
            SelectedDate = DateTime.Now;

            MeaningString = new WordExtra() { LinkType = 5, ExtraText = "" };

            for (int q = 0; q < Extras.Count; q++)
            {
                int linkType = Extras[q].FirstOrDefault().LinkType;

                Extras[q].Clear();
                Extras[q].Add(new WordExtra() { ExtraText = "", LinkType = linkType });
            }

        }

        public async Task SetDefinition(string word)
        { 
            CurrentDefinition = OnlineDictionaryService.GetDefenition(word);
            
            #region Word
            
            CurrentWord = CurrentDefinition._word;

            #endregion
            #region Part of speech

            PartOfSpeechSelectionComboBoxSelectedIndex = CurrentDefinition._part_of_speech;

            #endregion
            #region plural

            if (CurrentDefinition._plural != String.Empty)
                Extras[1].Add(new WordExtra() { LinkType = 1, ExtraText = CurrentDefinition._definitions._meaning });

            #endregion
            #region Meaning string
            
            MeaningString = MeaningString = new WordExtra() { LinkType = 5, ExtraText = CurrentDefinition._meaning_string };

            #endregion
            #region Meanings

            if (CurrentDefinition._definitions._meaning != String.Empty)
                Extras[6].Add(new WordExtra() { LinkType = 6, ExtraText = CurrentDefinition._definitions._meaning});

            #endregion
            #region Translations into english

            foreach (string translation in CurrentDefinition._definitions._translation)
                Extras[10].Add(new WordExtra() { LinkType = 6, ExtraText = translation });

            #endregion
            #region Examples

            foreach (Example example in CurrentDefinition._examples)
            {
                if (example.first_line != String.Empty && example.second_line != String.Empty)
                {
                    Extras[7].Add(new WordExtra() { LinkType = 7, ExtraText = String.Format("{0} -- {1}", example.first_line, example.second_line) });
                }
                else if (example.first_line != String.Empty && example.second_line == String.Empty)
                {
                    Extras[7].Add(new WordExtra() { LinkType = 7, ExtraText = String.Format("{0}", example.first_line) });
                }
                else if (example.first_line == String.Empty && example.second_line != String.Empty)
                {
                    Extras[7].Add(new WordExtra() { LinkType = 7, ExtraText = String.Format("{0}", example.second_line) });
                }
            }

            #endregion

            foreach (ObservableRangeCollection<WordExtra> extrasCollection in Extras)
                await LostFocus(extrasCollection);
            
        }
    }
}
