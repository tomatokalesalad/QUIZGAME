using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF;

public partial class MainWindow : Window
{
    private const string ConnStr =
@"Server=localhost\\SQLEXPRESS;Database=QuizGameDB;" +
"Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";


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
                "SELECT COUNT(*) FROM Users " +
                "WHERE Username=@u AND Password=@p AND Role=@r", con);
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

        var win = new SignUpWindow(role)
        {
            Owner = this
        };
        win.ShowDialog();
    }
}
