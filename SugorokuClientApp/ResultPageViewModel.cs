using System.ComponentModel;
using Xamarin.Forms;

namespace SugorokuClientApp
{
    public class ResultPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _playerName;

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerName)));
            }
        }

        private string _rank;

        public string RankText
        {
            get => _rank;
            set
            {
                _rank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RankText)));
            }
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSource)));
            }
        }
    }
}