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
        void SetDictionary(List<Entry> list);
        event EventHandler AddWordButtonClick;
        event EventHandler DeleteWordButtonClick;
        event EventHandler SaveDictionaryButtonClick;
    }

    public partial class DictionaryWindow : Window, IDictionaryWindow
    {
        private List<Entry> wordDictionary;
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

        private void wordGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //var row = wordGrid.SelectedItem as Entry;
            //if (row.Word == null || row.Translation == null || row.LastChangeDate == null)
            //{
            //    //Logger.LogWarning("Found empty value in item:" +);
            //    Translator translator = new Translator();
            //    translator.Key = KeyManager.LoadKey();
            //    if (row.Translation == null)
            //    {
            //        row.Translation = translator.GetTranslatedString(row.Word);
            //    }
            //    if (row.LastChangeDate == null)
            //    {
            //        row.LastChangeDate = DateTime.Now.ToString();
            //    }
            //}
            //RefreshGrid();
        }

        public void SetDictionary(List<Entry> list)
        {
            wordDictionary = list;
            wordGrid.ItemsSource = wordDictionary;
        }
    }
}
