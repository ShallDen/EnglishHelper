using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

namespace EnglishHelper.Core
{
    public interface IKeyManager
    {
        bool IsKeyValid { get; set; }
        string Key { get; set; }
        bool CreateKeyFile();
        void LoadKeyFromFile();
        void SaveKey();
        bool ValidateKey();
    }

    [Serializable]
    [XmlRootAttribute("KeyManager")]
    public class KeyManager : IKeyManager
    {
        private static string userKeyLocation = ConfigurationManager.AppSettings["KeyFileName"];

        public string Key { get; set; }
        [XmlIgnore]
        public bool IsKeyValid { get; set; }

        public bool ValidateKey()
        {
            Translator translator = new Translator { Key = Key };
            bool isValid = string.IsNullOrEmpty(translator.GetTranslatedString("Ping")) ? false : true;
            if (isValid)
                Logger.LogInfo("Key is valid");
            else
                Logger.LogError("Key is invalid");
            return isValid;
        }

        private bool CheckKeyFileExists()
        {
            return File.Exists(userKeyLocation);
        }

        public bool CreateKeyFile()
        {
            if (CheckKeyFileExists())
                return true;
            else
            {
                SerializationHelper.Serialize(userKeyLocation, this);
                Logger.LogInfo(userKeyLocation + "file was created.");
                return CheckKeyFileExists();
            }
        }

        public void SaveKey()
        {
            SerializationHelper.Serialize(userKeyLocation, this);
            Logger.LogInfo("Key was saved.");
        }

        public void LoadKeyFromFile()
        {
            Logger.LogInfo("Loading key from file...");
            Key = LoadKey();
            Logger.LogInfo("Using key: " + Key);
        }
        public static string LoadKey()
        {
            string key = (SerializationHelper.Deserialize(userKeyLocation, typeof(KeyManager)) as KeyManager).Key;
            return key ?? string.Empty;
        }
    }
}