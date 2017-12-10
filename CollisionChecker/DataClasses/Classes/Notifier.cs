using System.Windows;

namespace CollisionChecker
{
    public class Notifier : INotifier
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}