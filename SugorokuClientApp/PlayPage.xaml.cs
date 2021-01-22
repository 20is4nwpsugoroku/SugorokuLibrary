using System;
using System.Net;
using Newtonsoft.Json;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;
using SugorokuLibrary.ServerToClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SugorokuClientApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayPage
	{
		private readonly Player _player;
		private readonly PlayPageViewModel _viewModel;
		private bool _isZooming;

		public PlayPage(Player player)
		{
			InitializeComponent();

			_player = player;
			FieldImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.sugorokuField.png");
			PlayerKomaIcon.Source = ImageSource.FromResource($"SugorokuClientApp.ImageResource.koma_{player.PlayerID}.png");

			_viewModel = new PlayPageViewModel();
			UpdateNowPlayerText();
			PlayerGrid.BindingContext = _viewModel;
			Device.StartTimer(TimeSpan.FromSeconds(5), () =>
			{
				UpdateNowPlayerText();
				return true;
			});
		}

		private void UpdateNowPlayerText()
		{
			using var socket = ConnectServer.CreateSocket((IPAddress) Application.Current.Properties["serverIpAddress"],
				(int) Application.Current.Properties["serverPort"]);
			var requestMethod = new GetMatchInfoMessage(_player.MatchKey);
			var requestText = JsonConvert.SerializeObject(requestMethod);

			var (_, result, msg) = Connection.SendAndRecvMessage(requestText, socket, true);

			if (!result)
			{
				var failed = JsonConvert.DeserializeObject<FailedMessage>(msg);
				throw new Exception($"部屋が閉じられています: {failed.Message}");
			}

			var info = JsonConvert.DeserializeObject<MatchInfo>(msg);
			_viewModel.NowPlayer = info.NextPlayerID == _player.PlayerID ? "あなたのターン" : $"{info.NextPlayerID}Pのターン";
			_viewModel.IsMyTurn = info.NextPlayerID == _player.PlayerID;
		}

		private async void FieldImageZoomButtonClicked(object sender, EventArgs e)
		{
			var scale = _isZooming ? -1.0 : 1.0;
			_isZooming = !_isZooming;
			// TODO get position
			FieldLayout.AnchorX = PlayerKomaIcon.X + PlayerKomaIcon.Width / 2;
			FieldLayout.AnchorY = PlayerKomaIcon.Y + PlayerKomaIcon.Height / 2;
			await FieldLayout.RelScaleTo(scale, 1000);
		}
	}
}