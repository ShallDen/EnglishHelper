using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishHelper.Core;

namespace EnglishHelper.Client
{
    public class MainPresenter
    {
        private readonly ITranslator translator;
        private readonly IMainWindow mainWindow;
        private readonly IMessageManager messageManager;
        private readonly IKeyWindow keyWindow;
        private readonly IKeyManager keyManager;

        public MainPresenter(IMainWindow _mainWindow, ITranslator _translator, IMessageManager _messageManager,
                             IKeyWindow _keyWindow, IKeyManager _keyManager)
        {
            mainWindow = _mainWindow;
            translator = _translator;
            messageManager = _messageManager;
            keyWindow = _keyWindow;
            keyManager = _keyManager;

            mainWindow.ChangeLanguageButtonClick += MainWindow_ChangeLanguageButtonClick;
            mainWindow.TranslateButtonClick += MainWindow_TranslateButtonClick;
            mainWindow.AddToDictionaryButtonClick += MainWindow_AddToDictionaryButtonClick;
            mainWindow.ChangeTextButtonClick += MainWindow_ChangeTextButtonClick;
            mainWindow.FormLoaded += MainWindow_FormLoaded;
            
            mainWindow.LanguageOrientation = "Language: English->Russian";

            keyWindow.ApplyButtonClick += KeyWindow_ApplyButtonClick;
            keyWindow.WindowClosed += KeyWindow_WindowClosed;
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

            if(keyManager.ValidateKey())
            {
                translator.Key = keyManager.Key;
                keyManager.IsKeyValid = true;
                keyWindow.CloseWindow();
            }
            else
            {
                messageManager.ShowError("Key isn't valid");
                keyManager.Key = keyWindow.Key = string.Empty;
                keyManager.IsKeyValid = false;
            }
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
            keyWindow.OpenWindow();
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
            translator.Text = mainWindow.SourceText;
            string translatedString = translator.GetTranslatedString();
            if (!string.IsNullOrEmpty(translatedString))
                mainWindow.TranslationText = translatedString;
        }

        private void MainWindow_AddToDictionaryButtonClick(object sender, EventArgs e)
        {
            messageManager.ShowMessage("This haven't implemented yet :)");
        }

        private void MainWindow_ChangeTextButtonClick(object sender, EventArgs e)
        {
            mainWindow.SourceText = mainWindow.TranslationText;
            MainWindow_TranslateButtonClick(sender,e);
        }

        #endregion
    }
}
