using ColorCode;
using Microsoft.Toolkit.Uwp.Helpers;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Views;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls.Primitives;

namespace UWP_PROJECT_06.Services
{
    public static class MarkdownService
    {
        public async static Task<string> ReadWord(string word)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string folderName = await SettingsService.ReadPath("dictionary");
            
            await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            StorageFolder folder = await localFolder.GetFolderAsync(folderName);

            if (File.Exists(Path.Combine(folder.Path, word + ".md")))
                return await folder.ReadTextFromFileAsync(Path.Combine(folder.Path, word + ".md"));

            StorageFile file = await localFolder.CreateFileAsync("WORD_NOT_FOUND.md", CreationCollisionOption.ReplaceExisting);
            
            await Windows.Storage.FileIO.WriteTextAsync(file, $"# File {word}.md does not exist or path is wrong\n{folder.Path}\\{word}.md");
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }

        public async static Task WriteWord(Word word, IEnumerable<WordExtra> wordExtras)
        {
            string folderName = await SettingsService.ReadPath("dictionary");
            if (word.Language == 1) folderName += @"\Rus\WORDS";
            if (word.Language == 2) folderName += @"\Deu\WORDS";
            if (word.Language == 3) folderName += @"\Eng\WORDS";
            if (word.Language == 4) folderName += @"\Fra\WORDS";
            if (word.Language == 5) folderName += @"\Ita\WORDS";
            if (word.Language == 6) folderName += @"\Spa\WORDS";

            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(folderName);
            StorageFile file = await folder.CreateFileAsync(word.Word1 + ".md", CreationCollisionOption.ReplaceExisting);

            string[] extras = new string[14];

            #region Getting extras for word

            for (int q = 0; q < extras.Length; q++)
                extras[q] = "";

            int underscoresAmountWord = word.Word1.Split("_").Length - 1;
            string[] splittedTextWord = word.Word1.Split("_", underscoresAmountWord);

            foreach (string str in splittedTextWord)
                if (str != splittedTextWord.Last())
                    extras[0] += str;

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

                        int underscoresAmount = extraTextWithUnderscores.Split("_").Length - 1;
                        string[] splittedText = extraTextWithUnderscores.Split("_", underscoresAmount);
                        string extraText = "";

                        foreach (string str in splittedText)
                            if (str != splittedText.Last())
                                extraText += str;

                        if (extra != group.Items.Last())
                        {
                            extras[group.Key] += String.Format("[{0}]({1}.md), ", extraText, extraTextWithUnderscores);
                        }
                        else
                        {
                            extras[group.Key] += String.Format("[{0}]({1}.md)", extraText, extraTextWithUnderscores);
                        }
                    }
                }
            }

            #endregion

            #region Building string

            

            if (word.PartOfSpeech == 5)
            {
                var colorId = DictionaryService.ReadWord(extras[1].Split("(")[1].Split(".md)")[0]).PartOfSpeech;

                StringBuilder pluralOutput = new StringBuilder();

                pluralOutput.AppendLine(String.Format("## Слово: <mark style=\"background: {0};\">{1}</mark>", await SettingsService.ReadColor(colorId), extras[0]));
                pluralOutput.AppendLine(String.Format("**Язык:** {0}.", DictionaryService.ReadLanguage(word.Language)));
                pluralOutput.AppendLine();
                pluralOutput.AppendLine(String.Format("**См. слово:** {0}.", extras[1]));

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
                output.AppendLine(String.Format("**Часть речи:** {0}.", DictionaryService.ReadPartOfSpeech(word.PartOfSpeech).Replace("_"," ")));
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
                output.Append(word.Language == 4 ? "" : String.Format("\n**Перевод на итальянский:** {0}.", extras[11]));
                output.Append(word.Language == 5 ? "" : String.Format("\n**Перевод на испанский:** {0}.", extras[12]));
                output.Append(word.Language == 6 ? "" : String.Format("\n**Перевод на французский:** {0}.", extras[13]));


                await Windows.Storage.FileIO.WriteTextAsync(file, output.ToString());

            }

            #endregion

        }

    }
}
