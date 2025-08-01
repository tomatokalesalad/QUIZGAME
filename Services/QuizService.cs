using QuizGame1WPF.Models;

namespace QuizGame1WPF.Services
{
    /// <summary>
    /// Provides quiz-related services and operations.
    /// </summary>
    public interface IQuizService
    {
        /// <summary>
        /// Starts a new quiz session for a user.
        /// </summary>
        Task<QuizSession> StartQuizAsync(int userId, string username, string? category = null, string? difficulty = null, int questionCount = 10);
        /// <summary>
        /// Saves a quiz session.
        /// </summary>
        Task<bool> SaveQuizSessionAsync(QuizSession session);
        /// <summary>
        /// Gets the quiz history for a user.
        /// </summary>
        Task<List<QuizSession>> GetUserQuizHistoryAsync(int userId);
        /// <summary>
        /// Gets quiz statistics for a user.
        /// </summary>
        Task<List<QuizStatistics>> GetQuizStatisticsAsync(int userId);
        /// <summary>
        /// Gets questions for a quiz.
        /// </summary>
        Task<List<QuestionModel>> GetQuestionsAsync(string? category = null, string? difficulty = null, int count = 10);
    }

    /// <summary>
    /// Implements quiz-related services and operations.
    /// </summary>
    public class QuizService : IQuizService
    {
        private readonly IDatabaseService _databaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizService"/> class.
        /// </summary>
        public QuizService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <inheritdoc/>
        public async Task<QuizSession> StartQuizAsync(int userId, string username, string? category = null, string? difficulty = null, int questionCount = 10)
        {
            var questions = await GetQuestionsAsync(category, difficulty, questionCount);
            
            var session = new QuizSession
            {
                UserId = userId,
                Username = username,
                StartTime = DateTime.Now,
                Questions = questions,
                Category = category ?? "All",
                Difficulty = difficulty ?? "All"
            };

            return session;
        }

        /// <inheritdoc/>
        public async Task<bool> SaveQuizSessionAsync(QuizSession session)
        {
            session.EndTime = DateTime.Now;
            session.IsCompleted = true;
            
            // Here you would save to database
            // For now, returning true as placeholder
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<List<QuizSession>> GetUserQuizHistoryAsync(int userId)
        {
            // Placeholder - implement database retrieval
            return await Task.FromResult(new List<QuizSession>());
        }

        /// <inheritdoc/>
        public async Task<List<QuizStatistics>> GetQuizStatisticsAsync(int userId)
        {
            // Placeholder - implement statistics calculation
            return await Task.FromResult(new List<QuizStatistics>());
        }

        /// <inheritdoc/>
        public async Task<List<QuestionModel>> GetQuestionsAsync(string? category = null, string? difficulty = null, int count = 10)
        {
            var allQuestions = await _databaseService.GetQuestionsAsync();
            
            var filteredQuestions = allQuestions.AsQueryable();
            
            if (!string.IsNullOrEmpty(category) && category != "All")
                filteredQuestions = filteredQuestions.Where(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(difficulty) && difficulty != "All")
                filteredQuestions = filteredQuestions.Where(q => q.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase));
            
            return filteredQuestions
                .OrderBy(x => Guid.NewGuid()) // Randomize
                .Take(count)
                .ToList();
        }
    }

    /// <summary>
    /// Represents quiz statistics for a user or category.
    /// </summary>
    public class QuizStatistics
    {
        /// <summary>
        /// Gets or sets the category of the quiz.
        /// </summary>
        public string Category { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the difficulty of the quiz.
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the total number of quizzes taken.
        /// </summary>
        public int TotalQuizzes { get; set; }
        /// <summary>
        /// Gets or sets the average score.
        /// </summary>
        public double AverageScore { get; set; }
        /// <summary>
        /// Gets or sets the best score.
        /// </summary>
        public double BestScore { get; set; }
        /// <summary>
        /// Gets or sets the total number of questions answered.
        /// </summary>
        public int TotalQuestionsAnswered { get; set; }
        /// <summary>
        /// Gets or sets the number of correct answers.
        /// </summary>
        public int CorrectAnswers { get; set; }
        /// <summary>
        /// Gets the accuracy percentage.
        /// </summary>
        public double AccuracyPercentage => TotalQuestionsAnswered > 0 ? (double)CorrectAnswers / TotalQuestionsAnswered * 100 : 0;
    }
}