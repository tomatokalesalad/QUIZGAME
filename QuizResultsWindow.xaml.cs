using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace QuizGame1WPF
{
    public partial class QuizResultsWindow : Window
    {
        public QuizResultsWindow(List<QuizResultModel> results)
        {
            InitializeComponent();
            lvResults.ItemsSource = results;
        }

        public QuizResultsWindow()
            : this(new List<QuizResultModel> {
                new QuizResultModel { Question = "Sample Q1", UserAnswer = "A", CorrectAnswer = "B" },
                new QuizResultModel { Question = "Sample Q2", UserAnswer = "C", CorrectAnswer = "C" }
            })
        {
            // For designer support
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void lvResults_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }

    public class QuizResultModel
    {
        public string Question { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect => UserAnswer == CorrectAnswer;
        public string ResultText => IsCorrect ? "Correct" : "Wrong";
    }
}