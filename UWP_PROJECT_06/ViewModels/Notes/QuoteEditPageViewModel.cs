using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class QuoteEditPageViewModel : ViewModelBase
    {
        private Quote selectedQuote; public Quote SelectedQuote { get => selectedQuote; set => SetProperty(ref selectedQuote, value); }

        public QuoteEditPageViewModel() : this(0) { }

        public QuoteEditPageViewModel(int id)
        {
            SelectedQuote = NotesService.ReadQuote(id);
        }
    }
}
