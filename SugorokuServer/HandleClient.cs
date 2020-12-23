using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuServer
{
	public class HandleClient
	{
		private const int RecvBufSize = 1024;
		private readonly Dictionary<string, MatchInfo> _matches = new Dictionary<string, MatchInfo>();
		private int _playerCount;

		private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
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
			var (msgSize, _, body) = HeaderProtocol.ParseHeader(msg);

			while (recvSize >= msgSize)
			{
				buf = new byte[RecvBufSize];
				recvSize += clientSocket.Receive(buf);
				msg += Encoding.UTF8.GetString(buf);
			}

			return body;
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
			var message = JsonConvert.DeserializeObject<ClientMessage>(receivedMessage);
			var (methodSuccess, sendMessage) = message switch
			{
				CreatePlayerMessage cr => CreatePlayer(cr),
				CloseCreateMessage cl => CloseCreate(cl),
				GetMatchInfoMessage gm => GetMatchInfo(gm),
				_ => throw new NotImplementedException()
			};

			return HeaderProtocol.MakeHeader(sendMessage, methodSuccess);
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
				return (true, JsonConvert.SerializeObject(CreateMatch(message), _settings));

			// 以下、送られたFieldKeyのフィールドがすでに存在するとき
			// フィールドのユーザ新規作成が終了してる（ゲームが始まってる）とき、エラーを返す
			if (_matches[message.MatchKey].CreatePlayerClosed)
				return (false, JsonConvert.SerializeObject(new Dictionary<string, string>
				{
					{"message", "This field's create player is already closed"}
				}, _settings));

			var playerData = new Player
			{
				MatchKey = message.MatchKey,
				IsHost = false,
				PlayerID = ++_playerCount,
				PlayerName = message.PlayerName,
				Position = 0
			};

			_matches[message.MatchKey].PlayerIDs.Add(playerData.PlayerID);
			return (true, JsonConvert.SerializeObject(playerData, _settings));
		}

		private (bool, string) CloseCreate(CloseCreateMessage message)
		{
			if (!_matches.ContainsKey(message.MatchKey))
			{
				return (false, JsonConvert.SerializeObject(new Dictionary<string, string>
				{
					{"message", "This key's field is not created"}
				}, _settings));
			}

			if (_matches[message.MatchKey].CreatePlayerClosed)
			{
				return (false, JsonConvert.SerializeObject(new Dictionary<string, string>
				{
					{"message", "This key's field is already closed"}
				}, _settings));
			}

			_matches[message.MatchKey].CreatePlayerClosed = true;
			_matches[message.MatchKey].StartAtUnixTime = DateTime.Now.ToTimeStamp();
			_matches[message.MatchKey].Turn = 0;
			_matches[message.MatchKey].NextPlayerID = _matches[message.MatchKey].PlayerIDs[0];
			return (true, JsonConvert.SerializeObject(_matches[message.MatchKey], _settings));
		}

		private (bool, string) GetMatchInfo(GetMatchInfoMessage message)
		{
			if (!_matches.ContainsKey(message.MatchKey))
			{
				return (false, JsonConvert.SerializeObject(new Dictionary<string, string>
				{
					{"message", "This match key's match is not created"}
				}, _settings));
			}

			if (_matches[message.MatchKey].CreatePlayerClosed)
			{
				return (false, JsonConvert.SerializeObject(new Dictionary<string, string>
				{
					{"message", "This match is already closed"}
				}, _settings));
			}

			return (true, JsonConvert.SerializeObject(_matches[message.MatchKey], _settings));
		}
	}
}