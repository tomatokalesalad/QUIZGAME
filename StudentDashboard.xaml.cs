using QuizGame1WPF.Models;
using System.ComponentModel;
using System.Windows;
using QuizGame1WPF.Services;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for StudentDashboard.xaml
    /// </summary>
    public partial class StudentDashboard : Window, INotifyPropertyChanged
    {
        private readonly IQuizService? _quizService;
        private readonly IDatabaseService? _databaseService;
        private readonly Player _currentUser;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the welcome message for the current user.
        /// </summary>
        public string WelcomeMessage => $"Welcome back, {_currentUser.Username}!";

        /// <summary>
        /// Initializes a new instance of the StudentDashboard class.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="quizService">The quiz service.</param>
        /// <param name="databaseService">The database service.</param>
        public StudentDashboard(Player currentUser, IQuizService? quizService = null, IDatabaseService? databaseService = null)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _quizService = quizService;
            _databaseService = databaseService;
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the StudentDashboard class for design-time support.
        /// </summary>
        public StudentDashboard() : this(new Player
        {
            ID = 1,
            Username = "Test User",
            Role = "Student",
            StudentID = null,
            InstructorID = null,
            CreatedAt = DateTime.Now,
            LastLogin = null
        })
        {
            // For design-time support
        }

        private async void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (_quizService == null)
            {
                MessageBox.Show("Quiz service not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var quizSession = await _quizService.StartQuizAsync(
                    _currentUser.ID, 
                    _currentUser.Username, 
                    null, // category
                    null, // difficulty
                    10); // question count

                if (quizSession.Questions.Count == 0)
                {
                    MessageBox.Show("No questions found matching your criteria. Please try different settings.", 
                        "No Questions", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var quizWindow = new QuizWindow(_quizService, quizSession);
                quizWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting quiz: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (_quizService != null)
            {
                var statsWindow = new UserStatisticsWindow(_currentUser.ID, _quizService);
                statsWindow.Show();
            }
        }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"User Profile:\nUsername: {_currentUser.Username}\nRole: {_currentUser.Role}\nStudent ID: {_currentUser.StudentID}", 
                "User Profile", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewResults_Click(object sender, RoutedEventArgs e)
        {
            var resultsWindow = new QuizResultsWindow(new QuizSession {
                UserId = _currentUser.ID,
                Username = _currentUser.Username,
                Questions = new List<QuestionModel>(),
                Answers = new List<QuizAnswer>(),
                Category = "All",
                Difficulty = "All",
                StartTime = DateTime.Now
            });
            resultsWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
