using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnglishHelper.Core
{
    public interface IDictionaryManager
    {
        List<Entry> WordDictionary { get; set; }
        int WordCount { get; }
        void InitializeDictionary();
        bool IsContainWord(string word);
        bool AddWord(string word);
        void DeleteWord(string word);
        string GetTranslation(string word);
        bool CreateDictionaryFile();
        void SaveDictionaryToFile();
        void LoadDictionaryFromFile();
        void FillEmptyValuesInTable();
    }

    [Serializable]
    public class Entry : INotifyPropertyChanged
    {
        private string mWord;
        private string mTranslation;
        private string mLastChangeDate;

        public string Word
        {
            get { return mWord; }
            set
            {
                if (value != mWord)
                {
                    this.mWord = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Translation
        {
            get { return mTranslation; }
            set
            {
                if (value != mTranslation)
                {
                    this.mTranslation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string LastChangeDate
        {
            get { return mLastChangeDate; }
            set
            {
                if (value != mLastChangeDate)
                {
                    this.mLastChangeDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [Serializable]
    [XmlRootAttribute("DictionaryManager")]
    public class DictionaryManager: IDictionaryManager
    {
        private List<Entry> wordDictionary;
        private static string dictionaryLocation = ConfigurationManager.AppSettings["DictionaryFileName"];

        public event EventHandler DictionaryChanged;

        public List<Entry> WordDictionary
        {
            get { return wordDictionary; }
            set
            {
                wordDictionary = value;
                if (DictionaryChanged != null)
                    DictionaryChanged(this, EventArgs.Empty);
            }
        }

        public DictionaryManager()
        {
            wordDictionary = new List<Entry>();
        }
       
        public int WordCount { get { return wordDictionary.Count; } }

        public void InitializeDictionary()
        {
            Logger.LogInfo("Initialing existing dictionary...");

            bool isFileExist = CreateDictionaryFile();

            LoadDictionaryFromFile();

            if (WordDictionary == null)
            {
                Logger.LogError("Dictionary isn't valid");
                return;
            }

            Logger.LogInfo("Dictionary was initialized.");
        }

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
                string translation = Translator.Instance.Text.ToLower() == word.ToLower() ? Translator.Instance.Translation : Translator.Instance.GetTranslatedString(word);
                wordDictionary.Add(new Entry { Word = word, Translation = translation, LastChangeDate = DateTime.Now.ToString() });
                return true;
            }
        }

        public void DeleteWord(string word)
        {
            if (word != null)
            {
                if (IsContainWord(word))
                {
                    wordDictionary.Remove(wordDictionary.Where(c => c.Word.ToLower() == word.ToLower()).First());
                }
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

        public void FillEmptyValuesInTable()
        {
            foreach(var item in wordDictionary)
            {
                if (item.Word == null || item.Translation == null|| item.LastChangeDate == null)
                {
                    //Logger.LogWarning("Found empty value in item:" +);
                    if (string.IsNullOrWhiteSpace(item.Word) && !string.IsNullOrWhiteSpace(item.Translation))
                        item.Word = item.Translation;

                    if (!string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation))
                        item.Translation = Translator.Instance.GetTranslatedString(item.Word);

                    if (string.IsNullOrWhiteSpace(item.LastChangeDate))
                        item.LastChangeDate = DateTime.Now.ToString();

                    if (string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation) && !string.IsNullOrWhiteSpace(item.LastChangeDate))
                        item.Word = item.Translation = "Autofixed value";
                }
            }
        }
        public void FillEmptyValuesInRow(Entry row)
        {
            if (row.Word == null || row.Translation == null || row.LastChangeDate == null)
            {
                //Logger.LogWarning("Found empty value in item:" +);
                if (row.Translation == null)
                {
                    row.Translation = Translator.Instance.GetTranslatedString(row.Word);
                }
                if (row.LastChangeDate == null)
                {
                    row.LastChangeDate = DateTime.Now.ToString();
                }
            }

        }
    }
}
