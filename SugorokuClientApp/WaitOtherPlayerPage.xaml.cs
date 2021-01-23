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
    public partial class WaitOtherPlayerPage
    {
        private readonly Player _myPlayerInfo;
        private bool _hostPlayersPageMoved;

        public WaitOtherPlayerPage(Player myInfo)
        {
            InitializeComponent();

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

            PlayersView.ItemsSource = new ListQueue<Player> {myInfo};

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

            PlayersView.ItemsSource = _matchInfo.Players;
            return true;
        }

        private void GameStartButtonClicked(object sender, EventArgs e)
        {
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