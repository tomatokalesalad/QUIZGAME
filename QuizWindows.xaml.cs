using System.Windows;
using System.Windows.Controls;
using QuizGame1WPF.Models;
using QuizGame1WPF.Services;
using System.ComponentModel;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window, INotifyPropertyChanged
    {
        private readonly IQuizService _quizService;
        private TimerHelper _timer = null!;
        private QuizSession _quizSession;
        private DateTime _questionStartTime;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the current question text.
        /// </summary>
        public string QuestionText => CurrentQuestion?.Question ?? "";
        /// <summary>
        /// Gets option A text.
        /// </summary>
        public string OptionA => CurrentQuestion?.OptionA ?? "";
        /// <summary>
        /// Gets option B text.
        /// </summary>
        public string OptionB => CurrentQuestion?.OptionB ?? "";
        /// <summary>
        /// Gets option C text.
        /// </summary>
        public string OptionC => CurrentQuestion?.OptionC ?? "";
        /// <summary>
        /// Gets option D text.
        /// </summary>
        public string OptionD => CurrentQuestion?.OptionD ?? "";
        /// <summary>
        /// Gets the progress text for the quiz.
        /// </summary>
        public string ProgressText => $"Question {_quizSession.CurrentQuestionIndex + 1} of {_quizSession.Questions.Count}";
        /// <summary>
        /// Gets the score text for the quiz.
        /// </summary>
        public string ScoreText => $"Score: {_quizSession.TotalScore}";
        /// <summary>
        /// Gets the progress percentage for the quiz.
        /// </summary>
        public double ProgressPercentage => _quizSession.Questions.Count > 0 ? 
            (double)_quizSession.CurrentQuestionIndex / _quizSession.Questions.Count * 100 : 0;

        private QuestionModel? CurrentQuestion => _quizSession.CurrentQuestion;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizWindow"/> class.
        /// </summary>
        /// <param name="quizService">The quiz service.</param>
        /// <param name="quizSession">The quiz session.</param>
        public QuizWindow(IQuizService quizService, QuizSession quizSession)
        {
            InitializeComponent();
            _quizService = quizService;
            _quizSession = quizSession;
            DataContext = this;
            LoadCurrentQuestion();
        }

        /// <summary>
        /// Loads the current question and updates the UI.
        /// </summary>
        private void LoadCurrentQuestion()
        {
            if (CurrentQuestion == null)
            {
                EndQuiz();
                return;
            }

            _questionStartTime = DateTime.Now;
            
            // Clear previous selections
            optA.IsChecked = false;
            optB.IsChecked = false;
            optC.IsChecked = false;
            optD.IsChecked = false;
            
            // Update UI
            OnPropertyChanged(nameof(QuestionText));
            OnPropertyChanged(nameof(OptionA));
            OnPropertyChanged(nameof(OptionB));
            OnPropertyChanged(nameof(OptionC));
            OnPropertyChanged(nameof(OptionD));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(ScoreText));
            OnPropertyChanged(nameof(ProgressPercentage));

            StartTimer();
        }

        /// <summary>
        /// Starts the timer for the current question.
        /// </summary>
        private void StartTimer()
        {
            _timer?.Stop(); // Stop previous timer if exists
            _timer = new TimerHelper();
            
            int timeLimit = CurrentQuestion?.TimeLimitInSeconds ?? 30;
            
            _timer.OnTick += (seconds) =>
            {
                txtTimer.Text = $"Time Left: {seconds}s";
            };
            
            _timer.OnTimeUp += () =>
            {
                MessageBox.Show("Time's up!", "Time Expired", MessageBoxButton.OK, MessageBoxImage.Warning);
                SubmitAnswer(""); // Submit with no answer
            };
            
            _timer.Start(timeLimit);
        }

        /// <summary>
        /// Handles the click event for submitting an answer.
        /// </summary>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string selected = GetSelectedOption();
            if (string.IsNullOrEmpty(selected))
            {
                MessageBox.Show("Please select an answer before submitting.", "No Answer Selected", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            SubmitAnswer(selected);
        }

        /// <summary>
        /// Submits the selected answer and processes the result.
        /// </summary>
        /// <param name="selectedAnswer">The selected answer.</param>
        private void SubmitAnswer(string selectedAnswer)
        {
            _timer?.Stop();
            
            int timeSpent = (int)(DateTime.Now - _questionStartTime).TotalSeconds;
            _quizSession.AddAnswer(selectedAnswer, timeSpent);
            
            var currentQ = CurrentQuestion;
            if (currentQ != null)
            {
                bool isCorrect = selectedAnswer == currentQ.CorrectAnswer;
                var lastAnswer = _quizSession.Answers.LastOrDefault();
                
                string resultMessage = isCorrect 
                    ? $"Correct! +{lastAnswer?.PointsEarned ?? 0} points" 
                    : $"Wrong! The correct answer was: {currentQ.CorrectAnswer}";
                
                MessageBox.Show(resultMessage, isCorrect ? "Correct!" : "Incorrect", 
                    MessageBoxButton.OK, isCorrect ? MessageBoxImage.Information : MessageBoxImage.Error);
            }

            MoveToNextQuestion();
        }

        /// <summary>
        /// Gets the selected option from the UI.
        /// </summary>
        /// <returns>The selected option as a string.</returns>
        private string GetSelectedOption()
        {
            if (optA.IsChecked == true) return "A";
            if (optB.IsChecked == true) return "B";
            if (optC.IsChecked == true) return "C";
            if (optD.IsChecked == true) return "D";
            return "";
        }

        /// <summary>
        /// Moves to the next question in the quiz.
        /// </summary>
        private void MoveToNextQuestion()
        {
            if (_quizSession.HasNextQuestion)
            {
                _quizSession.NextQuestion();
                LoadCurrentQuestion();
            }
            else
            {
                EndQuiz();
            }
        }

        /// <summary>
        /// Ends the quiz and shows the results window.
        /// </summary>
        private async void EndQuiz()
        {
            _timer?.Stop();
            
            try
            {
                await _quizService.SaveQuizSessionAsync(_quizSession);
                
                var resultsWindow = new QuizResultsWindow(_quizSession);
                resultsWindow.Show();
                
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quiz results: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Handles the click event for skipping a question.
        /// </summary>
        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to skip this question? You will receive 0 points.", 
                "Skip Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                SubmitAnswer(""); // Submit with no answer
            }
        }

        /// <summary>
        /// Handles the window closing event.
        /// </summary>
        /// <param name="e">Cancel event arguments.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_quizSession.IsCompleted)
            {
                var result = MessageBox.Show("Are you sure you want to quit the quiz? Your progress will be lost.", 
                    "Quit Quiz", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            _timer?.Stop();
            base.OnClosing(e);
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
