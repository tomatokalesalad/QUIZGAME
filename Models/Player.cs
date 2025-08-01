namespace QuizGame1WPF.Models
{
    /// <summary>
    /// Represents a player in the Quiz Game application.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the unique identifier for the player.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the username of the player.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role of the player (e.g., Student, Teacher).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the student ID if the player is a student.
        /// </summary>
        public string? StudentID { get; set; }

        /// <summary>
        /// Gets or sets the instructor ID if the player is a teacher.
        /// </summary>
        public string? InstructorID { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the player was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date and time of the player's last login.
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player() { }
    }
}
