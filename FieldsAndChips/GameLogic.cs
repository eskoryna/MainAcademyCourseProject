using System;
using System.Collections.Generic;

namespace FieldsAndChips
{
    public class GameLogic : IGameLogic
    {
        IGameHistory gameHistory;
        IGameFront gameFront;

        public int XCells { get; set; }
        public int YCells { get; set; }
        public int GameState { get; set; }

        public List<List<int>> Fields { get; set; }
        public List<List<int>> Chips { get; set; }

        Random random = new Random();

        int xA = 0;
        int yA = 0;
        int chipA = 0;
        int fieldA = 0;
        int chipB = 0;
        int fieldB = 0;

        public GameLogic(IGameFront gameFront)
        {
            gameHistory = new GameHistory(gameFront, this);
            this.gameFront = gameFront;

            Fields = new List<List<int>>();
            Chips = new List<List<int>>();
        }

        public void SetGameLogic(int xCells, int yCells, int gameState)
        {
            XCells = xCells;
            YCells = yCells;
            GameState = gameState;
        }

        public void ClearBoard()
        {
            Fields.Clear();
            Chips.Clear();
            gameHistory.ClearHistory();
            GameState = 1;
        }

        public void SetInitialPosition()
        {
            for (int i = 0; i < XCells; i++)
            {
                Fields.Add(new List<int>());
                Chips.Add(new List<int>());

                for (int j = 0; j < YCells; j++)
                {
                    Fields[i].Add(8);
                    Chips[i].Add(0);
                    gameFront.ChangeBoardCell(i, j);
                }
            }

            gameHistory.SetStartingPositionString();
        }

        public void SetRandomStartingPosition()
        {
            int i = 0;
            int j = 0;
            for (int k = -10; k <= 10; k++)
            {
                if (k == 0)
                {
                    continue;
                }

                i = random.Next(0, XCells);
                j = random.Next(0, YCells);

                if (Chips[i][j] == 0)
                {
                    Chips[i][j] = k;
                    gameFront.ChangeBoardCell(i, j);
                }
                else
                {
                    k--;
                }
            }

            gameHistory.SetStartingPositionString();
        }

        public void StartMove(int x, int y)
        {
            switch (GameState)
            {
                case 1:
                    if (Chips[x][y] > 0)
                    {
                        ReserveCurrent(x, y);
                        GameState = 2;
                    }
                    break;
                case -1:
                    if (Chips[x][y] < 0)
                    {
                        ReserveCurrent(x, y);
                        GameState = -2;
                    }
                    break;
            }
        }

        public void FinalizeMove(int x, int y, bool promote)
        {
            fieldB = Fields[x][y];
            chipB = Chips[x][y];

            switch (GameState)
            {
                case 2:
                    if (Chips[x][y] == 0)
                    {
                        if (IsMovePossible(x, y))
                        {
                            Chips[xA][yA] = 0;
                            gameFront.ChangeBoardCell(xA, yA);
                            Chips[x][y] = chipA;

                            if (promote == true)
                            {
                                switch (Chips[x][y])
                                {
                                    case 9:
                                        if (Fields[x][y] == 8)
                                        {
                                            Chips[x][y] = 10;
                                        }
                                        break;
                                    case 8:
                                        if (Fields[x][y] >= 1 && Fields[x][y] <= 7)
                                        {
                                            Chips[x][y] = 9;
                                        }
                                        break;
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                        if (Chips[x][y] == Fields[x][y])
                                        {
                                            Chips[x][y] = ++Chips[x][y];
                                        }
                                        break;
                                }
                            }

                            gameFront.ChangeBoardCell(x, y);

                            gameHistory.AddToHistory(NormalizeGameState(GameState), xA, yA, chipA, fieldA, Chips[xA][yA], Fields[xA][yA],
                                x, y, chipB, fieldB, Chips[x][y], Fields[x][y]);

                            GameState = -1;
                        }
                    }

                    break;
                case -2:
                    if (Chips[x][y] == 0)
                    {
                        if (IsMovePossible(x, y))
                        {
                            Chips[xA][yA] = 0;
                            gameFront.ChangeBoardCell(xA, yA);
                            Chips[x][y] = chipA;

                            if (promote == true)
                            {
                                switch (Chips[x][y])
                                {
                                    case -9:
                                        if (Fields[x][y] == 8)
                                        {
                                            Chips[x][y] = -10;
                                        }
                                        break;
                                    case -8:
                                        if (Fields[x][y] >= 1 && Fields[x][y] <= 7)
                                        {
                                            Chips[x][y] = -9;
                                        }
                                        break;
                                    case -1:
                                    case -2:
                                    case -3:
                                    case -4:
                                    case -5:
                                    case -6:
                                    case -7:
                                        if (Chips[x][y] == -Fields[x][y])
                                        {
                                            Chips[x][y] = --Chips[x][y];
                                        }
                                        break;
                                }
                            }

                            gameFront.ChangeBoardCell(x, y);

                            gameHistory.AddToHistory(NormalizeGameState(GameState), xA, yA, chipA, fieldA, Chips[xA][yA], Fields[xA][yA],
                                x, y, chipB, fieldB, Chips[x][y], Fields[x][y]);

                            GameState = 1;
                        }
                    }

                    break;
            }
        }

