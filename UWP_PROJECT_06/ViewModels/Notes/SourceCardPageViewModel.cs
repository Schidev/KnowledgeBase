using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourceCardPageViewModel : ViewModelBase
    {
        Source currentSource; public Source CurrentSource { get => currentSource; set => SetProperty(ref currentSource, value); }
        string markdownText; public string MarkdownText { get => markdownText; set => SetProperty(ref markdownText, value); }

        public SourceCardPageViewModel() { }
        public SourceCardPageViewModel(int sourceId)
        {
            currentSource = NotesService.ReadSource(sourceId);

            Load();
        }

        async Task Load()
        {
            MarkdownText = await MarkdownService.ReadSource(currentSource);

            MarkdownText = String.Format("### Основная информация{0}", MarkdownText.Split("### Основная информация")[1]);
            MarkdownText = MarkdownText.Replace("\r", "\n");
            MarkdownText = MarkdownText.Replace("\n", "\n\n");
        }

    }
}
