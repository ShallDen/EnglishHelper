using System;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Text;

namespace EnglishHelper.Core
{
    public interface IKeyManager
    {
        bool IsKeyValid { get; }
        string Key { get; set; }
        bool CreateKeyFile();
        void LoadKeyFromFile();
        void SaveKey();
        bool ValidateKey(string key);
    }

    [Serializable]
    [XmlRootAttribute("KeyManager")]
    public class KeyManager : IKeyManager
    {
        private static string userKeyLocation = ConfigurationManager.AppSettings["KeyFileName"];
        private string mKey = string.Empty;
        private bool mIsKeyValid = false;

        private static KeyManager instance;
        protected KeyManager() { }

        public static KeyManager Instance
        {
            get { return instance ?? (instance = new KeyManager()); }
        }

        public string Key
        {
            get { return mKey; }
            set { mKey = value; }
        }

        [XmlIgnore]
        public bool IsKeyValid
        {
            get { return mIsKeyValid; }
            private set { mIsKeyValid = value; }
        }
        
        public bool ValidateKey()
        {
            return ValidateKey(mKey);
        }

        public bool ValidateKey(string key)
        {
            mKey = key;

            bool isValid = string.IsNullOrEmpty(Translator.Instance.GetTranslatedString("Ping")) ? false : true;
            if (isValid)
            {
                mIsKeyValid = true;
                Logger.LogInfo("Key is valid.");
            }
            else
            {
                mIsKeyValid = false;
                mKey = string.Empty;
                Logger.LogError("Key is invalid.");
            }

            return isValid;
        }

        private bool CheckKeyFileExists()
        {
            return File.Exists(userKeyLocation);
        }

        public bool CreateKeyFile()
        {
            if (CheckKeyFileExists())
            {
                Logger.LogInfo(userKeyLocation + " already exists.");
                return true;
            }
            else
            {
                File.WriteAllText(userKeyLocation, string.Empty, Encoding.Unicode);
                return false;
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
            string key = string.Empty;

            try
            {
                key = (SerializationHelper.Deserialize(userKeyLocation, typeof(KeyManager)) as KeyManager).Key;
            }
            catch (Exception)
            {
                key = key ?? string.Empty;
            } 
            finally
            {
                Logger.LogInfo("Using key: " + key);
                mKey = key;
            }
        }
    }
}