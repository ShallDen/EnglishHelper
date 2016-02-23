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
        string SourceText { get; set; }
        string TranslationText { get; set; }

        void HideWindow();
        void DisplayWindow();
        void CloseWindow();
        void SetEnglishLanguageOrientation();
        void SetRussianLanguageOrientation();
        void NotifyWindowsToClose();

        event EventHandler TranslateButtonClick;
        event EventHandler ChangeLanguageButtonClick;
        event EventHandler AddToDictionaryButtonClick;
        event EventHandler ChangeTextButtonClick;
        event EventHandler OpenDictionaryButtonClick;
        event RoutedEventHandler FormLoaded;
        event EventHandler PreClosingWindow;
    }

    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            KeyWindow keyWindow = new KeyWindow();
            DictionaryManager dictionaryManager = new DictionaryManager();
            MainPresenter presenter = new MainPresenter(this, keyWindow, dictionaryManager);

            changeLanguageButton.Click += changeLanguageButton_Click;
            translateButton.Click += translaleButton_Click;
            addToDictionaryButton.Click += addToDictionaryButton_Click;
            changeTextButton.Click += changeTextButton_Click;
            openDictionaryButton.Click += OpenDictionaryButton_Click;

            inputTextBox.Foreground = Brushes.Gray;
            outputTextBox.Foreground = Brushes.Gray;

            inputTextBox.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                if (inputTextBox.Text == "Type words to translate here...")
                {
                    inputTextBox.Foreground = Brushes.Black;
                    inputTextBox.Text = string.Empty;
                }
            };

            inputTextBox.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                if (string.IsNullOrWhiteSpace(inputTextBox.Text))
                {
                    inputTextBox.Foreground = Brushes.Gray;
                    inputTextBox.Text = "Type words to translate here...";
                }
            };

            outputTextBox.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                if (outputTextBox.Text == "Translated words will be here...")
                {
                    outputTextBox.Foreground = Brushes.Black;
                    outputTextBox.Text = string.Empty;
                }
            };

            outputTextBox.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                if (string.IsNullOrWhiteSpace(outputTextBox.Text))
                {
                    outputTextBox.Foreground = Brushes.Gray;
                    outputTextBox.Text = "Translated words will be here...";
                }
            };

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        public event EventHandler TranslateButtonClick;
        public event EventHandler ChangeLanguageButtonClick;
        public event EventHandler AddToDictionaryButtonClick;
        public event EventHandler ChangeTextButtonClick;
        public event EventHandler OpenDictionaryButtonClick;
        public event RoutedEventHandler FormLoaded;
        public event EventHandler PreClosingWindow;

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

        public void HideWindow()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void DisplayWindow()
        {
            this.Visibility = Visibility.Visible;
        }

        public void SetEnglishLanguageOrientation()
        {
            sourceTextFlag.Source = new BitmapImage(new Uri("Images/usa64.png", UriKind.Relative));
            translationTextFlag.Source = new BitmapImage(new Uri("Images/rus64.png", UriKind.Relative));
        }

        public void SetRussianLanguageOrientation()
        {
            sourceTextFlag.Source = new BitmapImage(new Uri("Images/rus64.png", UriKind.Relative));
            translationTextFlag.Source = new BitmapImage(new Uri("Images/usa64.png", UriKind.Relative));
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Logger.LogInfo("Application was closed.");
            Environment.Exit(0);
        }

        public void NotifyWindowsToClose()
        {
            if (PreClosingWindow != null)
                PreClosingWindow(this, EventArgs.Empty);
        }

        #region Events throwing

        private void changeLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeLanguageButtonClick != null)
                ChangeLanguageButtonClick(this, EventArgs.Empty);
        }
        private void translaleButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBox.Foreground = Brushes.Black;
            outputTextBox.Foreground = Brushes.Black;

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
            inputTextBox.Foreground = Brushes.Black;
            outputTextBox.Foreground = Brushes.Black;

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
