using System;
using System.Net;
using Newtonsoft.Json;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;
using SugorokuLibrary.ServerToClient;
using Xamarin.Forms;

namespace SugorokuClientApp
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LogoImage.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.logo.png");
            StartButton.Source = ImageSource.FromResource("SugorokuClientApp.ImageResource.startButton.png");
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            var serverIpAddress = ServerIpAddress.Text;
            var serverPort = ServerPort.Text;
            var playerName = PlayerNameEditor.Text;
            var roomName = RoomNameEditor.Text;
            if (string.IsNullOrEmpty(serverIpAddress) || string.IsNullOrEmpty(serverPort) ||
                string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(roomName))
            {
                await DisplayAlert("エラー", "サーバーのIPアドレス、ポート番号、プレイヤー名、部屋名のいずれかが不足しています", "OK");
                return;
            }

            if (!IPAddress.TryParse(serverIpAddress, out var serverIp) || !int.TryParse(serverPort, out var port))
            {
                await DisplayAlert("エラー", "IPアドレス、ポート番号の形式が正しくありません", "OK");
                return;
            }

            Application.Current.Properties["serverIpAddress"] = serverIp;
            Application.Current.Properties["serverPort"] = port;

            Player playerData = null;
            try
            {
                using var socket = ConnectServer.CreateSocket(serverIp, port);
                var createPlayerMessage = new CreatePlayerMessage(playerName, roomName);
                var msg = JsonConvert.SerializeObject(createPlayerMessage);
                var (_, result, recvMsg) = Connection.SendAndRecvMessage(msg, socket, true);

                if (!result)
                {
                    var failMessage = JsonConvert.DeserializeObject<FailedMessage>(recvMsg);
                    await DisplayAlert("部屋の作成に失敗", failMessage.Message, "OK");
                    return;
                }

                playerData = JsonConvert.DeserializeObject<Player>(recvMsg);
                // Application.Current.Properties["playerData"] = playerData;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await DisplayAlert("エラー", "ソケットのConnectで例外が発生しました。やり直してください。", "OK");
            }
            finally
            {
                if (playerData != null)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushAsync(new WaitOtherPlayerPage(playerData));
                    });
                }
            }
        }
    }
}