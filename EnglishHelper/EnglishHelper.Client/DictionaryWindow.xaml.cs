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
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для DictionaryWindow.xaml
    /// </summary>
    /// 
    public interface IDictionaryWindow
    {
        IList SelectedRows { get; }
        void OpenWindow();
        void CloseWindow();
        void RefreshGrid();
        void SetDictionary(ObservableCollection<Entry> list);
        event EventHandler AddWordButtonClick;
        event EventHandler DeleteWordButtonClick;
        event EventHandler SaveDictionaryButtonClick;
    }

    public partial class DictionaryWindow : Window, IDictionaryWindow
    {
        ObservableCollection<Entry> wordDictionary = null;
        public DictionaryWindow()
        {
            InitializeComponent();

            addWordButton.Click += AddWordButton_Click;
            deleteWordButton.Click += DeleteWordButton_Click;
            saveDictionaryButton.Click += SaveDictionaryButton_Click;
        }

        public event EventHandler AddWordButtonClick;
        public event EventHandler DeleteWordButtonClick;
        public event EventHandler SaveDictionaryButtonClick;

        public IList SelectedRows
        {
            get { return wordGrid.SelectedItems; }
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            wordDictionary.Add(new Entry { Word = "New", Translation = "Тест нового", LastChangeDate = "11111"});

           //  if (AddWordButtonClick != null)
           //       AddWordButtonClick(this, e);
        }
        private void DeleteWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteWordButtonClick != null)
                DeleteWordButtonClick(this, e);
         //   RefreshGrid();
        }
        private void SaveDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDictionaryButtonClick != null)
                SaveDictionaryButtonClick(this, e);
         //   RefreshGrid();
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

        private void wordGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var item = ((sender as DataGrid).SelectedItem) as Entry;
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (string.IsNullOrWhiteSpace(item.Word) && !string.IsNullOrWhiteSpace(item.Translation))
                    item.Word = item.Translation;

                if (!string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation))
                    item.Translation = Translator.Instance.GetTranslatedString(item.Word);

                if (string.IsNullOrWhiteSpace(item.LastChangeDate))
                    item.LastChangeDate = DateTime.Now.ToString();

                if (string.IsNullOrWhiteSpace(item.Word) && string.IsNullOrWhiteSpace(item.Translation) && !string.IsNullOrWhiteSpace(item.LastChangeDate))
                    item.Word = item.Translation = "Autofixed value";

            }
        }

        public void SetDictionary(ObservableCollection<Entry> list)
        {
            wordDictionary = list;
            wordGrid.DataContext = wordDictionary;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }
    }
}
