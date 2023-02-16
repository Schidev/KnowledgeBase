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

namespace UWP_PROJECT_06.Services
{
    public static class SettingsService
    {
        static string FileName = "Settings.xml";

        public async static Task Initialize()
        {
            await CreateSettingsFile();

            string vaultName = await SettingsService.ReadPath("vault");
            string folderName = await SettingsService.ReadPath("dictionary");
            folderName = System.IO.Path.Combine(vaultName, folderName);

            StorageFolder folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Rus\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Deu\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Eng\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Fra\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Ita\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            folderName = @"\Spa\WORDS";
            await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            //RecreateDictionaryVoult();
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

                                writer.WriteStartElement("videos");
                                writer.WriteString("VIDEOS");
                                writer.WriteEndElement();

                                writer.WriteStartElement("sounds");
                                writer.WriteString("SOUNDS");
                                writer.WriteEndElement();

                                writer.WriteStartElement("images");
                                writer.WriteString("IMAGES");
                                writer.WriteEndElement();

                                writer.WriteStartElement("documents");
                                writer.WriteString("DOCUMENTS");
                                writer.WriteEndElement();

                            writer.WriteEndElement();

                            writer.WriteStartElement("hotkeys");
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

                        writer.WriteStartElement("history");
                        writer.WriteEndElement();


                        writer.WriteEndElement();
                        
                        writer.WriteEndDocument();
                        writer.Flush();
                        await writer.FlushAsync();
                    }
                }

                await Windows.System.Launcher.LaunchFileAsync(storageFile);
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

        public async static Task<List<HistoryItem>> ReadHistory()
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
        
    }
}
