using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuClient.UI;
using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.ServerToClient;
using SugorokuLibrary.Match;




namespace SugorokuClient.Scene
{
	public class Game : IScene
	{
		private SugorokuFrame SugorokuFrame { get; set; }
		private static Timer EventTimer { get; set; }
		private static State state { get; set; }
		private static bool CanThrowDice { get; set; }
		private static bool IsGoal { get; set; }
		private static List<int> MyActionTurn { get; set; }
		private static Queue<SugorokuEvent> PlayerEvents { get; set; }
		private static List<int> Ranking { get; set; }


		private TextureButton DiceButton { get; set; }



		enum State
		{
			WaitMatchStart,
			SugorokuFrameInit,
			WaitOtherPlayer,
			WaitThrowDice,
			Goal,
			Error
		}

		

		public Game()
		{
		}


		public void Init()
		{
			DX.SetBackgroundColor(255, 255, 255);
			var handle = TextureAsset.Register("DiceImage",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png");
			DiceButton = new TextureButton(handle, 1120, 800, 160, 160);
			SugorokuFrame = new SugorokuFrame();
			PlayerEvents = new Queue<SugorokuEvent>();
			Ranking = new List<int>();
			MyActionTurn = new List<int>();
			EventTimer = new Timer();
			EventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, CommonData.Player.IsHost);
			EventTimer.Interval = 4000;
			EventTimer.AutoReset = true;
			EventTimer.Enabled = true;
			EventTimer.Start();
		}


		SugorokuEvent currentEvent;
		bool SugorokuFrameNotInit = true;

		public void Update()
		{
			if (state == State.SugorokuFrameInit)
			{
				SugorokuFrame.Init(CommonData.MatchInfo.Players);
				state = State.WaitOtherPlayer;
			}

			if (PlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			{
				currentEvent = PlayerEvents.Peek();
				if (CommonData.Player.PlayerID == currentEvent.PlayerId)
				{
					state = State.WaitThrowDice;
				}
				else
				{
					SugorokuFrame.ProcessEvent(currentEvent);
					PlayerEvents.Dequeue();
				}
			}

			if (state == State.WaitThrowDice 
				&& DiceButton.LeftClicked()
				&& !SugorokuFrame.IsProcessingEvent)
			{
				SugorokuFrame.ProcessEvent(currentEvent);
				PlayerEvents.Dequeue();
				state = State.WaitOtherPlayer;
			}
			else if (DiceButton.LeftClicked())
			{
				// まだターンじゃない表示
			}

			SugorokuFrame.Update();
		}

		public void Draw()
		{
			SugorokuFrame.Draw();
			DiceButton.Draw();
			DiceButton.MouseOverDraw();
		}


		private static async void PlayingMatchTask(object source, ElapsedEventArgs e)
		{
			var (r, match) = GetMatch(CommonData.MatchInfo.MatchKey);
			if (!r) return;

			if (match.MatchInfo.NextPlayerID == CommonData.Player.PlayerID)
			{
				await Task.Delay(12000);
				var (status, dice, start, end, rank) = ThrowDice(CommonData.Match.MatchInfo.MatchKey, CommonData.Player.PlayerID);
				switch (status)
				{
					case ReflectionStatus.NextSuccess:
						PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.MatchInfo.NextPlayerID));
						MyActionTurn.Add(match.MatchInfo.Turn);
						CommonData.Match = match;
						if (end == 7 || end == 17 || end == 21 || end == 26 || end == 28)
						{
							(r, match) = GetMatch(CommonData.MatchInfo.MatchKey);
							if (!r || match.MatchInfo.NextPlayerID != CommonData.Player.PlayerID) return;
							MyActionTurn.Add(match.MatchInfo.Turn);
							CommonData.Match = match;
							await Task.Delay(12000);
							(_, dice, start, end, _) = ThrowDice(CommonData.Match.MatchInfo.MatchKey, CommonData.Player.PlayerID);
							PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.MatchInfo.NextPlayerID));
						}
						break;

					case ReflectionStatus.PrevDiceSuccess:
						PlayerEvents.Enqueue(new SugorokuEvent(-dice, start, end, match.MatchInfo.NextPlayerID));
						MyActionTurn.Add(match.MatchInfo.Turn);
						CommonData.Match = match;
						break;

