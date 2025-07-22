using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    public partial class LoginWindow : Window
    {
        private string ConnStr = @"Server=localhost;Database=QuizGameDB;Trusted_Connection=True;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            using var con = new SqlConnection(ConnStr);
            con.Open();

            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p AND Role=@r", con);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);
            cmd.Parameters.AddWithValue("@r", role);

            int count = (int)cmd.ExecuteScalar();

            if (count > 0)
            {
                MessageBox.Show("Login successful!");
                Close(); // Replace with dashboard if needed
            }
            else
            {
                MessageBox.Show("Invalid credentials.");
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var signup = new SignUpWindow();
            signup.ShowDialog();
        }
    }
}
