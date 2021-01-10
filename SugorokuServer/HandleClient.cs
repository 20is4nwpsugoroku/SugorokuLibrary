using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Match;
using SugorokuLibrary.Protocol;

namespace SugorokuServer
{
	public class HandleClient
	{
		private const int RecvBufSize = 1024;
		private readonly Dictionary<string, MatchInfo> _matches = new Dictionary<string, MatchInfo>();
		private readonly Dictionary<string, MatchCore> _startedMatch = new Dictionary<string, MatchCore>();
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
				sentAllBytes += clientSocket.Send(Encoding.UTF8.GetBytes(message[sentAllBytes..]));
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
				GetAllMatchesMessage _ => GetAllMatches(),
				DiceMessage dm => ThrowDice(dm),
				_ => throw new NotImplementedException()
			};

			return HeaderProtocol.MakeHeader(sendMessage, methodSuccess);
		}

		private (bool, string) ThrowDice(DiceMessage diceMessage)
		{
			var matchInfo = _startedMatch[diceMessage.MatchKey];
			if (matchInfo.ActionSchedule.Peek() != diceMessage.PlayerId)
			{
				return (false, "まだあなたのターンではありません");
			}

			var dice = Dice();
			var action = new PlayerAction
			{
				PlayerID = diceMessage.PlayerId,
				Length = dice
			};
			matchInfo.ReflectAction(action);
			_startedMatch[diceMessage.MatchKey] = matchInfo;

			return (true, $"{dice}");
		}

		private static int Dice()
		{
			var random = new Random();
			return random.Next(1, 7);
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
				Players = new List<Player> {hostPlayerData},
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

			_matches[message.MatchKey].Players.Add(playerData);
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

			var match = _matches[message.MatchKey];

			match.CreatePlayerClosed = true;
			match.StartAtUnixTime = DateTime.Now.ToTimeStamp();
			match.Turn = 0;
			_startedMatch.Add(message.MatchKey, new MatchCore(match));
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

		private (bool, string) GetAllMatches()
		{
			return (true, JsonConvert.SerializeObject(_matches, _settings));
		}
	}
}