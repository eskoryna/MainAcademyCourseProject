using System;
using System.Windows;
using System.Windows.Controls;

namespace FieldsAndChips
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public int tryXCells;
        public int tryYCells;

        public Menu()
        {
            InitializeComponent();

            inputXCells.Text = (Application.Current.MainWindow as MainWindow).xCells.ToString();
            inputYCells.Text = (Application.Current.MainWindow as MainWindow).yCells.ToString();
        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).SetRandomStartingPosition();
            MenuWindow.Close();
            (Application.Current.MainWindow as MainWindow).isMenuOpened = false;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MenuWindow.Close();
            (Application.Current.MainWindow as MainWindow).isMenuOpened = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (tryXCells < 7 || tryXCells > 20)
            {
                MessageBox.Show("The number of horizontal cells has to be between 7 and 20.");
            }
            else if(tryYCells < 7 || tryYCells > 25)
            {
                MessageBox.Show("The number of vertical cells has to be between 7 and 25.");
            }
            else
            {
                (Application.Current.MainWindow as MainWindow).ConfigureFromMenu(tryXCells, tryYCells);
                
                MenuWindow.Close();
                (Application.Current.MainWindow as MainWindow).isMenuOpened = false;
            }
        }

        private void inputXCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputXCells.Text, out tryXCells);
            if (ok)
            {
            }
            else
            {
                inputXCells.Text = "";
            }
        }

        private void inputYCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputYCells.Text, out tryYCells);
            if (ok)
            {
            }
            else
            {
                inputYCells.Text = "";
            }
        }
    }
}
