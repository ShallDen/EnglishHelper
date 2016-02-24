﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnglishHelper.Client
{
    /// <summary>
    /// Логика взаимодействия для PopupWindow.xaml
    /// </summary>
    
    public interface IPopupWindow
    {
        string Title { get; set; }
        string Message { get; set; }
        void ShowMessage(string title, string message);
    }

    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();

            this.Closing += PopupWindow_Closing;
            this.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => this.DragMove();
        }
        public new string Title
        {
            get { return title.Text; }
            set { title.Text = value; }
        }

        public string Message
        {
            get { return message.Text; }
            set { message.Text = value; }
        }
    

        private void PopupWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(2));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        public void ShowMessage(string title, string message)
        {
            Title = title;
            Message = message;
            this.Show();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            PopupWindow_Closing(sender, new System.ComponentModel.CancelEventArgs());
        }
    }
}
