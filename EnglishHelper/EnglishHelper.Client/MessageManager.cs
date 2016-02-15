using System.Windows;

namespace EnglishHelper.Client
{
    public static class MessageManager 
    {
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string warning)
        {
            MessageBox.Show(warning, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string error)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
