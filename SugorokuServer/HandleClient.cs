using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;

namespace SugorokuServer
{
	public class HandleClient
	{
		private const int RecvBufSize = 1024;
		private readonly Dictionary<string, MatchInfo> _matches = new Dictionary<string, MatchInfo>();
		private int _playerCount;

		private readonly JsonSerializerOptions _options = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			PropertyNameCaseInsensitive = true
		};

		/// <summary>
		/// クライアント側ソケットからのSendを受信し受け取ったテキストのbodyを返す
		/// </summary>
		/// <param name="clientSocket">クライアント側ソケット</param>
		/// <returns>受信したメッセージのbody</returns>
		public static string ReceiveMessage(Socket clientSocket)
		{
			var buf = new byte[RecvBufSize];
			var recvSize = clientSocket.Receive(buf, RecvBufSize, SocketFlags.None);
			var msg = Encoding.UTF8.GetString(buf);
			var msgSize = GetBufSize(msg);

			while (recvSize >= msgSize)
			{
				buf = new byte[RecvBufSize];
				recvSize += clientSocket.Receive(buf);
				msg += Encoding.UTF8.GetString(buf);
			}

			return RemoveHeader(msg);
		}

		public static void SendMessage(Socket clientSocket, string message)
		{
			var sentAllBytes = clientSocket.Send(Encoding.UTF8.GetBytes(message));
			while (sentAllBytes >= message.Length)
			{
				sentAllBytes +=
					clientSocket.Send(
						Encoding.UTF8.GetBytes(string.Concat(Encoding.UTF8.GetBytes(message[sentAllBytes..]))));
			}
		}

		public string MakeSendMessage(string receivedMessage)
		{
			var message = JsonSerializer.Deserialize<IClientMessage>(receivedMessage);
			var (methodSuccess, sendMessage) = message!.MethodType switch
			{
				"createPlayer" => CreatePlayer((CreatePlayerMessage) message), // playerを作成する
				"closeCreate" => CloseCreate((CloseCreateMessage) message), // ユーザー作成メソッドを作る
				_ => throw new NotImplementedException() // default armの動作は未定
			};

			return MakeHeader(sendMessage, methodSuccess);
		}

		private Player CreateMatch(CreatePlayerMessage message)
		{
			var hostPlayerData = new Player
			{
				IsHost = true,
				PlayerID = ++_playerCount,
				PlayerName = message.PlayerName,
				Position = 0,
				MatchKey = message.MatchKey
			};

			var match = new MatchInfo
			{
				HostPlayerID = _playerCount,
				PlayerIDs = new List<int> {_playerCount},
				MatchKey = message.MatchKey,
			};
			_matches.Add(message.MatchKey, match);

			return hostPlayerData;
		}

		private (bool, string) CreatePlayer(CreatePlayerMessage message)
		{
			// 送られたFieldKeyのフィールドがまだ存在しないとき (送信元Playerがホストになる)
			if (!_matches.ContainsKey(message.MatchKey))
				return (true, JsonSerializer.Serialize(CreateMatch(message), _options));

			// 以下、送られたFieldKeyのフィールドがすでに存在するとき
			// フィールドのユーザ新規作成が終了してる（ゲームが始まってる）とき、エラーを返す
			if (_matches[message.MatchKey].CreatePlayerClosed)
				return (false, JsonSerializer.Serialize(new Dictionary<string, string>
				{
					{"message", "This field's create player is already closed"}
				}));

			var playerData = new Player
			{
				MatchKey = message.MatchKey,
				IsHost = false,
				PlayerID = ++_playerCount,
				PlayerName = message.PlayerName,
				Position = 0
			};

			_matches[message.MatchKey].PlayerIDs.Add(playerData.PlayerID);
			return (true, JsonSerializer.Serialize(playerData, _options));
		}

		private (bool, string) CloseCreate(CloseCreateMessage message)
		{
			if (!_matches.ContainsKey(message.FieldKey))
			{
				return (false, JsonSerializer.Serialize(new Dictionary<string, string>
				{
					{"message", "This key's field is not created"}
				}));
			}

			if (_matches[message.FieldKey].CreatePlayerClosed)
			{
				return (false, JsonSerializer.Serialize(new Dictionary<string, string>
				{
					{"message", "This key's field is already closed"}
				}, _options));
			}

			_matches[message.FieldKey].CreatePlayerClosed = true;
			return (true, JsonSerializer.Serialize(_matches[message.FieldKey], _options));
		}

		/// <summary>
		/// bodyのサイズを送信バッファの先頭に付与する
		/// </summary>
		/// <param name="bodyMessage">送信内容</param>
		/// <param name="methodSuccess">了承か不正操作か</param>
		/// <returns></returns>
		private static string MakeHeader(string bodyMessage, bool methodSuccess)
		{
			return $"{bodyMessage.Length},{(methodSuccess ? "OK" : "FAIL")}\n{bodyMessage}";
		}

		private static string RemoveHeader(string allMessage)
		{
			return string.Concat(allMessage.Split('\n').Skip(1)).TrimEnd('\0');
		}

		/// <summary>
		/// 受信するバッファのサイズをヘッダから取得する
		/// サイズ = ヘッダに付与されているbodyのサイズ + ヘッダのサイズ + 1(改行コード)
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private static int GetBufSize(string msg)
		{
			var sizeStr = msg.Split("\n")[0];
			return int.Parse(sizeStr) + sizeStr.Length + 1;
		}
	}
}