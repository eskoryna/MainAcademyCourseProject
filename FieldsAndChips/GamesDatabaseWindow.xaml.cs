using System.Data.Entity;
using System.Windows;

namespace FieldsAndChips
{
    public partial class GamesDatabaseWindow : Window
    {
        ApplicationContext db;
        IGameLogic gameLogic;

        public GamesDatabaseWindow(IGameLogic gameLogic)
        {
            InitializeComponent();
            this.gameLogic = gameLogic;
            db = new ApplicationContext();
            db.SavedGames.Load();
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
                gameLogic.LoadGame(gameToLoad);
                gamesDatabaseWindow.Close();
            }
        }

        private void gameDatabaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
    }
}
