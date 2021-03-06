﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace EnglishHelper.Core
{
    public interface IDictionaryManager
    {
        ObservableCollection<Entry> WordDictionary { get; set; }
        int WordCount { get; }
        void InitializeDictionary();
        bool IsContainWord(string word);
        int GetCountByWord(string word);
        bool AddWord(string word);
        void DeleteWord(string word);
        string GetTranslation(string word);
        bool CreateDictionaryFile();
        void SaveDictionaryToFile();
        void LoadDictionaryFromFile();
        void FillEmptyValuesInRow(Entry item);
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
        private ObservableCollection<Entry> wordDictionary;
        private static string dictionaryLocation = ConfigurationManager.AppSettings["DictionaryFileName"];

        public event EventHandler DictionaryChanged;

        public ObservableCollection<Entry> WordDictionary
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
            wordDictionary = new ObservableCollection<Entry>();
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

        public int GetCountByWord(string word)
        {
            int count = 0;
            if (wordDictionary.Count > 0 && word != null)
                count = wordDictionary.Where(c => c.Word.ToLower() == word.ToLower()).Count();
            return count;
        }

        public bool AddWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                Logger.LogWarning("Word is null or contains only spaces.");
                return false;
            }
            else if (IsContainWord(word))
            {
                Logger.LogWarning("Dictionary has already contains '" + word + "'.");
                return false;
            }
            else
            {
                var lastLangOrientation = Translator.Instance.LanguageOrienation;
                Translator.Instance.LanguageOrienation = "en-ru";

                string translation = Translator.Instance.Text.ToLower() == word.ToLower() ? Translator.Instance.Translation : Translator.Instance.GetTranslatedString(word);
                wordDictionary.Add(new Entry { Word = word.ToLower(), Translation = translation.ToLower(), LastChangeDate = DateTime.Now.ToString() });

                Translator.Instance.LanguageOrienation = lastLangOrientation;
                return true;
            }
        }

        public void DeleteWord(string word)
        {
            if (word != null)
            {
                if (IsContainWord(word))
                {
                    wordDictionary.Remove(wordDictionary.Where(c => c.Word.ToLower() == word.ToLower()).Last());
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

        public void FillEmptyValuesInRow(Entry item)
        {
            var lastLangOrientation = Translator.Instance.LanguageOrienation;
            Translator.Instance.LanguageOrienation = "en-ru";

            //If word is left
            if (string.IsNullOrWhiteSpace(item.Word) && !string.IsNullOrWhiteSpace(item.Translation))
            {
                Translator.Instance.LanguageOrienation = "ru-en";
                item.Word = Translator.Instance.GetTranslatedString(item.Translation);
                Translator.Instance.LanguageOrienation = "en-ru";
            }

            //If translation is left
            if (!string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation))
                item.Translation = Translator.Instance.GetTranslatedString(item.Word);

            //If change date is left
            if (string.IsNullOrWhiteSpace(item.LastChangeDate))
                item.LastChangeDate = DateTime.Now.ToString();

            //If word and translation are left
            if (string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation) && !string.IsNullOrWhiteSpace(item.LastChangeDate))
            {
                item.Word = "autofixed value";
                item.Translation = "автозаполняемое значение";
            }
                
            item.Word = item.Word.ToLower();
            item.Translation = item.Translation.ToLower();
            Translator.Instance.LanguageOrienation = lastLangOrientation;
        }
    }
}
