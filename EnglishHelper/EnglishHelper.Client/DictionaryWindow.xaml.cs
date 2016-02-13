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
using System.Windows.Shapes;
using EnglishHelper.Core;

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для DictionaryWindow.xaml
    /// </summary>
    /// 
    public interface IDictionaryWindow
    {
        List<Entry> WordDictionary { get; set; }
        Entry SelectedRow { get; }
        void OpenWindow();
        void CloseWindow();
        void RefreshGrid();
        event EventHandler AddWordButtonClick;
        event EventHandler DeleteWordButtonClick;
        event EventHandler SaveDictionaryButtonClick;
    }

    public partial class DictionaryWindow : Window, IDictionaryWindow
    {
        public DictionaryWindow()
        {
            InitializeComponent();

            addWordButton.Click += AddWordButton_Click;
            deleteWordButton.Click += DeleteWordButton_Click;
            saveDictionaryButton.Click += SaveDictionaryButton_Click;

            wordGrid.ItemsSource = WordDictionary;
        }

        public event EventHandler AddWordButtonClick;
        public event EventHandler DeleteWordButtonClick;
        public event EventHandler SaveDictionaryButtonClick;

        public List<Entry> WordDictionary
        {
            get { return wordGrid.ItemsSource as List<Entry>; }
            set { wordGrid.ItemsSource = value; }
        }
        public Entry SelectedRow
        {
            get { return wordGrid.SelectedItem as Entry; }
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddWordButtonClick != null)
                AddWordButtonClick(this, e);
        }
        private void DeleteWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteWordButtonClick != null)
                DeleteWordButtonClick(this, e);
            RefreshGrid();
        }
        private void SaveDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDictionaryButtonClick != null)
                SaveDictionaryButtonClick(this, e);
            RefreshGrid();
        }

        public void OpenWindow()
        {
            this.ShowDialog();
        }

        public void CloseWindow()
        {
            this.Close();
        }
        public void RefreshGrid()
        {
            wordGrid.Items.Refresh();
        }
    }
}
