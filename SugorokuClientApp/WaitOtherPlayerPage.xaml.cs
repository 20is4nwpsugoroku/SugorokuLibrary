using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class WaitOtherPlayerPage
    {
        private readonly Player _myPlayerInfo;
        private MatchInfo _matchInfo;
        private bool _hostPlayersPageMoved;

        private readonly List<ImageSource> _playerImages = new List<ImageSource>
        {
            ImageSource.FromResource("SugorokuClientApp.ImageResource.koma_1.png"),
            ImageSource.FromResource("SugorokuClientApp.ImageResource.koma_2.png"),
            ImageSource.FromResource("SugorokuClientApp.ImageResource.koma_3.png"),
            ImageSource.FromResource("SugorokuClientApp.ImageResource.koma_4.png")
        };

        public WaitOtherPlayerPage(Player myInfo)
        {
            InitializeComponent();

            GameStartButton.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.startButton.png");

            _myPlayerInfo = myInfo;
            if (myInfo.IsHost)
            {
                DisplayAlert("ホストになりました", "あなたは部屋のホストになりました。プレイヤーが集まったら「ゲーム開始」をタップしてください", "OK");
                GameStartButton.IsEnabled = true;
            }
            else
            {
                DisplayAlert("あなたは参加者です", $"あなたは部屋{myInfo.MatchKey}に参加しました。ホストの開始までしばらくお待ちください。", "OK");
                GameStartButton.IsEnabled = false;
            }

            PlayerInfoUpdate();

            Device.StartTimer(TimeSpan.FromSeconds(5), PlayerInfoUpdate);
        }

        private bool PlayerInfoUpdate()
        {
            using var socket = ConnectServer.CreateSocket((IPAddress) Application.Current.Properties["serverIpAddress"],
                (int) Application.Current.Properties["serverPort"]);
            var requestMethod = new GetMatchInfoMessage(_myPlayerInfo.MatchKey);
            var requestText = JsonConvert.SerializeObject(requestMethod);
            var (_, result, msg) = Connection.SendAndRecvMessage(requestText, socket, true);

            if (!result)
            {
                var failed = JsonConvert.DeserializeObject<FailedMessage>(msg);
                throw new Exception($"MatchKey Error: {failed.Message}");
            }

            if (_hostPlayersPageMoved) return false;

            _matchInfo = JsonConvert.DeserializeObject<MatchInfo>(msg);
            if (_matchInfo.CreatePlayerClosed)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await Navigation.PushAsync(new PlayPage(_myPlayerInfo, _matchInfo)));
                return false;
            }

            var viewModels = _matchInfo.Players.Select(p => new WaitOtherPlayerViewModel
            {
                IdInfo = $"{(p.PlayerID == _myPlayerInfo.PlayerID ? "自分 " : "")}ID: {p.PlayerID}",
                PlayerName = p.PlayerName,
                ImageSource = _playerImages[p.PlayerID - 1]
            });

            PlayersView.ItemsSource = viewModels;
            return true;
        }

        private void GameStartButtonClicked(object sender, EventArgs e)
        {
            GameStartButton.IsEnabled = false;
            using var socket = ConnectServer.CreateSocket((IPAddress) Application.Current.Properties["serverIpAddress"],
                (int) Application.Current.Properties["serverPort"]);
            var requestMethod = new CloseCreateMessage(_myPlayerInfo.MatchKey);
            var requestText = JsonConvert.SerializeObject(requestMethod);
            var (_, result, msg) = Connection.SendAndRecvMessage(requestText, socket, true);

            if (!result)
            {
                var failed = JsonConvert.DeserializeObject<FailedMessage>(msg);
                throw new Exception($"CloseCreate Error: {failed.Message}");
            }

            _hostPlayersPageMoved = true;
            Device.BeginInvokeOnMainThread(async () =>
                await Navigation.PushAsync(new PlayPage(_myPlayerInfo, _matchInfo)));
        }
    }
}