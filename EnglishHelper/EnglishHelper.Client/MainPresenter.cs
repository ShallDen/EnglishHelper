using System;
using System.Windows;
using EnglishHelper.Core;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
            mainWindow.PreClosingWindow += MainWindow_PreClosingWindow;

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
            if (!KeyManager.Instance.IsKeyValid)
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

        private void MainWindow_PreClosingWindow(object sender, EventArgs e)
        {
            if (dictionaryWindow != null)
                dictionaryWindow.CloseWindow();
        }

        #endregion

        #region Dictionary window handlers

        private void DictionaryWindow_DeleteWordButtonClick(object sender, EventArgs e)
        {
            var selectedRows = dictionaryWindow.SelectedRows;
            int rowCount = selectedRows.Count;
            string isSymbolSRequired = rowCount > 1 ? "s" : string.Empty;

            if (selectedRows != null)
            {
                while (selectedRows.Count > 0)
                {
                    var row = selectedRows[0] as Entry;

                    if (row != null)
                    {
                        row = selectedRows[0] as Entry;
                        Logger.LogInfo("Deleting '" + row.Word + "'");

                        if (row != null)
                            dictionaryManager.DeleteWord(row.Word);

                        Logger.LogInfo(rowCount + " word" + isSymbolSRequired + " deleted");
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void DictionaryWindow_SaveDictionaryButtonClick(object sender, EventArgs e)
        {
            var wordCount = dictionaryManager.WordCount;
            string isSymbolSRequired = wordCount > 1 ? "s" : string.Empty;

            dictionaryManager.SaveDictionaryToFile();

            Logger.LogInfo(wordCount + " item" + isSymbolSRequired + " saved to dictionary");
            MessageManager.ShowMessage(wordCount + " item" + isSymbolSRequired + " saved to dictionary");
        }


        private void DictionaryWindow_DictionaryChanged(object sender, DataGridRowEditEndingEventArgs e)
        {
            var item = ((sender as DataGrid).SelectedItem) as Entry;

            if (e.EditAction != DataGridEditAction.Commit)
                return;
            else if (dictionaryManager.GetCountByWord(item.Word) > 1)
            {
                Logger.LogWarning("Dictionary already contains '" + item.Word + "'.");
                MessageManager.ShowWarning("Dictionary already contains '" + item.Word + "'.");
                dictionaryManager.DeleteWord(item.Word);
            }
            else
            {
                dictionaryManager.FillEmptyValuesInRow(item);
            }

            //Check another one because item.Word can be copied from item.Translation
            if (dictionaryManager.GetCountByWord(item.Word) > 1)
            {
                Logger.LogWarning("Dictionary already contains '" + item.Word + "'.");
                MessageManager.ShowWarning("Dictionary already contains '" + item.Word + "'.");
                dictionaryManager.DeleteWord(item.Word);
            }
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

                mainWindow.HideWindow();
                keyWindow.OpenWindow();
                mainWindow.DisplayWindow();

                return;
            }

            KeyManager.Instance.LoadKeyFromFile();

            if (string.IsNullOrEmpty(KeyManager.Instance.Key))
            {
                Logger.LogInfo("Key is empty. Opening Key window for getting new api key...");

                mainWindow.HideWindow();
                keyWindow.OpenWindow();
                mainWindow.DisplayWindow();
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

                mainWindow.HideWindow();
                keyWindow.OpenWindow();
                mainWindow.DisplayWindow();
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
                mainWindow.SetRussianLanguageOrientation();
                Logger.LogInfo("Changing translate orientation to " + Translator.Instance.LanguageOrienation);
                return;
            }
            if (Translator.Instance.LanguageOrienation == "ru-en")
            {
                Translator.Instance.LanguageOrienation = "en-ru";
                mainWindow.SetEnglishLanguageOrientation();
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
                dictionaryManager.SaveDictionaryToFile();

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

        private void OpenDictionary()
        {
            dictionaryWindow = new DictionaryWindow();

            dictionaryWindow.DeleteWordButtonClick += DictionaryWindow_DeleteWordButtonClick;
            dictionaryWindow.SaveDictionaryButtonClick += DictionaryWindow_SaveDictionaryButtonClick;
            dictionaryWindow.DictionaryChanged += DictionaryWindow_DictionaryChanged;

            InitializeDictionary();
            dictionaryWindow.BindDictionaryWithGrid(dictionaryManager.WordDictionary);
            dictionaryWindow.OpenWindow();
        }

        #endregion

    }
}
