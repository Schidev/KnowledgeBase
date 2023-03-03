using ColorCode.Compilation.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using UWP_PROJECT_06.Models.Dictionary;
using Windows.UI.Xaml.Shapes;
using MvvmHelpers;
using UWP_PROJECT_06.Models.History;
using System.Globalization;
using Windows.Storage.Streams;
using Microsoft.Toolkit.Uwp.Helpers;
using System.ServiceModel.Channels;
using Windows.UI.Popups;
using UWP_PROJECT_06.Models.Notes;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;

namespace UWP_PROJECT_06.Services
{
    public static class SettingsService
    {
        static string FileName = "Settings.xml";

        public async static Task Initialize()
        {
            await DictionaryService.InitializeDatabase();
            await NotesService.InitializeDatabase();
            await ProblemsService.InitializeDatabase();
            await BookmarksService.InitializeDatabase();
            await HistoryService.InitializeDatabase();

            await CreateSettingsFile();

            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            List<string> codes = DictionaryService.ReadLanguagesCodes();

            for (int i = 0; i < codes.Count; i++)
            {
                folderName = String.Format("\\{0}\\WORDS", codes[i][0].ToString().ToUpper() + codes[i].Substring(1));
                await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }
        }

        public async static Task CreateSettingsFile()
        {
            try
            {
                if (await ApplicationData.Current.LocalFolder.FileExistsAsync(FileName))
                    return;

                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.FailIfExists);

                using (IRandomAccessStream writeStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream s = writeStream.AsStreamForWrite();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Async = true;
                    settings.Indent = true;

                    using (XmlWriter writer = XmlWriter.Create(s, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("settings");

                            writer.WriteStartElement("pathes");

                                writer.WriteStartElement("vault");
                                writer.WriteString("VAULT");
                                writer.WriteEndElement();

                                writer.WriteStartElement("dictionary");
                                writer.WriteString("DICTIONARY");
                                writer.WriteEndElement();

                        writer.WriteStartElement("bookmarks");
                        writer.WriteString("GTD\\BOOKMARKS");
                        writer.WriteEndElement();

                       

                        List<string> sources = NotesService.ReadSourceTypes();

                        foreach (string source in sources)
                        {
                            writer.WriteStartElement(source.ToLower());
                            writer.WriteString(source.ToUpper()  + "S");
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();


                        writer.WriteStartElement("hotkeys");
                        
                                List<Hotkey> hotkeys = new List<Hotkey>() 
                        {
                            new Hotkey { Name = "MenuBack", Modifiers = "Contol,Menu", Key = "Left", Tip = "Navigate Back (Ctrl+Alt+Left)" },
                            new Hotkey { Name = "MenuForward", Modifiers = "Control,Menu", Key = "Right", Tip = "Navigate Forward (Ctrl+Alt+Right)" },
                            new Hotkey { Name = "MenuMore", Modifiers = "Menu", Key = "F", Tip = "More options (Alt+F)" },
                            new Hotkey { Name = "OpenMenuRightPanel", Modifiers = "Control,Menu,Shift", Key = "R", Tip = "Collapse (Ctrl+Alt+Shift+R)" },
                            new Hotkey { Name = "OpenMenuLeftPanel", Modifiers = "Control,Menu,Shift", Key = "L", Tip = "Expand/Collapse (Ctrl+Alt+Shift+L)" },
                            
                            new Hotkey { Name = "FocusOnPrimarySearch", Modifiers = "Menu", Key = "W", Tip = "Focus (Alt+W)" },
                            new Hotkey { Name = "FocusOnSecondarySearch", Modifiers = "Menu,Shift", Key = "W", Tip = "Focus (Alt+Shift+W)" },
                            new Hotkey { Name = "FocusOnPrimaryLanguagePicker", Modifiers = "Menu", Key = "L", Tip = "Select (Alt+L)" },
                            new Hotkey { Name = "FocusOnSecondaryLanguagePicker", Modifiers = "Menu,Shift", Key = "L", Tip = "Select (Alt+Shift+L)" },
                            
                            new Hotkey { Name = "TakeAScreenshot", Modifiers = "Menu", Key = "T", Tip = "Take a screenshot (Alt+T)" },
                            new Hotkey { Name = "ChangeMode", Modifiers = "Menu", Key = "C", Tip = "Change mode (Alt+C)" },
                            new Hotkey { Name = "OpenInBrowser", Modifiers = "Menu", Key = "B", Tip = "Open browser (Alt+B)" },
                            new Hotkey { Name = "AddNewCard", Modifiers = "Menu", Key = "N", Tip = "Add new card (Alt+N)" },
                            new Hotkey { Name = "SaveCard", Modifiers = "Menu", Key = "S", Tip = "Save changes (Alt+S)" },
                            new Hotkey { Name = "CardBack", Modifiers = "Menu", Key = "Left", Tip = "Navigation back (Alt+Left)" },
                            new Hotkey { Name = "CardForward", Modifiers = "Menu", Key = "Right", Tip = "Navigation forward (Alt+Right)" },
                            new Hotkey { Name = "CardRefresh", Modifiers = "Menu", Key = "R", Tip = "Refresh (Alt+R)" },
                            new Hotkey { Name = "CardDelete", Modifiers = "Contol,Menu,Shift", Key = "D", Tip = "Delete (Ctrl+Alt+Shift+D)" },
                            new Hotkey { Name = "CardClear", Modifiers = "Contol,Menu,Shift", Key = "C", Tip = "Clear all fields (Ctrl+Alt+Shift+C)" },

                            new Hotkey { Name = "OpenDictionary", Modifiers = "Menu", Key = "Number1", Tip = "Open dictionary (Alt+1)" },
                            new Hotkey { Name = "OpenNotes", Modifiers = "Menu", Key = "Number2", Tip = "Open notes (Alt+2)" },
                            new Hotkey { Name = "OpenSettings", Modifiers = "Menu", Key = "Number0", Tip = "Open settings (Alt+0)" },
                            new Hotkey { Name = "OpenHistory", Modifiers = "Control", Key = "H", Tip = "Open history (Ctrl+H)" },

                            new Hotkey { Name = "AddNewTab", Modifiers = "Control", Key = "T", Tip = "New tab (Ctrl+T)" },
                            new Hotkey { Name = "OpenRecentlyClosedTab", Modifiers = "Control,Shift", Key = "T", Tip = "Open recently closed tab (Ctrl+Shift+T)" },
                            new Hotkey { Name = "CloseCurrentTab", Modifiers = "Control", Key = "W", Tip = "Close current tab (Ctrl+W)" },
                            new Hotkey { Name = "NextTab", Modifiers = "Control", Key = "Tab", Tip = "Next tab (Ctrl+Tab)" },
                            new Hotkey { Name = "PreviousTab", Modifiers = "Control,Shift", Key = "Tab", Tip = "Previous tab (Ctrl+Shift+Tab)" },
                            
                            new Hotkey { Name = "OpenFirstTab", Modifiers = "Control", Key = "Number1", Tip = "Navigation first tab (Ctrl+1)" },
                            new Hotkey { Name = "OpenSecondTab", Modifiers = "Control", Key = "Number2", Tip = "Navigation second tab (Ctrl+2)" },
                            new Hotkey { Name = "OpenThirdTab", Modifiers = "Control", Key = "Number3", Tip = "Navigation third tab (Ctrl+3)" },
                            new Hotkey { Name = "OpenFourthTab", Modifiers = "Control", Key = "Number4", Tip = "Navigation forth tab (Ctrl+4)" },
                            new Hotkey { Name = "OpenFifthTab", Modifiers = "Control", Key = "Number5", Tip = "Navigation fifth tab (Ctrl+5)" },
                            new Hotkey { Name = "OpenSixthTab", Modifiers = "Control", Key = "Number6", Tip = "Navigation sixth tab (Ctrl+6)" },
                            new Hotkey { Name = "OpenSeventhTab", Modifiers = "Control", Key = "Number7", Tip = "Navigation seventh tab (Ctrl+7)" },
                            new Hotkey { Name = "OpenEighthTab", Modifiers = "Control", Key = "Number8", Tip = "Navigation eight tab (Ctrl+8)" },
                            
                            new Hotkey { Name = "OpenLastTab", Modifiers = "Control", Key = "Number9", Tip = "Navigation last tab (Ctrl+9)" }
                        };

                                foreach (Hotkey hotkey in hotkeys)
                                {
                                    writer.WriteStartElement("hotkey");
                                    writer.WriteAttributeString("Name", hotkey.Name);
                                    writer.WriteAttributeString("Modifiers", hotkey.Modifiers);
                                    writer.WriteAttributeString("Key", hotkey.Key);
                                    writer.WriteString(hotkey.Tip);
                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();

                            writer.WriteStartElement("colors");

                                writer.WriteStartElement("color");
                                writer.WriteAttributeString("key", "существительное_мужского_рода");
                                writer.WriteString("#FFD180");
                                writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "существительное_женского_рода");
                            writer.WriteString("#E6CEFF");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "существительное_среднего_рода");
                            writer.WriteString("#B9F6CA");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "только_множественное_число");
                            writer.WriteString("#E6CEFF");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "множественное_число_от");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "глагол");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "прилагательное");
                            writer.WriteString("#F8BBD0");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "наречие");
                            writer.WriteString("#FFF9C4");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "предлог");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "числительное");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "местоимение");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "союз");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "частица");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "междометие");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "притяжательное_местоимение");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "определяющее_слово");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                            writer.WriteStartElement("color");
                            writer.WriteAttributeString("key", "префикс");
                            writer.WriteString("#B2E6F3");
                            writer.WriteEndElement();

                        writer.WriteEndElement();

                        ////////////////////////////////////////////////////////////

                        writer.WriteStartElement("languages");

                            writer.WriteStartElement("language");
                            writer.WriteAttributeString("name", "Русский");
                            writer.WriteString("русский");
                            writer.WriteEndElement();

                        writer.WriteStartElement("language");
                        writer.WriteAttributeString("name", "Deutsch");
                        writer.WriteString("немецкий");
                        writer.WriteEndElement();

                        writer.WriteStartElement("language");
                        writer.WriteAttributeString("name", "English");
                        writer.WriteString("английский");
                        writer.WriteEndElement();

                        writer.WriteStartElement("language");
                        writer.WriteAttributeString("name", "Français");
                        writer.WriteString("французкий");
                        writer.WriteEndElement();

                        writer.WriteStartElement("language");
                        writer.WriteAttributeString("name", "Italiano");
                        writer.WriteString("итальянский");
                        writer.WriteEndElement();

                        writer.WriteStartElement("language");
                        writer.WriteAttributeString("name", "Español");
                        writer.WriteString("испанский");
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                            
                        ///////////////////////////////////////////////////////////
                        writer.WriteStartElement("history");
                        writer.WriteEndElement();


                        writer.WriteEndElement();
                        
                        writer.WriteEndDocument();
                        writer.Flush();
                        await writer.FlushAsync();
                    }
                }

                //await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
            catch
            {

            }


        }

        public async static Task RecreateDictionaryVoult()
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            
            foreach (Word word in DictionaryService.ReadWords())
                await MarkdownService.WriteWord(word, DictionaryService.ReadWordExtras(word.Id));
            
            timer.Stop();

            MessageDialog msg = new MessageDialog(String.Format("Dictionary was synchronized for {0} second!", (int)(timer.ElapsedMilliseconds / 1000) ), "Notification.");
            await msg.ShowAsync();
        }
        public async static Task ClearVault()
        {
            string vaultName = await SettingsService.ReadPath("vault");

            List<string> types = NotesService.ReadSourceTypes();

            for (int i = 0; i < types.Count; i++)
            {
                var t = await ReadPath(types[i].ToLower());
                string folderName = System.IO.Path.Combine(vaultName, t);
                var currentFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                await currentFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }
        public async static Task RecreateSourcesVoult()
        {
            foreach (Source source in NotesService.ReadSources())
                await MarkdownService.WriteSource(source, NotesService.ReadQuotes(source.Id), NotesService.ReadNotes(source.Id), NotesService.ReadSourceExtras(source.Id));
        }

        public async static Task CreatePath(string localName, string path)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
            var doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

            var root = doc.GetElementsByTagName("pathes");

            Windows.Data.Xml.Dom.XmlElement item = doc.CreateElement(localName.ToLower());
            item.InnerText = path;

            root[0].AppendChild(item);
            
            await doc.SaveToFileAsync(file);
        }
        public async static Task UpdatePath(string oldLocalName, string newLocalName)
        {
            string path = await ReadPath(oldLocalName);
            
            await DeletePath(oldLocalName);
            await CreatePath(newLocalName, path);
        }
        public async static Task<string> ReadPath(string localName)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "pathes")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
                if (node.LocalName == localName)
                    return node.InnerText;

            return null;
        }
        public async static Task WritePath(string localName, string path)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return;

            while (currentNode.LocalName != "pathes")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
                if (node.LocalName == localName)
                    node.InnerText = path;

            document.Save(settingsPath);
        }
        public async static Task DeletePath(string localName)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return;

            while (currentNode.LocalName != "pathes")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
                if (node.LocalName == localName)
                    currentNode.RemoveChild(node);

            document.Save(settingsPath);
        }


        public async static Task<string> ReadColor(int partOfSpeechId)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "colors")
                currentNode = currentNode.NextSibling;

            var partOfSpeech = DictionaryService.ReadPartOfSpeech(partOfSpeechId);
            
            foreach (XmlNode node in currentNode.ChildNodes)
            {
                if (node.Attributes["key"].Value == partOfSpeech)
                        return node.InnerText;
            }

            return null;
        }
        public async static Task WriteColor(string partOfSpeech, string color)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return;

            while (currentNode.LocalName != "colors")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                if (node.Attributes["key"].Value == partOfSpeech)
                    node.InnerText = color;
            }

            document.Save(settingsPath);
        }

        public async static Task<string> ReadHotkey(string hotkeyName, string attributeName = "")
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);
            
            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "hotkeys")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                if (node.Attributes["Name"].Value != hotkeyName)
                    continue;
             
                if (attributeName == "")
                    return node.InnerText;

                return node.Attributes[attributeName].Value;
            }
            
            return null;
        }
        public async static Task<IEnumerable<Hotkey>> ReadHotkeys()
        {
            var hotkeys = new List<Hotkey>();
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "hotkeys")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                hotkeys.Add(new Hotkey()
                {
                    Name = node.Attributes["Name"].Value,
                    Modifiers = node.Attributes["Modifiers"].Value,
                    Key = node.Attributes["Key"].Value,
                    Tip = node.InnerText
                });
            }

            return hotkeys;
        }

        public async static Task UpdateHotkey(string hotkeyName, string modifiers = "None", string key = "None")
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return;

            string _key = key;
            for (int i = 0; i < 10; i++)
            {
                if (key == i.ToString())
                {
                    _key = String.Format("Number{0}", i);
                    break;
                }
            }
            
            while (currentNode.LocalName != "hotkeys")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                if (node.Attributes["Name"].Value != hotkeyName)
                {
                    node.InnerText = node.InnerText;
                    continue;
                }
                node.Attributes["Modifiers"].Value = modifiers;
                node.Attributes["Key"].Value = _key;

                node.InnerText = String.Format("{0}",
                        node.InnerText.Split(" (")[0]);

                node.InnerText += modifiers == "None" && key == "None"
                    ? ""
                    : String.Format(" ({0}{1}{2}{3})",
                        modifiers.Contains("Ctrl") || modifiers.Contains("Control") ? "Ctrl+" : "",
                        modifiers.Contains("Alt") || modifiers.Contains("Menu") ? "Alt+" : "",
                        modifiers.Contains("Shift") || modifiers.Contains("Shift") ? "Shift+" : "",
                        key);
            }

            document.Save(settingsPath);
        }
       
        
        public async static Task<List<HistoryItem>> ReadDictionaryHistory()
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "history")
                currentNode = currentNode.NextSibling;

            var result = new List<HistoryItem>();

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                var historyItem = new HistoryItem();

                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    if (innerNode.LocalName == "fullpath")
                        historyItem.FullPath = innerNode.InnerText;
                    else if (innerNode.LocalName == "date")
                        historyItem.Date = DateTime.Parse(innerNode.InnerText);
                    else if (innerNode.LocalName == "action")
                        historyItem.Action = innerNode.InnerText;
                }

                result.Add(historyItem);
            }

            return result;
        }
        public async static Task WriteDictionaryHistory(string action, string fullPath)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
            var doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

            var root = doc.GetElementsByTagName("history");

            Windows.Data.Xml.Dom.XmlElement item = doc.CreateElement("historyitem");

            Windows.Data.Xml.Dom.XmlElement fullPathElement = doc.CreateElement("fullpath");
            fullPathElement.InnerText = fullPath;

            Windows.Data.Xml.Dom.XmlElement dateElement = doc.CreateElement("date");
            dateElement.InnerText = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss");

            Windows.Data.Xml.Dom.XmlElement actionElement = doc.CreateElement("action");
            actionElement.InnerText = action;

            root[0].AppendChild(item);
            root[0].LastChild.AppendChild(fullPathElement);
            root[0].LastChild.AppendChild(dateElement);
            root[0].LastChild.AppendChild(actionElement);

            await doc.SaveToFileAsync(file);
        }

        public async static Task WriteHistory(HistoryItem item) 
        {
            HistoryService.CreateHistoryItem(item);
        }
        public async static Task<List<HistoryItem>> ReadHistory()
        {
            return HistoryService.ReadHistoryItems();
        }

    }
}
