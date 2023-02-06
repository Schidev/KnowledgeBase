using ColorCode.Compilation.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace UWP_PROJECT_06.Services
{
    public static class SettingsService
    {
        static string FileName = "Settings.xml";

        public async static Task<string> ReadPath(string localName)
        {
            XmlDocument document = new XmlDocument();

            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string settingsPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

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
            string settingsPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

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
            string settingsPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            document.Load(settingsPath);

            XmlNode root = document.DocumentElement;
            XmlNode currentNode = root.ChildNodes.Count != 0 ? root.ChildNodes.Item(0) : null;

            if (currentNode == null)
                return null;

            while (currentNode.LocalName != "colors")
                currentNode = currentNode.NextSibling;

            foreach (XmlNode node in currentNode.ChildNodes)
            {
                if (node.Attributes["key"].Value == DictionaryService.ReadPartOfSpeech(partOfSpeechId))
                        return node.InnerText;
            }

            return null;
        }


    }
}
