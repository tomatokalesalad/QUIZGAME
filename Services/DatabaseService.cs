using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuizGame1WPF.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuizGame1WPF.Services
{
    /// <summary>
    /// Provides database-related services and operations for the Quiz Game application.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Gets all questions from the database.
        /// </summary>
        Task<List<QuestionModel>> GetQuestionsAsync();
        /// <summary>
        /// Saves a question to the database.
        /// </summary>
        Task<bool> SaveQuestionAsync(QuestionModel question);
        /// <summary>
        /// Authenticates a user with the given credentials.
        /// </summary>
        Task<Player?> AuthenticateUserAsync(string username, string password, string role);
        /// <summary>
        /// Creates a new user account in the database.
        /// </summary>
        Task<bool> CreateUserAsync(string username, string password, string role, string id);
        /// <summary>
        /// Saves a quiz session to the database.
        /// </summary>
        Task<bool> SaveQuizSessionAsync(QuizSession session);
        /// <summary>
        /// Gets the quiz history for a user.
        /// </summary>
        Task<List<QuizSession>> GetUserQuizHistoryAsync(int userId);
        /// <summary>
        /// Deletes a question from the database.
        /// </summary>
        Task<bool> DeleteQuestionAsync(int questionId);
        /// <summary>
        /// Gets quiz statistics for a user.
        /// </summary>
        Task<List<QuizStatistics>> GetQuizStatisticsAsync(int userId);
    }

    /// <summary>
    /// Implements database-related services and operations for the Quiz Game application.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            // Always use local SQLEXPRESS for development
            _connectionString = "Server=localhost\\SQLEXPRESS;Database=QuizGameDB;Trusted_Connection=True;Encrypt=False;";
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<QuestionModel>> GetQuestionsAsync()
        {
            var questions = new List<QuestionModel>();
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("SELECT * FROM Questions ORDER BY Category, Difficulty", connection);
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    questions.Add(new QuestionModel
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Question = reader.GetString(reader.GetOrdinal("Question")),
                        OptionA = reader.GetString(reader.GetOrdinal("OptionA")),
                        OptionB = reader.GetString(reader.GetOrdinal("OptionB")),
                        OptionC = reader.GetString(reader.GetOrdinal("OptionC")),
                        OptionD = reader.GetString(reader.GetOrdinal("OptionD")),
                        CorrectAnswer = reader.GetString(reader.GetOrdinal("CorrectAnswer")),
                        Category = reader.GetString(reader.GetOrdinal("Category")),
                        Difficulty = reader.GetString(reader.GetOrdinal("Difficulty")),
                        TimeLimitInSeconds = reader.IsDBNull(reader.GetOrdinal("TimeLimitInSeconds")) ? 30 : reader.GetInt32(reader.GetOrdinal("TimeLimitInSeconds"))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving questions from database");
                throw;
            }
            
            return questions;
        }

        /// <inheritdoc/>
        public async Task<bool> SaveQuestionAsync(QuestionModel question)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                string sql = question.ID == 0 
                    ? "INSERT INTO Questions (Question, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, Category, Difficulty, TimeLimitInSeconds) VALUES (@Question, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @Category, @Difficulty, @TimeLimitInSeconds)"
                    : "UPDATE Questions SET Question=@Question, OptionA=@OptionA, OptionB=@OptionB, OptionC=@OptionC, OptionD=@OptionD, CorrectAnswer=@CorrectAnswer, Category=@Category, Difficulty=@Difficulty, TimeLimitInSeconds=@TimeLimitInSeconds WHERE ID=@ID";
                
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Question", question.Question);
                command.Parameters.AddWithValue("@OptionA", question.OptionA);
                command.Parameters.AddWithValue("@OptionB", question.OptionB);
                command.Parameters.AddWithValue("@OptionC", question.OptionC);
                command.Parameters.AddWithValue("@OptionD", question.OptionD);
                command.Parameters.AddWithValue("@CorrectAnswer", question.CorrectAnswer);
                command.Parameters.AddWithValue("@Category", question.Category);
                command.Parameters.AddWithValue("@Difficulty", question.Difficulty);
                command.Parameters.AddWithValue("@TimeLimitInSeconds", question.TimeLimitInSeconds);
                
                if (question.ID != 0)
                    command.Parameters.AddWithValue("@ID", question.ID);
                
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving question to database");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                using var command = new SqlCommand("DELETE FROM Questions WHERE ID = @ID", connection);
                command.Parameters.AddWithValue("@ID", questionId);
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting question from database");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<Player?> AuthenticateUserAsync(string username, string password, string role)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // Hash the password for comparison
                string hashedPassword = HashPassword(password);
                
                using var command = new SqlCommand("SELECT * FROM Users WHERE Username=@Username AND Password=@Password AND Role=@Role", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Role", role);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    // Update last login
                    var player = new Player
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ID")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Role = reader.GetString(reader.GetOrdinal("Role")),
                        StudentID = reader.IsDBNull(reader.GetOrdinal("StudentID")) ? null : reader.GetString(reader.GetOrdinal("StudentID")),
                        InstructorID = reader.IsDBNull(reader.GetOrdinal("InstructorID")) ? null : reader.GetString(reader.GetOrdinal("InstructorID")),
                        CreatedAt = DateTime.Now,
                        LastLogin = DateTime.Now
                    };
                    await reader.CloseAsync();
                    // Update last login time
                    using var updateCommand = new SqlCommand("UPDATE Users SET LastLogin = @LastLogin WHERE ID = @ID", connection);
                    updateCommand.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                    updateCommand.Parameters.AddWithValue("@ID", player.ID);
                    await updateCommand.ExecuteNonQueryAsync();
                    return player;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error authenticating user");
                throw;
            }
            
            return null;
        }

        /// <inheritdoc/>
        public async Task<bool> CreateUserAsync(string username, string password, string role, string id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                // Check if username already exists
                var checkCommand = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@Username", connection);
                checkCommand.Parameters.AddWithValue("@Username", username);
                var countObj = await checkCommand.ExecuteScalarAsync();
                if (countObj != null && Convert.ToInt32(countObj) > 0)
                {
                    return false; // Username already exists
                }

                // Hash the password
                string hashedPassword = HashPassword(password);

                // Create new user
                var insertCommand = new SqlCommand(
                    @"INSERT INTO Users (Username, Password, Role, StudentID, InstructorID, CreatedAt, LastLogin) 
                      VALUES (@Username, @Password, @Role, @StudentID, @InstructorID, @CreatedAt, @LastLogin)", 
                    connection);
                
                insertCommand.Parameters.AddWithValue("@Username", username);
                insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                insertCommand.Parameters.AddWithValue("@Role", role);
                insertCommand.Parameters.AddWithValue("@StudentID", role == "Student" ? id : DBNull.Value);
                insertCommand.Parameters.AddWithValue("@InstructorID", role == "Teacher" ? id : DBNull.Value);
                insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                insertCommand.Parameters.AddWithValue("@LastLogin", DateTime.Now);

                await insertCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating user account");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveQuizSessionAsync(QuizSession session)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    // Insert quiz session
                    var sessionCommand = new SqlCommand(@"
                        INSERT INTO QuizSessions (UserId, StartTime, EndTime, Category, Difficulty, TotalScore, MaxPossibleScore, IsCompleted) 
                        VALUES (@UserId, @StartTime, @EndTime, @Category, @Difficulty, @TotalScore, @MaxPossibleScore, @IsCompleted);
                        SELECT SCOPE_IDENTITY();", connection, transaction);
                    
                    sessionCommand.Parameters.AddWithValue("@UserId", session.UserId);
                    sessionCommand.Parameters.AddWithValue("@StartTime", session.StartTime);
                    sessionCommand.Parameters.AddWithValue("@EndTime", session.EndTime ?? DateTime.Now);
                    sessionCommand.Parameters.AddWithValue("@Category", session.Category);
                    sessionCommand.Parameters.AddWithValue("@Difficulty", session.Difficulty);
                    sessionCommand.Parameters.AddWithValue("@TotalScore", session.TotalScore);
                    sessionCommand.Parameters.AddWithValue("@MaxPossibleScore", session.MaxPossibleScore);
                    sessionCommand.Parameters.AddWithValue("@IsCompleted", session.IsCompleted);
                    
                    int sessionId = Convert.ToInt32(await sessionCommand.ExecuteScalarAsync());
                    
                    // Insert quiz answers
                    foreach (var answer in session.Answers)
                    {
                        var answerCommand = new SqlCommand(@"
                            INSERT INTO QuizAnswers (SessionId, QuestionId, SelectedAnswer, CorrectAnswer, IsCorrect, TimeSpent, PointsEarned, AnsweredAt) 
                            VALUES (@SessionId, @QuestionId, @SelectedAnswer, @CorrectAnswer, @IsCorrect, @TimeSpent, @PointsEarned, @AnsweredAt)", 
                            connection, transaction);
                        
                        answerCommand.Parameters.AddWithValue("@SessionId", sessionId);
                        answerCommand.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
                        answerCommand.Parameters.AddWithValue("@SelectedAnswer", answer.SelectedAnswer);
                        answerCommand.Parameters.AddWithValue("@CorrectAnswer", answer.CorrectAnswer);
                        answerCommand.Parameters.AddWithValue("@IsCorrect", answer.IsCorrect);
                        answerCommand.Parameters.AddWithValue("@TimeSpent", answer.TimeSpent);
                        answerCommand.Parameters.AddWithValue("@PointsEarned", answer.PointsEarned);
                        answerCommand.Parameters.AddWithValue("@AnsweredAt", answer.AnsweredAt);
                        
                        await answerCommand.ExecuteNonQueryAsync();
                    }
                    
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving quiz session");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<List<QuizSession>> GetUserQuizHistoryAsync(int userId)
        {
            var sessions = new List<QuizSession>();
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                using var command = new SqlCommand(@"
                    SELECT * FROM QuizSessions 
                    WHERE UserId = @UserId 
                    ORDER BY StartTime DESC", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var session = new QuizSession
                    {
                        SessionId = reader.GetInt32(reader.GetOrdinal("SessionId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                        EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                        Category = reader.GetString(reader.GetOrdinal("Category")),
                        Difficulty = reader.GetString(reader.GetOrdinal("Difficulty")),
                        TotalScore = reader.GetInt32(reader.GetOrdinal("TotalScore")),
                        IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted"))
                    };
                    sessions.Add(session);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving user quiz history");
                throw;
            }
            
            return sessions;
        }

        /// <inheritdoc/>
        public async Task<List<QuizStatistics>> GetQuizStatisticsAsync(int userId)
        {
            var statistics = new List<QuizStatistics>();
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                using var command = new SqlCommand(@"
                    SELECT 
                        Category,
                        Difficulty,
                        COUNT(*) as TotalQuizzes,
                        AVG(CAST(TotalScore as FLOAT) / MaxPossibleScore * 100) as AverageScore,
                        MAX(CAST(TotalScore as FLOAT) / MaxPossibleScore * 100) as BestScore,
                        SUM((SELECT COUNT(*) FROM QuizAnswers WHERE SessionId IN 
                            (SELECT SessionId FROM QuizSessions WHERE UserId = @UserId AND Category = qs.Category AND Difficulty = qs.Difficulty))) as TotalQuestionsAnswered,
                        SUM((SELECT COUNT(*) FROM QuizAnswers WHERE SessionId IN 
                            (SELECT SessionId FROM QuizSessions WHERE UserId = @UserId AND Category = qs.Category AND Difficulty = qs.Difficulty) AND IsCorrect = 1)) as CorrectAnswers
                    FROM QuizSessions qs
                    WHERE UserId = @UserId AND IsCompleted = 1
                    GROUP BY Category, Difficulty", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var stat = new QuizStatistics
                    {
                        Category = reader.GetString(reader.GetOrdinal("Category")),
                        Difficulty = reader.GetString(reader.GetOrdinal("Difficulty")),
                        TotalQuizzes = reader.GetInt32(reader.GetOrdinal("TotalQuizzes")),
                        AverageScore = reader.IsDBNull(reader.GetOrdinal("AverageScore")) ? 0 : reader.GetDouble(reader.GetOrdinal("AverageScore")),
                        BestScore = reader.IsDBNull(reader.GetOrdinal("BestScore")) ? 0 : reader.GetDouble(reader.GetOrdinal("BestScore")),
                        TotalQuestionsAnswered = reader.IsDBNull(reader.GetOrdinal("TotalQuestionsAnswered")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalQuestionsAnswered")),
                        CorrectAnswers = reader.IsDBNull(reader.GetOrdinal("CorrectAnswers")) ? 0 : reader.GetInt32(reader.GetOrdinal("CorrectAnswers"))
                    };
                    statistics.Add(stat);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving quiz statistics");
                throw;
            }
            
            return statistics;
        }

        /// <summary>
        /// Hashes a password using SHA256 and a salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password as a base64 string.</returns>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "QuizGameSalt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}