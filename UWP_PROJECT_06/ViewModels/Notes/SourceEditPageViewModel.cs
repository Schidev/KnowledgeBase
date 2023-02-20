using ColorCode;
using MvvmHelpers.Commands;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Services;
using System.Security.AccessControl;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using System.Text.RegularExpressions;

namespace UWP_PROJECT_06.ViewModels.Notes
{
    public class SourceEditPageViewModel : ViewModelBase
    {
        public int Id { get; set; }
        
        int selectedState; public int SelectedState { get => selectedState; set => SetProperty(ref selectedState, value); }
        int selectedTheme; public int SelectedTheme { get => selectedTheme; set => SetProperty(ref selectedTheme, value); }
        int selectedSourceType; public int SelectedSourceType { get => selectedSourceType; set => SetProperty(ref selectedSourceType, value); }
        bool isDownloaded; public bool IsDownloaded { get => isDownloaded; set => SetProperty(ref isDownloaded, value); }
        
       

        Source source; public Source Source { get => source; set => SetProperty(ref source, value); }
        Quote selectedQuote; public Quote SelectedQuote { get => selectedQuote; set => SetProperty(ref selectedQuote, value); }

        public ObservableRangeCollection<string> States { get; set; }
        public ObservableRangeCollection<string> Themes{ get; set; }
        public ObservableRangeCollection<string> SourceTypes { get; set; }

        public ObservableRangeCollection<Quote> Quotes { get; set; } 
        public ObservableRangeCollection<Note> Notes { get; set; } 
        public ObservableRangeCollection<SourceExtra> Extras { get; set; } 

        public AsyncCommand<object> DeleteCommand { get; set; }
        public AsyncCommand<object> StateSelectedCommand { get; }
        public AsyncCommand<object> ThemeSelectedCommand { get; }
        public AsyncCommand<object> SourceTypeSelectedCommand { get; }
        public AsyncCommand<object> LostFocusCommand { get; set; }

        public SourceEditPageViewModel() : this(0) { }
    
        public SourceEditPageViewModel(int id)
        {
            this.Id = id;

            States = new ObservableRangeCollection<string>() { "Select state" };
            Themes = new ObservableRangeCollection<string>() { "Select theme" };
            SourceTypes = new ObservableRangeCollection<string>() { "Select source type" };

            Quotes = new ObservableRangeCollection<Quote>();
            Notes = new ObservableRangeCollection<Note>();
            Extras = new ObservableRangeCollection<SourceExtra>();

            Load();

            DeleteCommand = new AsyncCommand<object>(Delete);

            StateSelectedCommand = new AsyncCommand<object>(StateSelected);
            ThemeSelectedCommand = new AsyncCommand<object>(ThemeSelected);
            SourceTypeSelectedCommand = new AsyncCommand<object>(SourceTypeSelected);

            LostFocusCommand = new AsyncCommand<object>(LostFocus);
        }

        async Task Load()
        {
            #region Source

            source = NotesService.ReadSource(Id);
            source.SourceName = source.Id == 0 ? "" : source.SourceName;
            source.Duration = source.Id == 0 ? 0 : source.Duration;
            source.ActualTime = source.Id == 0 ? 0 : source.ActualTime;
            IsDownloaded = source.Id == 0 ? false : source.IsDownloaded;
            source.IsDownloaded = IsDownloaded;
            source.Description = source.Id == 0 ? "" : source.Description;
            source.SourceLink = source.Id == 0 ? "" : source.SourceLink;

            #endregion
            #region States

            foreach (string state in NotesService.ReadStates())
                States.Add(state);

            SelectedState = source.Id == 0 ? 0 : source.State;

            #endregion
            #region Themes

            foreach (string theme in NotesService.ReadThemes())
                Themes.Add(theme);

            SelectedTheme = source.Id == 0 ? 0 : source.Theme;

            #endregion
            #region Source types

            foreach (string sourceType in NotesService.ReadSourceTypes())
                SourceTypes.Add(sourceType);

            SelectedSourceType = source.Id == 0 ? 0 : source.SourceType;

            #endregion
            #region Quotes

            List<Quote> quotes = NotesService.ReadQuotes(source.Id);

            Quotes.Clear();

            foreach (Quote quote in quotes.OrderBy(n => n.QuoteBegin).ToList())
                Quotes.Add(quote);

            #endregion
            #region Notes

            List<Note> notes = NotesService.ReadNotes(source.Id);

            Notes.Clear();

            foreach (Note note in notes.OrderBy(n => n.Stamp).ToList())
                Notes.Add(note);

            #endregion
            #region Extras

            List<SourceExtra> extras = NotesService.ReadSourceExtras(source.Id);

            Extras.Clear();

            foreach (SourceExtra extra in extras.OrderBy(n => n.Key).ToList())
                Extras.Add(extra);

            #endregion

            LostFocus(Extras);
            LostFocus(Notes);
            LostFocus(Quotes);
        }

