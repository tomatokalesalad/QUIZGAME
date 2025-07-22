using System.Windows;

namespace QuizGame1WPF
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            if (login.ShowDialog() == true)
            {
                var main = new MainWindow(); // or your GameModeSelectorWindow
                main.Show();
            }
            else
            {
                Shutdown(); // closes the app if login was cancelled
            }
        }
    }
}
