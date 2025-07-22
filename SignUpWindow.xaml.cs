using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    public partial class SignUpWindow : Window
    {
        private const string ConnStr =
            "Server=localhost\\SQLEXPRESS;Database=QuizGameDB;" +
            "Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        public SignUpWindow(string role)  // <-- constructor with role parameter
        {
            InitializeComponent();
            RoleBox.SelectedIndex = role == "Student" ? 0 : 1;  // Pre-select Student or Teacher
        }

        public SignUpWindow()  // default constructor for XAML design-time support (optional)
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string user = UserBox.Text.Trim();
            string pass = PassBox.Password;
            string role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string id = IdBox.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) ||
                string.IsNullOrEmpty(role) || string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            using var con = new SqlConnection(ConnStr);
            con.Open();

            var chk = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", con);
            chk.Parameters.AddWithValue("@u", user);
            if ((int)chk.ExecuteScalar() > 0)
            {
                MessageBox.Show("Username already taken.");
                return;
            }

            var cmd = new SqlCommand("INSERT INTO Users (Username, Password, Role, StudentID, InstructorID) VALUES (@u, @p, @r, @sid, @tid)", con);
            cmd.Parameters.AddWithValue("@u", user);
            cmd.Parameters.AddWithValue("@p", pass);
            cmd.Parameters.AddWithValue("@r", role);
            cmd.Parameters.AddWithValue("@sid", role == "Student" ? id : DBNull.Value);
            cmd.Parameters.AddWithValue("@tid", role == "Teacher" ? id : DBNull.Value);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Account created successfully!");
            Close();
        }
    }
}
