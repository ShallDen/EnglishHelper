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
using System.Windows.Navigation;
using System.Configuration;
using System.Windows.Media.Animation;

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    /// 
    public interface IKeyWindow
    {
        string Key { get; set; }
        string KeyHyperLink { get; set; }
        void OpenWindow();
        void CloseWindow();
        event EventHandler ApplyButtonClick;
        event RequestNavigateEventHandler GetKeyHyperLinkClick;
        event EventHandler WindowClosed;
    }

    public partial class KeyWindow : Window, IKeyWindow
    {
        public KeyWindow()
        {
            InitializeComponent();

            applyButton.Click += ApplyButton_Click;
            getKeyHyperlink.RequestNavigate += GetKeyHyperLink_Clicked;

            keyTextBox.Foreground = Brushes.Gray;

            keyTextBox.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                if (keyTextBox.Text == "Please input your key here...")
                {
                    keyTextBox.Foreground = Brushes.Black;
                    keyTextBox.Text = string.Empty;
                }
            };

            keyTextBox.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                if (string.IsNullOrWhiteSpace(keyTextBox.Text))
                {
                    keyTextBox.Foreground = Brushes.Gray;
                    keyTextBox.Text = "Please input your key here...";
                }
            };

            this.Closing += KeyWindow_Closing;
            this.Closed += KeyWindow_Closed;
        }

        public event EventHandler ApplyButtonClick;
        public event RequestNavigateEventHandler GetKeyHyperLinkClick;
        public event EventHandler WindowClosed;

        public string Key
        {
            get { return keyTextBox.Text; }
            set { keyTextBox.Text = value; }
        }

        public string KeyHyperLink
        {
            get { return ConfigurationManager.AppSettings["KeyHyperLink"]; }
            set { }
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
                ApplyButtonClick(this, e);
        }

        private void GetKeyHyperLink_Clicked(object sender, RequestNavigateEventArgs e)
        {
            if (GetKeyHyperLinkClick != null)
                GetKeyHyperLinkClick(this, e);
        }

        private void KeyWindow_Closed(object sender, EventArgs e)
        {
            if (WindowClosed != null)
                WindowClosed(this, e);
        }

        private void KeyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= KeyWindow_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromMilliseconds(500));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        #endregion

    }
}
