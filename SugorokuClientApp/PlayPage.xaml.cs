using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Match;
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
        private readonly MatchInfo _matchInfo;
        private readonly PlayPageViewModel _viewModel;
        private bool _isZooming;
        private bool _dicePlaying;
        private bool _isFinished;

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

        public PlayPage(Player player, MatchInfo matchInfo)
        {
            InitializeComponent();

            _player = player;
            _matchInfo = matchInfo;
            FieldImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.sugorokuField.png");
            PlayerKomaIcon.Source =
                ImageSource.FromResource($"SugorokuClientApp.ImageResource.koma_{player.PlayerID}.png");

            _viewModel = new PlayPageViewModel();
            UpdateNowPlayerText();
            PlayerGrid.BindingContext = _viewModel;
            Device.StartTimer(TimeSpan.FromSeconds(5), UpdateNowPlayerText);
        }

        private bool UpdateNowPlayerText()
        {
            if (_isFinished) return false;
            if (_dicePlaying) return true;

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
            if (info.NextPlayerID == Constants.FinishedPlayerID)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("終了", "他のプレイヤーがゴールしました。現在の位置で順位を決定します。", "OK");
                    using var rankSock = ConnectServer.CreateSocket(
                        (IPAddress) Application.Current.Properties["serverIpAddress"],
                        (int) Application.Current.Properties["serverPort"]);
                    var req = new GetRankingMessage(_player.MatchKey);
                    var reqText = JsonConvert.SerializeObject(req);
                    var (_, canCheck, rankMsg) = Connection.SendAndRecvMessage(reqText, rankSock, true);

                    if (!canCheck) throw new Exception("まだゴールしてない判定");
                    var rankMsgConvert = JsonConvert.DeserializeObject<RankingMessage>(rankMsg);
                    await Navigation.PushAsync(new ResultPage(rankMsgConvert.Ranking.ToList(),
                        _matchInfo.Players));
                });
                return false;
            }

            _viewModel.IsMyTurn = info.NextPlayerID == _player.PlayerID;
            _viewModel.NowPlayer = _viewModel.IsMyTurn ? "あなたのターン" : $"{info.NextPlayerID}Pのターン";
            return true;
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

        private async void DiceButtonClicked(object sender, EventArgs e)
        {
            _viewModel.IsMyTurn = false;
            _dicePlaying = true;
            using var socket = ConnectServer.CreateSocket((IPAddress) Application.Current.Properties["serverIpAddress"],
                (int) Application.Current.Properties["serverPort"]);
            var diceRequest = new DiceMessage(_player.MatchKey, _player.PlayerID);
            var requestMsg = JsonConvert.SerializeObject(diceRequest);

            var (_, _, response) = Connection.SendAndRecvMessage(requestMsg, socket, true);
            var serverMessage = JsonConvert.DeserializeObject<ServerMessage>(response);

            var task = serverMessage switch
            {
                DiceResultMessage dr => DiceEvent(dr),
                FailedMessage f => FailedAction(f),
                AlreadyFinishedMessage af => AlreadyFinishedAction(af),
                _ => throw new ArgumentException()
            };
            await task;
            _dicePlaying = false;
        }

        private async Task DiceEvent(DiceResultMessage diceResult)
        {
            var diceImg = new Image
            {
                Source = ImageSource.FromFile($"Resources/drawable/saikoro_{diceResult.Dice}.png"),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            StatusLayout.Children.RemoveAt(0);
            StatusLayout.Children.Add(diceImg);
            await Task.Delay(2000);

            var (firstX, firstY) = PlayerPositionToConstant[Math.Min(diceResult.FirstPosition, 30)];
            var firstXConstraint = Constraint.RelativeToView(FieldImage, (layout, view) => view.Width * firstX);
            var firstYConstraint = Constraint.RelativeToView(FieldImage, (layout, view) => view.Height * firstY);
            RelativeLayout.SetXConstraint(PlayerKomaIcon, firstXConstraint);
            RelativeLayout.SetYConstraint(PlayerKomaIcon, firstYConstraint);

            if (diceResult.FirstPosition >= 30)
            {
                _isFinished = true;
                await DisplayAlert("ゴール", "ゴールおめでとう！リザルトへ移動します", "OK");
                Device.BeginInvokeOnMainThread(async () =>
                    await Navigation.PushAsync(new ResultPage(diceResult.Ranking!.ToArray(),
                        _matchInfo.Players)));
                return;
            }

            if (!string.IsNullOrEmpty(diceResult.Message))
            {
                await DisplayAlert("イベントマス", diceResult.Message, "OK");
            }

            var (finalX, finalY) = PlayerPositionToConstant[diceResult.FinalPosition];
            var finalXConstraint = Constraint.RelativeToView(FieldImage, (layout, view) => view.Width * finalX);
            var finalYConstraint = Constraint.RelativeToView(FieldImage, (layout, view) => view.Height * finalY);
            RelativeLayout.SetXConstraint(PlayerKomaIcon, finalXConstraint);
            RelativeLayout.SetYConstraint(PlayerKomaIcon, finalYConstraint);

            _viewModel.NowPlayer = "現在のプレイヤーを確認中…";
            var statusLabel = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            statusLabel.SetBinding(Label.TextProperty, nameof(PlayPageViewModel.NowPlayer));
            StatusLayout.Children.RemoveAt(0);
            StatusLayout.Children.Add(statusLabel);

            DiceButton.BindingContext = _viewModel;
            DiceButton.SetBinding(IsEnabledProperty, nameof(PlayPageViewModel.IsMyTurn));
        }

        private async Task FailedAction(FailedMessage failed)
        {
            await Console.Error.WriteLineAsync($"Error: {failed.Message}");
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("ダイスエラー", "まだあなたのターンではありませんでした。", "OK"));
        }

        private async Task AlreadyFinishedAction(AlreadyFinishedMessage message)
        {
            await DisplayAlert("他のプレイヤーがゴール済み", "他のプレイヤーがゴールしました。リザルト画面へ移動します", "OK");

            Device.BeginInvokeOnMainThread(async () =>
                await Navigation.PushAsync(new ResultPage(message.Ranking!.ToArray(),
                    _matchInfo.Players)));
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}