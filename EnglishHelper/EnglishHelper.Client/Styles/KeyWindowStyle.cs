using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Collections;
using System.Windows.Media.Animation;

namespace EnglishHelper.Client
{
    public partial class KeyWindowStyle
    {
        void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                sender.ForWindowFromTemplate(w => w.Close());
            }
            else
            {
                sender.ForWindowFromTemplate(w =>
                    SendMessage(w.GetWindowHandle(), WM_SYSCOMMAND, (IntPtr)SC_KEYMENU, (IntPtr)' '));
            }
        }

        void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            sender.ForWindowFromTemplate(w => w.Close());
        }

        void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w => w.DragMove());
            }
        }

        void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w =>
                    {
                        if (w.WindowState == WindowState.Maximized)
                        {
                            w.BeginInit();
                            double adjustment = 40.0;
                            var mouse1 = e.MouseDevice.GetPosition(w);
                            var width1 = Math.Max(w.ActualWidth - 2 * adjustment, adjustment);
                            w.WindowState = WindowState.Normal;
                            var width2 = Math.Max(w.ActualWidth - 2 * adjustment, adjustment);
                            w.Left = (mouse1.X - adjustment) * (1 - width2 / width1);
                            w.Top = -7;
                            w.EndInit();
                            w.DragMove();
                        }
                    });
            }
        }

        #region P/Invoke

        const int WM_SYSCOMMAND = 0x112;
        const int SC_SIZE = 0xF000;
        const int SC_KEYMENU = 0xF100;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        void DragSize(IntPtr handle, SizingAction sizingAction)
        {
            SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizingAction), IntPtr.Zero);
            SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
        }

        public enum SizingAction
        {
            North = 3,
            South = 6,
            East = 2,
            West = 1,
            NorthEast = 5,
            NorthWest = 4,
            SouthEast = 8,
            SouthWest = 7
        }

        #endregion
    }
}