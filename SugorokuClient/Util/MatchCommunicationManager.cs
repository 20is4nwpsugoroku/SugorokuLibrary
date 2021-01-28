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

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MatchCommunicationManager()
		{
			MyPlayerId = 0;
			MatchKey = string.Empty;
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="myPlayerId"></param>
		/// <param name="matchKey"></param>
		public MatchCommunicationManager(int myPlayerId, string matchKey)
		{
			MyPlayerId = myPlayerId;
			MatchKey = matchKey;
			LastMatchInfo = new MatchInfo();
		}


		/// <summary>
		/// 試合が開始しているかどうか
		/// </summary>
		/// <returns></returns>
		public bool IsMatchStarted()
		{
			return IsMatchStarted(MatchKey);
		}


		/// <summary>
		/// 試合が開始しているかどうか
		/// </summary>
		/// <param name="matchKey">試合の部屋名</param>
		/// <returns></returns>
		public bool IsMatchStarted(string matchKey)
		{
			var getInfo = new GetStartedMatchMessage(matchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (r, _) = SocketManager.SendRecv(json);
			return r;
		}


		/// <summary>
		/// 試合を開始できるかどうか
		/// </summary>
		/// <param name="playerNum">プレイヤーの人数</param>
		/// <returns></returns>
		public bool CanStartMatch(int playerNum)
		{
			return CanStartMatch(MatchKey, playerNum);
		}


		/// <summary>
		/// 試合を開始できるかどうか
		/// </summary>
		/// <param name="matchKey">部屋名</param>
		/// <param name="playerNum">プレイヤーの人数</param>
		/// <returns></returns>
		public bool CanStartMatch(string matchKey, int playerNum)
		{
			var (r, info) = GetMatch(matchKey);
			if (r)
			{
				r = info.Players.Count >= playerNum;
			}
			return r;
		}


		/// <summary>
		/// 試合への参加ができないようにする
		/// </summary>
		public void CloseJoinMatch()
		{
			CloseJoinMatch(MatchKey);
		}


		/// <summary>
		/// 試合への参加ができないようにする
		/// </summary>
		/// <param name="matchKey">部屋名</param>
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


		/// <summary>
		/// 試合情報の取得
		/// </summary>
		/// <returns>試合情報</returns>
		public (bool, MatchInfo) GetMatchInfo()
		{
			return GetMatchInfo(MatchKey);
		}


		/// <summary>
		/// 試合情報の取得
		/// </summary>
		/// <param name="matchKey">部屋名</param>
		/// <returns>試合情報</returns>
		public (bool, MatchInfo) GetMatchInfo(string matchKey)
		{
			var sendMsg = new GetMatchInfoMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (true, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (false, new MatchInfo());
		}


		/// <summary>
		/// 試合情報の文字列を取得
		/// </summary>
		/// <returns></returns>
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


		/// <summary>
		/// 開始した試合情報を取得する
		/// </summary>
		/// <returns></returns>
		public (bool, MatchInfo) GetMatch()
		{
			return GetMatch(MatchKey);
		}


		/// <summary>
		/// 開始した試合情報を取得する
		/// </summary>
		/// <param name="matchKey">部屋名</param>
		/// <returns></returns>
		public (bool, MatchInfo) GetMatch(string matchKey)
		{
			var sendJson = JsonConvert.SerializeObject(new GetStartedMatchMessage(matchKey));
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (r, null);
		}


		/// <summary>
		/// 開始した試合情報の文字列を取得する
		/// </summary>
		/// <returns></returns>
		public (bool, string) GetMatchString()
		{
			return GetMatchString(MatchKey);
		}


		/// <summary>
		/// 開始した試合情報の文字列を取得する
		/// </summary>
		/// <param name="matchKey"></param>
		/// <returns></returns>
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


		/// <summary>
		/// ダイス投げた結果を取得する
		/// </summary>
		/// <returns></returns>
		public (ReflectionStatus, int, int, int, IEnumerable<int>) ThrowDice()
		{
			return ThrowDice(MatchKey, MyPlayerId);
		}


		/// <summary>
		/// ダイスを投げた結果を取得する
		/// </summary>
		/// <param name="matchKey"></param>
		/// <param name="playerId"></param>
		/// <returns></returns>
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


		/// <summary>
		/// 順位を取得する
		/// </summary>
		/// <returns></returns>
		public (bool, IEnumerable<int>) GetRanking()
		{
			return GetRanking(MatchKey);
		}


		/// <summary>
		/// 順位を取得する
		/// </summary>
		/// <param name="matchKey"></param>
		/// <returns></returns>
		public (bool, IEnumerable<int>) GetRanking(string matchKey)
		{
			var sendMsg = new GetRankingMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<RankingMessage>(recvJson).Ranking)
				: (r, new List<int>());
		}


		/// <summary>
		/// prevとnowから差分を取得する
		/// </summary>
		/// <param name="prevMatchInfoStr"></param>
		/// <param name="nowMatchInfoStr"></param>
		/// <param name="myPlayerID">自分のプレイヤーのID(もしくは除外するプレイヤーのID)</param>
		/// <returns></returns>
		public List<PlayerMoveEvent> ReverseEvent(string prevMatchInfoStr, string nowMatchInfoStr, int myPlayerID)
		{
			var eventList = new List<PlayerMoveEvent>();
			var Prev = JsonConvert.DeserializeObject<MatchInfo>(prevMatchInfoStr);
			var Now = JsonConvert.DeserializeObject<MatchInfo>(nowMatchInfoStr);

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