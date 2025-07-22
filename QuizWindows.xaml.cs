using System.Windows;
using System.Windows.Controls;
using QuizGame1WPF.Models;

namespace QuizGame1WPF
{
    public partial class QuizWindow : Window
    {
        private TimerHelper _timer;
        private int _timePerQuestion = 30; // seconds
        private QuestionModel _question;

        public QuizWindow(QuestionModel question)
        {
            InitializeComponent();
            _question = question;
            LoadQuestion();
            StartTimer();
        }

        private void LoadQuestion()
        {
            txtQuestion.Text = _question.Question;
            optA.Content = _question.OptionA;
            optB.Content = _question.OptionB;
            optC.Content = _question.OptionC;
            optD.Content = _question.OptionD;
        }

        private void StartTimer()
        {
            _timer = new TimerHelper();
            _timer.OnTick += (seconds) => txtTimer.Text = seconds + "s";
            _timer.OnTimeUp += () =>
            {
                MessageBox.Show("Time's up!");
                MoveToNextQuestion(); // optional
            };
            _timer.Start(_timePerQuestion);
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            string selected = GetSelectedOption();
            if (selected == _question.CorrectAnswer)
                MessageBox.Show("Correct!");
            else
                MessageBox.Show($"Wrong! Correct answer is: {_question.CorrectAnswer}");

            MoveToNextQuestion();
        }

        private string GetSelectedOption()
        {
            if (optA.IsChecked == true) return "A";
            if (optB.IsChecked == true) return "B";
            if (optC.IsChecked == true) return "C";
            if (optD.IsChecked == true) return "D";
            return "";
        }

        private void MoveToNextQuestion()
        {
            // Logic to load the next question or end quiz
            Close();
        }
    }
}
