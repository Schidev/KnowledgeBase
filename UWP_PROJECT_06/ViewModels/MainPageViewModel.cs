using Microsoft.UI.Xaml.Controls;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Bookmarks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Models.Problems;
using UWP_PROJECT_06.Services;
using UWP_PROJECT_06.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Visibility leftExtendedPanelVisibility;
        public Visibility LeftExtendedPanelVisibility
        {
            get => leftExtendedPanelVisibility;
            set => SetProperty(ref leftExtendedPanelVisibility, value);
        }

        Visibility rightExtendedPanelVisibility;
        public Visibility RightExtendedPanelVisibility
        {
            get => rightExtendedPanelVisibility;
            set => SetProperty(ref rightExtendedPanelVisibility, value);
        }


        public AsyncCommand<object> OpenCommand { get; }

        public AsyncCommand<object> OpenDictionaryPageCommand { get; }
        public AsyncCommand<object> OpenVideosListPageCommand { get; }
        public AsyncCommand<object> OpenWordPageCommand { get; }

        public AsyncCommand<object> AddTabCommand { get; }
        public AsyncCommand<object> OpenCloseExtraPaneCommand { get; }

        public MainPageViewModel()
        {
            //TestBookmarksService(); // Test Completed! :)
            //TestNotesService();
            //TestProblemsService();
            //TestDictionaryService();

            OpenCommand = new AsyncCommand<object>(OpenFile);

            OpenDictionaryPageCommand = new AsyncCommand<object>(OpenDictionaryPage);
            OpenVideosListPageCommand = new AsyncCommand<object>(OpenVideosListPage);
            OpenWordPageCommand = new AsyncCommand<object>(OpenWordPage);

            AddTabCommand = new AsyncCommand<object>(AddTab);
            OpenCloseExtraPaneCommand = new AsyncCommand<object>(OpenCloseExtraPane);
        }

        void TestDictionaryService()
        {
            var t = 0;

            var word = new Word()
            {
                Word1 = "Some_word",
                Language = 1,
                Status = 1,
                PartOfSpeech = 1,
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                LastRepeatedOn = DateTime.Now
            };
            DictionaryService.CreateWord(word);

            word = new Word()
            {
                Word1 = "Some_another_word",
                Language = 2,
                Status = 2,
                PartOfSpeech = 2,
                CreatedOn = DateTime.Now.AddDays(2),
                LastModifiedOn = DateTime.Now.AddDays(2),
                LastRepeatedOn = DateTime.Now.AddDays(2)
            };
            DictionaryService.CreateWord(word);
            word = new Word()
            {
                Word1 = "Some_another_word",
                Language = 2,
                Status = 2,
                PartOfSpeech = 2,
                CreatedOn = DateTime.Now.AddDays(2),
                LastModifiedOn = DateTime.Now.AddDays(2),
                LastRepeatedOn = DateTime.Now.AddDays(2)
            };
            DictionaryService.CreateWord(word);


            var wordExtra = new WordExtra()
            { 
                WordId = 1,
                LinkedWordId = 2,
                LinkType = 2,
                ExtraText = "Some_text",
            };
            DictionaryService.CreateWordExtra(wordExtra);

            wordExtra = new WordExtra()
            {
                WordId = 1,
                LinkedWordId = 2,
                LinkType = 2,
                ExtraText = "Some_anoter_text",
            };
            DictionaryService.CreateWordExtra(wordExtra);

            t = 2;

            var wd = DictionaryService.ReadWord(1);
            var wds = DictionaryService.ReadWords();

            var wde = DictionaryService.ReadWordExtra(1);
            var wdes = DictionaryService.ReadWordExtras(1);

            t = 3;

            wd.Word1 = "Udated_word";
            wd.Status = 3;
            wd.Language = 3;
            wd.PartOfSpeech = 3;
            wd.CreatedOn = DateTime.Now.AddDays(10);
            wd.LastModifiedOn = DateTime.Now.AddDays(10);
            wd.LastRepeatedOn = DateTime.Now.AddDays(10);

            DictionaryService.UpdateWord(wd);


            wde.WordId = 2;
            wde.LinkedWordId = 1;
            wde.LinkType = 6;
            wde.ExtraText = "some_updated_text";

            DictionaryService.UpdateWordExtra(wde);


            t = 4;

            DictionaryService.DeleteWordExtra(1);
            DictionaryService.DeleteWord(1);

            t = 5;

        }

        void TestBookmarksService()
        {
            var t = 0;

            var bookmark = new Bookmark()
            { 
                Date = DateTime.Now,
                Content = "Some content to test this database."
            };

            BookmarksService.CreateBookmark(bookmark);

            t = 1;

            var bm = BookmarksService.ReadBookmark(1);
            var bms =  BookmarksService.ReadBookmarks();

            t = 2;

            bm.Date = DateTime.Now.AddDays(13);
            bm.Content = "Some another content to test this database.";

            BookmarksService.UpdateBookmark(bm);

            t = 3;

            bm.Date = DateTime.Now.AddDays(1);
            bm.Content = "Some another another content to test this database.";

            BookmarksService.CreateBookmark(bm);

            t = 4;

            BookmarksService.DeleteBookmark(1);

            t = 5;

            var task = new DailyTask() 
            { 
                BookmarkID = 2,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(1),
                Task = "Some task to test"
            };

            var task1 = new DailyTask()
            {
                BookmarkID = 2,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(4),
                Task = "Some task1 to test"
            };

            var task2 = new DailyTask()
            {
                BookmarkID = 1,
                TimeBegin = DateTime.Now,
                TimeEnd = DateTime.Now.AddDays(18),
                Task = "Some task2 to test"
            };

            BookmarksService.CreateDailyTask(task);
            BookmarksService.CreateDailyTask(task1);
            BookmarksService.CreateDailyTask(task2);

            t = 6;

            var dt = BookmarksService.ReadDailyTask(2);
            var dts = BookmarksService.ReadDailyTasks(5);

            t = 7;

            dt.BookmarkID = 3;
            dt.TimeBegin = DateTime.Now.AddDays(-20);
            dt.TimeEnd = DateTime.Now.AddDays(-10);
            dt.Task = "Updated task";

            BookmarksService.UpdateDailyTask(dt);

            t = 8;

            BookmarksService.DeleteDailyTask(1);

            t = 9;

            BookmarksService.DeleteBookmark(2);

            t = 10;
        }
        void TestProblemsService()
        {
            var t = 0;

            var problem = new Problem()
            {
                IsDone = false,
                Problem1 = "Some Problem",
                Link = "Some Link",
                Week = 0,
                Category = "M",
                TimePeriodType = "L",
                DueDateTimeBegin = DateTime.Now,
                DueDateTimeEnd = DateTime.Now,
                IsMonday = false,
                IsTuesday = false,
                IsWednesday = false,
                IsThursday = false,
                IsFriday = false,
                IsSaturday = false,
                IsSunday = false,
                RepetitionFrequencyWeeks = 0,
                RepetitionFrequencyDays = 12,
                RepetitionDateFrom = DateTime.Now,
                RepetitionDateTo = DateTime.Now,
                Hash = "d3ac94",
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now
            };

            ProblemsService.CreateProblem(problem);
            problem.Hash = "Adjv83";
            ProblemsService.CreateProblem(problem);

            t = 1;

            var pm = ProblemsService.ReadProblem(1);
            var pms = ProblemsService.ReadProblems();

            t = 2;

            pm.IsDone = true;
            pm.Problem1 = "Some updated Problem";
            pm.Link = "Some Updated Link";
            pm.Week = 1;
            pm.Category = "B";
            pm.TimePeriodType = "P";
            pm.DueDateTimeBegin = DateTime.Now.AddDays(3);
            pm.DueDateTimeEnd = DateTime.Now.AddDays(3);
            pm.IsMonday = true;
            pm.IsTuesday = true;
            pm.IsWednesday = true;
            pm.IsThursday = true;
            pm.IsFriday = true;
            pm.IsSaturday = true;
            pm.IsSunday = true;
            pm.RepetitionFrequencyWeeks = 1;
            pm.RepetitionFrequencyDays = 13;
            pm.RepetitionDateFrom = DateTime.Now.AddDays(3);
            pm.RepetitionDateTo = DateTime.Now.AddDays(3);
            pm.Hash = "AAAc94";
            pm.CreatedOn = DateTime.Now.AddDays(3);
            pm.LastModifiedOn = DateTime.Now.AddDays(3);

            ProblemsService.UpdateProblem(pm);

            t = 3;

            ProblemsService.DeleteProblem(1);

            t = 4;
        }

        void TestNotesService()
        {
            var t = 0;

            var source = new Source()
            {
                SourceName = "Some source",
                Duration = 12,
                ActualTime = 15,
                State = 1,
                Theme = 2,
                SourceType = 1,
                IsDownloaded = false,
                Description = "Some description",
                SourceLink = "Some source link"
            };
            NotesService.CreateSource(source);
            source = new Source()
            {
                SourceName = "Some another source",
                Duration = 15,
                ActualTime = 20,
                State = 2,
                Theme = 3,
                SourceType = 2,
                IsDownloaded = true,
                Description = "Some another description",
                SourceLink = "Some another source link"
            };
            NotesService.CreateSource(source);

            t = 1;

            #region test notes
            //t = 1;

            //var note = new Note()
            //{
            //    SourceID = 2,
            //    Stamp = "00:00:00 - 00:01:12",
            //    Title = "Some title",
            //    Note1 = "Some note"
            //};
            ////NotesService.CreateNote(note);

            //note = new Note()
            //{
            //    SourceID = 2,
            //    Stamp = "00:01:12 - 00:05:43",
            //    Title = "Some another title",
            //    Note1 = "Some another note"
            //};
            ////NotesService.CreateNote(note);


            //note = new Note()
            //{
            //    SourceID = 3,
            //    Stamp = "00:03:12 - 00:45:47",
            //    Title = "Some another 3 title",
            //    Note1 = "Some another 4 note"
            //};
            ////NotesService.CreateNote(note);

            //note = new Note()
            //{
            //    SourceID = 3,
            //    Stamp = "01:01:12 - 20:05:43",
            //    Title = "Some title",
            //    Note1 = "Some another note"
            //};
            ////NotesService.CreateNote(note);

            //note = new Note()
            //{
            //    SourceID = 3,
            //    Stamp = "01:01:12 - 20:05:43",
            //    Title = "Some title",
            //    Note1 = "Some another note"
            //};
            ////NotesService.CreateNote(note);

            //t = 2;

            //var se = NotesService.ReadNote(5);
            //var ses = NotesService.ReadNotes(3);

            //t = 3;

            //se.SourceID = 2;
            //se.Stamp = "Updated stamp";
            //se.Title = "Some udated title";
            //se.Note1 = "Some updated note";

            //NotesService.UpdateNote(se);


            //t = 4;

            //NotesService.DeleteNote(1);
            #endregion
            #region test source extra
            t = 1;

            var sourseExtra = new SourceExtra()
            {
                SourceID = 2,
                Key = "Some key",
                Value = "Some value",
            };
            NotesService.CreateSourceExtra(sourseExtra);

            NotesService.DeleteSourceExtra(1);

            //sourseExtra = new SourceExtra()
            //{
            //    SourceID = 2,
            //    Key = "Some key",
            //    Value = "Some another value",
            //};
            //NotesService.CreateSourceExtra(sourseExtra);

            //sourseExtra = new SourceExtra()
            //{
            //    SourceID = 3,
            //    Key = "Some another key",
            //    Value = "Some value",
            //};
            //NotesService.CreateSourceExtra(sourseExtra);

            //sourseExtra = new SourceExtra()
            //{
            //    SourceID = 3,
            //    Key = "Some another key",
            //    Value = "Some value",
            //};
            //NotesService.CreateSourceExtra(sourseExtra);


            //t = 2;

            //var se = NotesService.ReadSourceExtra(4);
            //var ses = NotesService.ReadSourceExtras(2);

            //t = 3;

            //se.SourceID = 1;
            //se.Key = "Some updated key";
            //se.Value = "Some udated value";

            //NotesService.UpdateSourceExtra(se);

            //t = 4;

            #endregion

            #region test qoutes

            //t = 2;

            //var quote = new Quote()
            //{
            //    SourceID = 2,
            //    QuoteBegin = "Some begin",
            //    QuoteEnd = "Some end",
            //    OriginalQuote = "Some original qoute",
            //    TranslatedQuote = "Some transalted qoute",
            //};
            //NotesService.CreateQuote(quote);

            //quote = new Quote()
            //{
            //    SourceID = 2,
            //    QuoteBegin = "Some anoter begin",
            //    QuoteEnd = "Some another end",
            //    OriginalQuote = "Some another original qoute",
            //    TranslatedQuote = "Some another transalted qoute",
            //};
            //NotesService.CreateQuote(quote);

            //quote = new Quote()
            //{
            //    SourceID = 3,
            //    QuoteBegin = "Some third begin",
            //    QuoteEnd = "Some third end",
            //    OriginalQuote = "Some third original qoute",
            //    TranslatedQuote = "Some third transalted qoute",
            //};
            //NotesService.CreateQuote(quote);

            //quote = new Quote()
            //{
            //    SourceID = 3,
            //    QuoteBegin = "Some third begin",
            //    QuoteEnd = "Some third end",
            //    OriginalQuote = "Some third original qoute",
            //    TranslatedQuote = "Some third transalted qoute",
            //};
            //NotesService.CreateQuote(quote);


            //t = 2;

            //var se = NotesService.ReadQuote(3);
            //var ses = NotesService.ReadQuotes(2);

            //t = 3;

            //se.SourceID = 1;
            //se.QuoteBegin = "Updated Begin";
            //se.QuoteEnd = "Udated End";
            //se.OriginalQuote = "Udated original";
            //se.TranslatedQuote = "Updated translation";

            //NotesService.UpdateQuote(se);

            //t = 4;

            //NotesService.DeleteQuote(3);
            //t = 5;

            #endregion

            NotesService.DeleteSource(2);

            t = 5;

        }



        private async Task OpenDictionaryPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Dictionary";
                currentTab.Name = "dictionaryPage";

                var frame = new Frame();
                frame.Navigate(typeof(DictionaryPage));

                currentTab.Content = frame;

                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }
        }

        private async Task OpenVideosListPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Videos";
                currentTab.Name = "videoListPage";
                var frame = new Frame();
                frame.Navigate(typeof(VideoListPage));

                currentTab.Content = frame;


                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }

        }

        private async Task OpenWordPage(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                var index = tabControl.SelectedIndex;

                var currentTab = new TabViewItem();

                currentTab.Header = "Word";
                currentTab.Name = "wordPage";
                var frame = new Frame();
                frame.Navigate(typeof(WordPage));

                currentTab.Content = frame;


                tabControl.TabItems[index] = currentTab;
                tabControl.SelectedItem = currentTab;
            }

        }


        private async Task OpenFile(object arg)
        {
            var webBrowser = arg as WebView;

            if (webBrowser != null)
                webBrowser.Source = new Uri(@"https://www.google.com/search?q=hello+world");
        }

        private async Task AddTab(object arg)
        {
            var tabControl = arg as TabView;

            if (tabControl != null)
            {
                // New tab
                var newTab = new TabViewItem();
                // Name the header "Settings"
                newTab.Header = "New tab";
                // name the tab
                newTab.Name = "newTab";

                // Change the tab icon.
                //newTab.IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Setting };

                // Create a frame instance
                Frame frame = new Frame();

                // Add the frame to the tab
                newTab.Content = frame;

                // Navigate the frame to the settings page.
                frame.Navigate(typeof(DictionaryPage));

                // Add the tab to the tab control.
                tabControl.TabItems.Add(newTab);

                // Set the newly created tab as the selected tab.
                tabControl.SelectedItem = newTab;
            }
        }

        private async Task OpenCloseExtraPane(object arg)
        {
            var pane = arg as Grid;
            if (pane == null) return;

            var parent = pane.Parent as Grid;
            if (parent == null) return;

            int columnIndex = pane.Name == "leftPane" ? 2 : 6;

            pane.Visibility = pane.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

            parent.ColumnDefinitions[columnIndex].Width = pane.Visibility == Visibility.Collapsed
                                                ? new GridLength(0)
                                                : new GridLength(1, GridUnitType.Auto);
        }
    }
}
