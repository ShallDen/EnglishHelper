﻿using System;
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
        SerializableDictionary<string, string> WordDictionary { get; set; }
        int WordCount { get; }
        void AddWord(string word);
        void DeleteWord(string word);
        string GetTranslation(string word);
        bool CreateDictionaryFile();

        void SaveDictionaryToFile();

        void LoadDictionaryFromFile();
    }

    [Serializable]
    [XmlRootAttribute("DictionaryManager")]
    public class DictionaryManager: IDictionaryManager
    {
        private SerializableDictionary<string, string> wordDictionary;
        private static string dictionaryLocation = "Dictionary.xml";

        public SerializableDictionary<string, string> WordDictionary
        {
            get { return wordDictionary; }
            set { wordDictionary = value; }
        }

        public DictionaryManager()
        {
            wordDictionary = new SerializableDictionary<string, string>();
        }
        //{
        //    wordDictionary = new SerializableDictionary<string, string>();
        //    bool isFileExist = CreateDictionaryFile();

        //    LoadDictionaryFromFile();

        //    if (wordDictionary != null)
        //        return;
        //    else
        //    {
        //        //"Dictionary isn't valid");
        //    }
        //}

        public int WordCount { get { return wordDictionary.Count; } }

        public void AddWord(string word)
        {
            if (!wordDictionary.ContainsKey(word))
            {
                Translator translator = new Translator();
                translator.Key = KeyManager.LoadKey();
                string translation = translator.GetTranslatedString(word);
                wordDictionary.Add(word, translation);
                SaveDictionaryToFile();
            }
                
            //else
                //throw new Exception("Dictionary has already contains " + word);
        }

        public void DeleteWord(string word)
        {
            if (wordDictionary.ContainsKey(word))
            {
                wordDictionary.Remove(word);
            }
        }

        public string GetTranslation(string word)
        {
            string translation = string.Empty;

            if(wordDictionary.ContainsKey(word))
            {
                translation = wordDictionary.Where(c=>c.Key == word).Select(c=>c.Value).First();
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
