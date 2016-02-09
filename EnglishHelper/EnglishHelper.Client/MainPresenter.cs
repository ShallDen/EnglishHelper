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

        public MainPresenter(IMainWindow _mainWindow, ITranslator _translator, IMessageManager _messageManager)
        {
            mainWindow = _mainWindow;
            translator = _translator;
            messageManager = _messageManager;

            mainWindow.ChangeLanguageButtonClick += MainWindow_ChangeLanguageButtonClick;
            mainWindow.TranslateButtonClick += MainWindow_TranslateButtonClick;
            mainWindow.AddToDictionaryButtonClick += MainWindow_AddToDictionaryButtonClick;
            mainWindow.ChangeTextButtonClick += MainWindow_ChangeTextButtonClick;
            
            mainWindow.LanguageOrienation = "Language: English->Russian";
        }

        private void MainWindow_ChangeLanguageButtonClick(object sender, EventArgs e)
        {
            if (translator.LanguageOrienation == "en-ru")
            {
                translator.LanguageOrienation = "ru-en";
                mainWindow.LanguageOrienation = "Language: Russian->English";
                return;
            }
            if (translator.LanguageOrienation == "ru-en")
            {
                translator.LanguageOrienation = "en-ru";
                mainWindow.LanguageOrienation = "Language: English->Russian";
                return;
            }
        }
        private void MainWindow_TranslateButtonClick(object sender, EventArgs e)
        {
            translator.Text = mainWindow.SourceText;
            mainWindow.TranslationText = translator.GetTranslatedString();
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
    }
}
