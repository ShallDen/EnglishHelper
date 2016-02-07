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
    public partial class MainWindow : Window
    {
        private Translator translator = new Translator();

        public MainWindow()
        {
            InitializeComponent();
            languageLabel.Content = "Language: English->Russian";
        }

        private void transtateButton_Click(object sender, RoutedEventArgs e)
        {
            translator.Text = inputTextBox.Text;
            outputTextBox.Text = translator.GetTranslatedString();
        }

        private void changeLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if (translator.Language == "en-ru")
            {
                translator.Language = "ru-en";
                languageLabel.Content = "Language: Russian->English";
                return;
            }
            if (translator.Language == "ru-en")
            {
                translator.Language = "en-ru";
                languageLabel.Content = "Language: English->Russian";
                return;
            }
        }

        private void addToDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This haven't implemented yet :)");
        }

        private void changeTextButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBox.Text = outputTextBox.Text;
            translator.Text = inputTextBox.Text;
            outputTextBox.Text = translator.GetTranslatedString();
        }
    }
}
