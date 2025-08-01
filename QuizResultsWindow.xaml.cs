using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using QuizGame1WPF.Models;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using QuizGame1WPF.Services;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for QuizResultsWindow.xaml
    /// </summary>
    public partial class QuizResultsWindow : Window, INotifyPropertyChanged
    {
        private readonly IQuizService _quizService;
        private readonly int _userId;
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the quiz history rows for the logged-in student.
        /// </summary>
        public List<QuizHistoryRow> QuizHistory { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultsWindow"/> class for a single quiz session.
        /// </summary>
        /// <param name="quizSession">The quiz session to display.</param>
        public QuizResultsWindow(QuizSession quizSession)
        {
            InitializeComponent();
            DataContext = this;
            lvResults.ItemsSource = quizSession.Answers.Select(a => new QuizHistoryRow
            {
                Date = quizSession.StartTime,
                Category = quizSession.Category,
                Difficulty = quizSession.Difficulty,
                Score = quizSession.TotalScore,
                MaxScore = quizSession.MaxPossibleScore,
                Percentage = quizSession.ScorePercentage
            }).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultsWindow"/> class for quiz history.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="quizService">The quiz service.</param>
        public QuizResultsWindow(int userId, IQuizService quizService)
        {
            InitializeComponent();
            _quizService = quizService;
            _userId = userId;
            DataContext = this;
            LoadQuizHistory();
        }

        private async void LoadQuizHistory()
        {
            var sessions = await _quizService.GetUserQuizHistoryAsync(_userId);
            QuizHistory.Clear();
            foreach (var session in sessions)
            {
                QuizHistory.Add(new QuizHistoryRow
                {
                    Date = session.StartTime,
                    Category = session.Category,
                    Score = session.TotalScore,
                    MaxScore = session.MaxPossibleScore,
                    Percentage = session.ScorePercentage,
                    Difficulty = session.Difficulty
                });
            }
            lvResults.ItemsSource = QuizHistory;
            OnPropertyChanged(nameof(QuizHistory));
        }

        /// <summary>
        /// Represents a row in the quiz history DataGrid.
        /// </summary>
        public class QuizHistoryRow
        {
            /// <summary>
            /// Gets or sets the date of the quiz session.
            /// </summary>
            public DateTime Date { get; set; }
            /// <summary>
            /// Gets or sets the category of the quiz.
            /// </summary>
            public string Category { get; set; } = "";
            /// <summary>
            /// Gets or sets the difficulty of the quiz.
            /// </summary>
            public string Difficulty { get; set; } = "";
            /// <summary>
            /// Gets or sets the score achieved in the quiz.
            /// </summary>
            public int Score { get; set; }
            /// <summary>
            /// Gets or sets the maximum possible score for the quiz.
            /// </summary>
            public int MaxScore { get; set; }
            /// <summary>
            /// Gets or sets the percentage score for the quiz.
            /// </summary>
            public double Percentage { get; set; }
            /// <summary>
            /// Gets the formatted score display string.
            /// </summary>
            public string ScoreDisplay => $"{Score} / {MaxScore} ({Percentage:F1}%)";
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void lvResults_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // No-op for now
        }
    }
}