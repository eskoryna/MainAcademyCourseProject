using System.Windows;
using System.Windows.Controls;

namespace FieldsAndChips
{
    public partial class ChangeDimensions : Window
    {
        public int tryXCells;
        public int tryYCells;

        public ChangeDimensions()
        {
            InitializeComponent();

            inputXCells.Text = (Application.Current.MainWindow as MainWindow).xCells.ToString();
            inputYCells.Text = (Application.Current.MainWindow as MainWindow).yCells.ToString();
        }

        public void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            changeDimensionsWindow.Close();
        }

        public void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (tryXCells < 7 || tryXCells > 35)
            {
                MessageBox.Show("The number of horizontal cells has to be between 7 and 35.");
            }
            else if (tryYCells < 7 || tryYCells > 25)
            {
                MessageBox.Show("The number of vertical cells has to be between 7 and 25.");
            }
            else
            {
                (Application.Current.MainWindow as MainWindow).ConfigureFromMenu(tryXCells, tryYCells);
                changeDimensionsWindow.Close();
            }
        }

        public void inputXCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputXCells.Text, out tryXCells);
            if (!ok)
            {
                inputXCells.Text = "";
            }
        }

        public void inputYCells_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool ok = int.TryParse(inputYCells.Text, out tryYCells);
            if (!ok)
            {
                inputYCells.Text = "";
            }
        }
    }
}