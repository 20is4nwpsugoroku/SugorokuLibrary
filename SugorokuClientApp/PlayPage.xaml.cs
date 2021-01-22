using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;
using SugorokuLibrary.ServerToClient;
using Xamarin.Essentials;
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

		private static IReadOnlyDictionary<int, (double, double)> PlayerPositionToConstant =>
			new Dictionary<int, (double, double)>
			{
				{0, (9, 10)}, {1, (13, 10)}, {2, (17, 10)}, {3, (21, 10)}, {4, (25, 13)}, {5, (29, 13)}, {6, (33, 13)}
			};

		public PlayPage(Player player)
		{
			InitializeComponent();

			_player = player;
			FieldImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.sugorokuField.png");
			PlayerKomaIcon.Source = ImageSource.FromResource($"SugorokuClientApp.ImageResource.koma_{player.PlayerID}.png");

			_viewModel = new PlayPageViewModel();
			UpdateNowPlayerText();
			PlayerGrid.BindingContext = _viewModel;
			Device.StartTimer(TimeSpan.FromMinutes(1), () =>
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
			var (x, y) = PlayerPositionToConstant[_player.Position];
			_viewModel.PlayerXPosition = x;
			_viewModel.PlayerYPosition = y;
		}

		private async void FieldImageZoomButtonClicked(object sender, EventArgs e)
		{
			SizeChangeButton.IsEnabled = false;
			var scale = _isZooming ? -1.0 : 1.0;
			_isZooming = !_isZooming;
			FieldLayout.AnchorX = PlayerKomaIcon.X / FieldLayout.Width;
			FieldLayout.AnchorY = PlayerKomaIcon.Y / FieldLayout.Height;
			await FieldLayout.RelScaleTo(scale, 1000);
			SizeChangeButton.IsEnabled = true;
		}
	}
}