        async Task StateSelected(object arg)
        {
            source.State = (byte)SelectedState;
        }

        async Task ThemeSelected(object arg)
        {
            source.Theme = (byte)SelectedTheme;
        }

        async Task SourceTypeSelected(object arg)
        {
            source.SourceType = (byte)SelectedSourceType;
        }

        async Task Delete(object arg)
        {
            var grid = arg as Grid;

            foreach (object child in grid.Children)
            {
                var childGrid = child as Grid;

                if (childGrid == null)
                    continue;

                foreach (object innerChild in ((Grid)childGrid).Children)
                {
                    var textBox = innerChild as TextBox;

                    if (textBox == null)
                        continue;

                    textBox.Text = "";
                }

            }

            grid.Visibility = Visibility.Collapsed;
        }

        async Task LostFocus(object arg)
        {
            var SourceExtrasCollection = arg as ObservableRangeCollection<SourceExtra>;

            #region Extras

            if (SourceExtrasCollection != null)
            {
                
                for (int q = Extras.Count - 1; q >= 0; q--)
                {
                    Extras[q].Key = Extras[q].Key.Trim();
                    Extras[q].Value = Extras[q].Value.Trim();

                    if (Extras[q].Key == "" && Extras[q].Value == "")
                    {
                        if (Extras[q].Id != 0)
                        {
                            //NotesService.DeleteQuote(Quotes[q].Id);

                            //await MarkdownService.WriteWord(NotesService.ReadSource(Quotes[q].SourceID),
                            //    NotesService...);
                        }
                        Extras.RemoveAt(q);
                    }
                }

                Extras.Add(new SourceExtra()
                {
                    SourceID = Id,
                    Key = "",
                    Value = ""
                });
            }

            #endregion

            var NotesCollection = arg as ObservableRangeCollection<Note>;

            #region Notes

            if (NotesCollection != null)
            {
                for (int q = Notes.Count - 1; q >= 0; q--)
                {
                    Notes[q].Stamp = Notes[q].Stamp.Trim();
                    Notes[q].Title = Notes[q].Title.Trim();
                    Notes[q].Note1 = Notes[q].Note1.Trim();

                    if (Notes[q].Stamp == "" && Notes[q].Title == "" && Notes[q].Note1 == "")
                    {
                        if (Notes[q].Id != 0)
                        {
                            //NotesService.DeleteQuote(Quotes[q].Id);

                            //await MarkdownService.WriteWord(NotesService.ReadSource(Quotes[q].SourceID),
                            //    NotesService...);
                        }
                        Notes.RemoveAt(q);

                    }
                }

                Notes.Add(new Note()
                {
                    SourceID = Id,
                    Stamp = "",
                    Title = "",
                    Note1 = ""
                });
            }

            #endregion

            var QuotesCollection = arg as ObservableRangeCollection<Quote>;

            #region Quotes

            if (QuotesCollection != null)
            {
                for (int q = Quotes.Count - 1; q >= 0; q--)
                {
                    Quotes[q].OriginalQuote = Quotes[q].OriginalQuote.Trim();
                    Quotes[q].TranslatedQuote = Quotes[q].TranslatedQuote.Trim();
                    Quotes[q].QuoteBegin = Quotes[q].QuoteBegin.Trim();
                    Quotes[q].QuoteEnd = Quotes[q].QuoteEnd.Trim();

                    if (Quotes[q].OriginalQuote == "" && Quotes[q].TranslatedQuote == "" &&
                        Quotes[q].QuoteBegin == "" && Quotes[q].QuoteEnd == "")
                    {
                        if (Quotes[q].Id != 0)
                        {
                            //NotesService.DeleteQuote(Quotes[q].Id);

                            //await MarkdownService.WriteWord(NotesService.ReadSource(Quotes[q].SourceID),
                            //    NotesService...);
                        }
                        Quotes.RemoveAt(q);

                    }
                }

                Quotes.Add(new Quote()
                {
                    SourceID = Id,
                    QuoteBegin = "",
                    QuoteEnd = "",
                    OriginalQuote = "",
                    TranslatedQuote = ""
                });
            }

            #endregion

        }


    }

}
