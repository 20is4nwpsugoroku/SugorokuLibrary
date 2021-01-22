using System;
using System.Collections.Generic;
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

        private static IReadOnlyDictionary<int, (double, double)> PlayerPositionToConstant =>
            new Dictionary<int, (double, double)>
            {
                {0, (0.03, 0.22)}, {1, (0.12, 0.22)}, {2, (0.22, 0.22)}, {3, (0.31, 0.22)}, {4, (0.41, 0.22)},
                {5, (0.50, 0.22)}, {6, (0.59, 0.22)}, {7, (0.69, 0.22)}, {8, (0.77, 0.22)}, {9, (0.87, 0.25)},
                {10, (0.91, 0.34)}, {11, (0.87, 0.49)}, {12, (0.77, 0.49)}, {13, (0.69, 0.49)}, {14, (0.59, 0.49)},
                {15, (0.50, 0.49)}, {16, (0.41, 0.49)}, {17, (0.31, 0.49)}, {18, (0.22, 0.49)}, {19, (0.12, 0.49)},
                {20, (0.04, 0.50)}, {21, (0.00, 0.58)}, {22, (0.04, 0.70)}, {23, (0.12, 0.75)}, {24, (0.22, 0.75)},
                {25, (0.31, 0.75)}, {26, (0.41, 0.75)}, {27, (0.50, 0.75)}, {28, (0.59, 0.75)}, {29, (0.69, 0.75)},
                {30, (0.80, 0.73)}
            };

        public PlayPage(Player player)
        {
            InitializeComponent();

            _player = player;
            FieldImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.sugorokuField.png");
            PlayerKomaIcon.Source =
                ImageSource.FromResource($"SugorokuClientApp.ImageResource.koma_{player.PlayerID}.png");

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