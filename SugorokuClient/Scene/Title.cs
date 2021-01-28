using DxLibDLL;
using Newtonsoft.Json;
using SugorokuClient.UI;
using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using System;
using System.Net;
using System.Threading.Tasks;


namespace SugorokuClient.Scene
{
	/// <summary>
	/// タイトルシーンのクラス
	/// </summary>
	public class Title : IScene
	{
		/// <summary>
		/// 開始ボタン
		/// </summary>
		private TextureButton StartButton { get; set; }

		/// <summary>
		/// 終了ボタン
		/// </summary>
		private TextureButton EndButton { get; set; }

		/// <summary>
		/// 戻るボタン
		/// </summary>
		private TextureButton BackButton { get; set; }

		/// <summary>
		/// 確定ボタン
		/// </summary>
		private TextureButton SubmitButton { get; set; }

		/// <summary>
		/// 部屋を作成するボタン
		/// </summary>
		private TextureButton MakeRoomButton { get; set; }

		/// <summary>
		/// 部屋に参加するボタン
		/// </summary>
		private TextureButton JoinRoomButton { get; set; }

		/// <summary>
		/// 部屋を探すボタン
		/// </summary>
		private TextureButton FindRoomButton { get; set; }

		/// <summary>
		/// IPアドレスの設定を反映するボタン
		/// </summary>
		private TextureButton SaveIpAddressButton { get; set; }

		/// <summary>
		/// 部屋名を入力するテキストボックス
		/// </summary>
		private TextBox roomName { get; set; }

		/// <summary>
		/// プレイヤー名を入力するテキストボックス
		/// </summary>
		private TextBox playerName { get; set; }

		/// <summary>
		/// プレイヤーの人数を入力するテキストボックス
		/// </summary>
		private TextBox playerNum { get; set; }

		/// <summary>
		/// IPアドレスを入力するテキストボックス
		/// </summary>
		private TextBox IpAddressText { get; set; }

		/// <summary>
		/// ポート番号を入力するテキストボックス
		/// </summary>
		private TextBox PortNumberText { get; set; }

		/// <summary>
		/// 部屋を探すポップアップ画面のパーツ
		/// </summary>
		private FindRoomWindow FindRoomWindow { get; set; }

		/// <summary>
		/// ロード画面で使用するテクスチャ
		/// </summary>
		private TextureFade LoadTexture { get; set; }

		/// <summary>
		/// 乱数の生成に使用するクラス
		/// </summary>
		private Random Rand { get; set; }

		/// <summary>
		/// 部屋への参加を待っている状態かどうか
		/// </summary>
		private bool IsWaitJoin { get; set; }

		/// <summary>
		/// ロゴ画像のテクスチャの識別子
		/// </summary>
		private int LogoImageHandle { get; set; }

		/// <summary>
		/// 各シーンで共用するデータ
		/// </summary>
		private static CommonData Data { get; set; }

		/// <summary>
		/// タイトルシーン内での状態
		/// </summary>
		private State state { get; set; }


