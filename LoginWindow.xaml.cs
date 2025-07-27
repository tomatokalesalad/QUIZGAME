using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    public partial class LoginWindow : Window
    {
        private readonly string ConnStr = @"Server=localhost;Database=QuizGameDB;Trusted_Connection=True;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = (RoleBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(ConnStr);
                con.Open();

                var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p AND Role=@r", con);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", role);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Window dashboard = null;

                    if (role == "Teacher")
                    {
                        dashboard = new TeacherDashboard();
                    }
                    else if (role == "Student")
                    {
                        dashboard = new StudentDashboard();
                    }

                    if (dashboard != null)
                    {
                        dashboard.Show();
                        this.Close(); // Close login window after successful login
                    }
                    else
                    {
                        MessageBox.Show("Dashboard for this role is not available.", "Dashboard Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username, password, or role.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login:\n" + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var signup = new SignUpWindow();
            signup.ShowDialog(); // Modal
        }
    }
}
