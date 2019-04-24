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
    public partial class MainWindow : Window, IGameFront
    {
        IGameLogic gameLogic;
        public double cellSize;

        public BitmapImage[] pictures = new BitmapImage[31];

        public List<List<int>> fields = new List<List<int>>();
        public List<List<int>> chips = new List<List<int>>();
        public List<List<BoardCell>> boardCells = new List<List<BoardCell>>();

        public int leftClicked = 0;
        public int rightClicked = 0;
        public List<object> leftClicks = new List<object>();
        public List<object> rightClicks = new List<object>();

        ApplicationContext db;

        public MainWindow()
        {
            InitializeComponent();

            db = new ApplicationContext();
            gameLogic = new GameLogic(this);

            LoadPictures();
            SetColors();
            ConfigureFromFile();
            SetBoard();
            SetInitialPosition();
        }

        public void ChangeCellSize()
        {
            double xCellEstimateSize = Math.Floor(board.ActualWidth / (gameLogic.XCells));
            double yCellEstimateSize = Math.Floor(board.ActualHeight / gameLogic.YCells);
            cellSize = Math.Min(xCellEstimateSize, yCellEstimateSize);
        }

        public void SetBoard()
        {
            ChangeCellSize();
            board.Children.Clear();
            boardCells.Clear();

            for (int i = 0; i < gameLogic.XCells; i++)
            {
                boardCells.Add(new List<BoardCell>());
                for (int j = 0; j < gameLogic.YCells; j++)
                {
                    boardCells[i].Add(new BoardCell());
                    board.Children.Add(boardCells[i][j]);
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
        }

        public void SetColors()
        {
            buttonRed.Tag = 1;
            buttonOrange.Tag = 2;
            buttonYellow.Tag = 3;
            buttonGreen.Tag = 4;
            buttonAzure.Tag = 5;
            buttonBlue.Tag = 6;
            buttonViolet.Tag = 7;
        }

        public void ResizeBoard()
        {
            ChangeCellSize();
            for (int i = 0; i < gameLogic.XCells; i++)
            {
                for (int j = 0; j < gameLogic.YCells; j++)
                {
                    boardCells[i][j].Height = cellSize;
                    boardCells[i][j].Width = cellSize;
                    Canvas.SetLeft(boardCells[i][j], i * cellSize);
                    Canvas.SetTop(boardCells[i][j], j * cellSize);
                }
            }
        }

        public void ConfigureFromFile()
        {
            int xCells;
            int yCells;

            try
            {
                List<string> parameters = File.ReadAllLines(Directory.GetCurrentDirectory() + "/fac.cfg").ToList();
                xCells = int.Parse(parameters[0]);
                yCells = int.Parse(parameters[1]);

                if (xCells < 7 || xCells > 35 || yCells < 7 || yCells > 25)
                {
                    SetDefaultConfiguration();
                }
                else
                {
                    gameLogic.SetGameLogic(xCells, yCells, 1);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to set configuration. The configuration was set to default.");
                SetDefaultConfiguration();
            }
        }

        public void ConfigureFromMenu(int xCells, int yCells)
        {
            gameLogic.SetGameLogic(xCells, yCells, 1);
            SetBoard();
            SetInitialPosition();
        }

        public void SetDefaultConfiguration()
        {
            gameLogic.SetGameLogic(14, 11, 1);
        }

        public void ClearBoard()
        {
            gameLogic.ClearBoard();

            leftClicks.Clear();
            rightClicks.Clear();
        }

        public void SetInitialPosition()
        {
            ClearBoard();
            gameLogic.SetInitialPosition();

            ShowGameState();
        }

        public void ChangeBoardCell(int x, int y) 
        {
            int field = gameLogic.Fields[x][y];
            int chip = gameLogic.Chips[x][y];
            boardCells[x][y].FieldPicture.Source = pictures[field];
            if (chip == 0)
            {
                boardCells[x][y].ChipPicture.Visibility = Visibility.Hidden;
            }
            else
            {
                boardCells[x][y].ChipPicture.Visibility = Visibility.Visible;

                boardCells[x][y].ChipPicture.Source = pictures[gameLogic.ConvertChip(chip)];
            }
        }

        public void LoadPictures()
        {
            try
            {
                for (int i = 1; i < 31; i++)
                {
                    pictures[i] = new BitmapImage(new Uri("Images/" + i + ".png", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load an image.");
            }
        }

        public void SetRandomStartingPosition()
        {
            SetInitialPosition();
            gameLogic.SetRandomStartingPosition();
        }

        public int GetCellCoordinates(object sender, bool isX)
        {
            int[] coordinates = (int[])(sender as BoardCell).Tag;
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
            switch (gameLogic.GameState)
            {
                case 1:
                case -1:
                    gameLogic.StartMove(x, y);
                    ShowGameState();
                    break;
                case 2:
                case -2:
                    gameLogic.FinalizeMove(x, y, false);
                    ShowGameState();
                    break;
            }
        }

        public void RightSingleClick(int x, int y)
        {
            gameLogic.FinalizeMove(x, y, true);
        }

        public void LeftDoubleClick(int x, int y)
        {
            gameLogic.SetRandomColor(x, y);
            ShowGameState();
        }

        public void RightDoubleClick(int x, int y)
        {
            gameLogic.GameState = gameLogic.NormalizeGameState(gameLogic.GameState);
            ShowGameState();
        }

        public void NotifyHistoryLoadFailed()
        {
            MessageBox.Show("Failed to load history");
        }

        public void StepBack()
        {
            gameLogic.StepBack();    
        }

        public void StepForward()
        {
            gameLogic.StepForward();
        }

        public void MoveToStart()
        {
            gameLogic.MoveToStart();
        }

        public void MoveToEnd()
        {
            gameLogic.MoveToEnd();
        }

        public void ShowGameState()
        {
            switch (gameLogic.GameState)
            {
                case 1:
                    gameStatus.Text = "White: Left click to select a piece. Double click to colorize a square.";
                    whiteMove.Visibility = Visibility.Visible;
                    blackMove.Visibility = Visibility.Hidden;
                    break;
                case -1:
                    gameStatus.Text = "Black: Left click to select a piece. Double click to colorize a square.";
                    whiteMove.Visibility = Visibility.Hidden;
                    blackMove.Visibility = Visibility.Visible;
                    break;
                case 2:
                    gameStatus.Text = "White: Left click to move to a square. Right click to promote on a square.";
                    whiteMove.Visibility = Visibility.Visible;
                    blackMove.Visibility = Visibility.Hidden;
                    break;
                case -2:
                    gameStatus.Text = "Black: Left click to move to a square. Right click to promote on a square.";
                    whiteMove.Visibility = Visibility.Hidden;
                    blackMove.Visibility = Visibility.Visible;
                    break;
                case 3:
                    gameStatus.Text = "White: Setting random color on a chosen square.";
                    whiteMove.Visibility = Visibility.Visible;
                    blackMove.Visibility = Visibility.Hidden;
                    break;
                case -3:
                    gameStatus.Text = "Black: Setting random color on a chosen square.";
                    whiteMove.Visibility = Visibility.Hidden;
                    blackMove.Visibility = Visibility.Visible;
                    break;
                case 4:
                    gameStatus.Text = "White: Left click on a desired color to colorize a chosen square.";
                    whiteMove.Visibility = Visibility.Visible;
                    blackMove.Visibility = Visibility.Hidden;
                    break;
                case -4:
                    gameStatus.Text = "Black: Left click on a desired color to colorize a chosen square.";
                    whiteMove.Visibility = Visibility.Hidden;
                    blackMove.Visibility = Visibility.Visible;
                    break;
                case 5:
                    gameStatus.Text = "White: Left click to set a piece on a desired square.";
                    whiteMove.Visibility = Visibility.Visible;
                    blackMove.Visibility = Visibility.Hidden;
                    break;
                case -5:
                    gameStatus.Text = "Black: Left click to set a piece on a desired square.";
                    whiteMove.Visibility = Visibility.Hidden;
                    blackMove.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeBoard();
        }

        public void color_LeftClick(object sender, RoutedEventArgs e)
        {
            int color = (int)(sender as Button).Tag;
            gameLogic.DefineColor(color);
            ShowGameState();
        }

        public void changeDimensions_Click(object sender, RoutedEventArgs e)
        {
            new ChangeDimensions(gameLogic).ShowDialog();
        }

        public void toStart_Click(object sender, RoutedEventArgs e)
        {
            MoveToStart();
        }

        public void toEnd_Click(object sender, RoutedEventArgs e)
        {
            MoveToEnd();
        }

        public void stepBackward_Click(object sender, RoutedEventArgs e)
        {
            StepBack();
        }

        public void stepForward_Click(object sender, RoutedEventArgs e)
        {
            StepForward();
        }

        private async void cell_LeftClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                leftClicked++;
                leftClicks.Add(sender);

                if (leftClicks.Count > 1)
                {
                    if (leftClicks[0] == leftClicks[1])
                    {
                        LeftDoubleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
                        leftClicks.Clear();
                        rightClicks.Clear();
                    }
                    else
                    {
                        LeftSingleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
                        LeftSingleClick(GetCellCoordinates(leftClicks[1], true), GetCellCoordinates(leftClicks[1], false));
                        leftClicks.Clear();
                        rightClicks.Clear();
                    }
                }

                await Task.Delay(300);

                if (leftClicks.Count == 1)
                {
                    LeftSingleClick(GetCellCoordinates(leftClicks[0], true), GetCellCoordinates(leftClicks[0], false));
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

                if (rightClicks.Count > 1)
                {
                    if (rightClicks[0] == rightClicks[1])
                    {
                        RightDoubleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
                        rightClicks.Clear();
                        leftClicks.Clear();
                    }
                    else
                    {
                        RightSingleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
                        RightSingleClick(GetCellCoordinates(rightClicks[1], true), GetCellCoordinates(rightClicks[1], false));
                        rightClicks.Clear();
                        leftClicks.Clear();
                    }
                }

                await Task.Delay(300);

                if (rightClicks.Count == 1)
                {
                    RightSingleClick(GetCellCoordinates(rightClicks[0], true), GetCellCoordinates(rightClicks[0], false));
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

        public void random_Click(object sender, RoutedEventArgs e)
        {
            SetRandomStartingPosition();
        }

        public void gamesDatabase_Click(object sender, RoutedEventArgs e)
        {
            GamesDatabaseWindow gamesDatabaseWindow = new GamesDatabaseWindow(gameLogic);
            gamesDatabaseWindow.ShowDialog();
        }

        public void saveGame_Click(object sender, RoutedEventArgs e)
        {
            gameLogic.SaveGame();
        }

        public void saveGameAs_Click(object sender, RoutedEventArgs e)
        {
            gameLogic.SaveGameAs();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
    }
}