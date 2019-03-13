using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FieldsAndChips
{
    public class SavedGame : INotifyPropertyChanged
    {
        private string gameName;
        private string gameDate;
        private int horizontalCells;
        private int verticalCells;
        private string startingPosition;
        private string moves;

        public int Id { get; set; }

        public string GameName
        {
            get { return gameName; }
            set
            {
                gameName = value;
                OnPropertyChanged("GameName");
            }
        }

        public string GameDate
        {
            get { return gameDate; }
            set
            {
                gameDate = value;
                OnPropertyChanged("GameDate");
            }
        }

        public int HorizontalCells
        {
            get { return horizontalCells; }
            set
            {
                horizontalCells = value;
                OnPropertyChanged("HorizontalCells");
            }
        }

        public int VerticalCells
        {
            get { return verticalCells; }
            set
            {
                verticalCells = value;
                OnPropertyChanged("VerticalCells");
            }
        }

        public string StartingPosition
        {
            get { return startingPosition; }
            set
            {
                startingPosition = value;
                OnPropertyChanged("StartingPosition");
            }
        }

        public string Moves
        {
            get { return moves; }
            set
            {
                moves = value;
                OnPropertyChanged("Moves");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}