using System.ComponentModel.DataAnnotations;

namespace QuizGame1WPF.Models
{
    /// <summary>
    /// Represents a quiz session for a user.
    /// </summary>
    public class QuizSession
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public int SessionId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the start time of the quiz session.
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time of the quiz session.
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// Gets or sets the list of questions in the session.
        /// </summary>
        public List<QuestionModel> Questions { get; set; } = new();
        /// <summary>
        /// Gets or sets the list of answers in the session.
        /// </summary>
        public List<QuizAnswer> Answers { get; set; } = new();
        /// <summary>
        /// Gets or sets the category of the quiz.
        /// </summary>
        public string Category { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the difficulty of the quiz.
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the current question index.
        /// </summary>
        public int CurrentQuestionIndex { get; set; } = 0;
        /// <summary>
        /// Gets or sets the total score for the session.
        /// </summary>
        public int TotalScore { get; set; } = 0;
        /// <summary>
        /// Gets the maximum possible score for the session.
        /// </summary>
        public int MaxPossibleScore => Questions.Count * 10; // 10 points per question
        /// <summary>
        /// Gets or sets whether the session is completed.
        /// </summary>
        public bool IsCompleted { get; set; } = false;
        /// <summary>
        /// Gets the score percentage for the session.
        /// </summary>
        public double ScorePercentage => MaxPossibleScore > 0 ? (double)TotalScore / MaxPossibleScore * 100 : 0;
        /// <summary>
        /// Gets the current question in the session.
        /// </summary>
        public QuestionModel? CurrentQuestion => 
            CurrentQuestionIndex < Questions.Count ? Questions[CurrentQuestionIndex] : null;
        /// <summary>
        /// Gets whether there is a next question in the session.
        /// </summary>
        public bool HasNextQuestion => CurrentQuestionIndex < Questions.Count - 1;
        /// <summary>
        /// Moves to the next question in the session.
        /// </summary>
        public void NextQuestion()
        {
            if (HasNextQuestion)
                CurrentQuestionIndex++;
        }
        /// <summary>
        /// Adds an answer for the current question.
        /// </summary>
        /// <param name="selectedAnswer">The selected answer.</param>
        /// <param name="timeSpent">The time spent answering.</param>
        public void AddAnswer(string selectedAnswer, int timeSpent)
        {
            if (CurrentQuestion == null) return;
            
            var isCorrect = selectedAnswer == CurrentQuestion.CorrectAnswer;
            var pointsEarned = CalculatePoints(isCorrect, timeSpent, CurrentQuestion.TimeLimitInSeconds);
            
            var answer = new QuizAnswer
            {
                QuestionId = CurrentQuestion.ID,
                SelectedAnswer = selectedAnswer,
                CorrectAnswer = CurrentQuestion.CorrectAnswer,
                IsCorrect = isCorrect,
                TimeSpent = timeSpent,
                PointsEarned = pointsEarned,
                AnsweredAt = DateTime.Now
            };
            
            Answers.Add(answer);
            TotalScore += pointsEarned;
        }
        /// <summary>
        /// Calculates the points earned for a question.
        /// </summary>
        /// <param name="isCorrect">Whether the answer is correct.</param>
        /// <param name="timeSpent">The time spent answering.</param>
        /// <param name="timeLimit">The time limit for the question.</param>
        /// <returns>The points earned.</returns>
        private int CalculatePoints(bool isCorrect, int timeSpent, int timeLimit)
        {
            if (!isCorrect) return 0;
            
            // Base points for correct answer
            int basePoints = 10;
            
            // Bonus points for quick answers (up to 5 extra points)
            double timeBonus = Math.Max(0, (timeLimit - timeSpent) / (double)timeLimit * 5);
            
            return basePoints + (int)Math.Round(timeBonus);
        }
    }
    /// <summary>
    /// Represents an answer to a quiz question.
    /// </summary>
    public class QuizAnswer
    {
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        public int QuestionId { get; set; }
        /// <summary>
        /// Gets or sets the selected answer.
        /// </summary>
        public string SelectedAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the correct answer.
        /// </summary>
        public string CorrectAnswer { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets whether the answer is correct.
        /// </summary>
        public bool IsCorrect { get; set; }
        /// <summary>
        /// Gets or sets the time spent answering in seconds.
        /// </summary>
        public int TimeSpent { get; set; } // seconds
        /// <summary>
        /// Gets or sets the points earned for the answer.
        /// </summary>
        public int PointsEarned { get; set; }
        /// <summary>
        /// Gets or sets the date and time the answer was given.
        /// </summary>
        public DateTime AnsweredAt { get; set; }
    }
}