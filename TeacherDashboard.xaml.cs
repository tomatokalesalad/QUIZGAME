using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace QuizGame1WPF
{
    public partial class TeacherDashboard : Window
    {
        private const string ConnStr = @"Server=localhost;Database=QuizGameDB;Trusted_Connection=True;";
        private ObservableCollection<QuestionModel> questions = new ObservableCollection<QuestionModel>();

        public TeacherDashboard()
        {
            InitializeComponent();
            LoadQuestions();
        }

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
                    Question = reader["Question"].ToString(),
                    OptionA = reader["OptionA"].ToString(),
                    OptionB = reader["OptionB"].ToString(),
                    OptionC = reader["OptionC"].ToString(),
                    OptionD = reader["OptionD"].ToString(),
                    CorrectAnswer = reader["CorrectAnswer"].ToString(),
                    Category = reader["Category"].ToString(),
                    Difficulty = reader["Difficulty"].ToString()
                });
            }
            QuestionsGrid.ItemsSource = questions;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddEditQuestionWindow(); // No parameter = Add mode
            if (win.ShowDialog() == true)
                LoadQuestions();
        }

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

        private void ViewScores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View Scores feature is coming soon.");
        }
    }
}
