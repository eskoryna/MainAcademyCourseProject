using FieldsAndChips;
using System;
using System.Collections.Generic;

namespace FieldsAndChips
{
    public interface IGameLogic
    {
        int XCells { get; set; }
        int YCells { get; set; }
        int GameState { get; set; }

        List<List<int>> Fields { get; set; }
        List<List<int>> Chips { get; set; }

        void SetGameLogic(int xCells, int yCells, int gameState);
        void ClearBoard();
        void SetInitialPosition();
        void SetRandomStartingPosition();
        void StartMove(int x, int y);
        void FinalizeMove(int x, int y, bool promote);
        void SetRandomColor(int x, int y);
        void DefineColor(int color);

        void SaveGame();
        void SaveGameAs();
        void LoadGame(SavedGame savedGame);
        void StepBack();
        void StepForward();
        void MoveToStart();
        void MoveToEnd();

        int NormalizeGameState(int gameState);
        int ConvertField(int field);
        int DeconvertField(int cell);
        int ConvertChip(int chip);
        int DeconvertChip(int cell);
    }
}
