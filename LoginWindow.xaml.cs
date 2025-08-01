using Microsoft.Extensions.DependencyInjection;
using QuizGame1WPF.Models;
using QuizGame1WPF.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IDatabaseService? _databaseService;
        private readonly IQuizService? _quizService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class with database and quiz services.
        /// </summary>
        /// <param name="databaseService">The database service to use.</param>
        /// <param name="quizService">The quiz service to use.</param>
        public LoginWindow(IDatabaseService databaseService, IQuizService quizService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _quizService = quizService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class for design-time support.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
            // Fallback constructor for design-time support
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            Button? loginButton = sender as Button;

            if (_databaseService == null)
            {
                MessageBox.Show("Database service not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;
            string? role = (RoleBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (loginButton != null)
            {
                loginButton.IsEnabled = false;
                loginButton.Content = "Logging in...";
            }

            try
            {
                var player = await _databaseService.AuthenticateUserAsync(username, password, role);

                if (player != null)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Window? dashboard = null;

                    if (role == "Teacher")
                    {
                        dashboard = new TeacherDashboard();
                    }
                    else if (role == "Student" && _quizService != null)
                    {
                        dashboard = new StudentDashboard(player, _quizService, _databaseService);
                    }

                    if (dashboard != null)
                    {
                        dashboard.Show();
                        this.DialogResult = true;
                        this.Close();
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
            finally
            {
                if (loginButton != null)
                {
                    loginButton.IsEnabled = true;
                    loginButton.Content = "Login";
                }
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if (_databaseService != null)
            {
                var signup = new SignUpWindow(_databaseService);
                signup.ShowDialog();
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Password recovery feature coming soon!\n\nPlease contact your administrator for password reset.", 
                "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GuestLogin_Click(object sender, RoutedEventArgs e)
        {
            if (_quizService != null && _databaseService != null)
            {
                var guestPlayer = new Player
                {
                    ID = -1,
                    Username = "Guest",
                    Role = "Student",
                    StudentID = null,
                    InstructorID = null,
                    CreatedAt = DateTime.Now,
                    LastLogin = DateTime.Now
                };
                var dashboard = new StudentDashboard(guestPlayer, _quizService, _databaseService);
                dashboard.Show();
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
