using QuizGame1WPF.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private readonly IDatabaseService? _databaseService;

        /// <summary>
        /// Initializes a new instance of the SignUpWindow class with a database service and optional role.
        /// </summary>
        /// <param name="databaseService">The database service to use for user creation.</param>
        /// <param name="role">The user role ("Student" or "Teacher").</param>
        public SignUpWindow(IDatabaseService databaseService, string role = "")
        {
            InitializeComponent();
            _databaseService = databaseService;
            
            if (!string.IsNullOrEmpty(role))
            {
                RoleBox.SelectedIndex = role == "Student" ? 0 : 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SignUpWindow class for design-time support.
        /// </summary>
        public SignUpWindow()
        {
            InitializeComponent();
            // Fallback constructor for design-time support
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            if (_databaseService == null)
            {
                MessageBox.Show("Database service not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string user = UserBox.Text.Trim();
            string pass = PassBox.Password;
            string? role = (RoleBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string id = IdBox.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) ||
                string.IsNullOrEmpty(role) || string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            try
            {
                bool success = await _databaseService.CreateUserAsync(user, pass, role, id);
                
                if (success)
                {
                    MessageBox.Show("Account created successfully!");
                    Close();
                }
                else
                {
                    MessageBox.Show("Username already taken or an error occurred.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating account: " + ex.Message);
            }
        }
    }
}
