using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace FieldsAndChips
{
    public class GameHistory : IGameHistory
    {
        IGameLogic gameLogic;
        IGameFront gameFront;

        public string StartingPosition { get; set; }
        public string Moves { get; set; }
        public SavedGame SavedGame { get; set; }

        int historyPointer = 0;
        public List<List<int>> historyQueue = new List<List<int>>();

        ApplicationContext db;
        
        public GameHistory(IGameFront gameFront, IGameLogic gameLogic)
        {
            db = new ApplicationContext();
            this.gameFront = gameFront;
            this.gameLogic = gameLogic;
        }

        public void SetStartingPositionString()
        {
            int fieldAndChip = 0;
            StartingPosition = gameLogic.NormalizeGameState(gameLogic.GameState) + "#";
            for (int i = 0; i < gameLogic.XCells; i++)
            {
                for (int j = 0; j < gameLogic.YCells; j++)
                {
                    fieldAndChip = gameLogic.ConvertField(gameLogic.Fields[i][j]) + gameLogic.ConvertChip(gameLogic.Chips[i][j]);
                    StartingPosition += fieldAndChip + ";";
                }
            }
        }

        public void ClearHistory()
        {
            historyPointer = 0;
            historyQueue.Clear();
        }

        public void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField, int endX, int endY, int endInitialChip, int endInitialField,
            int endFinalChip, int endFinalField)
        {
            AddToHistory(gameState, startX, startY, startInitialChip, startInitialField, startFinalChip, startFinalField);

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
            for (int i = historyQueue.Count - 1; i >= historyPointer; i--)
            {
                historyQueue.RemoveAt(i);
            }

            historyPointer++;
            historyQueue.Add(new List<int>());
            historyQueue[historyQueue.Count - 1].Add(gameState);
            historyQueue[historyQueue.Count - 1].Add(startX);
            historyQueue[historyQueue.Count - 1].Add(startY);
            historyQueue[historyQueue.Count - 1].Add(startInitialChip);
            historyQueue[historyQueue.Count - 1].Add(startInitialField);
            historyQueue[historyQueue.Count - 1].Add(startFinalChip);
            historyQueue[historyQueue.Count - 1].Add(startFinalField);
        }

        public bool SaveGame()
        {
            if (SavedGame == null)
            {
                return false;
            }
            else
            {
                db = new ApplicationContext();
                db.SavedGames.Load();
                SavedGame game = db.SavedGames.FirstOrDefault(d => d.Id == SavedGame.Id);
                if (game != null)
                {
                    game.GameDate = CurrentDateToString();
                    game.HorizontalCells = gameLogic.XCells;
                    game.VerticalCells = gameLogic.YCells;
                    game.StartingPosition = StartingPosition;
                    game.Moves = HistoryToString();
                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
            }
        }

        public void SaveGameAs()
        {
            SaveGameWindow savedGameWindow = new SaveGameWindow(new SavedGame());
            if (savedGameWindow.ShowDialog() == true)
            {
                SavedGame = savedGameWindow.SavedGame;
                SavedGame.GameDate = CurrentDateToString();
                SavedGame.HorizontalCells = gameLogic.XCells;
                SavedGame.VerticalCells = gameLogic.YCells;
                SavedGame.StartingPosition = StartingPosition;
                SavedGame.Moves = HistoryToString();
                db.SavedGames.Add(SavedGame);
                db.SaveChanges();
            }
        }

        public void LoadGame(SavedGame savedGame)
        {
            try
            {
                gameFront.ClearBoard();
                int xCells = savedGame.HorizontalCells;
                int yCells = savedGame.VerticalCells;

                if (xCells >= 7 && xCells <= 35 && yCells >= 7 && yCells <= 25)
                {
                    gameLogic.XCells = xCells;
                    gameLogic.YCells = yCells;
                    gameFront.SetBoard();

                    string savedGameStartingPosition = savedGame.StartingPosition;
                    SetSavedGameStartingPosition(savedGameStartingPosition);
                    SetStartingPositionString();
                    SavedGame = savedGame;
                }
                else
                {
                    gameFront.NotifyHistoryLoadFailed();
                }
            }
            catch (Exception ex)
            {
                gameFront.NotifyHistoryLoadFailed();
                gameFront.SetDefaultConfiguration();
                gameFront.SetBoard();
                gameFront.SetInitialPosition();
            }

            try
            {
                SetSavedGameHistory(savedGame.Moves);
            }
            catch (Exception ex)
            {
                gameFront.NotifyHistoryLoadFailed();
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

        public void GetHistory(bool isForward)
        {
            if (historyQueue.Count > 0 && historyPointer > 0 && historyPointer <= historyQueue.Count)
            {
                int x;
                int y;
                int chip;
                int field;

                gameLogic.GameState = historyQueue[historyPointer - 1][0];
                if (isForward == true)
                {
                    gameLogic.GameState = -gameLogic.GameState;
                }
                gameFront.ShowGameState();

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

                gameLogic.Chips[x][y] = chip;
                gameLogic.Fields[x][y] = field;
                gameFront.ChangeBoardCell(x, y);

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

                    gameLogic.Chips[x][y] = chip;
                    gameLogic.Fields[x][y] = field;
                    gameFront.ChangeBoardCell(x, y);
                }
            }
        }

        public void SetSavedGameStartingPosition(string savedGameStartingPosition)
        {
            string[] startingPositionString = new string[2];
            List<string> startingPositionList = new List<string>();
            startingPositionString = savedGameStartingPosition.Split('#');
            gameLogic.GameState = int.Parse(startingPositionString[0]);
            startingPositionList = startingPositionString[1].Split(';').ToList();

            int cell = 0;
            int chip = 0;
            int field = 0;
            int k = 0;
            for (int i = 0; i < gameLogic.XCells; i++)
            {
                gameLogic.Fields.Add(new List<int>());
                gameLogic.Chips.Add(new List<int>());

                for (int j = 0; j < gameLogic.YCells; j++)
                {
                    cell = int.Parse(startingPositionList[k]);
                    chip = gameLogic.DeconvertChip(cell);
                    field = gameLogic.DeconvertField(cell);
                    gameLogic.Chips[i].Add(chip);
                    gameLogic.Fields[i].Add(field);
                    gameFront.ChangeBoardCell(i, j);
                    k++;
                }
            }
            gameFront.ShowGameState();
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

        public string CurrentDateToString()
        {
            string gameDate = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            return gameDate;
        }

        public string HistoryToString()
        {
            Moves = "";

            if (historyQueue.Count > 0)
            {
                for (int i = 0; i < historyQueue.Count; i++)
                {
                    for (int j = 0; j < historyQueue[i].Count; j++)
                    {
                        Moves += historyQueue[i][j];
                        if (j < historyQueue[i].Count - 1)
                        {
                            Moves += ";";
                        }
                    }

                    if (i < historyQueue.Count - 1)
                    {
                        Moves += "#";
                    }
                }
            }
            return Moves;
        }
    }
}