        public void SetRandomColor(int x, int y)
        {
            switch (GameState)
            {
                case 1:
                    if (Chips[x][y] == 0)
                    {
                        if (Fields[x][y] == 8)
                        {
                            GameState = 3;
                            Fields[x][y] = random.Next(1, 8);
                            gameFront.ChangeBoardCell(x, y);
                            gameHistory.AddToHistory(NormalizeGameState(GameState), x, y, 0, 8, 0, Fields[x][y]);
                            GameState = -1;
                        }
                    }
                    else if (((Chips[x][y] >= 2 && Chips[x][y] <= 8) || Chips[x][y] == 10) &&
                        Fields[x][y] >= 1 && Fields[x][y] <= 7)
                    {
                        ReserveCurrent(x, y);
                        GameState = 4;
                    }
                    else if (Chips[x][y] == 9 && Fields[x][y] >= 1 && Fields[x][y] <= 7)
                    {
                        fieldB = Fields[x][y];
                        GameState = 4;
                        Fields[x][y] = 8;
                        Chips[x][y] = 8;
                        gameFront.ChangeBoardCell(x, y);
                        gameHistory.AddToHistory(NormalizeGameState(GameState), x, y, 9, fieldB, 8, 8);
                        GameState = -1;
                    }
                    break;
                case -1:
                    if (Chips[x][y] == 0)
                    {
                        if (Fields[x][y] == 8)
                        {
                            GameState = -3;
                            Fields[x][y] = random.Next(1, 8);
                            gameFront.ChangeBoardCell(x, y);
                            gameHistory.AddToHistory(NormalizeGameState(GameState), x, y, 0, 8, 0, Fields[x][y]);
                            GameState = 1;
                        }
                    }
                    else if (((Chips[x][y] >= -8 && Chips[x][y] <= -2) || Chips[x][y] == -10) &&
                        Fields[x][y] >= 1 && Fields[x][y] <= 7)
                    {
                        ReserveCurrent(x, y);
                        GameState = -4;
                    }
                    else if (Chips[x][y] == -9 && Fields[x][y] >= 1 && Fields[x][y] <= 7)
                    {
                        fieldB = Fields[x][y];
                        GameState = 4;
                        Fields[x][y] = 8;
                        Chips[x][y] = -8;
                        gameFront.ChangeBoardCell(x, y);
                        gameHistory.AddToHistory(NormalizeGameState(GameState), x, y, -9, fieldB, -8, 8);
                        GameState = 1;
                    }
                    break;
            }
        }

