using System.ComponentModel.DataAnnotations;

namespace QuizGame1WPF.Models
{
    /// <summary>
    /// Represents a question in the quiz game.
    /// </summary>
    public class QuestionModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the question.
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        [Required(ErrorMessage = "Question text is required")]
        [StringLength(500, ErrorMessage = "Question must be less than 500 characters")]
        public string Question { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets option A for the question.
        /// </summary>
        [Required(ErrorMessage = "Option A is required")]
        public string OptionA { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets option B for the question.
        /// </summary>
        [Required(ErrorMessage = "Option B is required")]
        public string OptionB { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets option C for the question.
        /// </summary>
        [Required(ErrorMessage = "Option C is required")]
        public string OptionC { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets option D for the question.
        /// </summary>
        [Required(ErrorMessage = "Option D is required")]
        public string OptionD { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the correct answer for the question.
        /// </summary>
        [Required(ErrorMessage = "Correct answer is required")]
        [RegularExpression("^[ABCD]$", ErrorMessage = "Correct answer must be A, B, C, or D")]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the category of the question.
        /// </summary>
        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the difficulty of the question.
        /// </summary>
        [Required(ErrorMessage = "Difficulty is required")]
        public string Difficulty { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the time limit for answering the question in seconds.
        /// </summary>
        [Range(10, 300, ErrorMessage = "Time limit must be between 10 and 300 seconds")]
        public int TimeLimitInSeconds { get; set; } = 30;
    }
}