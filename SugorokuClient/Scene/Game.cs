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
		private static Timer WaitEventTimer { get; set; }
		private static Timer PlayingEventTimer { get; set; }
		private static State state { get; set; }
		private static List<int> MyActionTurn { get; set; }
		//private static Queue<SugorokuEvent> MyPlayerEvents { get; set; }
		//private static Queue<SugorokuEvent> OtherPlayerEvents { get; set; }

		//private static List<int> Ranking { get; set; }
		private static CommonData Data { get; set; }
		private static bool IsGoal { get; set; }
		private TextureButton DiceButton { get; set; }
		private DiceTexture DiceTexture { get; set; }
		private int UIFontTexture { get; set; }
		private string MessageText { get; set; }
		private static bool isProcessingPlayingEvent { get; set; }



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
			UIFontTexture = FontAsset.Register("GameSceneUI", size: 80);
			DX.SetBackgroundColor(255, 255, 255);
			var handle = TextureAsset.Register("Dice1",
				"../../../images/saikoro_1.png");
			DiceButton = new TextureButton(handle, 1120, 800, 160, 160);
			DiceTexture = new DiceTexture(960, 800, 160, 160);
			SugorokuFrame = new SugorokuFrame();
			//MyPlayerEvents = new Queue<SugorokuEvent>();
			//OtherPlayerEvents = new Queue<SugorokuEvent>();
			//Ranking = new List<int>();
			MyActionTurn = new List<int>();
			data.MatchManager = new MatchCommunicationManager(data.Player.PlayerID, data.RoomName);
			PlayingEventTimer = new Timer();
			WaitEventTimer = new Timer();
			WaitEventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, Data.Player.IsHost, data);
			WaitEventTimer.Interval = 4000;
			WaitEventTimer.AutoReset = true;
			WaitEventTimer.Enabled = true;
			WaitEventTimer.Start();
			Data = data;
			DX.putsDx(data.PlayerName);
		}


		SugorokuEvent currentEvent;
		//bool SugorokuFrameNotInit = true;

		public void Update()
		{
			DiceTexture.Update();
			SugorokuFrame.Update();
			MessageText = (state) switch
			{
				State.WaitMatchStart => "試合の開始を待っています",
				State.WaitOtherPlayer => "他のプレイヤーの行動を待っています",
				State.WaitThrowDice => "さいころを振ってください",
				State.Error => "エラーが発生しました",
				State.Goal => "ゴールしました。",
				_ => ""
			};


			if (state == State.SugorokuFrameInit)
			{
				SugorokuFrame.Init(Data.MatchInfo.Players);
				state = State.WaitOtherPlayer;
				WaitEventTimer.Dispose();
				isProcessingPlayingEvent = false;
			}

			//if (OtherPlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			//{
			//	SugorokuFrame.ProcessEvent(OtherPlayerEvents.Dequeue());
			//}
			/*else */if (Data.PlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			{
				currentEvent = Data.PlayerEvents.Peek();
				//state = State.WaitThrowDice;
				if (Data.Player.PlayerID == currentEvent.PlayerId)
				{
					state = State.WaitThrowDice;
				}
				else
				{
					SugorokuFrame.ProcessEvent(currentEvent);
					Data.PlayerEvents.Dequeue();
				}
			}


			if (state == State.WaitThrowDice
				&& !SugorokuFrame.IsProcessingEvent
				&& DiceTexture.AnimationFrame == 0)
			{
				SugorokuFrame.ProcessEvent(currentEvent);
				Data.PlayerEvents.Dequeue();
				state = State.WaitOtherPlayer;
			}
			else if (state == State.WaitThrowDice 
				&& DiceButton.LeftClicked()
				&& !SugorokuFrame.IsProcessingEvent)
			{
				DiceTexture.AnimationStart(currentEvent.Dice);
			}
			else if (DiceButton.LeftClicked())
			{
				// まだターンじゃない表示
			}
		}

		public void Draw()
		{
			SugorokuFrame.Draw();
			DiceButton.Draw();
			DiceButton.MouseOverDraw();
			DiceTexture.Draw();
			FontAsset.Draw(UIFontTexture, MessageText, 0, 800, DX.GetColor(50, 50, 50));
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


		private static void PlayingMatchTask(object _source, ElapsedEventArgs _e, CommonData data)
		{
			//DX.putsDx("..............");
			//var (temp_r, temp_match) = data.MatchManager.GetMatch(data.RoomName);
			//if (!temp_r) DX.putsDx("fffffffffffff");
			//if (temp_match == null) DX.putsDx("NNNNNNNNNNNNNNN");
			//DX.putsDx("..............");
			if (isProcessingPlayingEvent) return;

			var (r, match) = data.MatchManager.GetMatch();
			if (!r) return;
			isProcessingPlayingEvent = true;
			DX.putsDx("Playering Match Task Start");
			DX.putsDx("MyID ; " + data.Player.PlayerID.ToString());
			DX.putsDx("NextPlayerID" + match.NextPlayerID.ToString());

			if (match.NextPlayerID == data.Player.PlayerID)
			{	
				Task.Delay(4000);
				var (status, dice, start, end, rank) = data.MatchManager.ThrowDice();
				switch (status)
				{
					case ReflectionStatus.NextSuccess:
					case ReflectionStatus.PrevDiceSuccess:
						data.PlayerEvents.Enqueue(new SugorokuEvent(start, end, match.NextPlayerID, dice));
						MyActionTurn.Add(match.Turn);
						data.MatchInfo = match;
						//if (end == 7 || end == 17 || end == 21 || end == 26 || end == 28)
						//{
						//	(r, match) = data.MatchManager.GetMatch();
						//	if (!r || match.NextPlayerID != data.Player.PlayerID) return;
						//	MyActionTurn.Add(match.Turn);
						//	data.MatchInfo = match;
						//	Task.Delay(6000);
						//	(_, dice, start, end, _) = data.MatchManager.ThrowDice();
						//	PlayerEvents.Enqueue(new SugorokuEvent(start, end, match.NextPlayerID, dice));
						//}
						break;

					//case ReflectionStatus.PrevDiceSuccess:
					//	PlayerEvents.Enqueue(new SugorokuEvent(start, end, match.NextPlayerID, dice));
					//	MyActionTurn.Add(match.Turn);
					//	data.MatchInfo = match;
					//	break;

					case ReflectionStatus.AlreadyFinished:
						IsGoal = true;
						data.Ranking = new List<int>(rank);
						state = State.Goal;
						break;

					case ReflectionStatus.NotYourTurn:
						break;

					default:
						state = State.Error;
						break;
				}
			}
			else
			{
				//if (match.Turn == data.MatchInfo.Turn) return;
				var eventList = data.MatchManager.ReverseEvent(data.MatchInfo, match, data.Player.PlayerID);
				for (var i = 0; i < eventList.Count; i++)
				{
					data.PlayerEvents.Enqueue(eventList[i]);
				}
				data.MatchInfo = match;
			}

			foreach (var player in data.MatchInfo.Players)
			{
				if (player.Position == 30)
				{
					IsGoal = true;
					break;
				}
			}

			if (state == State.Goal || state == State.Error|| IsGoal)
			{
				PlayingEventTimer.Stop();
				var (ret, rank) = data.MatchManager.GetRanking();
				if (ret)
				{
					DX.putsDx("GOOOOOOOOOOOOOOOOOOOOOOOOOOOAL");
					data.Ranking = new List<int>(rank);
				}
				else
				{
					state = State.Error;
				}
				PlayingEventTimer.Enabled = false;
				isProcessingPlayingEvent = false;
				PlayingEventTimer.Dispose();
			}

			isProcessingPlayingEvent = false;
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


		private static void WaitStartMatchTask(object _source, ElapsedEventArgs _e, bool isHost, CommonData data)
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
				state = (data.MatchManager.IsMatchStarted()) ? State.SugorokuFrameInitBefore : State.WaitMatchStart;
			}

			if (state == State.SugorokuFrameInitBefore)
			{
				WaitEventTimer.Stop();
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
				PlayingEventTimer.Interval = 4000;
				PlayingEventTimer.AutoReset = true;
				PlayingEventTimer.Enabled = true;
				PlayingEventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e, data);
				PlayingEventTimer.Start();
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


		private static List<SugorokuEvent> ReverseEvent(MatchInfo prev, MatchInfo now, int myPlayerID)
		{
			DX.putsDx("Start Reverse Event");
			var eventList = new List<SugorokuEvent>();
			var alreadyReversePlayer = new List<int>();

			for (var i = 0; i < prev.Players.Count; i++)
			{
				int actionPlayer, nowPos, prevPos, dice;
				DX.putsDx($"Reverse {prev.Players[i].PlayerID}");
				if (prev.Players[i].PlayerID == myPlayerID) continue;

				for (var j = 0; j < now.Players.Count; j++)
				{
					if (now.Players[j].PlayerID == prev.Players[i].PlayerID
								&& now.Players[j].Position != prev.Players[i].Position)
					{
						actionPlayer = prev.Players[i].PlayerID;
						if (alreadyReversePlayer.Contains(actionPlayer)) continue;
						nowPos = now.Players[actionPlayer].Position;
						prevPos = prev.Players[actionPlayer].Position;
						dice = nowPos - prevPos;
						alreadyReversePlayer.Add(actionPlayer);
						eventList.Add(new SugorokuEvent(prevPos, nowPos, actionPlayer, dice));
						DX.putsDx(JsonConvert.SerializeObject(eventList[^1]));
						DX.putsDx($"End Reverse {prev.Players[i].PlayerID}");
						break;
					}
				}
			}


			//for (var i = prev.Turn; i < now.Turn; i++)
			//{
			//	int actionPlayer, nowPos, prevPos, dice;
			//	if (MyActionTurn.Contains(i)) continue;
			//	for (int j = 0; j < prev.Players.Count; j++)
			//	{
			//		if (myPlayerID == prev.Players[i].PlayerID) continue;
			//		for (int k = 0; k < now.Players.Count; k++)
			//		{
			//			if (myPlayerID == now.Players[i].PlayerID) continue;
			//			if (now.Players[i].PlayerID == prev.Players[i].PlayerID
			//				&& now.Players[i].Position != prev.Players[i].Position)
			//			{
			//				actionPlayer = prev.Players[i].PlayerID;
			//				if (alreadyReversePlayer.Contains(actionPlayer)) continue;
			//				nowPos = now.Players[actionPlayer].Position;
			//				prevPos = prev.Players[actionPlayer].Position;
			//				dice = nowPos - prevPos;
			//				alreadyReversePlayer.Add(actionPlayer);
			//				eventList.Add(new SugorokuEvent(nowPos, nowPos, actionPlayer, dice));
			//				break;
			//			}
			//		}
			//	}
			//}
			return eventList;
		}
	}
}
