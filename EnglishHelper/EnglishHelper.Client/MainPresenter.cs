using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishHelper.Core;
using System.Diagnostics;
using System.Windows.Navigation;

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
            keyWindow.KeyHyperLink = "https://tech.yandex.com/translate";
        }


        #region Key Window event handlers

        private void KeyWindow_ApplyButtonClick(object sender, EventArgs e)
        {
            keyManager.Key = keyWindow.Key.Replace("\r\n", "").Replace(" ", "");

            if (string.IsNullOrEmpty(keyManager.Key))
            {
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
    
        private void KeyWindow_GetKeyHyperLinkClick(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(keyWindow.KeyHyperLink);
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

        private void InitializeKey()
        {
            bool isFileExist = keyManager.CreateKeyFile();

            if (!isFileExist)
            {
                // Log - Unable to load key
                keyWindow.OpenWindow();
                return;
            }

            keyManager.LoadKeyFromFile();

            if (string.IsNullOrEmpty(keyManager.Key))
                keyWindow.OpenWindow();
            else if (keyManager.ValidateKey())
            {
                translator.Key = keyManager.Key;
                keyManager.IsKeyValid = true;
            }
            else
            {
                messageManager.ShowError("Key isn't valid");
                keyManager.Key = keyWindow.Key = string.Empty;
                keyManager.IsKeyValid = false;

                keyWindow.OpenWindow();
            }
        }
        private void InitializeDictionary()
        {
            bool isFileExist = dictionaryManager.CreateDictionaryFile();

            dictionaryManager.LoadDictionaryFromFile();

            if (dictionaryManager.WordDictionary == null)
            {
                //"Dictionary isn't valid");
            }
        }

        private void MainWindow_ChangeLanguageButtonClick(object sender, EventArgs e)
        {
            if (translator.LanguageOrienation == "en-ru")
            {
                translator.LanguageOrienation = "ru-en";
                mainWindow.LanguageOrientation = "Language: Russian->English";
                return;
            }
            if (translator.LanguageOrienation == "ru-en")
            {
                translator.LanguageOrienation = "en-ru";
                mainWindow.LanguageOrientation = "Language: English->Russian";
                return;
            }
        }
        private void MainWindow_TranslateButtonClick(object sender, EventArgs e)
        {
            string translatedString = translator.GetTranslatedString(mainWindow.SourceText);
            if (!string.IsNullOrEmpty(translatedString))
                mainWindow.TranslationText = translatedString;
        }

        private void MainWindow_AddToDictionaryButtonClick(object sender, EventArgs e)
        {
            bool isAdded = dictionaryManager.AddWord(mainWindow.SourceText);

            if(isAdded)
            {
                messageManager.ShowMessage("Word '" + mainWindow.SourceText + "' was added to dictionary.");
                //log that ok
                //Show it on form
            }
            else
            {
                messageManager.ShowError("Word '" + mainWindow.SourceText + "' wasn't added to dictionary.");
            }
        }

        private void MainWindow_ChangeTextButtonClick(object sender, EventArgs e)
        {
            mainWindow.SourceText = mainWindow.TranslationText;
            MainWindow_TranslateButtonClick(sender,e);
        }

        #endregion
    }
}
