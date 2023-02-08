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
using UWP_PROJECT_06.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class WordEditPageViewModel : ViewModelBase
    {
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
        
        DateTime selectedDate;
        public DateTime SelectedDate
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
        public AsyncCommand<object> DeleteCommand { get; }
        public AsyncCommand<object> LostFocusCommand { get; }

        public WordEditPageViewModel() : this(0) {}

        public WordEditPageViewModel(int id)
        {
            Languages = new ObservableRangeCollection<string>() { "Select language" };
            Statuses = new ObservableRangeCollection<string>() { "Select status" };
            PartsOfSpeech = new ObservableRangeCollection<string>() { "Select part of speech" };

            MeaningString = new WordExtra() { LinkType = 5, ExtraText = "" };
            extras = new List<ObservableRangeCollection<WordExtra>>();

            Load(id);

            LanguageSelectedCommand = new AsyncCommand(LanguageSelected);
            DeleteCommand = new AsyncCommand<object>(Delete);
            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        async Task Load(int id)
        {
            #region Word

            var word = DictionaryService.ReadWord(id);
            
            currentWord = word.Word1;
            LanguageSelectionComboBoxSelectedIndex = word.Language;
            StatusSelectionComboBoxSelectedIndex = word.Status;
            PartOfSpeechSelectionComboBoxSelectedIndex = word.PartOfSpeech;
            SelectedDate = word.LastRepeatedOn;

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
            var listView = arg as ListView;
            if (listView == null) return;

            var extras = listView.ItemsSource as ObservableRangeCollection<WordExtra>;
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
    }
}
