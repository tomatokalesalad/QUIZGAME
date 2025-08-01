using System.Windows;
using System.Windows.Controls;
namespace QuizGame1WPF
{
    /// <summary>
    /// Interaction logic for GameModeSelectorWindow.
    /// </summary>
    public partial class GameModeSelectorWindow : Window
    {
        /// <summary>
        /// Gets the selected game mode.
        /// </summary>
        public string SelectedGameMode { get; private set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModeSelectorWindow"/> class.
        /// </summary>
        public GameModeSelectorWindow()
        {
            InitializeComponent();
            SelectedGameMode = string.Empty; // Ensure non-null value
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGameModes.SelectedItem is ComboBoxItem selectedItem)
            {
                var content = selectedItem.Content as string;
                SelectedGameMode = content ?? string.Empty;
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
