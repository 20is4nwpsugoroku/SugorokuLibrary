using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.ServerToClient;
using SugorokuLibrary.Match;
using SugorokuClient.Scene;
using System.Text.Json;

namespace SugorokuClient.Util
{
	/// <summary>
	/// サーバーと通信を行うクラス
	/// </summary>
	public class MatchCommunicationManager
	{
		public int MyPlayerId { get; private set; }
		public string MatchKey { get; private set; }
		public MatchInfo LastMatchInfo { get; set; }


		public MatchCommunicationManager()
		{
			MyPlayerId = 0;
			MatchKey = string.Empty;
		}


		public MatchCommunicationManager(int myPlayerId, string matchKey)
		{
			MyPlayerId = myPlayerId;
			MatchKey = matchKey;
			LastMatchInfo = new MatchInfo();
		}


		public bool IsMatchStarted()
		{
			return IsMatchStarted(MatchKey);
		}

		public bool IsMatchStarted(string matchKey)
		{
			var getInfo = new GetStartedMatchMessage(matchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (r, _) = SocketManager.SendRecv(json);
			return r;
		}


		public bool CanStartMatch(int playerNum)
		{
			return CanStartMatch(MatchKey, playerNum);
		}


		public bool CanStartMatch(string matchKey, int playerNum)
		{
			var (r, info) = GetMatch(matchKey);
			if (r)
			{
				r = info.Players.Count >= playerNum;
			}
			return r;
		}


		public void CloseJoinMatch()
		{
			CloseJoinMatch(MatchKey);
		}


		public void CloseJoinMatch(string matchKey)
		{
			var sendMsg = new CloseCreateMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			while (!r)
			{
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);

			}
		}


		public (bool, MatchInfo) GetMatchInfo()
		{
			return GetMatchInfo(MatchKey);
		}


		public (bool, MatchInfo) GetMatchInfo(string matchKey)
		{
			var sendMsg = new GetMatchInfoMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (true, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (false, new MatchInfo());
		}


		public (bool, string) GetMatchInfoString()
		{
			return GetMatchInfoString(MatchKey);
		}


		public (bool, string) GetMatchInfoString(string matchKey)
		{
			var sendMsg = new GetMatchInfoMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (true, recvJson)
				: (false, "");
		}


		public (bool, MatchInfo) GetMatch()
		{
			return GetMatch(MatchKey);
		}


		public (bool, MatchInfo) GetMatch(string matchKey)
		{
			var sendJson = JsonConvert.SerializeObject(new GetStartedMatchMessage(matchKey));
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (r, null);
		}


		public (bool, string) GetMatchString()
		{
			return GetMatchString(MatchKey);
		}


		public (bool, string) GetMatchString(string matchKey)
		{
			var sendJson = JsonConvert.SerializeObject(new GetStartedMatchMessage(matchKey));
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			// // //////DX.putsDx(sendJson);
			////////DX.putsDx(recvJson);
			return (r)
				? (r, recvJson)
				: (r, "");
		}


		public (ReflectionStatus, int, int, int, IEnumerable<int>) ThrowDice()
		{
			return ThrowDice(MatchKey, MyPlayerId);
		}


		public (ReflectionStatus, int, int, int, IEnumerable<int>) ThrowDice(string matchKey, int playerId)
		{
			var ranking = new List<int>();
			var sendMsg = new DiceMessage(matchKey, playerId);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			while (!r)
			{
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);
			}
			if (!r && recvJson == string.Empty) return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			var tempMsg = JsonConvert.DeserializeObject<ServerMessage>(recvJson);
			switch (tempMsg.MethodType)
			{
				case "diceResult":
					var diceResult = JsonConvert.DeserializeObject<DiceResultMessage>(recvJson);
					var dice = diceResult.Dice;
					var startPos = diceResult.FirstPosition;
					var finishPos = diceResult.FinalPosition;
					return (startPos < 31)
						? (ReflectionStatus.NextSuccess, dice, startPos, finishPos, ranking)
						: (ReflectionStatus.PrevDiceSuccess, dice, startPos, finishPos, ranking);
				case "alreadyFinished":
					var alreadyFinished = JsonConvert.DeserializeObject<AlreadyFinishedMessage>(recvJson);
					var goalPlayerId = alreadyFinished.GoaledPlayerId;
					ranking = new List<int>(alreadyFinished.Ranking);
					return (ReflectionStatus.AlreadyFinished, goalPlayerId, -1, -1, ranking);

				case "failed":
				default:
					return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			}
		}


		public (bool, IEnumerable<int>) GetRanking()
		{
			return GetRanking(MatchKey);
		}


		public (bool, IEnumerable<int>) GetRanking(string matchKey)
		{
			var sendMsg = new GetRankingMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<RankingMessage>(recvJson).Ranking)
				: (r, new List<int>());
		}


		public List<PlayerMoveEvent> ReverseEvent(string prev, string now, int myPlayerID)
		{
			var eventList = new List<PlayerMoveEvent>();
			var Prev = JsonConvert.DeserializeObject<MatchInfo>(prev);
			var Now = JsonConvert.DeserializeObject<MatchInfo>(now);

			for (var i = 0; i < Prev.Players.Count; i++)
			{
				int actionPlayer, nowPos, prevPos, dice;
				for (var j = 0; j < Now.Players.Count; j++)
				{
					if (Now.Players[j].PlayerID == Prev.Players[i].PlayerID)
					{
						actionPlayer = Prev.Players[i].PlayerID;
						nowPos = Now.Players[j].Position;
						prevPos = Prev.Players[i].Position;
						dice = nowPos - prevPos;
						eventList.Add(new PlayerMoveEvent(prevPos, nowPos, actionPlayer, dice));
					}
				}
			}

			var array = eventList.ToArray();
			var resultEventList = new List<PlayerMoveEvent>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].PlayerId != myPlayerID)
				{
					resultEventList.Add(array[i]);
				}
			}
			return resultEventList;
		}
	}
}