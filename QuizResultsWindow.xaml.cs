using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using QuizGame1WPF.Models;
using System.Linq;
using System.ComponentModel;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for QuizResultsWindow.xaml
    /// </summary>
    public partial class QuizResultsWindow : Window, INotifyPropertyChanged
    {
        private readonly QuizSession _quizSession;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the quiz title for display.
        /// </summary>
        public string QuizTitle => $"Quiz Results - {_quizSession.Category} ({_quizSession.Difficulty})";
        /// <summary>
        /// Gets the final score for the quiz.
        /// </summary>
        public string FinalScore => $"{_quizSession.TotalScore} / {_quizSession.MaxPossibleScore}";
        /// <summary>
        /// Gets the percentage score for the quiz.
        /// </summary>
        public string Percentage => $"{_quizSession.ScorePercentage:F1}%";
        /// <summary>
        /// Gets the duration of the quiz.
        /// </summary>
        public string QuizDuration => _quizSession.EndTime.HasValue 
            ? $"{(_quizSession.EndTime.Value - _quizSession.StartTime).TotalMinutes:F1} minutes"
            : "Not completed";
        /// <summary>
        /// Gets the number of correct answers.
        /// </summary>
        public string CorrectAnswers => $"{_quizSession.Answers.Count(a => a.IsCorrect)} / {_quizSession.Answers.Count}";
        /// <summary>
        /// Gets the grade for the quiz.
        /// </summary>
        public string Grade => GetGrade(_quizSession.ScorePercentage);
        /// <summary>
        /// Gets the brush color for the grade.
        /// </summary>
        public Brush GradeBrush => GetGradeBrush(_quizSession.ScorePercentage);
        /// <summary>
        /// Gets the detailed results for each question.
        /// </summary>
        public List<QuizResultDetailModel> DetailedResults { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultsWindow"/> class.
        /// </summary>
        public QuizResultsWindow(QuizSession quizSession)
        {
            InitializeComponent();
            _quizSession = quizSession;
            
            DetailedResults = CreateDetailedResults();
            DataContext = this;
            lvResults.ItemsSource = DetailedResults;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizResultsWindow"/> class for designer support.
        /// </summary>
        public QuizResultsWindow()
            : this(CreateSampleSession())
        {
            // For designer support
        }

        /// <summary>
        /// Creates detailed results for each question in the quiz session.
        /// </summary>
        private List<QuizResultDetailModel> CreateDetailedResults()
        {
            var results = new List<QuizResultDetailModel>();
            
            for (int i = 0; i < _quizSession.Questions.Count; i++)
            {
                var question = _quizSession.Questions[i];
                var answer = _quizSession.Answers.FirstOrDefault(a => a.QuestionId == question.ID);
                
                results.Add(new QuizResultDetailModel
                {
                    QuestionNumber = i + 1,
                    Question = question.Question,
                    UserAnswer = GetAnswerText(question, answer?.SelectedAnswer ?? ""),
                    CorrectAnswer = GetAnswerText(question, question.CorrectAnswer),
                    IsCorrect = answer?.IsCorrect ?? false,
                    PointsEarned = answer?.PointsEarned ?? 0,
                    TimeSpent = answer?.TimeSpent ?? 0,
                    TimeLimitSeconds = question.TimeLimitInSeconds
                });
            }
            
            return results;
        }

        /// <summary>
        /// Gets the answer text for a given option.
        /// </summary>
        private string GetAnswerText(QuestionModel question, string option)
        {
            return option switch
            {
                "A" => $"A) {question.OptionA}",
                "B" => $"B) {question.OptionB}",
                "C" => $"C) {question.OptionC}",
                "D" => $"D) {question.OptionD}",
                _ => "No answer"
            };
        }

        /// <summary>
        /// Gets the grade letter for a given percentage.
        /// </summary>
        private string GetGrade(double percentage)
        {
            return percentage switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _ => "F"
            };
        }

        /// <summary>
        /// Gets the brush color for a given grade percentage.
        /// </summary>
        private Brush GetGradeBrush(double percentage)
        {
            return percentage switch
            {
                >= 90 => Brushes.Green,
                >= 80 => Brushes.LimeGreen,
                >= 70 => Brushes.Orange,
                >= 60 => Brushes.Gold,
                _ => Brushes.Red
            };
        }

        /// <summary>
        /// Creates a sample quiz session for designer support.
        /// </summary>
        private static QuizSession CreateSampleSession()
        {
            var sampleSession = new QuizSession
            {
                UserId = 1,
                Username = "Sample User",
                StartTime = DateTime.Now.AddMinutes(-5),
                EndTime = DateTime.Now,
                Category = "Sample Category",
                Difficulty = "Medium",
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        ID = 1,
                        Question = "Sample Question 1?",
                        OptionA = "Option A",
                        OptionB = "Option B",
                        OptionC = "Option C",
                        OptionD = "Option D",
                        CorrectAnswer = "B",
                        Category = "Sample Category",
                        Difficulty = "Medium",
                        TimeLimitInSeconds = 30
                    }
                },
                Answers = new List<QuizAnswer>
                {
                    new QuizAnswer 
                    { 
                        QuestionId = 1, 
                        SelectedAnswer = "A", 
                        CorrectAnswer = "B", 
                        IsCorrect = false, 
                        PointsEarned = 0, 
                        TimeSpent = 15 
                    }
                }
            };
            
            return sampleSession;
        }

        /// <summary>
        /// Handles the click event to close the window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the click event to retry the quiz.
        /// </summary>
        private void RetryQuiz_Click(object sender, RoutedEventArgs e)
        {
            // Signal that user wants to retry
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the click event to view statistics.
        /// </summary>
        private void ViewStatistics_Click(object sender, RoutedEventArgs e)
        {
            // Open statistics window
            var statsWindow = new UserStatisticsWindow(_quizSession.UserId);
            statsWindow.Show();
        }

        /// <summary>
        /// Handles the selection changed event for the results list view.
        /// </summary>
        private void lvResults_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // You can add logic here if you want to handle selection changes in the results list view.
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

    /// <summary>
    /// Represents the detailed result for a quiz question.
    /// </summary>
    public class QuizResultDetailModel
    {
        /// <summary>
        /// Gets or sets the question number.
        /// </summary>
        public int QuestionNumber { get; set; }
        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string Question { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the user's answer.
        /// </summary>
        public string UserAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the correct answer.
        /// </summary>
        public string CorrectAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether the answer was correct.
        /// </summary>
        public bool IsCorrect { get; set; }
        /// <summary>
        /// Gets or sets the points earned for the question.
        /// </summary>
        public int PointsEarned { get; set; }
        /// <summary>
        /// Gets or sets the time spent on the question.
        /// </summary>
        public int TimeSpent { get; set; }
        /// <summary>
        /// Gets or sets the time limit for the question in seconds.
        /// </summary>
        public int TimeLimitSeconds { get; set; }
        /// <summary>
        /// Gets the result text for display.
        /// </summary>
        public string ResultText => IsCorrect ? "? Correct" : "? Wrong";
        /// <summary>
        /// Gets the brush color for the result.
        /// </summary>
        public Brush ResultBrush => IsCorrect ? Brushes.Green : Brushes.Red;
        /// <summary>
        /// Gets the time spent text for display.
        /// </summary>
        public string TimeText => $"{TimeSpent}s / {TimeLimitSeconds}s";
    }

    /// <summary>
    /// Represents the result for a quiz question (legacy model).
    /// </summary>
    public class QuizResultModel
    {
        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string Question { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the user's answer.
        /// </summary>
        public string UserAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the correct answer.
        /// </summary>
        public string CorrectAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets whether the answer was correct.
        /// </summary>
        public bool IsCorrect => UserAnswer == CorrectAnswer;
        /// <summary>
        /// Gets the result text for display.
        /// </summary>
        public string ResultText => IsCorrect ? "Correct" : "Wrong";
    }
}