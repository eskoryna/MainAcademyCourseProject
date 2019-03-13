using System.Windows;

namespace FieldsAndChips
{
    public partial class SaveGameWindow : Window
    {
        public SavedGame SavedGame { get; private set; }

        public SaveGameWindow(SavedGame s)
        {
            InitializeComponent();
            SavedGame = s;
            DataContext = SavedGame;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
