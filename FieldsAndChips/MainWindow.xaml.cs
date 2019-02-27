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
        int xA = 0;
        int yA = 0;
        int chipA = 0;
        int fieldA = 0;

        int chipB = 0;
        int fieldB = 0;

        public double xMinimalResolution = 320;
        public double yMinimalResolution = 200;

        public double xResolution = 799;
        public double yResolution = 580;

        public double sideMargin = 38;
        public double sideAreaPercentage = 10;
        public double sideArea = 0;

        public int xCells;
        public int yCells;

        //public int menuXCells;
        //public int menuYCells;

        public double cellSize;

        public BitmapImage[] pictures = new BitmapImage[31];
        public BitmapImage[] menuPictures = new BitmapImage[5];

        public List<List<int>> fields = new List<List<int>>();
        public List<List<int>> chips = new List<List<int>>();
        public List<List<BoardCell>> boardCells = new List<List<BoardCell>>();
        public List<BoardCell> menuCells = new List<BoardCell>();

        public bool isMenuOpened = false;

        public int leftClicked = 0;
        public int rightClicked = 0;
        public List<object> leftClicks = new List<object>();
        public List<object> rightClicks = new List<object>();

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
            sideArea = Math.Floor(Math.Max(yResolution, yMinimalResolution) * sideAreaPercentage / 100);

            double xCellEstimateSize = Math.Floor((Math.Max(xResolution, xMinimalResolution) - sideMargin - sideArea) /
                (xCells + 2));
            //double xCellEstimateSize = Math.Floor((Math.Max(xResolution, xMinimalResolution) - sideArea) / (xCells + 2));
            double yCellEstimateSize = Math.Floor((Math.Max(yResolution, yMinimalResolution) - sideMargin) / yCells);

            cellSize = Math.Min(xCellEstimateSize, yCellEstimateSize);

            //MessageBox.Show("cellSize = " + cellSize + " sideArea = " + sideArea);

        }

        public void ConfigureBoard()
        {
            SetBoard();

            SetButtons();

            SetInitialPosition();
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

            ResizeButtons();
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
            menuCells.Clear();

            for (int i = 0; i < 4; i++)
            {
                menuCells.Add(new BoardCell());
                Board.Children.Add(menuCells[i]);
                menuCells[i].FieldPicture.Visibility = Visibility.Visible;
                menuCells[i].FieldPicture.Source = menuPictures[i];
            }

            menuCells[3].ChipPicture.Visibility = Visibility.Hidden;
            menuCells[3].ChipPicture.Source = menuPictures[4];

            menuCells[0].MouseLeftButtonDown += new MouseButtonEventHandler(menu_LeftClick);
            menuCells[1].MouseLeftButtonDown += new MouseButtonEventHandler(stepBackward_LeftClick);
            menuCells[2].MouseLeftButtonDown += new MouseButtonEventHandler(stepForward_LeftClick);

            ResizeButtons();
        }

        public void ResizeButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                menuCells[i].Height = sideArea;
                menuCells[i].Width = sideArea;
                Canvas.SetLeft(menuCells[i], (xCells + 2) * cellSize);
                Canvas.SetTop(menuCells[i], i * sideArea);
            }
        }

        public void ClearBoard()
        {
            fields.Clear();
            chips.Clear();
            ClearHistory();
            gameState = 1;
            leftClicks.Clear();
            rightClicks.Clear();
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

            ShowGameState();

            //MessageBox.Show("OK!");
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
                menuPictures[0] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/M.png"));
                menuPictures[1] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/B.png"));
                menuPictures[2] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/F.png"));
                menuPictures[3] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/Wh.png"));
                menuPictures[4] = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/images/Bl.png"));

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
                        ShowGameState();
                    }
                    break;
                case -1:
                    if (chips[x][y] < 0)
                    {
                        ReserveCurrent(x, y);

                        gameState = -2;
                        ShowGameState();
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
            xA = x;
            yA = y;
            fieldA = fields[x][y];
            chipA = chips[x][y];
        }

        public void FinalizeMove(int x, int y, bool promote)
        {
            fieldB = fields[x][y];
            chipB = chips[x][y];

            switch (gameState)
            {
                case 2:
                    if (chips[x][y] == 0)
                    {
                        if (IsMovePossible(x, y))
                        {
                            chips[xA][yA] = 0;
                            ChangeBoardCell(xA, yA);
                            chips[x][y] = chipA;

                            if (promote == true)
                            {
                                switch (chips[x][y])
                                {
                                    case 9:
                                        if (fields[x][y] == 8)
                                        {
                                            chips[x][y] = 10;
                                        }
                                        break;
                                    case 8:
                                        if (fields[x][y] >= 1 && fields[x][y] <= 7)
                                        {
                                            chips[x][y] = 9;
                                        }
                                        break;
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                        if (chips[x][y] == fields[x][y])
                                        {
                                            chips[x][y] = ++chips[x][y];
                                        }
                                        break;
                                }
                            }

                            ChangeBoardCell(x, y);

                            AddToHistory(gameState, xA, yA, chipA, fieldA, chips[xA][yA], fields[xA][yA],
                                x, y, chipB, fieldB, chips[x][y], fields[x][y]);

                            gameState = -1;
                            ShowGameState();
                        }
                    }

                    break;
                case -2:
                    if (chips[x][y] == 0)
                    {
                        if (IsMovePossible(x, y))
                        {
                            chips[xA][yA] = 0;
                            ChangeBoardCell(xA, yA);
                            chips[x][y] = chipA;

                            if (promote == true)
                            {
                                switch (chips[x][y])
                                {
                                    case -9:
                                        if (fields[x][y] == 8)
                                        {
                                            chips[x][y] = -10;
                                        }
                                        break;
                                    case -8:
                                        if (fields[x][y] >= 1 && fields[x][y] <= 7)
                                        {
                                            chips[x][y] = -9;
                                        }
                                        break;
                                    case -1:
                                    case -2:
                                    case -3:
                                    case -4:
                                    case -5:
                                    case -6:
                                    case -7:
                                        if (chips[x][y] == -fields[x][y])
                                        {
                                            chips[x][y] = --chips[x][y];
                                        }
                                        break;
                                }
                            }

                            ChangeBoardCell(x, y);

                            AddToHistory(gameState, xA, yA, chipA, fieldA, chips[xA][yA], fields[xA][yA],
                                x, y, chipB, fieldB, chips[x][y], fields[x][y]);

                            gameState = 1;
                            ShowGameState();
                        }
                    }

                    break;
            }
        }

        public bool IsMovePossible(int x, int y)
        {
            if (Math.Abs(x - xA) == 1 && Math.Abs(y - yA) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearHistory()
        {
            historyQueue.Clear();
            historyStack.Clear();
        }

        public void AddToHistory(int state, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField, int endX, int endY, int endInitialChip, int endInitialField,
            int endFinalChip, int endFinalField)
        {
            AddToHistory(state, startX, startY, startInitialChip, startInitialField, startFinalChip, startFinalField);

            historyQueue[historyQueue.Count - 1].Add(endX);
            historyQueue[historyQueue.Count - 1].Add(endY);
            historyQueue[historyQueue.Count - 1].Add(endInitialChip);
            historyQueue[historyQueue.Count - 1].Add(endInitialField);
            historyQueue[historyQueue.Count - 1].Add(endFinalChip);
            historyQueue[historyQueue.Count - 1].Add(endFinalField);
        }

        public void AddToHistory(int state, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField)
        {
            switch (state)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    state = 1;
                    break;
                case -1:
                case -2:
                case -3:
                case -4:
                case -5:
                    state = -1;
                    break;
            }

            historyStack.Clear();
            historyQueue.Add(new List<int>());

            historyQueue[historyQueue.Count - 1].Add(state);
            historyQueue[historyQueue.Count - 1].Add(startX);
            historyQueue[historyQueue.Count - 1].Add(startY);
            historyQueue[historyQueue.Count - 1].Add(startInitialChip);
            historyQueue[historyQueue.Count - 1].Add(startInitialField);
            historyQueue[historyQueue.Count - 1].Add(startFinalChip);
            historyQueue[historyQueue.Count - 1].Add(startFinalField);
        }

        public void StepBack()
        {
            if (historyQueue.Count > 0)
            {
                int x;
                int y;
                int chip;
                int field;

                gameState = historyQueue[historyQueue.Count - 1][0];
                ShowGameState();

                x = historyQueue[historyQueue.Count - 1][1];
                y = historyQueue[historyQueue.Count - 1][2];
                chip = historyQueue[historyQueue.Count - 1][3];
                field = historyQueue[historyQueue.Count - 1][4];

                chips[x][y] = chip;
                fields[x][y] = field;
                ChangeBoardCell(x, y);

                historyStack.Add(new List<int>());

                for (int i = 0; i < 7; i++)
                {
                    historyStack[historyStack.Count - 1].Add(historyQueue[historyQueue.Count - 1][i]);
                }

                if (historyQueue[historyQueue.Count - 1].Count == 13)
                {
                    x = historyQueue[historyQueue.Count - 1][7];
                    y = historyQueue[historyQueue.Count - 1][8];
                    chip = historyQueue[historyQueue.Count - 1][9];
                    field = historyQueue[historyQueue.Count - 1][10];

                    chips[x][y] = chip;
                    fields[x][y] = field;
                    ChangeBoardCell(x, y);

                    for (int i = 7; i < 13; i++)
                    {
                        historyStack[historyStack.Count - 1].Add(historyQueue[historyQueue.Count - 1][i]);
                    }
                }

                historyQueue.RemoveAt(historyQueue.Count - 1);
            }
            else
            {
                MessageBox.Show("There are no more moves back in history.");
            }
        }

        public void StepForward()
        {
            if (historyStack.Count > 0)
            {
                int x;
                int y;
                int chip;
                int field;

                gameState = -historyStack[historyStack.Count - 1][0];
                ShowGameState();

                x = historyStack[historyStack.Count - 1][1];
                y = historyStack[historyStack.Count - 1][2];
                chip = historyStack[historyStack.Count - 1][5];
                field = historyStack[historyStack.Count - 1][6];

                chips[x][y] = chip;
                fields[x][y] = field;
                ChangeBoardCell(x, y);

                historyQueue.Add(new List<int>());

                for (int i = 0; i < 7; i++)
                {
                    historyQueue[historyQueue.Count - 1].Add(historyStack[historyStack.Count - 1][i]);
                }

                if (historyStack[historyStack.Count - 1].Count == 13)
                {
                    x = historyStack[historyStack.Count - 1][7];
                    y = historyStack[historyStack.Count - 1][8];
                    chip = historyStack[historyStack.Count - 1][11];
                    field = historyStack[historyStack.Count - 1][12];

                    chips[x][y] = chip;
                    fields[x][y] = field;
                    ChangeBoardCell(x, y);

                    for (int i = 7; i < 13; i++)
                    {
                        historyQueue[historyQueue.Count - 1].Add(historyStack[historyStack.Count - 1][i]);
                    }
                }

                historyStack.RemoveAt(historyStack.Count - 1);
            }
            else
            {
                MessageBox.Show("There are no moves forward in history.");
            }
        }

        public void ShowGameState()
        {
            if (gameState > 0)
            {
                menuCells[3].FieldPicture.Visibility = Visibility.Visible;
                menuCells[3].ChipPicture.Visibility = Visibility.Hidden;
            }
            if (gameState < 0)
            {
                menuCells[3].FieldPicture.Visibility = Visibility.Hidden;
                menuCells[3].ChipPicture.Visibility = Visibility.Visible;
            }
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

        public void stepBackward_LeftClick(object sender, MouseButtonEventArgs e)
        {
            StepBack();
        }

        public void stepForward_LeftClick(object sender, MouseButtonEventArgs e)
        {
            StepForward();
        }

        private async void cell_LeftClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                leftClicked++;
                leftClicks.Add(sender);

                //if (rightClicked == 0 && leftClicks.Count > 1)
                if (leftClicks.Count > 1)
                {
                    if (leftClicks[0] == leftClicks[1])
                    {
                        LeftDoubleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
                        //MessageBox.Show("! Left Double " + GetCellCoordinates(leftClicks[0], true) + " " + GetCellCoordinates(leftClicks[0], false));
                        leftClicks.Clear();
                        rightClicks.Clear();
                    }
                    else
                    {
                        LeftSingleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
                        LeftSingleClick(GetCellCoordinates(leftClicks[1], true), GetCellCoordinates(leftClicks[1], false));
                        //MessageBox.Show("@ Left " + GetCellCoordinates(leftClicks[0], true) + " " + GetCellCoordinates(leftClicks[0], false));
                        //MessageBox.Show("# Left " + GetCellCoordinates(leftClicks[1], true) + " " + GetCellCoordinates(leftClicks[1], false));
                        leftClicks.Clear();
                        rightClicks.Clear();
                    }
                }

                await Task.Delay(300);

                //if (rightClicked == 0 && leftClicks.Count == 1)
                if (leftClicks.Count == 1)
                {
                    LeftSingleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
                    //MessageBox.Show("$ Left " + GetCellCoordinates(leftClicks[0], true) + " " + GetCellCoordinates(leftClicks[0], false));
                    leftClicks.Clear();
                    rightClicks.Clear();
                }

                leftClicked = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }

        private async void cell_RightClick(object sender, MouseEventArgs e)
        {
            try
            {
                rightClicked++;
                rightClicks.Add(sender);

                //if (leftClicked == 0 && rightClicks.Count > 1)
                if (rightClicks.Count > 1)
                {
                    if (rightClicks[0] == rightClicks[1])
                    {
                        LeftDoubleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
                        //MessageBox.Show("! Right Double " + GetCellCoordinates(rightClicks[0], true) + " " + GetCellCoordinates(rightClicks[0], false));
                        rightClicks.Clear();
                        leftClicks.Clear();
                    }
                    else
                    {
                        LeftSingleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
                        LeftSingleClick(GetCellCoordinates(rightClicks[1], true), GetCellCoordinates(rightClicks[1], false));
                        //MessageBox.Show("@ Right " + GetCellCoordinates(rightClicks[0], true) + " " + GetCellCoordinates(rightClicks[0], false));
                        //MessageBox.Show("# Right " + GetCellCoordinates(rightClicks[1], true) + " " + GetCellCoordinates(rightClicks[1], false));
                        rightClicks.Clear();
                        leftClicks.Clear();
                    }
                }

                await Task.Delay(300);

                //if (leftClicked == 0 && rightClicks.Count == 1)
                if (rightClicks.Count == 1)
                {
                    LeftSingleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
                    //MessageBox.Show("$ Right " + GetCellCoordinates(rightClicks[0], true) + " " + GetCellCoordinates(rightClicks[0], false));
                    rightClicks.Clear();
                    leftClicks.Clear();
                }

                rightClicked = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
