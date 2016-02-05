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
using LitJson;
using System.Net;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string mLanguage = "en-ru";

        public MainWindow()
        {
            InitializeComponent();
            languageLabel.Content = "Language: English->Russian";
        }

        private void transtateButton_Click(object sender, RoutedEventArgs e)
        {
            string text = inputTextBox.Text;

            Translate(text, mLanguage);
        }

        public void Translate(string text, string language)
        {
            outputTextBox.Text = string.Empty;

            string strUri = "https://translate.yandex.net/api/v1.5/tr.json/translate?"
               + "key=trnsl.1.1.20160204T223148Z.ffe338b0a2031691.9161850fdfcb8c026e81dc08e5f3f2ffaae6a602"
               + "&text=" + text
               + "&lang=" + language;

            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string strJson = wc.DownloadString(strUri);

            JsonObject obj = JsonMapper.ToObject<JsonObject>(strJson);
          
            foreach(var str in obj.text)
            {
                outputTextBox.Text += str;
            }
        }

        private void changeLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            string temp = string.Empty;

            temp = inputTextBox.Text;
            inputTextBox.Text = outputTextBox.Text;
            outputTextBox.Text = temp;

            if (mLanguage == "en-ru")
            {
                mLanguage = "ru-en";
                languageLabel.Content = "Language: Russian->English";
                return;
            }
            if (mLanguage == "ru-en")
            {
                mLanguage = "en-ru";
                languageLabel.Content = "Language: English->Russian";
                return;
            }
        }

        private void addToDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This haven't implemented yet :)");
        }
    }

    public class JsonObject
    {
        public int code;
        public string language;
        public List<string> text;
    }
}