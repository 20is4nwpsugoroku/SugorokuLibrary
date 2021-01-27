using DxLibDLL;
using Newtonsoft.Json;
using SugorokuClient.UI;
using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.Match;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;


namespace SugorokuClient.Scene
{
	public class Game : IScene
	{
		private static Timer WaitEventTimer { get; set; }
		private static Timer PlayingEventTimer { get; set; }
		private static State state { get; set; }
		private static CommonData Data { get; set; }
		private string MessageText { get; set; }
		private static bool isProcessingPlayingEvent { get; set; }
		private static bool IsGoal { get; set; }
		private SugorokuFrame SugorokuFrame { get; set; }
		private TextureButton DiceButton { get; set; }
		private TextureButton CloseButton { get; set; }
		private TextureButton MyPlayer { get; set; }
		private TextureButton MyPlayerName { get; set; }
		private DiceTexture DiceTexture { get; set; }
		private int UIFontTexture { get; set; }


		enum State
		{
			WaitMatchStart,
			SugorokuFrameInitBefore,
			SugorokuFrameInit,
			WaitOtherPlayer,
			WaitThrowDice,
			Goal,
			DrawRanking,
			Error
		}

		
		public Game()
		{
		}


		public void Init(CommonData data)
		{
			UIFontTexture = FontAsset.Register("GameSceneUI", size: 40);
			var dicefont = FontAsset.Register("sugorokuDiceButton", size: 20);	
			var handle = TextureAsset.Register("ButtonBase2", "../../../images/ButtonBase2.png");
			DiceButton = new TextureButton(handle, 960, 800, 320, 160, "サイコロを振る", DX.GetColor(0, 103, 167), dicefont);
			CloseButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 800, 400, 50, "終わる", DX.GetColor(0, 103, 167), FontAsset.GetFontHandle("Default"));
			MyPlayer = new TextureButton(TextureAsset.Register("Toumei", "../../../images/toumei.png"), 680, 800, 120, 120);
			MyPlayerName = new TextureButton(TextureAsset.GetTextureHandle("Toumei"), 680, 920, 120, 40, "自分の駒", DX.GetColor(100, 100, 100), dicefont);
			DiceTexture = new DiceTexture(800, 800, 160, 160);
			SugorokuFrame = new SugorokuFrame();
			data.MatchManager = new MatchCommunicationManager(data.Player.PlayerID, data.RoomName);
			PlayingEventTimer = new Timer();
			WaitEventTimer = new Timer();
			WaitEventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, Data.Player.IsHost, data);
			WaitEventTimer.Interval = 2000;
			WaitEventTimer.AutoReset = true;
			WaitEventTimer.Enabled = true;
			WaitEventTimer.Start();
			Data = data;
			DX.SetBackgroundColor(255, 255, 255);
		}


		public void Update()
		{
			if (state == State.SugorokuFrameInit)
			{
				SugorokuFrame.Init(Data.MatchInfo.Players);
				MyPlayer = new TextureButton(SugorokuFrame.PlayerTextureHandle[Data.Player.PlayerID], 680, 800, 120, 120);
				state = State.WaitOtherPlayer;
				WaitEventTimer.Dispose();
				isProcessingPlayingEvent = false;
			}

			if (Data.PlayerEvents.Count == 0 && (state == State.Goal || IsGoal))
			{
				state = State.DrawRanking;
				SugorokuFrame.DrawRanking(Data.Ranking);
			}

			if (state == State.DrawRanking)
			{
				if (CloseButton.LeftClicked())
				{
					DX.DxLib_End();
				}
				return;
			}

			if (Data.PlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			{
				if (Data.Player.PlayerID == Data.PlayerEvents.Peek().PlayerId)
				{
					state = State.WaitThrowDice;
				}
				else
				{
					SugorokuFrame.ProcessEvent(Data.PlayerEvents.Peek());
					Data.PlayerEvents.Dequeue();
				}
			}

			if (state == State.WaitThrowDice && !SugorokuFrame.IsProcessingEvent && DiceTexture.AnimationFrame == 0)
			{
				SugorokuFrame.ProcessEvent(Data.PlayerEvents.Peek());
				Data.PlayerEvents.Dequeue();
				state = State.WaitOtherPlayer;
			}
			else if (state == State.WaitThrowDice && DiceButton.LeftClicked() && !SugorokuFrame.IsProcessingEvent)
			{
				DiceTexture.AnimationStart(Data.PlayerEvents.Peek().Dice);
			}

			DiceTexture.Update();
			SugorokuFrame.Update();
			MessageText = (state) switch
			{
				State.WaitMatchStart => "試合の開始を待っています",
				State.WaitOtherPlayer => "他のプレイヤーの行動を待っています",
				State.WaitThrowDice => "さいころを振ってください",
				State.Error => "エラーが発生しました",
				State.Goal => "ゴールしました",
				_ => ""
			};
		}

		public void Draw()
		{
			SugorokuFrame.Draw();
			DiceButton.Draw();
			DiceButton.MouseOverDraw();
			DiceTexture.Draw();
			MyPlayer.Draw();
			MyPlayerName.DrawText();
			FontAsset.Draw(UIFontTexture, MessageText, 0, 800, DX.GetColor(50, 50, 50));
			if (state == State.DrawRanking)
			{
				SugorokuFrame.Draw();
				CloseButton.Draw();
				CloseButton.DrawText();
			}
		}


		private static void PlayingMatchTask(object _source, ElapsedEventArgs _e, CommonData data)
		{
			if (isProcessingPlayingEvent) return;
			var (r, matchStr) = data.MatchManager.GetMatchString();
			if (!r) return;
			isProcessingPlayingEvent = true;
			if (JsonConvert.DeserializeObject<MatchInfo>(matchStr).NextPlayerID == data.Player.PlayerID)
			{
				PlayingEventTimer.Stop();
				Task.Delay(8000);
				PlayingEventTimer.Start();
				var (status, dice, start, end, rank) = data.MatchManager.ThrowDice();
				switch (status)
				{
					case ReflectionStatus.NextSuccess:
					case ReflectionStatus.PrevDiceSuccess:
						data.PlayerEvents.Enqueue(new SugorokuEvent(start, end, JsonConvert.DeserializeObject<MatchInfo>(matchStr).NextPlayerID, dice));
						break;

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

			(r, matchStr) = data.MatchManager.GetMatchString();

			if (r)
			{
				var eventList = data.MatchManager.ReverseEvent(JsonConvert.DeserializeObject<MatchInfo>(data.MatchInfoStr), JsonConvert.DeserializeObject<MatchInfo>(matchStr), data.Player.PlayerID);
				for (var i = 0; i < eventList.Count; i++)
				{
					data.PlayerEvents.Enqueue(eventList[i]);
				}
				data.MatchInfoStr = "" + matchStr;
				data.MatchInfo = JsonConvert.DeserializeObject<MatchInfo>(data.MatchInfoStr);
			}

			foreach (var player in JsonConvert.DeserializeObject<MatchInfo>(data.MatchInfoStr).Players)
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


		private static void WaitStartMatchTask(object _source, ElapsedEventArgs _e, bool isHost, CommonData data)
		{
			var matchKey = data.RoomName;
			if (isHost)
			{
				state = (data.MatchManager.CanStartMatch(data.PlayerNum)) ? State.SugorokuFrameInitBefore : State.WaitMatchStart;
				if (state == State.SugorokuFrameInitBefore) data.MatchManager.CloseJoinMatch();
			}
			else
			{
				state = (data.MatchManager.IsMatchStarted()) ? State.SugorokuFrameInitBefore : State.WaitMatchStart;
			}

			if (state == State.SugorokuFrameInitBefore)
			{
				WaitEventTimer.Stop();
				var (r, matchStr) = data.MatchManager.GetMatchInfoString();
				data.MatchInfoStr = "" + matchStr;
				for (int i = 0; !r; i++)
				{
					Task.Delay(5000);
					(r, matchStr) = data.MatchManager.GetMatchInfoString();
					data.MatchInfoStr = "" + matchStr;
					if (i > 5)
					{
						state = State.Error;
						return;
					}
				}
				data.MatchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchStr);
				PlayingEventTimer.Interval = 4000;
				PlayingEventTimer.AutoReset = true;
				PlayingEventTimer.Enabled = true;
				PlayingEventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e, data);
				PlayingEventTimer.Start();
				state = State.SugorokuFrameInit;
			}
		}
	}
}
