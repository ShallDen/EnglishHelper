using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishHelper.Core
{
    public interface IKeyManager
    {
        bool IsKeyValid { get; set; }
        string Key { get; set; }
        bool ValidateKey();
    }

    public class KeyManager: IKeyManager
    {
        private static string userKeyLocation = "API key.xml";
        public string userKey = string.Empty;

        public string Key
        {
            get { return userKey; }
            set { userKey = value; }
        }

        public bool IsKeyValid { get; set; }
        
        public bool ValidateKey()
        {
            Translator translator = new Translator();
            translator.Text = "Ping";
            translator.Key = Key;

            if (!string.IsNullOrEmpty(translator.GetTranslatedString()))
                return true;
            else
                return false;
        }
    }
}
