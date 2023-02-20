using ColorCode;
using Microsoft.Toolkit.Uwp.Helpers;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Views;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
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

            if (word.Language == 1) folderName += @"\Rus\WORDS";
            if (word.Language == 2) folderName += @"\Deu\WORDS";
            if (word.Language == 3) folderName += @"\Eng\WORDS";
            if (word.Language == 4) folderName += @"\Fra\WORDS";
            if (word.Language == 5) folderName += @"\Ita\WORDS";
            if (word.Language == 6) folderName += @"\Spa\WORDS";

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteDictionaryHistory("Read", path);

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

            if (word.Language == 1) folderName += @"\Rus\WORDS";
            if (word.Language == 2) folderName += @"\Deu\WORDS";
            if (word.Language == 3) folderName += @"\Eng\WORDS";
            if (word.Language == 4) folderName += @"\Fra\WORDS";
            if (word.Language == 5) folderName += @"\Ita\WORDS";
            if (word.Language == 6) folderName += @"\Spa\WORDS";

            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteDictionaryHistory("Updated", path);
            }
            else
            {
                var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                await SettingsService.WriteDictionaryHistory("Created", path);
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

                        string extraText = CheckWord(extraTextWithUnderscores);

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
                    await SettingsService.WriteDictionaryHistory("Updated", path);
                }
                else
                {
                    var path = System.IO.Path.Combine(folder.Path, word.Word1 + ".md");
                    await SettingsService.WriteDictionaryHistory("Created", path);
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
                output.Append(word.Language == 1 ? "" : String.Format("\n**Перевод на русский:** {0}.", extras[8]));
                output.Append(word.Language == 2 ? "" : String.Format("\n**Перевод на немецкий:** {0}.", extras[9]));
                output.Append(word.Language == 3 ? "" : String.Format("\n**Перевод на английский:** {0}.", extras[10]));
                output.Append(word.Language == 5 ? "" : String.Format("\n**Перевод на итальянский:** {0}.", extras[11]));
                output.Append(word.Language == 6 ? "" : String.Format("\n**Перевод на испанский:** {0}.", extras[12]));
                output.Append(word.Language == 4 ? "" : String.Format("\n**Перевод на французский:** {0}.", extras[13]));

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

            if (oldWord.Language == 1) fullPath += @"\Rus\WORDS";
            if (oldWord.Language == 2) fullPath += @"\Deu\WORDS";
            if (oldWord.Language == 3) fullPath += @"\Eng\WORDS";
            if (oldWord.Language == 4) fullPath += @"\Fra\WORDS";
            if (oldWord.Language == 5) fullPath += @"\Ita\WORDS";
            if (oldWord.Language == 6) fullPath += @"\Spa\WORDS";

            StorageFolder folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);

            string fileInnerText = "";
            
            if (await folder.FileExistsAsync(oldWord.Word1 + ".md"))
            {
                StorageFile oldFile = await folder.CreateFileAsync(oldWord.Word1 + ".md", CreationCollisionOption.OpenIfExists);
                fileInnerText = await Windows.Storage.FileIO.ReadTextAsync(oldFile);

                var path = oldFile.Path;
                await SettingsService.WriteDictionaryHistory("Deleted", path);

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

                if (newWord.Language == 1) fullPath += @"\Rus\WORDS";
                if (newWord.Language == 2) fullPath += @"\Deu\WORDS";
                if (newWord.Language == 3) fullPath += @"\Eng\WORDS";
                if (newWord.Language == 4) fullPath += @"\Fra\WORDS";
                if (newWord.Language == 5) fullPath += @"\Ita\WORDS";
                if (newWord.Language == 6) fullPath += @"\Spa\WORDS";

                folder = await localFolder.GetFolderAsync(fullPath);
            }

            StorageFile newFile = await folder.CreateFileAsync(newWord.Word1 + ".md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(newFile, fileInnerText);

            var FullPath = newFile.Path;
            await SettingsService.WriteDictionaryHistory("Created", FullPath);

            #endregion
        }
        public async static Task DeleteFile(Word word)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);


            if (word.Language == 1) folderName += @"\Rus\WORDS";
            if (word.Language == 2) folderName += @"\Deu\WORDS";
            if (word.Language == 3) folderName += @"\Eng\WORDS";
            if (word.Language == 4) folderName += @"\Fra\WORDS";
            if (word.Language == 5) folderName += @"\Ita\WORDS";
            if (word.Language == 6) folderName += @"\Spa\WORDS";

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(word.Word1 + ".md"))
            {
                StorageFile file = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();

                string path = file.Path;
                await SettingsService.WriteDictionaryHistory("Deleted", path);
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

            if (word.Language == 1) fullPath += @"\Rus\WORDS";
            if (word.Language == 2) fullPath += @"\Deu\WORDS";
            if (word.Language == 3) fullPath += @"\Eng\WORDS";
            if (word.Language == 4) fullPath += @"\Fra\WORDS";
            if (word.Language == 5) fullPath += @"\Ita\WORDS";
            if (word.Language == 6) fullPath += @"\Spa\WORDS";

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

            var F = "";
            if (source.SourceType == 1) F = "videos";
            if (source.SourceType == 2) F = "sounds";
            if (source.SourceType == 3) F = "images";
            if (source.SourceType == 4) F = "documents";

            string folderName = await SettingsService.ReadPath(F);

            folderName = System.IO.Path.Combine(vaultName, folderName);

            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteDictionaryHistory("Read", path);

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
            if (source.SourceType == 1) F = "videos";
            if (source.SourceType == 2) F = "sounds";
            if (source.SourceType == 3) F = "images";
            if (source.SourceType == 4) F = "documents";
            
            string folderName = await SettingsService.ReadPath(F);
            
            
            folderName = System.IO.Path.Combine(vaultName, folderName);
            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteDictionaryHistory("Updated", path);
            }
            else
            {
                var path = System.IO.Path.Combine(folder.Path, source.SourceName + ".md");
                await SettingsService.WriteDictionaryHistory("Created", path);
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
            Regex noteRegex1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)");
            Regex noteRegex2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
            Regex noteRegex3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s-]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

            Regex noteRegex4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)([0-9]+)([\s]*)([lL]*)([\s]+)([0-9]+)([\s]*)");
            Regex noteRegex5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s-]([\s]*)([pP]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");

            foreach (Note note in notes)
            {
                string result = "";

                if (noteRegex4.IsMatch(note.Stamp) || noteRegex5.IsMatch(note.Stamp))
                {
                    var temp = Regex.Replace(note.Stamp, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                    result = String.Format("page {0} line {1} - page {2} line {3}", temp[0], temp[1], temp[2], temp[3]);
                }
                else if (noteRegex1.IsMatch(note.Stamp) && !noteRegex2.IsMatch(note.Stamp) && !noteRegex3.IsMatch(note.Stamp))
                {
                    var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                    result = String.Format("00:00:{0}-00:00:{1}", temp[0], temp[1]);
                }
                else if (noteRegex1.IsMatch(note.Stamp) && noteRegex2.IsMatch(note.Stamp) && !noteRegex3.IsMatch(note.Stamp))
                {
                    var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                    result = String.Format("00:{0}:{1}-00:{2}:{3}", temp[0], temp[1], temp[2], temp[3]);
                }
                else if (noteRegex1.IsMatch(note.Stamp) && noteRegex2.IsMatch(note.Stamp) && noteRegex3.IsMatch(note.Stamp))
                {
                    var temp = Regex.Replace(note.Stamp, @"[\s:_-]+", " ").Trim().Split(' ');

                    result = String.Format("{0}:{1}:{2}-{3}:{4}:{5}", temp[0], temp[1], temp[2], temp[3], temp[4], temp[5]);
                }
                else 
                {
                    result = note.Stamp;
                }

                output.AppendLine(String.Format("{0}. {1} - {2}.", counter++, result, note.Title));
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

            Regex quoteString1 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)");
            Regex quoteString2 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");
            Regex quoteString3 = new Regex(@"([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)[\s:]([\s]*)[0-9][0-9]([\s]*)");

            Regex quoteString4 = new Regex(@"([\s]*)[pP]([\s]*)([0-9]+)([\s]+)([lL]*)([\s]*)([0-9]+)([\s]*)");
            Regex quoteString5 = new Regex(@"([\s]*)[pP]([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)[\s:]([\s]*)([lL]*)([\s]*)[\s_]([\s]*)([0-9]+)([\s]*)");

            foreach (Quote quote in quotes)
            {
                #region Quote begin

                if (quoteString4.IsMatch(quote.QuoteBegin) || quoteString5.IsMatch(quote.QuoteBegin))
                {
                    var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                    quote.QuoteBegin = String.Format("page {0} line {1}", temp[0], temp[1]);
                }
                else if (quoteString1.IsMatch(quote.QuoteBegin) && !quoteString2.IsMatch(quote.QuoteBegin) && !quoteString3.IsMatch(quote.QuoteBegin))
                {
                    var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteBegin = String.Format("00:00:{0}", temp[0]);
                }
                else if (quoteString1.IsMatch(quote.QuoteBegin) && quoteString2.IsMatch(quote.QuoteBegin) && !quoteString3.IsMatch(quote.QuoteBegin))
                {
                    var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteBegin = String.Format("00:{0}:{1}", temp[0], temp[1]);
                }
                else if (quoteString1.IsMatch(quote.QuoteBegin) && quoteString2.IsMatch(quote.QuoteBegin) && quoteString3.IsMatch(quote.QuoteBegin))
                {
                    var temp = Regex.Replace(quote.QuoteBegin, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteBegin = String.Format("{0}:{1}:{2}", temp[0], temp[1], temp[2]);
                }
                else 
                {
                    quote.QuoteBegin = quote.QuoteBegin;
                }

                #endregion
                #region Quote end
                
                if (quoteString4.IsMatch(quote.QuoteEnd) || quoteString5.IsMatch(quote.QuoteEnd))
                {
                    var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-lLpP]+", " ").Trim().Split(' ');

                    quote.QuoteEnd = String.Format("page {0} line {1}", temp[0], temp[1]);
                }
                else if (quoteString1.IsMatch(quote.QuoteEnd) && !quoteString2.IsMatch(quote.QuoteEnd) && !quoteString3.IsMatch(quote.QuoteEnd))
                {
                    var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteEnd = String.Format("00:00:{0}", temp[0]);
                }
                else if (quoteString1.IsMatch(quote.QuoteEnd) && quoteString2.IsMatch(quote.QuoteEnd) && !quoteString3.IsMatch(quote.QuoteEnd))
                {
                    var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteEnd = String.Format("00:{0}:{1}", temp[0], temp[1]);
                }
                else if (quoteString1.IsMatch(quote.QuoteEnd) && quoteString2.IsMatch(quote.QuoteEnd) && quoteString3.IsMatch(quote.QuoteEnd))
                {
                    var temp = Regex.Replace(quote.QuoteEnd, @"[\s:_-]+", " ").Trim().Split(' ');

                    quote.QuoteEnd = String.Format("{0}:{1}:{2}", temp[0], temp[1], temp[2]);
                }
                else 
                {
                    quote.QuoteEnd = quote.QuoteEnd;
                }

                #endregion
                #region Original quote

                quote.OriginalQuote = quote.OriginalQuote.Replace("<br>","\r\r");
                quote.OriginalQuote = quote.OriginalQuote.Replace("\r\r","\r>");

                #endregion
                #region Original quote

                quote.TranslatedQuote = quote.TranslatedQuote.Replace("<br>", "\r\r");
                quote.TranslatedQuote = quote.TranslatedQuote.Replace("\r\r", "\r>");

                #endregion

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
            if (oldSource.SourceType == 1) F = "videos";
            if (oldSource.SourceType == 2) F = "sounds";
            if (oldSource.SourceType == 3) F = "images";
            if (oldSource.SourceType == 4) F = "documents";

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
                await SettingsService.WriteDictionaryHistory("Deleted", path);

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
                if (newSource.SourceType == 1) F = "videos";
                if (newSource.SourceType == 2) F = "sounds";
                if (newSource.SourceType == 3) F = "images";
                if (newSource.SourceType == 4) F = "documents";

                folderName = await SettingsService.ReadPath(F);

                folderName = System.IO.Path.Combine(vaultName, folderName);

                fullPath = folderName;
                folder = await localFolder.CreateFolderAsync(fullPath, CreationCollisionOption.OpenIfExists);
            }

            StorageFile newFile = await folder.CreateFileAsync(newSource.SourceName + ".md", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteTextAsync(newFile, fileInnerText);

            var FullPath = newFile.Path;
            await SettingsService.WriteDictionaryHistory("Created", FullPath);

            #endregion
        }
        public async static Task DeleteFile(Source source)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string vaultName = await SettingsService.ReadPath("vault");

            var F = "";
            if (source.SourceType == 1) F = "videos";
            if (source.SourceType == 2) F = "sounds";
            if (source.SourceType == 3) F = "images";
            if (source.SourceType == 4) F = "documents";

            string folderName = await SettingsService.ReadPath(F);

            folderName = System.IO.Path.Combine(vaultName, folderName);



            StorageFolder folder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            if (await folder.FileExistsAsync(source.SourceName + ".md"))
            {
                StorageFile file = await folder.CreateFileAsync(source.SourceName + ".md", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();

                string path = file.Path;
                await SettingsService.WriteDictionaryHistory("Deleted", path);
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
        public static string CheckWord(string wordWithUnderscores)
        {
            string result = "";

            if (wordWithUnderscores == null)
                return result;

            int underscoresAmount = wordWithUnderscores.Split("_").Length - 1;
            string[] splittedWord = wordWithUnderscores.Split("_", underscoresAmount);

            foreach (string str in splittedWord)
                if (str != splittedWord.Last())
                    result += str;

            return result;
        }

    }
}
