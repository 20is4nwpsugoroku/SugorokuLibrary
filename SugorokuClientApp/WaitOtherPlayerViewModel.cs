using System.ComponentModel;
using Xamarin.Forms;

namespace SugorokuClientApp
{
    public class WaitOtherPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private string _idInfo;

        public string IdInfo
        {
            get => _idInfo;
            set
            {
                _idInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IdInfo)));
            }
        }
    }
}