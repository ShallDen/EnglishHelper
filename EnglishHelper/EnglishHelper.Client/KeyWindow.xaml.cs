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

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    /// 
    public interface IKeyWindow
    {
        string Key { get; set; }
        void OpenWindow();
        void CloseWindow();
        event EventHandler ApplyButtonClick;
        event EventHandler WindowClosed;
    }

    public partial class KeyWindow : Window, IKeyWindow
    {
        public KeyWindow()
        {
            InitializeComponent();

            applyButton.Click += ApplyButton_Click;
            this.Closed += KeyWindow_Closed;
        }

        public event EventHandler ApplyButtonClick;
        public event EventHandler WindowClosed;

        public string Key
        {
            get { return keyTextBox.Text; }
            set { keyTextBox.Text = value; }
        }

        public void OpenWindow()
        {
            this.ShowDialog();
        }

        public void CloseWindow()
        {
            this.Close();
        }

        #region Events throwing

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyButtonClick != null)
                ApplyButtonClick(this, EventArgs.Empty);
        }
        private void KeyWindow_Closed(object sender, EventArgs e)
        {
            if (WindowClosed != null)
                WindowClosed(this, EventArgs.Empty);
        }

        #endregion
    }
}
