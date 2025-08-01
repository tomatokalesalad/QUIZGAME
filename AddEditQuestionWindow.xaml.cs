using System;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using QuizGame1WPF.Models;

namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for AddEditQuestionWindow.xaml
    /// </summary>
    public partial class AddEditQuestionWindow : Window
    {
        private const string ConnStr = @"Server=localhost\SQLEXPRESS;Database=QuizGameDB;Trusted_Connection=True;Encrypt=False;";
        private readonly QuestionModel? editingQuestion;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditQuestionWindow"/> class.
        /// </summary>
        public AddEditQuestionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddEditQuestionWindow"/> class for editing an existing question.
        /// </summary>
        /// <param name="question">The question to edit.</param>
        public AddEditQuestionWindow(QuestionModel question) : this()
        {
            editingQuestion = question;

            // Pre-fill fields
            txtQuestion.Text = question.Question;
            txtOptionA.Text = question.OptionA;
            txtOptionB.Text = question.OptionB;
            txtOptionC.Text = question.OptionC;
            txtOptionD.Text = question.OptionD;
            cmbCorrect.Text = question.CorrectAnswer;
            txtCategory.Text = question.Category;
            cmbDifficulty.Text = question.Difficulty;
        }

        /// <summary>
        /// Handles the click event for saving a question.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string q = txtQuestion.Text.Trim();
            string a = txtOptionA.Text.Trim();
            string b = txtOptionB.Text.Trim();
            string c = txtOptionC.Text.Trim();
            string d = txtOptionD.Text.Trim();
            string? correct = (cmbCorrect.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string cat = txtCategory.Text.Trim();
            string? diff = (cmbDifficulty.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrEmpty(q) || string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b) ||
                string.IsNullOrEmpty(c) || string.IsNullOrEmpty(d) || string.IsNullOrEmpty(correct) ||
                string.IsNullOrEmpty(cat) || string.IsNullOrEmpty(diff))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            using var con = new SqlConnection(ConnStr);
            con.Open();

            SqlCommand cmd;
            if (editingQuestion == null)
            {
                // Insert new question
                cmd = new SqlCommand(@"INSERT INTO Questions (Question, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, Category, Difficulty)
                                       VALUES (@q, @a, @b, @c, @d, @correct, @cat, @diff)", con);
            }
            else
            {
                // Update existing
                cmd = new SqlCommand(@"UPDATE Questions SET 
                        Question=@q, OptionA=@a, OptionB=@b, OptionC=@c, OptionD=@d,
                        CorrectAnswer=@correct, Category=@cat, Difficulty=@diff
                        WHERE ID=@id", con);
                cmd.Parameters.AddWithValue("@id", editingQuestion.ID);
            }

            cmd.Parameters.AddWithValue("@q", q);
            cmd.Parameters.AddWithValue("@a", a);
            cmd.Parameters.AddWithValue("@b", b);
            cmd.Parameters.AddWithValue("@c", c);
            cmd.Parameters.AddWithValue("@d", d);
            cmd.Parameters.AddWithValue("@correct", correct);
            cmd.Parameters.AddWithValue("@cat", cat);
            cmd.Parameters.AddWithValue("@diff", diff);

            cmd.ExecuteNonQuery();
            DialogResult = true; // close window and return success
        }

        /// <summary>
        /// Handles the click event for cancelling the operation.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
