using System;

namespace FieldsAndChips
{
    public interface IGameHistory
    {
        string StartingPosition { get; set; }
        string Moves { get; set; }
        SavedGame SavedGame { get; set; }

        void SetStartingPositionString();
        void ClearHistory();
        void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField);
        void AddToHistory(int gameState, int startX, int startY, int startInitialChip, int startInitialField,
            int startFinalChip, int startFinalField, int endX, int endY, int endInitialChip, int endInitialField,
            int endFinalChip, int endFinalField);
        bool SaveGame();
        void SaveGameAs();
        void LoadGame(SavedGame savedGame);
        void StepBack();
        void StepForward();
        void MoveToStart();
        void MoveToEnd();
    }
}
