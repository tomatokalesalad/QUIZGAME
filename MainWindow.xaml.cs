using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using QuizGame1WPF.Services;

namespace QuizGame1WPF
{
    public partial class MainWindow : Window
    {
        // ✅ Use only ONE connection string
        private const string ConnStr =
            @"Server=.\SQLEXPRESS;Database=QuizGameDB;Trusted_Connection=True;Encrypt=False;";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;
            string role = ((ComboBoxItem)RoleBox.SelectedItem)?.Content?.ToString() ?? "";

            using var con = new SqlConnection(ConnStr);
            try
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p AND Role=@r", con);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                cmd.Parameters.AddWithValue("@r", role);

                int match = (int)cmd.ExecuteScalar();
                if (match == 1)
                    MessageBox.Show($"✅ Logged in as {role}!");
                else
                    MessageBox.Show("❌ Invalid credentials.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string role = ((ComboBoxItem)RoleBox.SelectedItem)?.Content?.ToString() ?? "";
            if (role == "")
            {
                MessageBox.Show("Please select a role first.");
                return;
            }

            var mockDbService = new DatabaseService(null!, null!);
            var win = new SignUpWindow(mockDbService, role)
            {
                Owner = this
            };
            win.ShowDialog();
        }
    }
}
