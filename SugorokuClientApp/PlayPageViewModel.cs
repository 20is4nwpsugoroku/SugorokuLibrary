using System.ComponentModel;

namespace SugorokuClientApp
{
	public class PlayPageViewModel
	{
		public event PropertyChangedEventHandler PropertyChangedHandler;

		private string _nowPlayer;
		public string NowPlayer
		{
			get => _nowPlayer;
			set
			{
				if (_nowPlayer == value) return;
				_nowPlayer = value;
				PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs("NowPlayer"));
			}
		}

		private bool _isMyTurn;
		public bool IsMyTurn
		{
			get => _isMyTurn;
			set
			{
				if (_isMyTurn == value) return;
				_isMyTurn = value;
				PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs("IsMyTurn"));
			}
		}
	}
}