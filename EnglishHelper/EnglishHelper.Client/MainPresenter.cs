using System;
using System.Windows;
using EnglishHelper.Core;
using System.Diagnostics;
using System.Windows.Navigation;

namespace EnglishHelper.Client
{
    public class MainPresenter
    {
        private readonly IMainWindow mainWindow;
        private readonly IKeyWindow keyWindow;
        private readonly IDictionaryManager dictionaryManager;
        private IDictionaryWindow dictionaryWindow;

        public MainPresenter(IMainWindow _mainWindow, IKeyWindow _keyWindow, IDictionaryManager _dictionaryManager)
        {
            mainWindow = _mainWindow;
            keyWindow = _keyWindow;
            dictionaryManager = _dictionaryManager;

            mainWindow.ChangeLanguageButtonClick += MainWindow_ChangeLanguageButtonClick;
            mainWindow.TranslateButtonClick += MainWindow_TranslateButtonClick;
            mainWindow.AddToDictionaryButtonClick += MainWindow_AddToDictionaryButtonClick;
            mainWindow.ChangeTextButtonClick += MainWindow_ChangeTextButtonClick;
            mainWindow.OpenDictionaryButtonClick += MainWindow_OpenDictionaryButtonClick;
            mainWindow.FormLoaded += MainWindow_FormLoaded;
            mainWindow.LanguageOrientation = "Language: English->Russian";

            keyWindow.ApplyButtonClick += KeyWindow_ApplyButtonClick;
            keyWindow.GetKeyHyperLinkClick += KeyWindow_GetKeyHyperLinkClick;
            keyWindow.WindowClosed += KeyWindow_WindowClosed;
        }

        #region Key Window event handlers

        private void KeyWindow_ApplyButtonClick(object sender, EventArgs e)
        {
            ApplyKey();
        }

        private void KeyWindow_GetKeyHyperLinkClick(object sender, RequestNavigateEventArgs e)
        {
            OpenHyperink();
        }

        private void KeyWindow_WindowClosed(object sender, EventArgs e)
        {
            if (!KeyManager.Instance.IsKeyValid) // change it to click own close button 
                mainWindow.CloseWindow();
        }

        #endregion

        #region Main Window event handlers

        private void MainWindow_FormLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeKey();
            InitializeDictionary();
        }
     
        private void MainWindow_ChangeLanguageButtonClick(object sender, EventArgs e)
        {
            ChangeTranslationOrientation();
        }

        private void MainWindow_TranslateButtonClick(object sender, EventArgs e)
        {
            Translate();
        }

        private void MainWindow_AddToDictionaryButtonClick(object sender, EventArgs e)
        {
            AddToDictionary();
        }

        private void MainWindow_ChangeTextButtonClick(object sender, EventArgs e)
        {
            ChangeText(sender, e);
        }

        private void MainWindow_OpenDictionaryButtonClick(object sender, EventArgs e)
        {
            OpenDictionary();
        }


        #endregion

        #region Private Methods

        private void ApplyKey()
        {
            string key = keyWindow.Key.Replace("\r\n", "").Replace(" ", "");

            if (string.IsNullOrEmpty(key))
            {
                Logger.LogWarning("Key is null or empty. Try to enter anothe key.");
                keyWindow.Key = string.Empty;
                return;
            }

            if (KeyManager.Instance.ValidateKey(key))
            {
                KeyManager.Instance.SaveKey();
                keyWindow.CloseWindow();
            }
            else
            {
                MessageManager.ShowError("Key isn't valid");
                keyWindow.Key = string.Empty;
            }
        }

        private void OpenHyperink()
        {
            Logger.LogInfo("Opening link: " + keyWindow.KeyHyperLink);
            Process.Start(keyWindow.KeyHyperLink);
        }

