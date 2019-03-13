using System.Data.Entity;
using System.Windows;

namespace FieldsAndChips
{
    public partial class GamesDatabaseWindow : Window
    {
        ApplicationContext db;
        public GamesDatabaseWindow()
        {
            InitializeComponent();

            db = new ApplicationContext();
            db.SavedGames.Load();
            //DataContext = db.SavedGames.Local.ToBindingList();
            savedGamesGrid.ItemsSource = db.SavedGames.Local.ToBindingList();

            Closing += gameDatabaseWindow_Closing;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            SavedGame savedGame = savedGamesGrid.SelectedItem as SavedGame;
            if (savedGame != null)
            {
                db.SavedGames.Remove(savedGame);
            }
            db.SaveChanges();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            SavedGame gameToLoad = (SavedGame)savedGamesGrid.SelectedItem;

            if (gameToLoad != null)
            {
                //MessageBox.Show(gameToLoad.GameDate + " " + gameToLoad.GameName + " " + gameToLoad.StartingPosition +
                //" " + gameToLoad.Moves);
                (Application.Current.MainWindow as MainWindow).LoadGame(gameToLoad);
                gamesDatabaseWindow.Close();
            }

            //MessageBox.Show(gameToLoad.Moves);

        }

        private void gameDatabaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
    }
}
