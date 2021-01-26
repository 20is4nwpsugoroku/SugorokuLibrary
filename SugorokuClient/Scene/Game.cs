﻿using System;
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
		private static CommonData Data { get; set; }
		private static MatchCommunicationManager MatchManager { get; set; }




		enum State
		{
			WaitMatchStart,
			SugorokuFrameInitBefore,
			SugorokuFrameInit,
			WaitOtherPlayer,
			WaitThrowDice,
			Goal,
			Error
		}

		

		public Game()
		{
		}


		public void Init(CommonData data)
		{
			DX.SetBackgroundColor(255, 255, 255);
			var handle = TextureAsset.Register("Dice1",
				"../../../images/saikoro_1.png");
			DiceButton = new TextureButton(handle, 1120, 800, 160, 160);
			SugorokuFrame = new SugorokuFrame();
			PlayerEvents = new Queue<SugorokuEvent>();
			Ranking = new List<int>();
			MyActionTurn = new List<int>();
			data.MatchManager = new MatchCommunicationManager(data.Player.PlayerID, data.RoomName);
			EventTimer = new Timer();
			EventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, Data.Player.IsHost, data);
			EventTimer.Interval = 4000;
			EventTimer.AutoReset = true;
			EventTimer.Enabled = true;
			EventTimer.Start();
			Data = data;
			DX.putsDx(data.PlayerName);
		}


		SugorokuEvent currentEvent;
		bool SugorokuFrameNotInit = true;

		public void Update()
		{
			if (state == State.SugorokuFrameInit)
			{
				SugorokuFrame.Init(Data.MatchInfo.Players);
				state = State.WaitOtherPlayer;
			}

			if (PlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			{
				currentEvent = PlayerEvents.Peek();
				if (Data.Player.PlayerID == currentEvent.PlayerId)
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


		//private static void PlayingMatchTask(object source, ElapsedEventArgs e, CommonData data)
		//{
		//	var (r, match) = GetMatch(data.RoomName);
		//	if (!r) return;
		//	DX.putsDx("Playering Match Task Start");

		//	if (match.MatchInfo.NextPlayerID == data.Player.PlayerID)
		//	{
		//		Task.Delay(6000);
		//		var (status, dice, start, end, rank) = ThrowDice(data.RoomName, data.Player.PlayerID);
		//		switch (status)
		//		{
		//			case ReflectionStatus.NextSuccess:
		//				PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.MatchInfo.NextPlayerID));
		//				MyActionTurn.Add(match.MatchInfo.Turn);
		//				data.Match = match;
		//				if (end == 7 || end == 17 || end == 21 || end == 26 || end == 28)
		//				{
		//					(r, match) = GetMatch(data.RoomName);
		//					if (!r || match.MatchInfo.NextPlayerID != data.Player.PlayerID) return;
		//					MyActionTurn.Add(match.MatchInfo.Turn);
		//					data.Match = match;
		//					Task.Delay(6000);
		//					(_, dice, start, end, _) = ThrowDice(data.RoomName, data.Player.PlayerID);
		//					PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.MatchInfo.NextPlayerID));
		//				}
		//				break;

		//			case ReflectionStatus.PrevDiceSuccess:
		//				PlayerEvents.Enqueue(new SugorokuEvent(-dice, start, end, match.MatchInfo.NextPlayerID));
		//				MyActionTurn.Add(match.MatchInfo.Turn);
		//				data.Match = match;
		//				break;

		//			case ReflectionStatus.AlreadyFinished:
		//				IsGoal = true;
		//				Ranking = new List<int>(rank);
		//				state = State.Goal;
		//				break;

		//			case ReflectionStatus.NotYourTurn:
		//			default:
		//				break;
		//		}
		//	}
		//	else
		//	{
		//		if (match.MatchInfo.Turn == data.Match.MatchInfo.Turn) return;
		//		var eventList = ReverseEvent(data.Match, match);
		//		foreach(var playerEvent in eventList)
		//		{
		//			PlayerEvents.Enqueue(playerEvent);
		//		}
		//		data.Match = match;
		//	}

		//	if (state == State.Goal || state == State.Error)
		//	{
		//		EventTimer.Stop();
		//	}
		//}


		private static void PlayingMatchTask(object source, ElapsedEventArgs e, CommonData data)
		{
			var (r, match) = data.MatchManager.GetMatch();
			if (!r) return;
			DX.putsDx("Playering Match Task Start");

			if (match.NextPlayerID == data.Player.PlayerID)
			{
				Task.Delay(6000);
				var (status, dice, start, end, rank) = data.MatchManager.ThrowDice();
				switch (status)
				{
					case ReflectionStatus.NextSuccess:
						PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.NextPlayerID));
						MyActionTurn.Add(match.Turn);
						data.MatchInfo = match;
						if (end == 7 || end == 17 || end == 21 || end == 26 || end == 28)
						{
							(r, match) = data.MatchManager.GetMatch();
							if (!r || match.NextPlayerID != data.Player.PlayerID) return;
							MyActionTurn.Add(match.Turn);
							data.MatchInfo = match;
							Task.Delay(6000);
							(_, dice, start, end, _) = data.MatchManager.ThrowDice();
							PlayerEvents.Enqueue(new SugorokuEvent(dice, start, end, match.NextPlayerID));
						}
						break;

					case ReflectionStatus.PrevDiceSuccess:
						PlayerEvents.Enqueue(new SugorokuEvent(-dice, start, end, match.NextPlayerID));
						MyActionTurn.Add(match.Turn);
						data.MatchInfo = match;
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
				if (match.Turn == data.MatchInfo.Turn) return;
				var eventList = ReverseEvent(data.MatchInfo, match, Data.Player.PlayerID);
				foreach (var playerEvent in eventList)
				{
					PlayerEvents.Enqueue(playerEvent);
				}
				data.MatchInfo = match;
			}

			if (state == State.Goal || state == State.Error)
			{
				EventTimer.Stop();
			}
		}


		//private static void WaitStartMatchTask(object source, ElapsedEventArgs e, bool isHost, CommonData data)
		//{
		//	DX.putsDx("Wait Start Match Task" + isHost.ToString());
		//	var matchKey = data.RoomName;
		//	if (isHost)
		//	{
		//		state = (CanStartMatch(matchKey, data.PlayerNum))
		//			? State.SugorokuFrameInit : State.WaitMatchStart;
		//		if (state == State.SugorokuFrameInit) CloseJoinMatch(matchKey);
		//	}
		//	else
		//	{
		//		state = (IsMatchStarted(matchKey)) ? State.SugorokuFrameInit : State.WaitMatchStart;
		//	}

		//	if (state == State.SugorokuFrameInit)
		//	{
		//		EventTimer.Stop();
		//		var (r, match) = GetMatch(matchKey);		
		//		for (int i = 0; !r; i++)
		//		{
		//			Task.Delay(5000);
		//			(r, match) = GetMatch(matchKey);
		//			if (i > 5)
		//			{
		//				state = State.Error;
		//				return;
		//			}
		//		}
		//		data.Match = match;
		//		data.MatchInfo = match.MatchInfo;
		//		EventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e, data);
		//		EventTimer.Start();
		//	}
		//}


		private static void WaitStartMatchTask(object source, ElapsedEventArgs e, bool isHost, CommonData data)
		{
			DX.putsDx("Wait Start Match Task" + isHost.ToString());
			var matchKey = data.RoomName;
			if (isHost)
			{
				state = (data.MatchManager.CanStartMatch(data.PlayerNum))
					? State.SugorokuFrameInitBefore : State.WaitMatchStart;
				if (state == State.SugorokuFrameInitBefore) data.MatchManager.CloseJoinMatch();
			}
			else
			{
				state = (data.MatchManager.IsMatchStarted()) ? State.SugorokuFrameInit : State.WaitMatchStart;
			}

			if (state == State.SugorokuFrameInitBefore)
			{
				EventTimer.Stop();
				var (r, match) = data.MatchManager.GetMatch();
				for (int i = 0; !r; i++)
				{
					Task.Delay(5000);
					(r, match) = data.MatchManager.GetMatch();
					if (i > 5)
					{
						state = State.Error;
						return;
					}
				}
				//data.Match = match;
				data.MatchInfo = match;
				EventTimer.Elapsed -= (source, e) => WaitStartMatchTask(source, e, isHost, data);
				EventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e, data);
				EventTimer.Start();
				state = State.SugorokuFrameInit;
			}
		}


		//private static IEnumerable<SugorokuEvent> ReverseEvent(MatchCore prev, MatchCore now)
		//{
		//	DX.putsDx("Start Reverse Event");
		//	var eventList = new List<SugorokuEvent>();
		//	var alreadyReversePlayer = new List<int>();
		//	if (now.MatchInfo.Turn == prev.MatchInfo.Turn) return eventList;
		//	for(var i = prev.MatchInfo.Turn; i < now.MatchInfo.Turn; i++)
		//	{
		//		if (MyActionTurn.Contains(i)) continue;
		//		var actionPlayer = prev.ActionSchedule.Dequeue();
		//		if (alreadyReversePlayer.Contains(actionPlayer)) continue;
		//		var nowPos = now.Players[actionPlayer].Position;
		//		var prevPos = prev.Players[actionPlayer].Position;
		//		var dice = nowPos - prevPos;
		//		alreadyReversePlayer.Add(actionPlayer);
		//		eventList.Add(new SugorokuEvent(nowPos, nowPos, actionPlayer, dice));
		//	}
		//	return eventList;
		//}


		private static IEnumerable<SugorokuEvent> ReverseEvent(MatchInfo prev, MatchInfo now, int myPlayerID)
		{
			DX.putsDx("Start Reverse Event");
			var eventList = new List<SugorokuEvent>();
			var alreadyReversePlayer = new List<int>();
			if (now.Turn == prev.Turn) return eventList;
			for (var i = prev.Turn; i < now.Turn; i++)
			{
				int actionPlayer, nowPos, prevPos, dice;
				if (MyActionTurn.Contains(i)) continue;
				for (int j = 0; j < prev.Players.Count; j++)
				{
					if (myPlayerID == prev.Players[i].PlayerID) continue;
					for (int k = 0; k < now.Players.Count; k++)
					{
						if (myPlayerID == now.Players[i].PlayerID) continue;
						if (now.Players[i].PlayerID == prev.Players[i].PlayerID
							&& now.Players[i].Position != prev.Players[i].Position)
						{
							actionPlayer = prev.Players[i].PlayerID;
							if (alreadyReversePlayer.Contains(actionPlayer)) continue;
							nowPos = now.Players[actionPlayer].Position;
							prevPos = prev.Players[actionPlayer].Position;
							dice = nowPos - prevPos;
							alreadyReversePlayer.Add(actionPlayer);
							eventList.Add(new SugorokuEvent(nowPos, nowPos, actionPlayer, dice));
							break;
						}
					}
				}
			}
			return eventList;
		}
	}
}
