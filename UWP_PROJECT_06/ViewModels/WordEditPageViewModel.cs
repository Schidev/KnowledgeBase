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

        public ObservableRangeCollection<string> Languages { get; set; }
        public ObservableRangeCollection<string> Statuses { get; set; }
        public ObservableRangeCollection<string> PartsOfSpeech { get; set; }

        public ObservableRangeCollection<WordExtra> Plurals { get; set; }
        public ObservableRangeCollection<WordExtra> Synonyms { get; set; }
        public ObservableRangeCollection<WordExtra> Antonyms { get; set; }
        public ObservableRangeCollection<WordExtra> FigurativeMeanings { get; set; }
        public ObservableRangeCollection<WordExtra> Meanings { get; set; }
        public ObservableRangeCollection<WordExtra> Examples { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoRussian { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoGerman { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoEnglish { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoItalian { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoSpanish { get; set; }
        public ObservableRangeCollection<WordExtra> TranslationsIntoFrench { get; set; }

        public AsyncCommand<object> KeyDownCommand { get; }
        public AsyncCommand<object> DeleteCommand { get; }
        public AsyncCommand<object> LostFocusCommand { get; }

        public WordEditPageViewModel()
        {
            Languages = new ObservableRangeCollection<string>() { "Select language" };

            foreach (string language in DictionaryService.ReadLanguages())
                Languages.Add(language);

            Statuses = new ObservableRangeCollection<string>() { "Select status" };

            foreach (string status in DictionaryService.ReadStatuses())
                Statuses.Add(status);

            PartsOfSpeech = new ObservableRangeCollection<string>() { "Select part of speech" };

            foreach (string partOfSpeech in DictionaryService.ReadPartsOfSpeech())
                PartsOfSpeech.Add(partOfSpeech);

            Plurals = new ObservableRangeCollection<WordExtra>() { new WordExtra(){ ExtraText = "", LinkType = 1 }};
            Synonyms = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 2 } };
            Antonyms = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 3 } };
            FigurativeMeanings = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 4 } };
            Meanings = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 6 } };
            Examples = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 7 } };
            TranslationsIntoRussian = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 8 } };
            TranslationsIntoGerman = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 9 } };
            TranslationsIntoEnglish = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 10 } };
            TranslationsIntoItalian = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 11 } };
            TranslationsIntoSpanish = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 12 } };
            TranslationsIntoFrench = new ObservableRangeCollection<WordExtra>() { new WordExtra() { ExtraText = "", LinkType = 13 } };

            DeleteCommand = new AsyncCommand<object>(Delete);
            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        async Task LostFocus(object arg)
        {
            ListView listView = arg as ListView;

            if (listView != null)
            {
                ObservableRangeCollection<WordExtra> extras = listView.ItemsSource as ObservableRangeCollection<WordExtra>;

                if (extras != null)
                {
                    int linkType = extras.FirstOrDefault().LinkType;

                    Regex regex = new Regex(@"\s+");

                    for (int q = extras.Count - 1; q >= 0; q--)
                    {
                        extras[q].ExtraText = extras[q].ExtraText.Trim();

                        if (regex.IsMatch(extras[q].ExtraText) || extras[q].ExtraText == "")
                            extras.RemoveAt(q);
                    }

                    extras.Add(new WordExtra() { LinkType = linkType, ExtraText = "" });
                }
            }

        }

        async Task Delete(object arg)
        {
            TextBox textBox = arg as TextBox; 

            if (textBox != null)
            {
                textBox.Text = "";

                Grid parent = textBox.Parent as Grid;
                parent.Visibility = Visibility.Collapsed;
            }
        }
    }
}