					case ReflectionStatus.AlreadyFinished:
						IsGoal = true;
						Ranking = new List<int>(rank);
						state = State.Goal;
						break;

					case ReflectionStatus.NotYourTurn:
					default:
						break;
				}
			}
			else
			{
				if (match.MatchInfo.Turn == CommonData.Match.MatchInfo.Turn) return;
				var eventList = ReverseEvent(CommonData.Match, match);
				foreach(var playerEvent in eventList)
				{
					PlayerEvents.Enqueue(playerEvent);
				}
				CommonData.Match = match;
			}

			if (state == State.Goal || state == State.Error)
			{
				EventTimer.Stop();
			}
		}


		private static async void WaitStartMatchTask(object source, ElapsedEventArgs e, bool isHost)
		{
			var matchKey = CommonData.MatchInfo.MatchKey;
			if (isHost)
			{
				state = (CanStartMatch(matchKey, CommonData.PlayerNum))
					? State.WaitOtherPlayer : State.WaitMatchStart;
				if (state == State.WaitOtherPlayer) CloseJoinMatch(matchKey);
			}
			else
			{
				state = (IsMatchStarted(matchKey)) ? State.SugorokuFrameInit : State.WaitMatchStart;
			}

			if (state == State.SugorokuFrameInit)
			{
				EventTimer.Stop();
				var (r, match) = GetMatch(matchKey);		
				for (int i = 0; !r; i++)
				{
					await Task.Delay(5000);
					(r, match) = GetMatch(matchKey);
					if (i > 5)
					{
						state = State.Error;
						return;
					}
				}
				CommonData.Match = match;
				CommonData.MatchInfo = match.MatchInfo;
				EventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e);
				EventTimer.Start();
			}
		}


		private static bool IsMatchStarted(string matchKey)
		{
			var getInfo = new GetStartedMatchMessage(matchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (r, _) = SocketManager.SendRecv(json);
			return r;
		}


		private static bool CanStartMatch(string matchKey, int playerNum)
		{
			var (r, info) = GetMatchInfo(matchKey);
			if (r)
			{
				r = info.Players.Count >= playerNum;
			}
			return r;
		}


		private static void CloseJoinMatch(string matchKey)
		{
			var sendMsg = new CloseCreateMessage(CommonData.MatchInfo.MatchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (_, _) = SocketManager.SendRecv(sendJson);
		}


		private static (bool, MatchInfo) GetMatchInfo(string matchKey)
		{
			var sendMsg = new GetMatchInfoMessage(matchKey);
			var json = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(json);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchInfo>(recvJson))
				: (r, new MatchInfo());
		}


		private static (bool, MatchCore) GetMatch(string matchKey)
		{
			var sendMsg = new GetStartedMatchMessage(matchKey);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchCore>(recvJson))
				: (r, null);
		}


		private static (ReflectionStatus, int, int, int, IEnumerable<int>)  ThrowDice(string matchKey, int playerId)
		{
			var ranking = new List<int>();
			var sendMsg = new DiceMessage(matchKey, playerId);
			var sendJson = JsonConvert.SerializeObject(sendMsg);
			var (r, recvJson) = SocketManager.SendRecv(sendJson);
			if (!r && recvJson == string.Empty) return (ReflectionStatus.NotYourTurn, -1, -1, -1, ranking);
			var tempMsg = JsonConvert.DeserializeObject<ServerMessage>(recvJson);
			

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


		private static IEnumerable<SugorokuEvent> ReverseEvent(MatchCore prev, MatchCore now)
		{
			var eventList = new List<SugorokuEvent>();
			var alreadyReversePlayer = new List<int>();
			if (now.MatchInfo.Turn == prev.MatchInfo.Turn) return eventList;
			for(var i = prev.MatchInfo.Turn; i < now.MatchInfo.Turn; i++)
			{
				if (MyActionTurn.Contains(i)) continue;
				var actionPlayer = prev.ActionSchedule.Dequeue();
				if (alreadyReversePlayer.Contains(actionPlayer)) continue;
				var nowPos = now.Players[actionPlayer].Position;
				var prevPos = prev.Players[actionPlayer].Position;
				var dice = nowPos - prevPos;
				alreadyReversePlayer.Add(actionPlayer);
				eventList.Add(new SugorokuEvent(nowPos, nowPos, actionPlayer, dice));
			}
			return eventList;
		}
	}
}
