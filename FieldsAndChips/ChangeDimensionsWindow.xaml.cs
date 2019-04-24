using System.Windows;
using System.Windows.Controls;

namespace FieldsAndChips
{
    public partial class ChangeDimensions : Window
    {
        public int xCells;
        public int yCells;

        public ChangeDimensions(IGameLogic gameLogic)
        {
            InitializeComponent();
            inputXCells.Text = gameLogic.XCells.ToString();
            inputYCells.Text = gameLogic.YCells.ToString();
        }

        public void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            changeDimensionsWindow.Close();
        }

        public void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (xCells < 7 || xCells > 35)
            {
                MessageBox.Show("The number of horizontal cells has to be between 7 and 35.");
            }
            else if (yCells < 7 || yCells > 25)
            {
                MessageBox.Show("The number of vertical cells has to be between 7 and 25.");
            }
            else
            {
                (Application.Current.MainWindow as MainWindow).ConfigureFromMenu(xCells, yCells);
                changeDimensionsWindow.Close();
            }
        }

        public void inputXCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputXCells.Text, out xCells);
            if (!ok)
            {
                inputXCells.Text = "";
            }
        }

        public void inputYCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputYCells.Text, out yCells);
            if (!ok)
            {
                inputYCells.Text = "";
            }
        }
    }
}