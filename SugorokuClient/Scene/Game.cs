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
	/// <summary>
	/// ゲームシーンのクラス
	/// </summary>
	public class Game : IScene
	{
		/// <summary>
		/// 試合開始まで待つ際のイベントに使用するタイマー
		/// </summary>
		private static Timer WaitEventTimer { get; set; }

		/// <summary>
		/// 試合開始後に使用するタイマー
		/// </summary>
		private static Timer PlayingEventTimer { get; set; }

		/// <summary>
		/// ゲームシーン内での状態
		/// </summary>
		private static State state { get; set; }

		/// <summary>
		/// 各シーンで共用するクラス
		/// </summary>
		private static CommonData Data { get; set; }

		/// <summary>
		/// PlayingEventTimerがElaspedに登録されたイベントを処理しているかどうか
		/// </summary>
		private static bool isProcessingPlayingEvent { get; set; }

		/// <summary>
		/// ゴールした
		/// </summary>
		private static bool IsGoal { get; set; }

		/// <summary>
		/// すごろくのUIパーツ
		/// </summary>
		private SugorokuFrame SugorokuFrame { get; set; }

		/// <summary>
		/// ダイスを振るボタン
		/// </summary>
		private TextureButton DiceButton { get; set; }

		/// <summary>
		/// 試合終了後に押す終了ボタン
		/// </summary>
		private TextureButton CloseButton { get; set; }

		/// <summary>
		/// 自分の駒のテクスチャ
		/// </summary>
		private TextureButton MyPlayer { get; set; }

		/// <summary>
		/// "自分の駒"というテキストが格納されているテクスチャ
		/// </summary>
		private TextureButton MyPlayerText { get; set; }

		/// <summary>
		/// ダイスのテクスチャクラス
		/// </summary>
		private DiceTexture DiceTexture { get; set; }

		/// <summary>
		/// メッセージテキスト
		/// </summary>
		private int MessageTextFont { get; set; }

		/// <summary>
		/// 画面下部に表示される文字
		/// </summary>
		private string MessageText { get; set; }


		/// <summary>
		/// ゲームシーン内の状態
		/// </summary>
		enum State
		{
			WaitMatchStart,// 試合の開始待ち
			SugorokuFrameInitBefore, // SugorokuFrameの初期化準備
			SugorokuFrameInit, // SugorokuFrameの初期化
			WaitOtherPlayer, // 別のプレイヤーの行動待ち
			WaitThrowDice, // ダイスを振るのを待つ
			Goal, // ゴール
			DrawRanking, // 順位の描画
			Error // エラー
		}

		
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Game()
		{
		}


		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="data">各シーンで共用するクラス</param>
		public void Init(CommonData data)
		{
			MessageTextFont = FontAsset.Register("GameSceneUI", size: 40);
			var dicefont = FontAsset.Register("sugorokuDiceButton", size: 20);	
			var handle = TextureAsset.Register("ButtonBase2", "../../../images/ButtonBase2.png");
			DiceButton = new TextureButton(handle, 960, 800, 320, 160, "サイコロを振る", DX.GetColor(0, 103, 167), dicefont);
			CloseButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 800, 400, 50, "終わる", DX.GetColor(0, 103, 167), FontAsset.GetFontHandle("Default"));
			MyPlayer = new TextureButton(TextureAsset.Register("Toumei", "../../../images/toumei.png"), 680, 800, 120, 120);
			MyPlayerText = new TextureButton(TextureAsset.GetTextureHandle("Toumei"), 680, 920, 120, 40, "自分の駒", DX.GetColor(100, 100, 100), dicefont);
			DiceTexture = new DiceTexture(800, 800, 160, 160);
			SugorokuFrame = new SugorokuFrame();
			data.MatchManager = new MatchCommunicationManager(data.Player.PlayerID, data.RoomName);
			PlayingEventTimer = new Timer();
			WaitEventTimer = new Timer();
			WaitEventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, Data.Player.IsHost, data);
			WaitEventTimer.Interval = 5000;
			WaitEventTimer.AutoReset = true;
			WaitEventTimer.Enabled = true;
			WaitEventTimer.Start();
			Data = data;
			DX.SetBackgroundColor(255, 255, 255);
		}


		/// <summary>
		/// ゲームシーンの更新処理
		/// </summary>
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

			if (Data.PlayerEvents.Count == 0 
				&& (state == State.Goal || IsGoal) 
				&& SugorokuFrame.IsEndAllAnimation())
			{
				state = State.DrawRanking;
				SugorokuFrame.SetDrawRanking(Data.Ranking);
			}
			if (state == State.DrawRanking)
			{
				if (CloseButton.LeftClicked())
				{
					DX.DxLib_End();
				}
				return;
			}

			ProcessPlayerEvent();
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


		/// <summary>
		/// プレイヤーイベントの処理を行う関数(Updateの処理を移動したもの)
		/// </summary>
		private void ProcessPlayerEvent()
		{
			if (Data.PlayerEvents.Count != 0 && !SugorokuFrame.IsProcessingEvent)
			{
				if (Data.Player.PlayerID == Data.PlayerEvents.Peek().PlayerId)
				{
					state = State.WaitThrowDice;
				}
				else
				{
					SugorokuFrame.ProcessEvent(Data.PlayerEvents.Dequeue());	
				}
			}

			// ダイスを振る処理
			if (state == State.WaitThrowDice && !SugorokuFrame.IsProcessingEvent && DiceTexture.AnimationFrame == 0)
			{
				SugorokuFrame.ProcessEvent(Data.PlayerEvents.Dequeue());
				state = State.WaitOtherPlayer;
			}
			else if (state == State.WaitThrowDice && DiceButton.LeftClicked() && !SugorokuFrame.IsProcessingEvent)
			{
				DiceTexture.AnimationStart(Data.PlayerEvents.Peek().Dice);
			}

			DiceTexture.Update();
		}


		/// <summary>
		/// ゲームシーンの描画処理
		/// </summary>
		public void Draw()
		{
			SugorokuFrame.Draw();
			DiceButton.Draw();
			DiceButton.MouseOverDraw();
			DiceTexture.Draw();
			MyPlayer.Draw();
			MyPlayerText.DrawText();
			FontAsset.Draw(MessageTextFont, MessageText, 0, 800, DX.GetColor(50, 50, 50));
			if (state == State.DrawRanking)
			{
				SugorokuFrame.Draw();
				CloseButton.Draw();
				CloseButton.DrawText();
			}
		}


		/// <summary>
		/// 試合中の通信の処理などを行う関数
		/// </summary>
		/// <param name="_source"></param>
		/// <param name="_e"></param>
		/// <param name="data">各シーンで共用する変数</param>
		private static void PlayingMatchTask(object _source, ElapsedEventArgs _e, CommonData data)
		{
			if (isProcessingPlayingEvent) return;
			var (r, matchStr) = data.MatchManager.GetMatchString();
			if (!r) return;
			isProcessingPlayingEvent = true;

			// ダイスを投げる要求を行う
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
						data.PlayerEvents.Enqueue(new SugorokuEvent(start, end,
							JsonConvert.DeserializeObject<MatchInfo>(matchStr).NextPlayerID, dice));
						break;
					case ReflectionStatus.AlreadyFinished:
						data.Ranking = new List<int>(rank);
						IsGoal = true;
						state = State.Goal;
						break;
					case ReflectionStatus.NotYourTurn:
						break;
					default:
						state = State.Error;
						break;
				}
			}

			var infoStr = data.MatchManager.GetMatchString();
			if (infoStr.Item1)
			{ 
				var eventList = data.MatchManager.ReverseEvent(JsonConvert.DeserializeObject<MatchInfo>(data.MatchInfoStr),
					JsonConvert.DeserializeObject<MatchInfo>(infoStr.Item2), data.Player.PlayerID);
				for (var i = 0; i < eventList.Count; i++)
				{
					data.PlayerEvents.Enqueue(eventList[i]);
				}
				data.MatchInfoStr = "" + infoStr.Item2;
				data.MatchInfo = JsonConvert.DeserializeObject<MatchInfo>(data.MatchInfoStr);
			}
			foreach (var player in JsonConvert.DeserializeObject<MatchInfo>(infoStr.Item2).Players)
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


		/// <summary>
		/// 試合開始までの待機中に通信などの処理を行う関数
		/// </summary>
		/// <param name="_source"></param>
		/// <param name="_e"></param>
		/// <param name="isHost"></param>
		/// <param name="data"></param>
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
