using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Match;
using SugorokuLibrary.Protocol;
using SugorokuLibrary.ServerToClient;

namespace SugorokuServer
{
	public class HandleClient
	{
		private readonly Dictionary<string, MatchInfo> _matches = new Dictionary<string, MatchInfo>();
		private readonly Dictionary<string, MatchCore> _startedMatch = new Dictionary<string, MatchCore>();

		private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

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
				GetStartedMatchMessage gsm => GetStatedMatch(gsm),
				_ => throw new NotImplementedException()
			};

			return HeaderProtocol.MakeHeader(sendMessage, methodSuccess);
		}

		private (bool, string) ThrowDice(DiceMessage diceMessage)
		{
			var matchInfo = _startedMatch[diceMessage.MatchKey];
			if (matchInfo.ActionSchedule.Peek() != diceMessage.PlayerId)
			{
				return (false, JsonConvert.SerializeObject(new FailedMessage("まだあなたのターンではありません")));
			}

			var dice = Dice();
			var action = new PlayerAction
			{
				PlayerID = diceMessage.PlayerId,
				Length = dice
			};
			matchInfo.ReflectAction(action);
			_startedMatch[diceMessage.MatchKey] = matchInfo;

			var pos = matchInfo.Players[diceMessage.PlayerId].Position;
			// return (true, $"{dice} {pos} {Field.Squares[pos].Event}");
			return (true,
				JsonConvert.SerializeObject(new DiceResultMessage(dice, Field.Squares[pos].Event.ToString()!, pos)));
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
				PlayerID = 1,
				PlayerName = message.PlayerName,
				Position = 0,
				MatchKey = message.MatchKey
			};

			var match = new MatchInfo
			{
				HostPlayerID = 1,
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
				return (false,
					JsonConvert.SerializeObject(new FailedMessage("すでにゲームが開始しているか、プレイヤーが定員の4名になったため参加できませんでした")));

			var playerData = new Player
			{
				MatchKey = message.MatchKey,
				IsHost = false,
				PlayerID = _matches[message.MatchKey].Players.Count + 1,
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
				return (false, JsonConvert.SerializeObject(new FailedMessage("This key's field is not created")));
			}

			if (_matches[message.MatchKey].CreatePlayerClosed)
			{
				return (false, JsonConvert.SerializeObject(new FailedMessage("This key's field is already closed")));
			}

			var match = _matches[message.MatchKey];

			match.CreatePlayerClosed = true;
			match.StartAtUnixTime = DateTime.Now.ToTimeStamp();
			match.Turn = 0;
			_startedMatch.Add(message.MatchKey, new MatchCore(match));
			_startedMatch[message.MatchKey].Start();
			return (true, JsonConvert.SerializeObject(_matches[message.MatchKey], _settings));
		}

		private (bool, string) GetMatchInfo(GetMatchInfoMessage message)
		{
			return _matches.ContainsKey(message.MatchKey)
				? (true, JsonConvert.SerializeObject(_matches[message.MatchKey], _settings))
				: (false, JsonConvert.SerializeObject(new FailedMessage("This match key's match is not created")));
		}

		private (bool, string) GetAllMatches()
		{
			return (true, JsonConvert.SerializeObject(_matches, _settings));
		}

		private (bool, string) GetStatedMatch(GetStartedMatchMessage message)
		{
			return _startedMatch.ContainsKey(message.MatchKey)
				? (true, JsonConvert.SerializeObject(_startedMatch[message.MatchKey]))
				: (false, JsonConvert.SerializeObject(new FailedMessage("This match key's match is not started")));
		}
	}
}