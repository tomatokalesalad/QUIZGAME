using System.ComponentModel;
using System.Windows;
using QuizGame1WPF.Services;
using QuizGame1WPF.Models;
using System.Collections.ObjectModel;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for UserStatisticsWindow.xaml
    /// </summary>
    public partial class UserStatisticsWindow : Window, INotifyPropertyChanged
    {
        private readonly IQuizService? _quizService;
        private readonly int _userId;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the recent quizzes for the user.
        /// </summary>
        public ObservableCollection<QuizSession> RecentQuizzes { get; set; } = new();
        /// <summary>
        /// Gets or sets the category statistics for the user.
        /// </summary>
        public ObservableCollection<QuizStatistics> CategoryStats { get; set; } = new();

        /// <summary>
        /// Gets the total number of quizzes taken.
        /// </summary>
        public string TotalQuizzes { get; private set; } = "0";
        /// <summary>
        /// Gets the average score across quizzes.
        /// </summary>
        public string AverageScore { get; private set; } = "0%";
        /// <summary>
        /// Gets the best score achieved.
        /// </summary>
        public string BestScore { get; private set; } = "0%";
        /// <summary>
        /// Gets the total time spent on quizzes.
        /// </summary>
        public string TotalTimeSpent { get; private set; } = "0 min";
        /// <summary>
        /// Gets the user's favorite category.
        /// </summary>
        public string FavoriteCategory { get; private set; } = "None";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatisticsWindow"/> class.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="quizService">The quiz service.</param>
        public UserStatisticsWindow(int userId, IQuizService? quizService = null)
        {
            // For now, comment out InitializeComponent to avoid compilation error
            // InitializeComponent();
            _userId = userId;
            _quizService = quizService;
            DataContext = this;
            LoadStatistics();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStatisticsWindow"/> class for designer support.
        /// </summary>
        public UserStatisticsWindow() : this(1) // For design-time support
        {
        }

        /// <summary>
        /// Loads statistics for the user.
        /// </summary>
        private async void LoadStatistics()
        {
            if (_quizService == null) return;

            try
            {
                // Load recent quizzes
                var recentQuizzes = await _quizService.GetUserQuizHistoryAsync(_userId);
                RecentQuizzes.Clear();
                foreach (var quiz in recentQuizzes.Take(10)) // Show last 10 quizzes
                {
                    RecentQuizzes.Add(quiz);
                }

                // Load category statistics
                var categoryStats = await _quizService.GetQuizStatisticsAsync(_userId);
                CategoryStats.Clear();
                foreach (var stat in categoryStats)
                {
                    CategoryStats.Add(stat);
                }

                // Calculate summary statistics
                UpdateSummaryStatistics(recentQuizzes, categoryStats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates summary statistics based on quizzes and stats.
        /// </summary>
        /// <param name="quizzes">The list of quizzes.</param>
        /// <param name="stats">The list of statistics.</param>
        private void UpdateSummaryStatistics(List<QuizSession> quizzes, List<QuizStatistics> stats)
        {
            if (quizzes.Any())
            {
                TotalQuizzes = quizzes.Count.ToString();
                AverageScore = $"{quizzes.Average(q => q.ScorePercentage):F1}%";
                BestScore = $"{quizzes.Max(q => q.ScorePercentage):F1}%";
                
                var totalMinutes = quizzes
                    .Where(q => q.EndTime.HasValue)
                    .Sum(q => (q.EndTime!.Value - q.StartTime).TotalMinutes);
                TotalTimeSpent = $"{totalMinutes:F0} min";

                FavoriteCategory = stats
                    .OrderByDescending(s => s.TotalQuizzes)
                    .FirstOrDefault()?.Category ?? "None";
            }

            OnPropertyChanged(nameof(TotalQuizzes));
            OnPropertyChanged(nameof(AverageScore));
            OnPropertyChanged(nameof(BestScore));
            OnPropertyChanged(nameof(TotalTimeSpent));
            OnPropertyChanged(nameof(FavoriteCategory));
        }

        /// <summary>
        /// Handles the click event to close the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the click event to export data.
        /// </summary>
        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement data export functionality
            MessageBox.Show("Export functionality coming soon!", "Export", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Handles the click event to refresh data.
        /// </summary>
        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
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