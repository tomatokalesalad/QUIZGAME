using System.Windows;

namespace QuizGame1WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            // Temporary login check
            if (username == "admin" && password == "1234")
            {
                MainWindow main = new MainWindow(username);
                main.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = "Invalid username or password.";
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signup = new SignUpWindow();
            signup.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
