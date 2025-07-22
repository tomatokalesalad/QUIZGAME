using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame1WPF
{
    public partial class AddEditQuestionWindow : Window
    {
        private const string ConnStr = @"Server=localhost;Database=QuizGameDB;Trusted_Connection=True;";
        private readonly QuestionModel editingQuestion;

        public AddEditQuestionWindow()
        {
            InitializeComponent();
        }

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string q = txtQuestion.Text.Trim();
            string a = txtOptionA.Text.Trim();
            string b = txtOptionB.Text.Trim();
            string c = txtOptionC.Text.Trim();
            string d = txtOptionD.Text.Trim();
            string correct = (cmbCorrect.SelectedItem as ComboBoxItem)?.Content.ToString();
            string cat = txtCategory.Text.Trim();
            string diff = (cmbDifficulty.SelectedItem as ComboBoxItem)?.Content.ToString();

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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
