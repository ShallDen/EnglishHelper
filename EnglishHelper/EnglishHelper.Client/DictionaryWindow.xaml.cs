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
using System.Windows.Media.Animation;

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
        void BindDictionaryWithGrid(ObservableCollection<Entry> list);
        event EventHandler DeleteWordButtonClick;
        event EventHandler SaveDictionaryButtonClick;
        event EventHandler<DataGridRowEditEndingEventArgs> DictionaryChanged;
    }

    public partial class DictionaryWindow : Window, IDictionaryWindow
    {
        public DictionaryWindow()
        {
            InitializeComponent();

            deleteWordButton.Click += DeleteWordButton_Click;
            saveDictionaryButton.Click += SaveDictionaryButton_Click;
            wordGrid.RowEditEnding += WordGrid_RowEditEnding;
            this.Closing += DictionaryWindow_Closing;
        }

        public event EventHandler DeleteWordButtonClick;
        public event EventHandler SaveDictionaryButtonClick;
        public event EventHandler<DataGridRowEditEndingEventArgs> DictionaryChanged;

        public IList SelectedRows
        {
            get { return wordGrid.SelectedItems; }
        }

        #region Event throwing
        private void DeleteWordButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteWordButtonClick != null)
                DeleteWordButtonClick(this, e);
        }
        private void SaveDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDictionaryButtonClick != null)
                SaveDictionaryButtonClick(this, e);
        }
        #endregion

        private void WordGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (DictionaryChanged != null)
                DictionaryChanged(sender, e);
        }

        public void OpenWindow()
        {
            this.Show();
        }

        public void CloseWindow()
        {
            this.Close();
        }

        public void BindDictionaryWithGrid(ObservableCollection<Entry> list)
        {
            wordGrid.DataContext = list;
            wordCountLabel.DataContext = list;
        }

        private void DictionaryWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= DictionaryWindow_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
