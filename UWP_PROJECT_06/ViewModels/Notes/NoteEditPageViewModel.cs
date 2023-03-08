using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class NoteEditPageViewModel : ViewModelBase
    {
        private Note selectedNote; public Note SelectedNote { get => selectedNote; set => SetProperty(ref selectedNote, value); }
        
        public NoteEditPageViewModel() : this(0) { }
        
        public NoteEditPageViewModel(int id) 
        { 
            SelectedNote = NotesService.ReadNote(id);
        }
    }
}