        public void DefineColor(int color)
        {
            chipB = Chips[xA][yA];
            fieldB = Fields[xA][yA];

            switch (GameState)
            {
                case 4:
                    if ((Chips[xA][yA] == 10 || (Chips[xA][yA] >= 2 && Chips[xA][yA] <= 8)) && fieldB != color)
                    {
                        if (Chips[xA][yA] == 10)
                        {
                            Chips[xA][yA] = 8;
                        }
                        else
                        {
                            Chips[xA][yA] = --Chips[xA][yA];
                        }

                        Fields[xA][yA] = color;
                        gameFront.ChangeBoardCell(xA, yA);
                        gameHistory.AddToHistory(NormalizeGameState(GameState), xA, yA, chipB, fieldB, Chips[xA][yA], Fields[xA][yA]);
                        GameState = -1;
                    }
                    break;
                case -4:
                    if ((Chips[xA][yA] == -10 || (Chips[xA][yA] >= -8 && Chips[xA][yA] <= -2)) && fieldB != color)
                    {
                        if (Chips[xA][yA] == -10)
                        {
                            Chips[xA][yA] = -8;
                        }
                        else
                        {
                            Chips[xA][yA] = ++Chips[xA][yA];
                        }

                        Fields[xA][yA] = color;
                        gameFront.ChangeBoardCell(xA, yA);
                        gameHistory.AddToHistory(NormalizeGameState(GameState), xA, yA, chipB, fieldB, Chips[xA][yA], Fields[xA][yA]);
                        GameState = 1;
                    }
                    break;
            }
        }

        public void SaveGame()
        {
            bool saveGameResult = gameHistory.SaveGame();
            if (saveGameResult == false)
            {
                SaveGameAs();
            }
        }

        public void SaveGameAs()
        {
            gameHistory.SaveGameAs();
        }

        public void LoadGame(SavedGame savedGame)
        {
            gameHistory.LoadGame(savedGame);
        }

        public void StepBack()
        {
            gameHistory.StepBack();
        }

        public void StepForward()
        {
            gameHistory.StepForward();
        }

        public void MoveToStart()
        {
            gameHistory.MoveToStart();
        }

        public void MoveToEnd()
        {
            gameHistory.MoveToEnd();

        }

        public int NormalizeGameState(int gameState)
        {
            if (gameState > 0)
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

        public int ConvertChip(int chip)
        {
            if (chip >= 1 && chip <= 10)
            {
                chip += 10;
                return chip;
            }
            else if (chip >= -10 && chip <= -1)
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

        public void ReserveCurrent(int x, int y)
        {
            xA = x;
            yA = y;
            fieldA = Fields[x][y];
            chipA = Chips[x][y];
        }

        public bool IsMovePossible(int x, int y)
        {
            if ((Math.Abs(x - xA) == 1 || Math.Abs(x - xA) == XCells - 1) &&
                (Math.Abs(y - yA) == 1 || Math.Abs(y - yA) == YCells - 1))
            {
                return true;
            }
            else if (x == xA)
            {
                int plusChipMove = yA + Math.Abs(chipA);
                if (plusChipMove > YCells - 1)
                {
                    plusChipMove -= YCells;
                }

                int minusChipMove = yA - Math.Abs(chipA);
                if (minusChipMove < 0)
                {
                    minusChipMove += YCells;
                }

                int plusFieldMove = yA + fieldA;
                if (plusFieldMove > YCells - 1)
                {
                    plusFieldMove -= YCells;
                }

                int minusFieldMove = yA - fieldA;
                if (minusFieldMove < 0)
                {
                    minusFieldMove += YCells;
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
                if (plusChipMove > XCells - 1)
                {
                    plusChipMove -= XCells;
                }

                int minusChipMove = xA - Math.Abs(chipA);
                if (minusChipMove < 0)
                {
                    minusChipMove += XCells;
                }

                int plusFieldMove = xA + fieldA;
                if (plusFieldMove > XCells - 1)
                {
                    plusFieldMove -= XCells;
                }

                int minusFieldMove = xA - fieldA;
                if (minusFieldMove < 0)
                {
                    minusFieldMove += XCells;
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
    }
}
