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

namespace SugorokuClient.Util
{
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
			//r = info.Players.Count >= playerNum;
			// // //DX.putsDx("canstartMatch:" + r.ToString());
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
				
				// // //DX.putsDx(sendJson);
				// // //DX.putsDx("///////Close Result/////////////");
				// // //DX.putsDx(recvJson);
				// // //DX.putsDx("///////Close Result/////////////");
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);

			}
			// // //DX.putsDx("close match");
			// // //DX.putsDx("***********Closed************");

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
			// // //DX.putsDx(r.ToString() + sendJson);
			// // //DX.putsDx(r.ToString() + recvJson);
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
			// // //DX.putsDx(sendJson);
			////DX.putsDx(recvJson);
			return (r)
				? (true, recvJson)
				: (false, "");
		}


		//private static (bool, MatchCore) GetMatch(string matchKey)
		//{
		//	var sendMsg = new GetStartedMatchMessage(matchKey);
		//	var sendJson = JsonConvert.SerializeObject(sendMsg);
		//	var (r, recvJson) = SocketManager.SendRecv(sendJson);
		//	// // //DX.putsDx(sendJson);
		//	// // //DX.putsDx(recvJson);
		//	return (r)
		//		? (r, JsonConvert.DeserializeObject<MatchCore>(recvJson))
		//		: (r, null);
		//}

		public (bool, MatchInfo) GetMatch()
		{
			return GetMatch(MatchKey);
		}


		public (bool, MatchInfo) GetMatch(string matchKey)
		{
			var sendJson = JsonConvert.SerializeObject(new GetStartedMatchMessage(matchKey));
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			// // //DX.putsDx(sendJson);
			////DX.putsDx(recvJson);
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
			// // //DX.putsDx(sendJson);
			////DX.putsDx(recvJson);
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
			// // //DX.putsDx(sendJson);
			// // //DX.putsDx(recvJson);
			while (!r)
			{
				// // //DX.putsDx("///////Close Result/////////////");
				// // //DX.putsDx(sendJson);
				// // //DX.putsDx(recvJson);
				// // //DX.putsDx("---------------------");
				sendJson = JsonConvert.SerializeObject(sendMsg);
				(r, recvJson) = SocketManager.SendRecv(sendJson);

			}
			// // //DX.putsDx("fice Start");
			// // //DX.putsDx("***********Dice************");


			if (!r && recvJson == string.Empty) return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			var tempMsg = JsonConvert.DeserializeObject<ServerMessage>(recvJson);
			// // //DX.putsDx("Start Throw Dice");
			// // //DX.putsDx(recvJson);
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


		public List<SugorokuEvent> ReverseEvent(MatchInfo prev, MatchInfo now, int myPlayerID)
		{
			//DX.putsDx("Start Reverse Event");
			var eventList = new List<SugorokuEvent>();
			var alreadyReversePlayer = new List<int>();

			for (var i = 0; i < prev.Players.Count; i++)
			{
				int actionPlayer, nowPos, prevPos, dice;
				//DX.putsDx($"Reverse {prev.Players[i].PlayerID}");
				if (prev.Players[i].PlayerID == myPlayerID) continue;

				for (var j = 0; j < now.Players.Count; j++)
				{
					if (now.Players[j].PlayerID == prev.Players[i].PlayerID
								&& now.Players[j].Position != prev.Players[i].Position)
					{
						actionPlayer = prev.Players[i].PlayerID;
						//if (alreadyReversePlayer.Contains(actionPlayer)) continue;
						nowPos = now.Players[j].Position;
						prevPos = prev.Players[i].Position;
						dice = nowPos - prevPos;
						//alreadyReversePlayer.Add(actionPlayer);
						var eve = new SugorokuEvent(prevPos, nowPos, actionPlayer, dice);
						eventList.Add(eve);
						//DX.putsDx(JsonConvert.SerializeObject(eve));
						//DX.putsDx($"End Reverse {prev.Players[i].PlayerID}");
						break;
					}
				}
			}
			return eventList;
		}
	}
}
