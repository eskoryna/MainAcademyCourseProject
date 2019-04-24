using System;

namespace FieldsAndChips
{
    public interface IGameFront
    {
        void ClearBoard();
        void SetBoard();
        void SetInitialPosition();
        void SetDefaultConfiguration();
        void ChangeBoardCell(int x, int y);
        void ShowGameState();
        void NotifyHistoryLoadFailed();
    }
}