        private void InitializeKey()
        {
            Logger.LogInfo("Initialing key...");

            bool isFileExist = KeyManager.Instance.CreateKeyFile();

            if (!isFileExist)
            {
                Logger.LogWarning("Unable to load key. Opening Key window for getting new api key...");
                keyWindow.OpenWindow();
                return;
            }

            KeyManager.Instance.LoadKeyFromFile();

            if (string.IsNullOrEmpty(KeyManager.Instance.Key))
            {
                Logger.LogInfo("Key is empty. Opening Key window for getting new api key...");
                keyWindow.OpenWindow();
            }
            else if (KeyManager.Instance.ValidateKey())
            {
                Logger.LogInfo("Key was successfully validated.");
                Logger.LogInfo("Key was initialized.");
            }
            else
            {
                MessageManager.ShowError("Key isn't valid");
                keyWindow.Key = string.Empty;
                Logger.LogError("Key isn't valid");

                keyWindow.OpenWindow();
            }
        }

        private void InitializeDictionary()
        {
            dictionaryManager.InitializeDictionary();
        }

        private void ChangeTranslationOrientation()
        {
            if (Translator.Instance.LanguageOrienation == "en-ru")
            {
                Translator.Instance.LanguageOrienation = "ru-en";
                mainWindow.LanguageOrientation = "Language: Russian->English";
                Logger.LogInfo("Changing translate orientation to " + Translator.Instance.LanguageOrienation);
                return;
            }
            if (Translator.Instance.LanguageOrienation == "ru-en")
            {
                Translator.Instance.LanguageOrienation = "en-ru";
                mainWindow.LanguageOrientation = "Language: English->Russian";
                Logger.LogInfo("Changing translate orientation to " + Translator.Instance.LanguageOrienation);
                return;
            }
        }

        private void Translate()
        {
            if (!string.IsNullOrWhiteSpace(mainWindow.SourceText))
            {
                string translatedString = Translator.Instance.GetTranslatedString(mainWindow.SourceText);
                if (!string.IsNullOrEmpty(translatedString))
                    mainWindow.TranslationText = translatedString;
            }
        }

        private void AddToDictionary()
        {
            bool isAdded = dictionaryManager.AddWord(mainWindow.SourceText);

            if (isAdded)
            {
                Logger.LogInfo("Word '" + mainWindow.SourceText + "' was added to dictionary.");
                MessageManager.ShowMessage("Word '" + mainWindow.SourceText + "' was added to dictionary.");
            }
            else
            {
                Logger.LogError("Word '" + mainWindow.SourceText + "' wasn't added to dictionary.");
                MessageManager.ShowError("Word '" + mainWindow.SourceText + "' wasn't added to dictionary.");
            }
        }

        private void ChangeText(object sender, EventArgs e)
        {
            mainWindow.SourceText = mainWindow.TranslationText;
            MainWindow_TranslateButtonClick(sender, e);
        }
        #endregion


        private void DictionaryWindow_AddWordButtonClick(object sender, EventArgs e)
        {

        }
        private void DictionaryWindow_DeleteWordButtonClick(object sender, EventArgs e)
        {
            var selectedRows = dictionaryWindow.SelectedRows;
            if(selectedRows!=null)
            {
                foreach (var selectedRow in selectedRows)
                {
                    var row = selectedRow as Entry;
                    if(row!=null)
                        dictionaryManager.DeleteWord(row.Word);
                }

                dictionaryWindow.SetDictionary(dictionaryManager.WordDictionary);

                Logger.LogInfo(selectedRows.Count + " word(s) deleted");
            }
        }

        private void DictionaryWindow_SaveDictionaryButtonClick(object sender, EventArgs e)
        {
            var wordCount = dictionaryManager.WordCount;
            dictionaryManager.FillEmptyValuesInTable();

            dictionaryManager.SaveDictionaryToFile();

            Logger.LogInfo(wordCount + " item(s) saved to dictionary");
            MessageManager.ShowMessage(wordCount + " item(s) saved to dictionary");
        }

        private void OpenDictionary()
        {
            dictionaryWindow = new DictionaryWindow();

            dictionaryWindow.AddWordButtonClick += DictionaryWindow_AddWordButtonClick;
            dictionaryWindow.DeleteWordButtonClick += DictionaryWindow_DeleteWordButtonClick;
            dictionaryWindow.SaveDictionaryButtonClick += DictionaryWindow_SaveDictionaryButtonClick;

            InitializeDictionary();
            dictionaryWindow.SetDictionary(dictionaryManager.WordDictionary);
            dictionaryWindow.OpenWindow();
        }
    }
}
