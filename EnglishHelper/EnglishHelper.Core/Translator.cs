using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace EnglishHelper.Core
{
    public interface ITranslator
    {
        string LanguageOrienation { get; set; }
        string Text { get; set; }
        string Key { get; set; }
        string GetTranslatedString();
    }

    public class Translator : ITranslator
    {
        private string mUri = string.Empty;
        private string mAddress = string.Empty;
        private string mKey = string.Empty;
        private string mLanguageOrienation = string.Empty;
        private string mText = string.Empty;

        public Translator()
        {
            mAddress = "https://translate.yandex.net/api/v1.5/tr.json/translate?";
            mLanguageOrienation = "en-ru";
            mText = string.Empty;
            BuildUri();
        }
        public string LanguageOrienation
        {
            get { return mLanguageOrienation; }
            set { mLanguageOrienation = value; }
        }

        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        public string Key
        {
            get { return mKey; }
            set { mKey = value; }
        }

        private void BuildUri()
        {
            mUri = string.Concat(mAddress, "key=", mKey, "&lang=", LanguageOrienation, "&text=", Text);
        }

        private string GetJsonString()
        {
            string jsonString = string.Empty;

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            try
            {
                jsonString = wc.DownloadString(mUri);
            }
            catch (WebException wex)
            {
                jsonString = null;
            }

            return jsonString;
        }

        public string GetTranslatedString()
        {
            string jsonString = string.Empty;

            BuildUri();
            jsonString = GetJsonString();
            if (string.IsNullOrEmpty(jsonString))
                return null;

            return JsonHelper.Parse(jsonString);
        }
    }
}
