using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using QuizGame1WPF.Models;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for TeacherDashboard.xaml
    /// </summary>
    public partial class TeacherDashboard : Window
    {
        private const string ConnStr = @"Server=localhost\SQLEXPRESS;Database=QuizGameDB;Trusted_Connection=True;Encrypt=False;";
        private ObservableCollection<QuestionModel> questions = new ObservableCollection<QuestionModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherDashboard"/> class.
        /// </summary>
        public TeacherDashboard()
        {
            InitializeComponent();
            LoadQuestions();
        }

        /// <summary>
        /// Loads questions from the database and populates the grid.
        /// </summary>
        private void LoadQuestions()
        {
            questions.Clear();
            using var con = new SqlConnection(ConnStr);
            con.Open();

            var cmd = new SqlCommand("SELECT * FROM Questions", con);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new QuestionModel
                {
                    ID = (int)reader["ID"],
                    Question = reader["Question"]?.ToString() ?? "",
                    OptionA = reader["OptionA"]?.ToString() ?? "",
                    OptionB = reader["OptionB"]?.ToString() ?? "",
                    OptionC = reader["OptionC"]?.ToString() ?? "",
                    OptionD = reader["OptionD"]?.ToString() ?? "",
                    CorrectAnswer = reader["CorrectAnswer"]?.ToString() ?? "",
                    Category = reader["Category"]?.ToString() ?? "",
                    Difficulty = reader["Difficulty"]?.ToString() ?? ""
                });
            }
            QuestionsGrid.ItemsSource = questions;
        }

        /// <summary>
        /// Handles the click event for adding a new question.
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEditQuestionWindow(); // No parameter = Add mode
            if (win.ShowDialog() == true)
                LoadQuestions();
        }

        /// <summary>
        /// Handles the click event for editing a selected question.
        /// </summary>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsGrid.SelectedItem is QuestionModel selected)
            {
                var win = new AddEditQuestionWindow(selected); // With parameter = Edit mode
                if (win.ShowDialog() == true)
                    LoadQuestions();
            }
            else
            {
                MessageBox.Show("Please select a question to edit.");
            }
        }

        /// <summary>
        /// Handles the click event for deleting a selected question.
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionsGrid.SelectedItem is QuestionModel selected)
            {
                var result = MessageBox.Show("Are you sure you want to delete this question?", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using var con = new SqlConnection(ConnStr);
                    con.Open();
                    var cmd = new SqlCommand("DELETE FROM Questions WHERE ID = @id", con);
                    cmd.Parameters.AddWithValue("@id", selected.ID);
                    cmd.ExecuteNonQuery();
                    LoadQuestions();
                }
            }
            else
            {
                MessageBox.Show("Please select a question to delete.");
            }
        }

        /// <summary>
        /// Handles the click event for viewing scores.
        /// </summary>
        private void ViewScores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View Scores feature is coming soon.");
        }

        /// <summary>
        /// Handles the selection changed event for the questions grid.
        /// </summary>
        private void QuestionsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
