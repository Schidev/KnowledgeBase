using Microsoft.Toolkit.Uwp.Helpers;
using MvvmHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.UI.Popups;
using static SQLite.SQLite3;

namespace UWP_PROJECT_06.Services
{
    public static class MarkdownService
    {
        public async static Task<string> ReadWord(Word word)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            List<string> codes = DictionaryService.ReadLanguagesCodes();
            folderName += String.Format("\\{0}\\WORDS", codes[word.Language - 1][0].ToString().ToUpper() + codes[word.Language - 1].Substring(1));

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Read", FullPath = path, Date = DateTime.UtcNow });

                return await Windows.Storage.FileIO.ReadTextAsync(await folder.GetFileAsync(word.Word1 + ".md"));
            }

            StorageFile file = await localFolder.CreateFileAsync("FILE_NOT_FOUND.md", CreationCollisionOption.ReplaceExisting);

            await Windows.Storage.FileIO.WriteTextAsync(file, $"# File {word.Word1}.md does not exist or path is wrong\n{folder.Path}\\{word.Word1.Replace("_", "\\_")}.md");
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }
        public async static Task WriteWord(Word word, IEnumerable<WordExtra> wordExtras)
        {
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            List<string> codes = DictionaryService.ReadLanguagesCodes();
            folderName += String.Format("\\{0}\\WORDS", codes[word.Language - 1][0].ToString().ToUpper() + codes[word.Language - 1].Substring(1));

            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Updated", FullPath = path, Date = DateTime.UtcNow });
            }
            else
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = path, Date = DateTime.UtcNow });
            }

            StorageFile file = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.ReplaceExisting);

            string[] extras = new string[14];

            #region Getting extras for word

            for (int q = 0; q < extras.Length; q++)
                extras[q] = "";

            extras[0] = CheckWord(word.Word1);

            List<Grouping<int, WordExtra>> extrasGroups = new List<Grouping<int, WordExtra>>();
            List<int> linkTypes = new List<int>();

            foreach (WordExtra extra in wordExtras)
                if (!linkTypes.Contains(extra.LinkType))
                    linkTypes.Add(extra.LinkType);

            foreach (int linkType in linkTypes)
            {
                extrasGroups.Add(new Grouping<int, WordExtra>(
                    linkType,
                    wordExtras.Where(e => e.LinkType == linkType)));
            }

            foreach (Grouping<int, WordExtra> group in extrasGroups)
            {
                foreach (WordExtra extra in group.Items)
                {

                    if (group.Key == 5)
                    {
                        extras[group.Key] += String.Format("{0}", extra.LinkedWordId == 0
                                                                            ? (extra.ExtraText == "" || extra.ExtraText == null ? "" : extra.ExtraText)
                                                                            : DictionaryService.ReadWord(extra.LinkedWordId).Word1);
                    }
                    else if (group.Key == 6)
                    {
                        extras[group.Key] += String.Format("{0} {1}", (extra.ExtraText == "" || extra.ExtraText == null
                                                                                ? ""
                                                                                : String.Format(" ==({0})", Regex.Matches(extras[group.Key], "==").Count + 1)),
                                                                       extra.ExtraText);
                    }
                    else if (group.Key == 7)
                    {
                        extras[group.Key] += String.Format("\n{0}. {1}", Regex.Matches(extras[group.Key], "\\n").Count + 1, extra.ExtraText);
                    }
                    else
                    {
                        string extraTextWithUnderscores = extra.LinkedWordId == 0
                                                        ? (extra.ExtraText == "" || extra.ExtraText == null ? "" : extra.ExtraText)
                                                        : DictionaryService.ReadWord(extra.LinkedWordId).Word1;

                        string extraText = CheckWord(extraTextWithUnderscores).Replace("_", " ");

                        if (extra != group.Items.Last())
                        {
                            extras[group.Key] += extraText == ""
                                              ? String.Format("{0}, ", extraTextWithUnderscores)
                                              : String.Format("[{0}]({1}.md), ", extraText, extraTextWithUnderscores);
                        }
                        else
                        {
                            extras[group.Key] += extraText == ""
                                              ? String.Format("{0}", extraTextWithUnderscores)
                                              : String.Format("[{0}]({1}.md)", extraText, extraTextWithUnderscores);
                        }
                    }
                }
            }

            #endregion

            #region Building string



            if (word.PartOfSpeech == 5)
            {

                var foundWord = new Word();
                if (extras[1].Contains("(") && extras[1].Contains(".md)"))
                    DictionaryService.ReadWord(extras[1].Split("(")[1].Split(".md)")[0]);

                var colorId = foundWord.Word1 == null ? word.PartOfSpeech : foundWord.PartOfSpeech;

                StringBuilder pluralOutput = new StringBuilder();

                pluralOutput.AppendLine(String.Format("## Слово: <mark style=\"background: {0};\">{1}</mark>", await SettingsService.ReadColor(colorId), extras[0]));
                pluralOutput.AppendLine(String.Format("**Язык:** {0}.", DictionaryService.ReadLanguage(word.Language)));
                pluralOutput.AppendLine();
                pluralOutput.AppendLine(String.Format("**См. слово:** {0}.", extras[1]));

                if (await folder.FileExistsAsync(word.Word1 + ".md"))
                {
                    var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                    await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Updated", FullPath = path, Date = DateTime.UtcNow });
                }
                else
                {
                    var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                    await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = path, Date = DateTime.UtcNow });
                }

                StorageFile filePlural = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(filePlural, pluralOutput.ToString());
            }
            else
            {
                StringBuilder output = new StringBuilder();

                if (word.PartOfSpeech == 6)
                {
                    output.AppendLine(String.Format("## Слово: **{0}**", extras[0]));
                }
                else
                {
                    output.AppendLine(String.Format("## Слово: <mark style=\"background: {0};\">{1}</mark>", await SettingsService.ReadColor(word.PartOfSpeech), extras[0]));
                }

                output.AppendLine(String.Format("**Язык:** {0}.", DictionaryService.ReadLanguage(word.Language)));
                output.AppendLine(String.Format("**Статус:** {0}.", DictionaryService.ReadStatus(word.Status)));
                output.AppendLine(String.Format("**Дата:** {0}%%Формат даты: ММ-ДД-ГГГГ:ЧЧ-ММ-СС%%.", word.LastRepeatedOn.ToString("MM-dd-yyyy")));
                output.AppendLine(String.Format(""));
                output.AppendLine(String.Format("**Часть речи:** {0}.", DictionaryService.ReadPartOfSpeech(word.PartOfSpeech).Replace("_", " ")));
                output.AppendLine(String.Format("**Множественное число:** {0}.", extras[1]));
                output.AppendLine(String.Format("**Синонимы:** {0}.", extras[2]));
                output.AppendLine(String.Format("**Антонимы:** {0}.", extras[3]));
                output.AppendLine(String.Format("**Переносное значение:** {0}.", extras[4]));
                output.AppendLine(String.Format(""));
                output.AppendLine(String.Format(""));
                output.AppendLine(String.Format("**{0}**{1}.", extras[5], extras[6]));
                output.AppendLine(String.Format(""));
                output.AppendLine(String.Format("**Примеры употребления:** {0}", extras[7]));

                var languages = DictionaryService.ReadLanguages();

                output.Append(word.Language == 1 ? "" : String.Format("\n**Перевод на русский:** {0}.", extras[8]));
                output.Append(word.Language == 2 ? "" : String.Format("\n**Перевод на немецкий:** {0}.", extras[9]));
                output.Append(word.Language == 3 ? "" : String.Format("\n**Перевод на английский:** {0}.", extras[10]));
                output.Append(word.Language == 4 ? "" : String.Format("\n**Перевод на французский:** {0}.", extras[13]));
                output.Append(word.Language == 5 ? "" : String.Format("\n**Перевод на итальянский:** {0}.", extras[11]));
                output.Append(word.Language == 6 ? "" : String.Format("\n**Перевод на испанский:** {0}.", extras[12]));
                

                await Windows.Storage.FileIO.WriteTextAsync(file, output.ToString());
            }

            #endregion

        }
        public async static Task RenameFile(Word oldWord, Word newWord)
        {
            #region Read
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            string fullPath = folderName;

            List<string> codes = DictionaryService.ReadLanguagesCodes();
            folderName += String.Format("\\{0}\\WORDS", codes[oldWord.Language - 1][0].ToString().ToUpper() + codes[oldWord.Language - 1].Substring(1));


            StorageFolder folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);

            string fileInnerText = "";

            if (await folder.FileExistsAsync(oldWord.Word1 + ".md"))
            {
                StorageFile oldFile = await folder.CreateFileAsync(oldWord.Word1 + ".md", CreationCollisionOption.OpenIfExists);
                fileInnerText = await Windows.Storage.FileIO.ReadTextAsync(oldFile);

                var path = oldFile.Path;
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Deleted", FullPath = path, Date = DateTime.UtcNow });

                #region Delete

                await oldFile.DeleteAsync();

                #endregion
            }
            else
            {
                MessageDialog message = new MessageDialog(String.Format("File {0}\\{1}.md doesn't exist.", folder.Path, oldWord.Word1), "Error in file path");
                await message.ShowAsync();

                return;
            }

            #endregion
            #region Write

            if (oldWord.Language != newWord.Language)
            {
                fullPath = folderName;

                folderName += String.Format("\\{0}\\WORDS", codes[newWord.Language - 1][0].ToString().ToUpper() + codes[newWord.Language - 1].Substring(1));


                folder = await localFolder.GetFolderAsync(fullPath);
            }

            StorageFile newFile = await folder.CreateFileAsync(newWord.Word1 + ".md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(newFile, fileInnerText);

            var FullPath = newFile.Path;
            await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = FullPath, Date = DateTime.UtcNow });

            #endregion
        }
        public async static Task DeleteFile(Word word)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            List<string> codes = DictionaryService.ReadLanguagesCodes();
            folderName += String.Format("\\{0}\\WORDS", codes[word.Language - 1][0].ToString().ToUpper() + codes[word.Language - 1].Substring(1));


            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                StorageFile file = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();

                string path = file.Path;
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Deleted", FullPath = path, Date = DateTime.UtcNow });
            }
            else
            {
                MessageDialog message = new MessageDialog(String.Format("File {0}\\{1}.md doesn't exist.", folder.Path, word.Word1), "Error in file path");
                await message.ShowAsync();

                return;
            }
        }
        public async static Task<string> FindWordCardFullPath(Word word)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            string fullPath = folderName;

            List<string> codes = DictionaryService.ReadLanguagesCodes();
            folderName += String.Format("\\{0}\\WORDS", codes[word.Language - 1][0].ToString().ToUpper() + codes[word.Language - 1].Substring(1));


            StorageFolder folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                StorageFile oldFile = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.OpenIfExists);
                return oldFile.Path;
            }
            else
            {
                return word.Word1;
            }
        }


        public async static Task<string> ReadSource(Source source)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            
            string F = "";
            F = String.Format("{0}", NotesService.ReadSourceType(source.SourceType).ToLower());

            string folderName = await SettingsService.ReadPath(F);

            folderName = System.IO.Path.Combine(vaultName, folderName);

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Read", FullPath = path, Date = DateTime.UtcNow });

                return await Windows.Storage.FileIO.ReadTextAsync(await folder.GetFileAsync(source.SourceName + ".md"));
            }

            StorageFile file = await localFolder.CreateFileAsync("FILE_NOT_FOUND.md", CreationCollisionOption.ReplaceExisting);

            await Windows.Storage.FileIO.WriteTextAsync(file, $"# File {source.SourceName}.md does not exist or path is wrong\n{folder.Path}\\{source.SourceName.Replace("_", "\\_")}.md");
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }
        public async static Task WriteSource(Source source, IEnumerable<Quote> quotes, IEnumerable<Note> notes, IEnumerable<SourceExtra> extras)
        {
            string vaultName = await SettingsService.ReadPath("vault");

            var F = "";
            F = String.Format("{0}", NotesService.ReadSourceType(source.SourceType).ToLower());

            string folderName = await SettingsService.ReadPath(F);


            folderName = System.IO.Path.Combine(vaultName, folderName);
            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Updated", FullPath = path, Date = DateTime.UtcNow });
            }
            else
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = path, Date = DateTime.UtcNow });
            }

            StorageFile file = await folder.CreateFileAsync(source.SourceName + ".md", CreationCollisionOption.ReplaceExisting);

            StringBuilder output = new StringBuilder();

            output.AppendLine(String.Format("## Источник: **{0}**", source.SourceName));
            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("![|400]()"));
            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("### Основная информация"));
            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("**Длительность:** {0}.", source.Duration));
            output.AppendLine(String.Format("**Время посмотра:** {0}%% в полных минутах %%.", source.ActualTime));
            output.AppendLine(String.Format("**Состояние:** {0}%% просмотрено, просматривается, просмотрится %%.", NotesService.ReadState(source.State)));
            output.AppendLine(String.Format("**Тема:** {0}%% развлечение, музыка, фильм, образование, спорт, питание, разработка %%.", NotesService.ReadTheme(source.Theme)));
            output.AppendLine(String.Format("**Тип источника:** {0}%% видео, аудио, изображение, документ %%.", NotesService.ReadSourceType(source.SourceType)));
            output.AppendLine(String.Format("**Загружено:** {0}%% да, нет %%.", source.IsDownloaded ? "да" : "нет"));
            output.AppendLine(String.Format("**Описание:** {0}.", source.Description));
            output.AppendLine(String.Format("**Ссылка:** {0}.", source.SourceLink));
            output.AppendLine(String.Format("**Временные метки:**"));

            int counter = 1;

            foreach (Note note in notes)
            {
                output.AppendLine(String.Format("{0}. {1} - {2}.", counter++, MarkdownService.CheckNoteStamp(note.Stamp), note.Title));
            }

            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("### Дополнительная информация"));

            foreach (SourceExtra extra in extras)
                output.AppendLine(String.Format("**{0}:** {1}.", extra.Key, extra.Value));

            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("### Заметки"));
            output.AppendLine(String.Format(""));

            counter = 1;
            foreach (Note note in notes)
            {
                output.AppendLine(String.Format("{0}. {1}.", counter++, note.Note1));
            }

            output.AppendLine(String.Format(""));
            output.AppendLine(String.Format("### Цитаты"));
            output.AppendLine(String.Format(""));

            foreach (Quote quote in quotes)
            {
                quote.QuoteBegin = MarkdownService.CheckQuoteStamp(quote.QuoteBegin);
                quote.QuoteEnd = MarkdownService.CheckQuoteStamp(quote.QuoteEnd);

                quote.OriginalQuote = quote.OriginalQuote.Replace("<br>", "\r\r");
                quote.OriginalQuote = quote.OriginalQuote.Replace("\r\r", "\r>");

                quote.TranslatedQuote = quote.TranslatedQuote.Replace("<br>", "\r\r");
                quote.TranslatedQuote = quote.TranslatedQuote.Replace("\r\r", "\r>");


                output.AppendLine(String.Format(">{0}", quote.OriginalQuote));
                output.AppendLine(String.Format(">"));
                output.AppendLine(String.Format(">{0}", quote.TranslatedQuote));
                output.AppendLine(String.Format(">"));
                output.AppendLine(String.Format(">{0} - {1}", quote.QuoteBegin, quote.QuoteEnd));
            }

            await Windows.Storage.FileIO.WriteTextAsync(file, output.ToString());
        }
        public async static Task RenameFile(Source oldSource, Source newSource)
        {
            #region Read
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");

            var F = "";
            List<string> types = NotesService.ReadSourceTypes();
            F = String.Format("{0}", NotesService.ReadSourceType(oldSource.SourceType).ToLower());


            string folderName = await SettingsService.ReadPath(F);

            folderName = System.IO.Path.Combine(vaultName, folderName);

            string fullPath = folderName;

            StorageFolder folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);

            string fileInnerText = "";

            if (await folder.FileExistsAsync(oldSource.SourceName + ".md"))
            {
                StorageFile oldFile = await folder.CreateFileAsync(oldSource.SourceName + ".md", CreationCollisionOption.OpenIfExists);
                fileInnerText = await Windows.Storage.FileIO.ReadTextAsync(oldFile);

                var path = oldFile.Path;
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Deleted", FullPath = path, Date = DateTime.UtcNow });

                #region Delete

                await oldFile.DeleteAsync();

                #endregion
            }
            else
            {
                MessageDialog message = new MessageDialog(String.Format("File {0}\\{1}.md doesn't exist.", folder.Path, oldSource.SourceName), "Error in file path");
                await message.ShowAsync();

                return;
            }

            #endregion
            #region Write

            if (oldSource.SourceType != newSource.SourceType)
            {
                F = String.Format("{0}", NotesService.ReadSourceType(newSource.SourceType).ToLower());

                folderName = await SettingsService.ReadPath(F);

                folderName = System.IO.Path.Combine(vaultName, folderName);

                fullPath = folderName;
                folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);
            }

            StorageFile newFile = await folder.CreateFileAsync(newSource.SourceName + ".md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(newFile, fileInnerText);

            var FullPath = newFile.Path;
            await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Created", FullPath = FullPath, Date = DateTime.UtcNow });

            #endregion
        }
        public async static Task DeleteFile(Source source)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");

            var F = "";
            List<string> types = NotesService.ReadSourceTypes();
            F = String.Format("{0}", NotesService.ReadSourceType(source.SourceType).ToLower());


            string folderName = await SettingsService.ReadPath(F);

            folderName = System.IO.Path.Combine(vaultName, folderName);

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                StorageFile file = await folder.CreateFileAsync(source.SourceName + ".md", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();

                string path = file.Path;
                await SettingsService.WriteHistory(new Models.History.HistoryItem { Action = "Deleted", FullPath = path, Date = DateTime.UtcNow });
            }
            else
            {
                MessageDialog message = new MessageDialog(String.Format("File {0}\\{1}.md doesn't exist.", folder.Path, source.SourceName), "Error in file path");
                await message.ShowAsync();

                return;
            }
        }


        public async static Task<string> ReadNoCardsOpen()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("NO_CARDS_OPEN.md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(file, $"# NO CARDS OPEN\r\nStart typing to find the word that you are looking for.", Windows.Storage.Streams.UnicodeEncoding.Utf8);

            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }
        public async static Task<string> ReadWebEmptyWord()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("EMPTY_SEARCH.md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(file, $"# EMPTY SEARCH\r\nSearch request must contain at least one character.", Windows.Storage.Streams.UnicodeEncoding.Utf8);

            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }


        public static string CheckText(string str)
        {
            str = str.ToLower();

            if (str.Contains("ß")) { str = str.Replace("ß", "ss"); }
            if (str.Contains("ä") || str.Contains("Ä")) { str = str.Replace("ä", "a"); str = str.Replace("Ä", "a"); }
            if (str.Contains("ö") || str.Contains("Ö")) { str = str.Replace("ö", "o"); str = str.Replace("Ö", "o"); }
            if (str.Contains("ü") || str.Contains("Ü")) { str = str.Replace("ü", "u"); str = str.Replace("Ü", "u"); }
            if (str.Contains("ё") || str.Contains("Ё")) { str = str.Replace("ё", "е"); str = str.Replace("Ё", "е"); }
            if (str.Contains("ъ") || str.Contains("Ъ")) { str = str.Replace("ъ", "ь"); str = str.Replace("Ъ", "ь"); }

            return str;
        }
        public static string CheckWord(string word)
        {
            if (word == null)
                return "";

            bool contains = false;

            foreach (string code in DictionaryService.ReadLanguagesCodes())
                contains = contains || word.Contains("_" + code);

            if (contains)
            {
                string result = "";

                if (word == null)
                    return result;

                int underscoresAmount = word.Split("_").Length - 1;
                string[] splittedWord = word.Split("_", underscoresAmount);

                foreach (string str in splittedWord)
                    if (str != splittedWord.Last())
                        result += str + " ";

                return result.Trim().Replace(" ", "_");
            }

            return word.Trim().Replace(" ", "_");
        }
        public static string CheckSource(string source)
        {
            List<string> types = NotesService.ReadSourceTypes();

            foreach (string type in types)
                source = source.Replace(type + "_", "");
            
            return source.Replace("_", " ").Trim().ToUpper();
        }
        public static string CheckSourceType(string str)
        {
            if (str == null) return str;

            if (Regex.IsMatch(str, @"^[a-zA-Z]+$"))
                return str;
            
            return null;
        }


        public static string CheckQuoteStamp(string quoteStamp)
        {
            Regex quoteString1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)");
            Regex quoteString2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
            Regex quoteString3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

            Regex quoteString4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)");
            Regex quoteString5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");


            if (quoteString4.IsMatch(quoteStamp) || quoteString5.IsMatch(quoteStamp))
            {
                var temp = Regex.Replace(quoteStamp, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                return String.Format("page {0} line {1}", temp[0], temp[1]);
            }
            else if (quoteString1.IsMatch(quoteStamp) && !quoteString2.IsMatch(quoteStamp) && !quoteString3.IsMatch(quoteStamp))
            {
                var temp = Regex.Replace(quoteStamp, @"[\s:_-]+", " ").Trim().Split(' ');

                return String.Format("00:00:{0}", temp[0]);
            }
            else if (quoteString1.IsMatch(quoteStamp) && quoteString2.IsMatch(quoteStamp) && !quoteString3.IsMatch(quoteStamp))
            {
                var temp = Regex.Replace(quoteStamp, @"[\s:_-]+", " ").Trim().Split(' ');

                return String.Format("00:{0}:{1}", temp[0], temp[1]);
            }
            else if (quoteString1.IsMatch(quoteStamp) && quoteString2.IsMatch(quoteStamp) && quoteString3.IsMatch(quoteStamp))
            {
                var temp = Regex.Replace(quoteStamp, @"[\s:_-]+", " ").Trim().Split(' ');
                return String.Format("{0}:{1}:{2}", temp[0], temp[1], temp[2]);
            }

            return quoteStamp;
        }
        public static string CheckNoteStamp(string noteStamp)
        {
            Regex noteRegex1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)");
            Regex noteRegex2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
            Regex noteRegex3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

            Regex noteRegex4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)([0-9]+)([\s]*)([lL]*)([\s]+)([0-9]+)([\s]*)");
            Regex noteRegex5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");


            if (noteRegex4.IsMatch(noteStamp) || noteRegex5.IsMatch(noteStamp))
            {
                var temp = Regex.Replace(noteStamp, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                return String.Format("page {0} line {1} - page {2} line {3}", temp[0], temp[1], temp[2], temp[3]);
            }
            else if (noteRegex1.IsMatch(noteStamp) && !noteRegex2.IsMatch(noteStamp) && !noteRegex3.IsMatch(noteStamp))
            {
                var temp = Regex.Replace(noteStamp, @"[\s:_-]+", " ").Trim().Split(' ');

                return String.Format("00:00:{0}-00:00:{1}", temp[0], temp[1]);
            }
            else if (noteRegex1.IsMatch(noteStamp) && noteRegex2.IsMatch(noteStamp) && !noteRegex3.IsMatch(noteStamp))
            {
                var temp = Regex.Replace(noteStamp, @"[\s:_-]+", " ").Trim().Split(' ');

                return String.Format("00:{0}:{1}-00:{2}:{3}", temp[0], temp[1], temp[2], temp[3]);
            }
            else if (noteRegex1.IsMatch(noteStamp) && noteRegex2.IsMatch(noteStamp) && noteRegex3.IsMatch(noteStamp))
            {
                var temp = Regex.Replace(noteStamp, @"[\s:_-]+", " ").Trim().Split(' ');

                return String.Format("{0}:{1}:{2}-{3}:{4}:{5}", temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
            }

            return noteStamp;
        }

    }
}
