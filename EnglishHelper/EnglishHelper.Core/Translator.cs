﻿using System.Text;
using System.Net;
using System.Configuration;

namespace EnglishHelper.Core
{
    public interface ITranslator
    {
        string LanguageOrienation { get; set; }
        string Text { get; set; }
        string GetTranslatedString(string word);
    }

    public class Translator : ITranslator
    {
        private static string mUri = string.Empty;
        private static string mAddress = ConfigurationManager.AppSettings["TranslateAddress"];
        private static string mLanguageOrienation = "en-ru";
        private static string mText = string.Empty;
        private static string mTranslation = string.Empty;

        private static Translator instance;

        protected Translator() { }

        public static Translator Instance
        {
            get { return instance ?? (instance = new Translator()); }
        }

        public string LanguageOrienation
        {
            get { return mLanguageOrienation; }
            set { mLanguageOrienation = value; }
        }

        private string Key
        {
            get { return KeyManager.Instance.Key; }
        }

        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }
        public string Translation
        {
            get { return mTranslation; }
            set { mTranslation = value; }
        }

        private void BuildUri()
        {
            mUri = string.Concat(mAddress, "key=", Key, "&lang=", LanguageOrienation, "&text=", Text);
        }

        private string GetJsonString()
        {
            string jsonString = string.Empty;

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            try
            {
                Logger.LogInfo("Retrieving translation...");
                Logger.LogInfo("Using URI: " + mUri);

                jsonString = wc.DownloadString(mUri);

                Logger.LogInfo("Translation was sucessfully received.");
            }
            catch (WebException wex)
            {
                Logger.LogError("Can't get string from server:" + wex.Message);
                jsonString = null;
            }

            return jsonString;
        }

        public string GetTranslatedString(string word)
        {
            mText = word;
            string jsonString = string.Empty;

            BuildUri();
            jsonString = GetJsonString();
            if (string.IsNullOrEmpty(jsonString))
                return null;

            string translation = JsonHelper.Parse(jsonString);
            mTranslation = translation;

            return translation;
        }
    }
}
