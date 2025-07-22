using System.Windows;

namespace QuizGame1WPF
{
    public partial class GameModeSelectorWindow : Window
    {
        public string SelectedGameMode { get; private set; }

        public GameModeSelectorWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGameModes.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedGameMode = selectedItem.Content.ToString();
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a game mode.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
