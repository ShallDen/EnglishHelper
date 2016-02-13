using System;
using System.Windows;
using EnglishHelper.Core;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Configuration;

namespace EnglishHelper.Client
{
    public class MainPresenter
    {
        private readonly ITranslator translator;
        private readonly IMainWindow mainWindow;
        private readonly IMessageManager messageManager;
        private readonly IKeyWindow keyWindow;
        private readonly IKeyManager keyManager;
        private readonly IDictionaryManager dictionaryManager;

        public MainPresenter(IMainWindow _mainWindow, ITranslator _translator, IMessageManager _messageManager,
                             IKeyWindow _keyWindow, IKeyManager _keyManager, IDictionaryManager _dictionaryManager)
        {
            mainWindow = _mainWindow;
            translator = _translator;
            messageManager = _messageManager;
            keyWindow = _keyWindow;
            keyManager = _keyManager;
            dictionaryManager = _dictionaryManager;

            mainWindow.ChangeLanguageButtonClick += MainWindow_ChangeLanguageButtonClick;
            mainWindow.TranslateButtonClick += MainWindow_TranslateButtonClick;
            mainWindow.AddToDictionaryButtonClick += MainWindow_AddToDictionaryButtonClick;
            mainWindow.ChangeTextButtonClick += MainWindow_ChangeTextButtonClick;
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
            if (!keyManager.IsKeyValid)
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

        #endregion

        #region Private Methods

        private void ApplyKey()
        {
            keyManager.Key = keyWindow.Key.Replace("\r\n", "").Replace(" ", "");

            if (string.IsNullOrEmpty(keyManager.Key))
            {
                Logger.LogWarning("Key is null or empty. Try to enter anothe key.");
                keyWindow.Key = string.Empty;
                return;
            }

            if (keyManager.ValidateKey())
            {
                translator.Key = keyManager.Key;
                keyManager.IsKeyValid = true;

                keyManager.SaveKey();
                keyWindow.CloseWindow();
            }
            else
            {
                messageManager.ShowError("Key isn't valid");
                keyManager.Key = keyWindow.Key = string.Empty;
                keyManager.IsKeyValid = false;
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

            bool isFileExist = keyManager.CreateKeyFile();

            if (!isFileExist)
            {
                Logger.LogWarning("Unable to load key. Opening Key window for getting new api key...");
                keyWindow.OpenWindow();
                return;
            }

            keyManager.LoadKeyFromFile();

            if (string.IsNullOrEmpty(keyManager.Key))
            {
                Logger.LogInfo("Key is null or empty. Opening Key window for getting new api key...");
                keyWindow.OpenWindow();
            }
            else if (keyManager.ValidateKey())
            {
                translator.Key = keyManager.Key;
                keyManager.IsKeyValid = true;
                Logger.LogInfo("Key was successfully validated.");
                Logger.LogInfo("Key was initialized.");
            }
            else
            {
                messageManager.ShowError("Key isn't valid");
                keyManager.Key = keyWindow.Key = string.Empty;
                keyManager.IsKeyValid = false;
                Logger.LogError("Key isn't valid");

                keyWindow.OpenWindow();
            }
        }

        private void InitializeDictionary()
        {
            Logger.LogInfo("Initialing existing dictionary...");

            bool isFileExist = dictionaryManager.CreateDictionaryFile();

            dictionaryManager.LoadDictionaryFromFile();

            if (dictionaryManager.WordDictionary == null)
            {
                Logger.LogError("Dictionary isn't valid");
                return;
            }

            Logger.LogInfo("Dictionary was initialized.");
        }

        private void ChangeTranslationOrientation()
        {
            if (translator.LanguageOrienation == "en-ru")
            {
                translator.LanguageOrienation = "ru-en";
                mainWindow.LanguageOrientation = "Language: Russian->English";
                Logger.LogInfo("Changing translate orientation to " + translator.LanguageOrienation);
                return;
            }
            if (translator.LanguageOrienation == "ru-en")
            {
                translator.LanguageOrienation = "en-ru";
                mainWindow.LanguageOrientation = "Language: English->Russian";
                Logger.LogInfo("Changing translate orientation to " + translator.LanguageOrienation);
                return;
            }
        }

        private void Translate()
        {
            if (!string.IsNullOrWhiteSpace(mainWindow.SourceText))
            {
                string translatedString = translator.GetTranslatedString(mainWindow.SourceText);
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
                messageManager.ShowMessage("Word '" + mainWindow.SourceText + "' was added to dictionary.");
            }
            else
            {
                Logger.LogError("Word '" + mainWindow.SourceText + "' wasn't added to dictionary.");
                messageManager.ShowError("Word '" + mainWindow.SourceText + "' wasn't added to dictionary.");
            }
        }

        private void ChangeText(object sender, EventArgs e)
        {
            mainWindow.SourceText = mainWindow.TranslationText;
            MainWindow_TranslateButtonClick(sender, e);
        }

        #endregion
    }
}
