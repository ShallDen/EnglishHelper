namespace EnglishHelper.Client
{
    public static class MessageManager 
    {
        public static void ShowMessage(string message)
        {
            PopupWindow popup = new PopupWindow();
            popup.ShowMessage("Information", message);
        }

        public static void ShowWarning(string warning)
        {
            PopupWindow popup = new PopupWindow();
            popup.ShowMessage("Warning", warning);
        }

        public static void ShowError(string error)
        {
            PopupWindow popup = new PopupWindow();
            popup.ShowMessage("Error", error);
        }
    }
}