		/// <summary>
		/// タイトルシーン内での状態
		/// </summary>
		enum State
		{
			Start, // スタートボタンを押す画面
			Select,　// 部屋を作るか、部屋に参加するか選ぶ画面
			MakeRoom, // 部屋を作る画面
			FindRoom, // 部屋に参加する画面
			Load, // ロード画面
			Popup, // 部屋を探す画面
			ChangeGameScene // ゲームシーンに遷移する処理を行う状態
		}


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Title()
		{
		}


		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		/// <param name="data">各シーンで共用するクラス</param>
		public void Init(CommonData data)
		{
			state = State.Start;
			Data = data;
			Rand = new Random();
			LogoImageHandle = TextureAsset.Register("Logo", "../../../images/TitleLogo.png");
			var buttonFont = FontAsset.Register("Default", size: 40);
			var textBoxFont = FontAsset.Register("TextBox", size: 20);
			var blueColor = DX.GetColor(0, 103, 167);

			StartButton = new TextureButton(textureHandle:TextureAsset.Register(assetName:"ButtonBase1", path:"../../../images/ButtonBase1.png"),
				x:440, y:500, width:400, height:50, text:"はじめる", textColor:blueColor, fontHandle:buttonFont);

			EndButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 570, 400, 50, "終わる", blueColor, buttonFont);
			BackButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 500, 400, 50, "戻る", blueColor, buttonFont);
			MakeRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 570, 400, 50, "部屋を作る", blueColor, buttonFont);
			JoinRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 640, 400, 50, "部屋に参加する", blueColor, buttonFont);
			FindRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 1000, 570, 100, 50, "探す", blueColor, buttonFont);
			SubmitButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 490, 780, 300, 50, "確定",blueColor, buttonFont);

			var grayColor = DX.GetColor(50, 50, 50);
			roomName = new TextBox(490, 570, 500, 50, textBoxFont);
			roomName.FrameColor = grayColor;
			roomName.Text = $"Room{Rand.Next(100, 100000)}";
			playerName = new TextBox(490, 640, 500, 50, textBoxFont);
			playerName.FrameColor = grayColor;
			playerName.Text = $"Player{Rand.Next(100, 100000)}";
			playerNum = new TextBox(490, 710, 500, 50, textBoxFont);
			playerNum.FrameColor = grayColor;
			playerNum.Text = "4";

			IpAddressText = new TextBox(0, 0, 400, 50, textBoxFont);
			IpAddressText.Text = Data.Address;
			PortNumberText = new TextBox(0, 50, 400, 50, textBoxFont);
			PortNumberText.Text = Data.Port.ToString();
			SaveIpAddressButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"),
				0, 100, 400, 50, "設定を反映する", DX.GetColor(0, 103, 167), buttonFont);

			IsWaitJoin = false;
			LoadTexture = new TextureFade(TextureAsset.Register("LoadImage", "../../../images/TitleLoad.png"), 590, 600, 100, 100, 60, 60, 1);

			SocketManager.SetAddress(IpAddressText.Text, int.Parse(PortNumberText.Text));
			FindRoomWindow = new FindRoomWindow(340, 180, 600, 610);
			
			DX.SetBackgroundColor(255, 255, 255); // 背景色を白に設定

			// 文字入力時の各パーツの色設定
			DX.SetKeyInputStringColor(DX.GetColor(50, 50, 50), DX.GetColor(50, 50, 50), DX.GetColor(255, 255, 255),
				DX.GetColor(50, 50, 50), DX.GetColor(50, 50, 50), DX.GetColor(0, 55, 255),
				DX.GetColor(50, 50, 50), DX.GetColor(50, 50, 50), DX.GetColor(50, 50, 50),
				DX.GetColor(50, 50, 50), DX.GetColor(50, 50, 50), DX.GetColor(0, 55, 255),
				DX.GetColor(0, 55, 255), DX.GetColor(0, 55, 255), DX.GetColor(0, 55, 255));
		}


		/// <summary>
		/// シーン内の更新処理
		/// </summary>
		public void Update()
		{
			// IPアドレス関連の更新
			IpAddressText.Update();
			PortNumberText.Update();
			if(SaveIpAddressButton.LeftClicked())
			{
				if (IPAddress.TryParse(IpAddressText.Text, out _)
					&& int.TryParse(PortNumberText.Text, out _))
				{
					SocketManager.SetAddress(IpAddressText.Text, int.Parse(PortNumberText.Text));
				}
			}

			// メインの更新処理
			switch (state)
			{
				// スタートボタンを押す画面
				case State.Start:
					if (StartButton.LeftClicked())
					{
						state = State.Select;
					}
					if (EndButton.LeftClicked())
					{
						DX.DxLib_End();
						return;
					}
					break;

				// 部屋を作るか、部屋に参加するか選ぶ画面
				case State.Select:
					if (BackButton.LeftClicked())
					{
						state = State.Start;
					}
					if (MakeRoomButton.LeftClicked())
					{
						state = State.MakeRoom;
					}
					if (JoinRoomButton.LeftClicked())
					{
						state = State.FindRoom;
						roomName.Text = string.Empty;
					}
					break;

				// 部屋を作る画面
				case State.FindRoom:
					if (BackButton.LeftClicked())
					{
						state = State.Start;
					}
					if (SubmitButton.LeftClicked())
					{
						if (roomName.Text.Length != 0
							&& playerName.Text.Length != 0)
						{
							state = State.Load;
							Data.PlayerName = playerName.Text;
							Data.RoomName = roomName.Text;
						}
						IsWaitJoin = false;
					}
					if (FindRoomButton.LeftClicked())
					{
						state = State.Popup;
						FindRoomWindow.IsVisible = true;
					}
					roomName.Update();
					playerName.Update();
					break;

				// 部屋に参加する画面
				case State.MakeRoom:
					if (BackButton.LeftClicked())
					{
						state = State.Start;
					}
					if (SubmitButton.LeftClicked())
					{
						if (roomName.Text.Length != 0
							&& playerName.Text.Length != 0
							&& playerNum.Text.Length != 0)
						{
							state = State.Load;
							Data.PlayerName = playerName.Text;
							Data.RoomName = roomName.Text;
							var num = int.Parse(playerNum.Text);
							if (num > 4 || num < 0)
							{
								num = 4;
								playerNum.Text = num.ToString();
							}
							Data.PlayerNum = num;
							IsWaitJoin = false;
						}

					}
					roomName.Update();
					playerName.Update();
					playerNum.Update();
					break;

				// ロード画面
				case State.Load:
					if (BackButton.LeftClicked())
					{
						state = State.Start;
					}
					if (!IsWaitJoin)
					{
						IsWaitJoin = true;
						Task.Run(()=>JoinMatch(Data.RoomName, Data.PlayerName));
					}
					if (!LoadTexture.IsVisible())
					{
						LoadTexture.Start();
					}
					LoadTexture.Update();
					break;

				// 部屋を探す画面
				case State.Popup:
					FindRoomWindow.Update();
					if (!FindRoomWindow.IsVisible)
					{
						Data.MatchInfo = FindRoomWindow.GetSelectedMatch();
						roomName.Text = Data.MatchInfo.MatchKey;
						state = State.FindRoom;
					}
					break;

				// ゲームシーンに遷移する処理を行う状態
				case State.ChangeGameScene:
					SceneManager.ChangeScene(SceneManager.SceneName.Game);
					break;

				default:
					break;
			}
		}


		/// <summary>
		/// シーン内の描画処理
		/// </summary>
		public void Draw()
		{
			TextureAsset.Draw(LogoImageHandle, 490, 90, 300, 300, DX.TRUE);
			IpAddressText.Draw();
			PortNumberText.Draw();
			SaveIpAddressButton.Draw();
			switch (state)
			{
				case State.Start:
					StartButton.Draw();
					StartButton.DrawText();
					EndButton.Draw();
					EndButton.DrawText();
					break;

				case State.Select:
					BackButton.Draw();
					BackButton.DrawText();
					MakeRoomButton.Draw();
					MakeRoomButton.DrawText();
					JoinRoomButton.Draw();
					JoinRoomButton.DrawText();
					break;

				case State.FindRoom:
					BackButton.Draw();
					BackButton.DrawText();
					FindRoomButton.Draw();
					FindRoomButton.DrawText();
					SubmitButton.Draw();
					SubmitButton.DrawText();
					roomName.Draw();
					playerName.Draw();
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 250, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 250, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					break;

				case State.MakeRoom:
					BackButton.Draw();
					BackButton.DrawText();
					SubmitButton.Draw();
					SubmitButton.DrawText();
					roomName.Draw();
					playerName.Draw();
					playerNum.Draw();
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 260, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 260, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "人数", playerNum.x1 - 260, playerNum.y1 + 5, DX.GetColor(125, 125, 125));
					break;

				case State.Load:
					BackButton.Draw();
					BackButton.DrawText();
					LoadTexture.Draw();
					break;

				case State.Popup:
					if (FindRoomWindow.IsVisible)
					{
						FindRoomWindow.Draw();
					}
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// 試合に参加する通信処理などを行う関数
		/// </summary>
		/// <param name="roomName"></param>
		/// <param name="playerName"></param>
		private void JoinMatch(string roomName, string playerName)
		{
			if (IPAddress.TryParse(IpAddressText.Text, out _)
					&& int.TryParse(PortNumberText.Text, out _))
			{
				SocketManager.SetAddress(IpAddressText.Text, int.Parse(PortNumberText.Text));
			}
			else
			{
				IsWaitJoin = false;
				return;
			}
			IsWaitJoin = true;
			var cpmsg = new CreatePlayerMessage(playerName, roomName);
			var json = JsonConvert.SerializeObject(cpmsg);
			var (result, msg) = SocketManager.SendRecv(json);
			if (result)
			{
				Data.Player = JsonConvert.DeserializeObject<Player>(msg);
				state = State.ChangeGameScene;
			}
			else
			{
				IsWaitJoin = false;
				state = State.Start;
			}
		}
	}
}
