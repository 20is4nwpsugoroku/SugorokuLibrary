using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.ServerToClient;
using SugorokuLibrary.Match;

namespace SugorokuClient.Util
{
	public class MatchCommunicationManager
	{
		public int MyPlayerId { get; private set; }
		public string MatchKey { get; private set; }


		public MatchCommunicationManager()
		{
			MyPlayerId = 0;
			MatchKey = string.Empty;
		}


		public MatchCommunicationManager(int myPlayerId, string matchKey)
		{
			MyPlayerId = myPlayerId;
			MatchKey = matchKey;
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
			var (r, info) = GetMatchInfo(matchKey);
			if (r)
			{
				r = info.Players.Count >= playerNum;
			}
			DX.putsDx("canstartMatch:" + r.ToString());
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
			var (_, _) = SocketManager.SendRecv(sendJson);
			DX.putsDx(sendJson);
			DX.putsDx("close match");
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
			DX.putsDx(sendJson);
			DX.putsDx(recvJson);
			var ret = new MatchInfo();
			return (r)
				? (true, ret = JsonConvert.DeserializeObject<MatchInfo>(recvJson))
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
			DX.putsDx(sendJson);
			DX.putsDx(recvJson);
			return (r)
				? (true, recvJson)
				: (false, "");
		}


		//private static (bool, MatchCore) GetMatch(string matchKey)
		//{
		//	var sendMsg = new GetStartedMatchMessage(matchKey);
		//	var sendJson = JsonConvert.SerializeObject(sendMsg);
		//	var (r, recvJson) = SocketManager.SendRecv(sendJson);
		//	DX.putsDx(sendJson);
		//	DX.putsDx(recvJson);
		//	return (r)
		//		? (r, JsonConvert.DeserializeObject<MatchCore>(recvJson))
		//		: (r, null);
		//}

		public (bool, MatchInfo) GetMatch()
		{
			return GetMatchInfo(MatchKey);
		}



		public (bool, MatchInfo) GetMatch(string matchKey)
		{
			var sendMsg = new GetStartedMatchMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			DX.putsDx(sendJson);
			DX.putsDx(recvJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (r, null);
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
			if (!r && recvJson == string.Empty) return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			var tempMsg = JsonConvert.DeserializeObject<ServerMessage>(recvJson);
			DX.putsDx("Start Throw Dice");

			switch (tempMsg.MethodType)
			{
				case "diceResult":
					var diceResult = JsonConvert.DeserializeObject<DiceResultMessage>(recvJson);
					var dice = diceResult.Dice;
					var startPos = diceResult.FirstPosition;
					var finishPos = diceResult.FinalPosition;
					return (diceResult.Message != "")
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
	}
}
