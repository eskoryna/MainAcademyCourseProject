using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FieldsAndChips
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Canvas testBoard = new Canvas();

        Random random = new Random();
        int gameState = 1;
        int xZ = 0;
        int yZ = 0;
        int chipZ = 0;
        int fieldZ = 0;

        public double xMinimalResolution = 320;
        public double yMinimalResolution = 200;

        public double xResolution = 799;
        public double yResolution = 580;

        public double sideMargin = 38;

        public int xCells;
        public int yCells;

        public int menuXCells;
        public int menuYCells;

        public double cellSize;

        public BitmapImage[] pictures = new BitmapImage[31];
        public BitmapImage menuPicture = new BitmapImage();

        public List<List<int>> fields = new List<List<int>>();
        public List<List<int>> chips = new List<List<int>>();
        public List<List<BoardCell>> boardCells = new List<List<BoardCell>>();
        BoardCell menuCell = new BoardCell();

        public bool isMenuOpened = false;

        public int leftClicked = 0;
        public int rightClicked = 0;
        public int[,] clicked = new int[2, 2];

        public List<List<int>> historyQueue = new List<List<int>>();
        public List<List<int>> historyStack = new List<List<int>>();

        public MainWindow()
        {
            InitializeComponent();

            SetIcon();

            LoadPictures();

            ConfigureFromFile();

            ConfigureBoard();

            
        }

        public void SetResolution()
        {
            //MessageBox.Show(SystemParameters.WorkArea.Width + " " + SystemParameters.WorkArea.Height);

            if (WindowState == WindowState.Maximized)
            {
                xResolution = SystemParameters.WorkArea.Width;
                yResolution = SystemParameters.WorkArea.Height;

                //MessageBox.Show(SystemParameters.WorkArea.Width + " " + SystemParameters.WorkArea.Height);
            }
            else
            {
                xResolution = ActualWidth;
                yResolution = ActualHeight;

                //MessageBox.Show(ActualWidth + " " + ActualHeight);
            }

            //MessageBox.Show(SystemParameters.WorkArea.Width + " " + SystemParameters.WorkArea.Height);

        }

        public void ChangeCellSize()
        {
            double xCellEstimateSize = Math.Floor((Math.Max(xResolution, xMinimalResolution) - sideMargin) / (xCells + 2));
            double yCellEstimateSize = Math.Floor((Math.Max(yResolution, yMinimalResolution) - sideMargin) / yCells);

            cellSize = Math.Min(xCellEstimateSize, yCellEstimateSize);

            //MessageBox.Show("cellSize = " + cellSize);

        }

        public void ConfigureBoard()
        {
            SetBoard();

            SetInitialPosition();

            SetButtons();
        }

        public void SetBoard()
        {
            ChangeCellSize();

            Board.Children.Clear();

            boardCells.Clear();

            for (int i = 0; i < xCells; i++)
            {
                boardCells.Add(new List<BoardCell>());

                for (int j = 0; j < yCells; j++)
                {
                    boardCells[i].Add(new BoardCell());

                    Board.Children.Add(boardCells[i][j]);

                    boardCells[i][j].MouseLeftButtonDown += new MouseButtonEventHandler(cell_LeftClick);
                    boardCells[i][j].MouseRightButtonDown += new MouseButtonEventHandler(cell_RightClick);

                    int[] coordinates = new int[2];
                    coordinates[0] = i;
                    coordinates[1] = j;
                    boardCells[i][j].Tag = coordinates;
                    //boardCells[i][j].Tag = i * 100 + j;

                    boardCells[i][j].Width = cellSize;
                    boardCells[i][j].Height = cellSize;
                    Canvas.SetLeft(boardCells[i][j], i * cellSize);
                    Canvas.SetTop(boardCells[i][j], j * cellSize);

                    
                }
            }

            //MessageBox.Show(boardCells[0].Count.ToString() + " " + boardCells.Count.ToString());
            
        }

        public void ResizeBoard()
        {
            ChangeCellSize();

            for (int i = 0; i < xCells; i++)
            {
                for (int j = 0; j < yCells; j++)
                {
                    boardCells[i][j].Width = cellSize;
                    boardCells[i][j].Height = cellSize;
                    Canvas.SetLeft(boardCells[i][j], i * cellSize);
                    Canvas.SetTop(boardCells[i][j], j * cellSize);
                }
            }

            menuCell.Height = cellSize;
            menuCell.Width = cellSize;
            Canvas.SetLeft(menuCell, (xCells + 1) * cellSize);
            Canvas.SetTop(menuCell, 0);
        }

        public void ConfigureFromFile()
        {
            try
            {
                List<string> parameters = File.ReadAllLines(Directory.GetCurrentDirectory() + "/fac.cfg").ToList();
                xMinimalResolution = double.Parse(parameters[0]);
                yMinimalResolution = double.Parse(parameters[1]);
                xCells = int.Parse(parameters[2]);
                yCells = int.Parse(parameters[3]);
                sideMargin = double.Parse(parameters[4]);

                if (xMinimalResolution < 320 || yMinimalResolution < 200 || xCells < 7 || xCells > 20 || 
                    yCells < 7 || yCells > 25 || sideMargin < 0 || sideMargin > 100)
                //if (xMinimalResolution < 320 || yMinimalResolution < 200 || xCells < 5 || xCells > 20 || yCells < 5
                //      || yCells > 25)
                {
                    SetDefaultConfiguration();
                }
               
               // MessageBox.Show(xMinimalResolution + " " + yMinimalResolution + " " + xCells + " " + yCells + " " + cellsMargin);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to set configuration. The configuration was set to default.");
                SetDefaultConfiguration();
            }
        }

        public void ConfigureFromMenu(int tryXCells, int tryYCells)
        {
            xCells = tryXCells;
            yCells = tryYCells;
            //MessageBox.Show(xCells.ToString());

            ConfigureBoard();

        }

        public void SetDefaultConfiguration()
        {
            xMinimalResolution = 320;
            yMinimalResolution = 200;
            xCells = 11;
            yCells = 14;
            sideMargin = 38;
        }

        public void SetIcon()
        {
            try
            {
                FieldsAndChipsMainWindow.Icon = BitmapFrame.Create(new Uri(Directory.GetCurrentDirectory() + "/fac.ico"));
            }
            catch (Exception)
            {
            }
        }

        public void SetButtons()
        {
            
            Board.Children.Add(menuCell);
            menuCell.Height = cellSize;
            menuCell.Width = cellSize;
            Canvas.SetLeft(menuCell, (xCells + 1) * cellSize);
            Canvas.SetTop(menuCell, 0);
            menuCell.MovePicture.Visibility = Visibility.Visible;
            menuCell.MovePicture.Source = menuPicture;

            menuCell.MouseLeftButtonDown += new MouseButtonEventHandler(menu_LeftClick);
        }

        public void ClearBoard()
        {
            fields.Clear();
            chips.Clear();
        }

        public void SetInitialPosition()
        {
            ClearBoard();

            for (int i = 0; i < xCells; i++)
            {
                fields.Add(new List<int>());
                chips.Add(new List<int>());

                for (int j = 0; j < yCells; j++)
                {
                    fields[i].Add(8);
                    chips[i].Add(0);

                    ChangeBoardCell(i, j);
                }
            }

            //MessageBox.Show(fields[xCells - 1][yCells - 1].ToString());
        }

        public void ChangeBoardCell(int x, int y) 
        {
            int field = fields[x][y];
            int chip = chips[x][y];

            boardCells[x][y].FieldPicture.Source = pictures[field];
            if (chip == 0)
            {
                boardCells[x][y].ChipPicture.Visibility = Visibility.Hidden;
            }
            else
            {
                boardCells[x][y].ChipPicture.Visibility = Visibility.Visible;

                if (chip >=1 && chip <= 10)
                {
                    chip += 10;
                }
                if (chip >=-10 && chip <= -1)
                {
                    chip = -chip;
                    chip += 20;
                }

                boardCells[x][y].ChipPicture.Source = pictures[chip];
            }
        }

        public void LoadPictures()
        {
            try
            {
                menuPicture = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/M.png"));

                for (int i = 0; i < 31; i++)
                {
                    pictures[i] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/" + i + ".png"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load images. " + ex.StackTrace);
            }
        }

        public void SetRandomStartingPosition()
        {
            SetInitialPosition();

            int i = 0;
            int j = 0;

            for (int k = -10; k <= 10; k++)
            {
                if (k == 0)
                {
                    continue;
                }

                i = random.Next(0, xCells);
                j = random.Next(0, yCells);

                if (chips[i][j] == 0)
                {
                    chips[i][j] = k;
                    ChangeBoardCell(i, j);
                }
                else
                {
                    k--;
                }
            }
        }

        public int GetCellCoordinates(object sender, bool isX)
        {
            int[] coordinates = (int[])(sender as BoardCell).Tag;
            //MessageBox.Show(coordinates[0] + " " + coordinates[1]);
            if (isX)
            {
                return coordinates[0];
            }
            else
            {
                return coordinates[1];
            }
        }

        public void LeftSingleClick(int x, int y)
        {
            switch (gameState)
            {
                case 1:
                    if (chips[x][y] > 0)
                    {
                        ReserveCurrent(x, y);

                        gameState = 2;
                    }
                    break;
                case -1:
                    if (chips[x][y] < 0)
                    {
                        ReserveCurrent(x, y);

                        gameState = -2;
                    }
                    break;
                case 2:
                case -2:
                    FinalizeMove(x, y, false);

                    break;
            }
            
            //MessageBox.Show("Left " + x + " " + y);
        }

        public void RightSingleClick(int x, int y)
        {

        }

        public void LeftDoubleClick(int x, int y)
        {

        }

        public void RightDoubleClick(int x, int y)
        {

        }

        public void ReserveCurrent(int x, int y)
        {
            xZ = x;
            yZ = y;
            fieldZ = fields[x][y];
            chipZ = chips[x][y];
        }

        public void FinalizeMove(int x, int y, bool promote)
        {

        }

        public void ClearHistory()
        {
            historyQueue.Clear();
            historyStack.Clear();
        }

        public void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField, int endX, int endY, int endInitialChip, int endInitialField,
            int endFinalChip, int endFinalField)
        {

        }

        public void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField)
        {

        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetResolution();

            ResizeBoard();
        }

        public void menu_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (isMenuOpened == false)
            {
                isMenuOpened = true;
                new Menu().Show();
            }
        }

        private async void cell_LeftClick(object sender, MouseButtonEventArgs e)
        {

            leftClicked++;
            clicked[0, 1] = clicked[0, 0];
            clicked[0, 0] = sender.GetHashCode();

            if (leftClicked > 1 && clicked[0, 0] == clicked[0, 1])
            {
                LeftDoubleClick(GetCellCoordinates(sender, true), GetCellCoordinates(sender, false));
                //MessageBox.Show("Left Double " + GetCellCoordinates(sender, true) + " " + GetCellCoordinates(sender, false));
            }

            await Task.Delay(400);

            if (leftClicked == 1)
            {
                LeftSingleClick(GetCellCoordinates(sender, true), GetCellCoordinates(sender, false));
                //MessageBox.Show("Left " + GetCellCoordinates(sender, true) + " " + GetCellCoordinates(sender, false));
            }

            leftClicked = 0;
        }

        private async void cell_RightClick(object sender, MouseEventArgs e)
        {
            rightClicked++;
            clicked[1, 1] = clicked[1, 0];
            clicked[1, 0] = sender.GetHashCode();

            if (rightClicked > 1 && clicked[1, 0] == clicked[1, 1])
            {
                RightDoubleClick(GetCellCoordinates(sender, true), GetCellCoordinates(sender, false));
                //MessageBox.Show("Roght Double " + GetCellCoordinates(sender, true) + " " + GetCellCoordinates(sender, false));
            }

            await Task.Delay(400);

            if (rightClicked == 1)
            {
                RightSingleClick(GetCellCoordinates(sender, true), GetCellCoordinates(sender, false));
                //MessageBox.Show("Right " + GetCellCoordinates(sender, true) + " " + GetCellCoordinates(sender, false));
            }

            rightClicked = 0;
        }

    }
}
