using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace EnglishHelper.Core
{
    public interface IDictionaryManager
    {
        List<Entry> WordDictionary { get; set; }
        int WordCount { get; }
        bool IsContainWord(string word);
        bool AddWord(string word);
        void DeleteWord(string word);
        string GetTranslation(string word);
        bool CreateDictionaryFile();

        void SaveDictionaryToFile();

        void LoadDictionaryFromFile();
    }

    [Serializable]
    public class Entry
    {
        public string Word { get; set; }
        public string Translation { get; set; }
    }

    [Serializable]
    [XmlRootAttribute("DictionaryManager")]
    public class DictionaryManager: IDictionaryManager
    {
        private List<Entry> wordDictionary;
        private static string dictionaryLocation = "Dictionary.xml";

        public List<Entry> WordDictionary
        {
            get { return wordDictionary; }
            set { wordDictionary = value; }
        }

        public DictionaryManager()
        {
            wordDictionary = new List<Entry>();
        }

        public int WordCount { get { return wordDictionary.Count; } }

        public bool IsContainWord(string word)
        {
            bool isContains = false;
            if(wordDictionary.Count > 0)
                isContains = wordDictionary.Where(c=>c.Word.ToLower() == word.ToLower()).Any();
            return isContains;
        }

        public bool AddWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;
            else if (IsContainWord(word))
                return false;      //throw new Exception("Dictionary has already contains " + word);
            else
            {
                Translator translator = new Translator();
                translator.Key = KeyManager.LoadKey();
                string translation = translator.GetTranslatedString(word);
                wordDictionary.Add(new Entry { Word = word, Translation = translation });
                SaveDictionaryToFile();
                return true;
            }
        }

        public void DeleteWord(string word)
        {
            if (IsContainWord(word))
            {
                wordDictionary.Remove(wordDictionary.Where(c=>c.Word.ToLower() == word.ToLower()).First());
            }
        }

        public string GetTranslation(string word)
        {
            string translation = string.Empty;

            if(IsContainWord(word))
            {
                translation = wordDictionary.Where(c=>c.Word.ToLower() == word.ToLower()).Select(c=>c.Translation).First();
            }

            return translation;
        }

        private bool CheckDictionaryFileExists()
        {
            return File.Exists(dictionaryLocation);
        }

        public bool CreateDictionaryFile()
        {
            if (CheckDictionaryFileExists())
                return true;
            else
            {
                SerializationHelper.Serialize(dictionaryLocation, this);
                return CheckDictionaryFileExists();
            }
        }

        public void SaveDictionaryToFile()
        {
            SerializationHelper.Serialize(dictionaryLocation, this);
        }

        public void LoadDictionaryFromFile()
        {
            wordDictionary = (SerializationHelper.Deserialize(dictionaryLocation, typeof(DictionaryManager)) as DictionaryManager).wordDictionary;
        }
    }
}
