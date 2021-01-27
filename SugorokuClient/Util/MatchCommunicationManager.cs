﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		public MatchInfo LastMatchInfo { get; set; }
		public bool LastGetSuccess { get; set; }



		public MatchCommunicationManager()
		{
			MyPlayerId = 0;
			MatchKey = string.Empty;
		}


		public MatchCommunicationManager(int myPlayerId, string matchKey)
		{
			MyPlayerId = myPlayerId;
			MatchKey = matchKey;
			LastGetSuccess = false;
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
			//r = info.Players.Count >= playerNum;
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
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			while (!r)
			{
				
				DX.putsDx(sendJson);
				DX.putsDx("///////Close Result/////////////");
				DX.putsDx(recvJson);
				DX.putsDx("///////Close Result/////////////");
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);

			}
			DX.putsDx("close match");
			DX.putsDx("***********Closed************");

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
			DX.putsDx(r.ToString() + sendJson);
			DX.putsDx(r.ToString() + recvJson);
			var ret = new MatchInfo();
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
			DX.putsDx(sendJson);
			DX.putsDx(recvJson);
			while (!r)
			{
				DX.putsDx("///////Close Result/////////////");
				DX.putsDx(sendJson);
				DX.putsDx(recvJson);
				DX.putsDx("---------------------");
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);

			}
			DX.putsDx("fice Start");
			DX.putsDx("***********Dice************");


			if (!r && recvJson == string.Empty) return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			var tempMsg = JsonConvert.DeserializeObject<ServerMessage>(recvJson);
			DX.putsDx("Start Throw Dice");
			DX.putsDx(recvJson);
			switch (tempMsg.MethodType)
			{
				case "diceResult":
					var diceResult = JsonConvert.DeserializeObject<DiceResultMessage>(recvJson);
					var dice = diceResult.Dice;
					var startPos = diceResult.FirstPosition;
					var finishPos = diceResult.FinalPosition;
					return (startPos < 31)
						? (ReflectionStatus.NextSuccess, dice, startPos, finishPos, ranking)
						: (ReflectionStatus.PrevDiceSuccess, -dice, startPos, finishPos, ranking);
				//? (ReflectionStatus.NextSuccess, dice, startPos, finishPos, ranking)
				//: (ReflectionStatus.PrevDiceSuccess,- dice, finishPos + dice, finishPos, ranking);

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


		public (bool, IEnumerable<int>) GetRanking(string matchKey)
		{
			var sendMsg = new GetRankingMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<RankingMessage>(recvJson).Ranking)
				: (r, new List<int>());
		}
	}
}
