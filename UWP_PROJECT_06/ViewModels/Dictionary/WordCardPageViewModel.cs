using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Services;

namespace UWP_PROJECT_06.ViewModels
{
    public class WordCardPageViewModel : ViewModelBase
    {
        private Word currentWord; public Word CurrentWord { get => currentWord; set => SetProperty(ref currentWord, value); }
        private string markdownText; public string MarkdownText { get => markdownText; set => SetProperty(ref markdownText, value); }

        public WordCardPageViewModel() {}

        public WordCardPageViewModel(int wordId)
        {
            currentWord = DictionaryService.ReadWord(wordId);
            
            Load();
        }

        private async Task Load()
        {
            MarkdownText = await MarkdownService.ReadWord(currentWord);
            
            MarkdownText = String.Format("**Язык:**{0}",  MarkdownText.Split("**Язык:**")[1]);
            MarkdownText = MarkdownText.Replace("\r", "\n")
                                       .Replace("\n", "\n\n");
        }
    }
}
