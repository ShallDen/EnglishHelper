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
        string GetTranslatedString();
    }

    public class Translator: ITranslator
    {
        private string mUri = string.Empty;
        private string mAddress = string.Empty;
        private string mKey = string.Empty;
        private string mLanguageOrienation = string.Empty;
        private string mText = string.Empty;

        public Translator()
        {
            mAddress = "https://translate.yandex.net/api/v1.5/tr.json/translate?";
            mKey = "key=trnsl.1.1.20160204T223148Z.ffe338b0a2031691.9161850fdfcb8c026e81dc08e5f3f2ffaae6a602";
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
            set { mText =  value; }
        }

        private void BuildUri()
        {
            mUri = string.Concat(mAddress, mKey, "&lang=", LanguageOrienation, "&text=", Text);
        }

        private string GetJsonString()
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            return wc.DownloadString(mUri);
        }

        public string GetTranslatedString()
        {
            string jsonString = string.Empty;

            BuildUri();
            jsonString = GetJsonString();

            return JsonHelper.Parse(jsonString);
        }
    }
}
