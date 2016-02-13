using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using EnglishHelper.Core;

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public interface IMainWindow
    {
        string LanguageOrientation { get; set; }
        string SourceText { get; set; }
        string TranslationText { get; set; }

        void CloseWindow();

        event EventHandler TranslateButtonClick;
        event EventHandler ChangeLanguageButtonClick;
        event EventHandler AddToDictionaryButtonClick;
        event EventHandler ChangeTextButtonClick;
        event EventHandler OpenDictionaryButtonClick;
        event RoutedEventHandler FormLoaded;
    }

    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Translator translator = new Translator();
            MessageManager messageManager = new MessageManager();
            KeyWindow keyWindow = new KeyWindow();
            KeyManager keyManager = new KeyManager();
            DictionaryManager dictionaryManager = new DictionaryManager();
            DictionaryWindow dictionaryWindow = new DictionaryWindow();
            MainPresenter presenter = new MainPresenter(this, translator, messageManager, keyWindow, keyManager, dictionaryManager, dictionaryWindow);

            changeLanguageButton.Click += changeLanguageButton_Click;
            translateButton.Click += translaleButton_Click;
            addToDictionaryButton.Click += addToDictionaryButton_Click;
            changeTextButton.Click += changeTextButton_Click;
            openDictionaryButton.Click += OpenDictionaryButton_Click;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        public event EventHandler TranslateButtonClick;
        public event EventHandler ChangeLanguageButtonClick;
        public event EventHandler AddToDictionaryButtonClick;
        public event EventHandler ChangeTextButtonClick;
        public event EventHandler OpenDictionaryButtonClick;
        public event RoutedEventHandler FormLoaded;

        public string LanguageOrientation
        {
            get { return languageLabel.Content.ToString(); }
            set { languageLabel.Content = value; }
        }

        public string SourceText
        {
            get { return inputTextBox.Text; }
            set { inputTextBox.Text = value; }
        }

        public string TranslationText
        {
            get { return outputTextBox.Text; }
            set { outputTextBox.Text = value; }
        }

        public void CloseWindow()
        {
            this.Close();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Logger.LogInfo("Application was closed.");
            Environment.Exit(0);
        }

        #region Events throwing

        private void changeLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeLanguageButtonClick != null)
                ChangeLanguageButtonClick(this, EventArgs.Empty);
        }
        private void translaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (TranslateButtonClick != null)
                TranslateButtonClick(this, EventArgs.Empty);
        }

        private void addToDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddToDictionaryButtonClick != null)
                AddToDictionaryButtonClick(this, EventArgs.Empty);
        }

        private void changeTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeTextButtonClick != null)
                ChangeTextButtonClick(this, EventArgs.Empty);
        }

        private void OpenDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (OpenDictionaryButtonClick != null)
                OpenDictionaryButtonClick(this, EventArgs.Empty);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded != null)
                FormLoaded(this, e);
        }
        #endregion
    }
}
