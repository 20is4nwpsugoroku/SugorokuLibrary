using System.ComponentModel;
using Xamarin.Forms;

namespace SugorokuClientApp
{
    public class PlayPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _nowPlayer;

        public string NowPlayer
        {
            get => _nowPlayer;
            set
            {
                if (_nowPlayer == value) return;
                _nowPlayer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NowPlayer)));
            }
        }

        private bool _isMyTurn;

        public bool IsMyTurn
        {
            get => _isMyTurn;
            set
            {
                _isMyTurn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMyTurn)));
            }
        }

        private ImageSource _diceView;

        public ImageSource DiceView
        {
            get => _diceView;
            set
            {
                _diceView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DiceView)));
            }
        }
    }
}