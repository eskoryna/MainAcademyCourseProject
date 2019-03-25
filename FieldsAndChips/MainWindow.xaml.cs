using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FieldsAndChips
{
    public partial class MainWindow : Window
    {
        public int xCells;
        public int yCells;

        public string startingPosition;
        public string moves;

        Random random = new Random();

        int gameState = 1;
        int xA = 0;
        int yA = 0;
        int chipA = 0;
        int fieldA = 0;

        int chipB = 0;
        int fieldB = 0;

        public double cellSize;

        public BitmapImage[] pictures = new BitmapImage[31];

        public List<List<int>> fields = new List<List<int>>();
        public List<List<int>> chips = new List<List<int>>();
        public List<List<BoardCell>> boardCells = new List<List<BoardCell>>();
        public List<BoardCell> colorCells = new List<BoardCell>();

        public int leftClicked = 0;
        public int rightClicked = 0;
        public List<object> leftClicks = new List<object>();
        public List<object> rightClicks = new List<object>();

        int historyPointer = 0;
        public List<List<int>> historyQueue = new List<List<int>>();
        public List<List<int>> historyStack = new List<List<int>>();

        ApplicationContext db;
        SavedGame savedGame;

        public MainWindow()
        {
            InitializeComponent();

            db = new ApplicationContext();

            LoadPictures();
            SetColors();
            ConfigureFromFile();
            ConfigureBoard();
        }

        public void ChangeCellSize()
        {
            double xCellEstimateSize = Math.Floor(board.ActualWidth / (xCells));
            double yCellEstimateSize = Math.Floor(board.ActualHeight / yCells);
            cellSize = Math.Min(xCellEstimateSize, yCellEstimateSize);
        }

        public void ConfigureBoard()
        {
            SetBoard();
            SetInitialPosition();
        }

        public void SetBoard()
        {
            ChangeCellSize();
            board.Children.Clear();
            boardCells.Clear();

            for (int i = 0; i < xCells; i++)
            {
                boardCells.Add(new List<BoardCell>());
                for (int j = 0; j < yCells; j++)
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
            colorRed.Source = pictures[1];
            buttonRed.Tag = 1;
            colorOrange.Source = pictures[2];
            buttonOrange.Tag = 2;
            colorYellow.Source = pictures[3];
            buttonYellow.Tag = 3;
            colorGreen.Source = pictures[4];
            buttonGreen.Tag = 4;
            colorAzure.Source = pictures[5];
            buttonAzure.Tag = 5;
            colorBlue.Source = pictures[6];
            buttonBlue.Tag = 6;
            colorViolet.Source = pictures[7];
            buttonViolet.Tag = 7;
        }

        public void ResizeBoard()
        {
            ChangeCellSize();
            for (int i = 0; i < xCells; i++)
            {
                for (int j = 0; j < yCells; j++)
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
            try
            {
                List<string> parameters = File.ReadAllLines(Directory.GetCurrentDirectory() + "/fac.cfg").ToList();
                xCells = int.Parse(parameters[0]);
                yCells = int.Parse(parameters[1]);

                if (xCells < 7 || xCells > 35 || yCells < 7 || yCells > 25)
                {
                    SetDefaultConfiguration();
                }
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
            ConfigureBoard();
        }

        public void SetDefaultConfiguration()
        {
            xCells = 14;
            yCells = 11;
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
            SetStartingPositionString();
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

                boardCells[x][y].ChipPicture.Source = pictures[ConvertChip(chip)];
            }
        }

        public int ConvertChip(int chip)
        {
            if (chip >=1 && chip <= 10)
            {
                chip += 10;
                return chip;
            }
            else  if (chip >=-10 && chip <= -1)
            {
                chip = -chip;
                chip += 20;
                return chip;
            }
            else
            {
                return 0;
            }
        }

        public int DeconvertChip(int cell)
        {
            int chip = 0;
            int field = 0;
            field = cell / 100;
            chip = cell - field * 100;
            if (chip >= 11 && chip <= 20)
            {
                chip -= 10;
                return chip;
            }
            else if (chip >= 21 && chip <= 30)
            {
                chip -= 20;
                chip = -chip;
                return chip;
            }
            else
            {
                return 0;
            }
        }

        public int ConvertField(int field)
        {
            field *= 100;
            return field;
        }

        public int DeconvertField(int cell)
        {
            int field = cell / 100;
            return field;
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
                MessageBox.Show("Unable to load an image. " + ex.Message + " " + ex.StackTrace);
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

            SetStartingPositionString();
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
        }

        public void RightSingleClick(int x, int y)
        {
            if (Math.Abs(gameState) == 2 && ((Math.Abs(chipA) == 9 && fields[x][y] == 8) ||
                (Math.Abs(chipA) == 8 && fields[x][y] >= 1 && fields[x][y] <= 7) ||
                Math.Abs(chipA) >= 1 && Math.Abs(chipA) <= 7 && Math.Abs(chipA) == fields[x][y]))
            {
                FinalizeMove(x, y, true);
            }
        }

        public void LeftDoubleClick(int x, int y)
        {
            switch (gameState)
            {
                case 1:
                    if (chips[x][y] == 0)
                    {
                        if (fields[x][y] == 8)
                        {
                            gameState = 3;
                            ShowGameState();
                            fields[x][y] = random.Next(1, 8);
                            ChangeBoardCell(x, y);
                            AddToHistory(NormalizeGameState(gameState), x, y, 0, 8, 0, fields[x][y]);
                            gameState = -1;
                            ShowGameState();
                        }
                    }
                    else if(((chips[x][y] >= 2 && chips[x][y] <= 8) || chips[x][y] == 10) && 
                        fields[x][y] >= 1 && fields[x][y] <= 7)
                    {
                        ReserveCurrent(x, y);
                        gameState = 4;
                        ShowGameState();
                    }
                    else if (chips[x][y] == 9 && fields[x][y] >= 1 && fields[x][y] <= 7)
                    {
                        fieldB = fields[x][y];
                        gameState = 4;
                        ShowGameState();
                        fields[x][y] = 8;
                        chips[x][y] = 8;
                        ChangeBoardCell(x, y);
                        AddToHistory(NormalizeGameState(gameState), x, y, 9, fieldB, 8, 8);
                        gameState = -1;
                        ShowGameState();
                    }
                    break;
                case -1:
                    if (chips[x][y] == 0)
                    {
                        if (fields[x][y] == 8)
                        {
                            gameState = -3;
                            ShowGameState();
                            fields[x][y] = random.Next(1, 8);
                            ChangeBoardCell(x, y);
                            AddToHistory(NormalizeGameState(gameState), x, y, 0, 8, 0, fields[x][y]);
                            gameState = 1;
                            ShowGameState();
                        }
                    }
                    else if (((chips[x][y] >= -8 && chips[x][y] <= -2) || chips[x][y] == -10) &&
                        fields[x][y] >= 1 && fields[x][y] <= 7)
                    {
                        ReserveCurrent(x, y);
                        gameState = -4;
                        ShowGameState();
                    }
                    else if (chips[x][y] == -9 && fields[x][y] >= 1 && fields[x][y] <= 7)
                    {
                        fieldB = fields[x][y];
                        gameState = 4;
                        ShowGameState();
                        fields[x][y] = 8;
                        chips[x][y] = -8;
                        ChangeBoardCell(x, y);
                        AddToHistory(NormalizeGameState(gameState), x, y, -9, fieldB, -8, 8);
                        gameState = 1;
                        ShowGameState();
                    }
                    break;
            }
        }

        public void RightDoubleClick(int x, int y)
        {
            gameState = NormalizeGameState(gameState);
            ShowGameState();
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

                            AddToHistory(NormalizeGameState(gameState), xA, yA, chipA, fieldA, chips[xA][yA], fields[xA][yA],
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

                            AddToHistory(NormalizeGameState(gameState), xA, yA, chipA, fieldA, chips[xA][yA], fields[xA][yA],
                                x, y, chipB, fieldB, chips[x][y], fields[x][y]);

                            gameState = 1;
                            ShowGameState();
                        }
                    }

                    break;
            }
        }

        public void DefineColor(int color)
        {
            chipB = chips[xA][yA];
            fieldB = fields[xA][yA];

            switch (gameState)
            {
                case 4:
                    if ((chips[xA][yA] == 10 || (chips[xA][yA] >= 2 && chips[xA][yA] <= 8)) && fieldB != color)
                    {
                        if (chips[xA][yA] == 10)
                        {
                            chips[xA][yA] = 8;
                        }
                        else
                        {
                            chips[xA][yA] = --chips[xA][yA];
                        }

                        fields[xA][yA] = color;
                        ChangeBoardCell(xA, yA);
                        AddToHistory(NormalizeGameState(gameState), xA, yA, chipB, fieldB, chips[xA][yA], fields[xA][yA]);
                        gameState = -1;
                        ShowGameState();
                    }
                    break;
                case -4:
                    if ((chips[xA][yA] == -10 || (chips[xA][yA] >= -8 && chips[xA][yA] <= -2)) && fieldB != color)
                    {
                        if (chips[xA][yA] == -10)
                        {
                            chips[xA][yA] = -8;
                        }
                        else
                        {
                            chips[xA][yA] = ++chips[xA][yA];
                        }

                        fields[xA][yA] = color;
                        ChangeBoardCell(xA, yA);
                        AddToHistory(NormalizeGameState(gameState), xA, yA, chipB, fieldB, chips[xA][yA], fields[xA][yA]);
                        gameState = 1;
                        ShowGameState();
                    }
                    break;
            }
        }

        public int NormalizeGameState(int gameState)
        {
            if (gameState > 0 )
            {
                return 1;
            }
            else if (gameState < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public bool IsMovePossible(int x, int y)
        {
            if ((Math.Abs(x - xA) == 1 || Math.Abs(x - xA) == xCells - 1) &&
                (Math.Abs(y - yA) == 1 || Math.Abs(y - yA) == yCells - 1))
            {
                return true;
            }
            else if (x == xA)
            {
                int plusChipMove = yA + Math.Abs(chipA);
                if (plusChipMove > yCells - 1)
                {
                    plusChipMove -= yCells;
                }

                int minusChipMove = yA - Math.Abs(chipA);
                if (minusChipMove < 0)
                {
                    minusChipMove += yCells;
                }

                int plusFieldMove = yA + fieldA;
                if (plusFieldMove > yCells - 1)
                {
                    plusFieldMove -= yCells;
                }

                int minusFieldMove = yA - fieldA;
                if (minusFieldMove < 0)
                {
                    minusFieldMove += yCells;
                }

                if (y == plusChipMove || y == minusChipMove || (y == plusFieldMove && fieldA < 8) ||
                    (y == minusFieldMove && fieldA < 8))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (y == yA)
            {
                int plusChipMove = xA + Math.Abs(chipA);
                if (plusChipMove > xCells - 1)
                {
                    plusChipMove -= xCells;
                }

                int minusChipMove = xA - Math.Abs(chipA);
                if (minusChipMove < 0)
                {
                    minusChipMove += xCells;
                }

                int plusFieldMove = xA + fieldA;
                if (plusFieldMove > xCells - 1)
                {
                    plusFieldMove -= xCells;
                }

                int minusFieldMove = xA - fieldA;
                if (minusFieldMove < 0)
                {
                    minusFieldMove += xCells;
                }

                if (x == plusChipMove || x == minusChipMove || (x == plusFieldMove && fieldA < 8) ||
                    (x == minusFieldMove && fieldA < 8))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string CurrentDateToString()
        {
            string gameDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            return gameDate;
        }

        public void SetStartingPositionString()
        {
            int fieldAndChip = 0;
            startingPosition = NormalizeGameState(gameState) + "#";
            for (int i = 0; i < xCells; i++)
            {
                for (int j = 0; j < yCells; j++)
                {
                    fieldAndChip = ConvertField(fields[i][j]) + ConvertChip(chips[i][j]);
                    startingPosition += fieldAndChip + ";";
                }
            }
        }
            
        public string HistoryToString()
        {
            moves = "";

            if (historyQueue.Count > 0)
            {
                for (int i = 0; i < historyQueue.Count; i++)
                {
                    for (int j = 0; j < historyQueue[i].Count; j++)
                    {
                        moves += historyQueue[i][j];
                        if (j < historyQueue[i].Count - 1)
                        {
                            moves += ";";
                        }
                    }

                    if (i < historyQueue.Count - 1)
                    {
                        moves += "#";
                    }
                }
            }

            if (historyStack.Count >0)
            {
                for (int i = historyStack.Count - 1; i > -1 ; i--)
                {
                    for (int j = 0; j < historyStack[i].Count; j++)
                    {
                        moves += historyStack[i][j] + ";";
                    }
                    moves += "#";
                }
            }

            return moves;
        }

        public void ClearHistory()
        {
            historyPointer = 0;
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

        public void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField)
        {
            int state = NormalizeGameState(gameState);
            for (int i = historyQueue.Count - 1; i >= historyPointer; i--)
            {
                historyQueue.RemoveAt(i);
            }

            historyPointer++;
            historyQueue.Add(new List<int>());
            historyQueue[historyQueue.Count - 1].Add(state);
            historyQueue[historyQueue.Count - 1].Add(startX);
            historyQueue[historyQueue.Count - 1].Add(startY);
            historyQueue[historyQueue.Count - 1].Add(startInitialChip);
            historyQueue[historyQueue.Count - 1].Add(startInitialField);
            historyQueue[historyQueue.Count - 1].Add(startFinalChip);
            historyQueue[historyQueue.Count - 1].Add(startFinalField);
        }

        public void GetHistory(bool isForward)
        {
            if (historyQueue.Count > 0 && historyPointer > 0 && historyPointer <= historyQueue.Count)
            {
                int x;
                int y;
                int chip;
                int field;

                gameState = historyQueue[historyPointer - 1][0];
                if (isForward == true)
                {
                    gameState = -gameState;
                }
                ShowGameState();

                x = historyQueue[historyPointer - 1][1];
                y = historyQueue[historyPointer - 1][2];
                if (isForward == false)
                {
                    chip = historyQueue[historyPointer - 1][3];
                    field = historyQueue[historyPointer - 1][4];
                }
                else
                {
                    chip = historyQueue[historyPointer - 1][5];
                    field = historyQueue[historyPointer - 1][6];
                }

                chips[x][y] = chip;
                fields[x][y] = field;
                ChangeBoardCell(x, y);

                if (historyQueue[historyPointer - 1].Count == 13)
                {
                    x = historyQueue[historyPointer - 1][7];
                    y = historyQueue[historyPointer - 1][8];
                    if (isForward == false)
                    {
                        chip = historyQueue[historyPointer - 1][9];
                        field = historyQueue[historyPointer - 1][10];
                    }
                    else
                    {
                        chip = historyQueue[historyPointer - 1][11];
                        field = historyQueue[historyPointer - 1][12];
                    }

                    chips[x][y] = chip;
                    fields[x][y] = field;
                    ChangeBoardCell(x, y);
                }
            }
        }

        public void StepBack()
        {
            if (historyQueue.Count > 0 && historyPointer > 0 && historyPointer <= historyQueue.Count)
            {
                GetHistory(false);
                historyPointer--;
            }    
        }

        public void StepForward()
        {
            if (historyQueue.Count > 0 && historyPointer < historyQueue.Count)
            {
                historyPointer++;
                GetHistory(true);
            }
        }

        public void MoveToStart()
        {
            if (historyQueue.Count > 0)
            {
                while (historyPointer >= 1)
                {
                    StepBack();
                }
            }
        }

        public void MoveToEnd()
        {
            if (historyQueue.Count > 0)
            {
                while (historyPointer < historyQueue.Count)
                {
                    StepForward();
                }
            }
        }

        public void ShowGameState()
        {
            switch (gameState)
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

        public void LoadGame(SavedGame savedGame)
        {
            try
            {
                ClearBoard();
                int tryXCells = savedGame.HorizontalCells;
                int tryYCells = savedGame.VerticalCells;

                if (tryXCells >= 7 && tryXCells <= 35 && tryYCells >= 7 && tryYCells <= 25)
                {
                    xCells = tryXCells;
                    yCells = tryYCells;
                    SetBoard();

                    string savedGameStartingPosition = savedGame.StartingPosition;
                    SetSavedGameStartingPosition(savedGameStartingPosition);
                    SetStartingPositionString();
                    this.savedGame = savedGame;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load game. The default configuration will be loaded. " + ex.Message + " " + ex.StackTrace);
                SetDefaultConfiguration();
                ConfigureBoard();
            }

            try
            {
                SetSavedGameHistory(savedGame.Moves);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load game history. " + ex.Message + " " + ex.StackTrace);
            }
        }

        public void SetSavedGameStartingPosition(string savedGameStartingPosition)
        {
            string[] startingPositionString = new string[2];
            List<string> startingPositionList = new List<string>();
            startingPositionString = savedGameStartingPosition.Split('#');
            gameState = int.Parse(startingPositionString[0]);
            startingPositionList = startingPositionString[1].Split(';').ToList();

            int cell = 0;
            int chip = 0;
            int field = 0;
            int k = 0;
            for (int i = 0; i < xCells; i++)
            {
                fields.Add(new List<int>());
                chips.Add(new List<int>());

                for (int j = 0; j < yCells; j++)
                {
                    cell = int.Parse(startingPositionList[k]);
                    chip = DeconvertChip(cell);
                    field = DeconvertField(cell);
                    chips[i].Add(chip);
                    fields[i].Add(field);
                    ChangeBoardCell(i, j);
                    k++;
                }
            }
            ShowGameState();
        }

        public void SetSavedGameHistory(string moves)
        {
            if (moves.Length > 0)
            {
                List<string> movesString = new List<string>();
                List<string> moveLine = new List<string>();
                movesString = moves.Split('#').ToList();
                for (int i = 0; i < movesString.Count; i++)
                {
                    moveLine.Clear();
                    moveLine = movesString[i].Split(';').ToList();
                    historyQueue.Add(new List<int>());
                    for (int j = 0; j < moveLine.Count; j++)
                    {
                        int moveElement = int.Parse(moveLine[j]);
                        historyQueue[i].Add(moveElement);
                    }
                }
            }
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeBoard();
        }

        public void color_LeftClick(object sender, RoutedEventArgs e)
        {
            switch (gameState)
            {
                case 4:
                case -4:
                    int color = (int)(sender as Button).Tag;
                    DefineColor(color);
                    break;
            }
        }

        public void changeDimensions_Click(object sender, RoutedEventArgs e)
        {
            new ChangeDimensions().ShowDialog();
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
            GamesDatabaseWindow gamesDatabaseWindow = new GamesDatabaseWindow();
            gamesDatabaseWindow.Show();
        }

        public void saveGame_Click(object sender, RoutedEventArgs e)
        {
            if (savedGame == null)
            {
                saveGameAs_Click(sender, e);
            }
            else
            {
                db = new ApplicationContext();
                db.SavedGames.Load();
                SavedGame game = db.SavedGames.FirstOrDefault(d => d.Id == savedGame.Id);
                if (game != null)
                {
                    game.GameDate = CurrentDateToString();
                    game.HorizontalCells = xCells;
                    game.VerticalCells = yCells;
                    game.StartingPosition = startingPosition;
                    game.Moves = HistoryToString();
                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void saveGameAs_Click(object sender, RoutedEventArgs e)
        {
            SaveGameWindow savedGameWindow = new SaveGameWindow(new SavedGame());
            if (savedGameWindow.ShowDialog() == true)
            {
                savedGame = savedGameWindow.SavedGame;
                savedGame.GameDate = CurrentDateToString();
                savedGame.HorizontalCells = xCells;
                savedGame.VerticalCells = yCells;
                savedGame.StartingPosition = startingPosition;
                savedGame.Moves = HistoryToString();
                db.SavedGames.Add(savedGame);
                db.SaveChanges();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }
    }
}