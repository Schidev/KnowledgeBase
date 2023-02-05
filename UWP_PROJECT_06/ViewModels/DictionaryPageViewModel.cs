using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Services;
using Windows.Globalization;

namespace UWP_PROJECT_06.ViewModels
{
    public class DictionaryPageViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Grouping<String, Word>> WordsGroups { get; set; }
        
        public DictionaryPageViewModel()
        {
            WordsGroups = new ObservableRangeCollection<Grouping<string, Word>>();
            
            Load();

        }

        void Load()
        {
            List<Word> words = DictionaryService.ReadWords();
            List<int> languages = new List<int>();

            foreach (Word word in words)
                if (!languages.Contains(word.Language))
                    languages.Add(word.Language);

            WordsGroups.Clear();

            foreach (int languageId in languages)
            {
                string language = DictionaryService.ReadLanguage(languageId);
                
                WordsGroups.Add(new Grouping<string, Word>(
                    language,
                    words.Where(e => e.Language == languageId)));
            }
        }

    }
}
