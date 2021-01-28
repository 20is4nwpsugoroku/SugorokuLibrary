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
	public class Title : IScene
	{
		/// <summary>
		/// 開始ボタン
		/// </summary>
		private TextureButton startButton { get; set; }

		/// <summary>
		/// 終了ボタン
		/// </summary>
		private TextureButton endButton { get; set; }

		/// <summary>
		/// 戻るボタン
		/// </summary>
		private TextureButton backButton { get; set; }

		/// <summary>
		/// 確定ボタン
		/// </summary>
		private TextureButton submitButton { get; set; }

		/// <summary>
		/// 部屋を作成するボタン
		/// </summary>
		private TextureButton makeRoomButton { get; set; }

		/// <summary>
		/// 部屋に参加するボタン
		/// </summary>
		private TextureButton joinRoomButton { get; set; }

		/// <summary>
		/// 部屋を探すボタン
		/// </summary>
		private TextureButton findRoomButton { get; set; }

		/// <summary>
		/// IPアドレスを反映するボタン
		/// </summary>
		private TextureButton saveIpAddress { get; set; }
		private TextBox roomName { get; set; }
		private TextBox playerName { get; set; }
		private TextBox playerNum { get; set; }
		private TextBox ipAddress { get; set; }
		private TextBox portNumber { get; set; }
		private FindRoomWindow findRoomWindow { get; set; }
		private TextureFade loadTexture { get; set; }
		private Random rand { get; set; }
		private bool isWaitJoin { get; set; }
		private int LogoImageHandle { get; set; }
		private static CommonData Data { get; set; }
		private State state { get; set; }


		enum State
		{
			Start,
			Select,
			MakeRoom,
			FindRoom,
			Load,
			Popup,
			ChangeGameScene
		}


		public Title()
		{
		}


		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		/// <param name="data">各シーンで共用するクラス</param>
		public void Init(CommonData data)
		{
			rand = new Random();
			Data = data;
			state = State.Start;
			LogoImageHandle = TextureAsset.Register("Logo", "../../../images/TitleLogo.png");
			var buttonFont = FontAsset.Register("Default", size: 40);
			var textBoxFont = FontAsset.Register("TextBox", size: 20);
			startButton = new TextureButton(TextureAsset.Register("ButtonBase1", "../../../images/ButtonBase1.png"), 440, 500, 400, 50, "はじめる", DX.GetColor(0, 103, 167), buttonFont);
			endButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 570, 400, 50, "終わる", DX.GetColor(0, 103, 167), buttonFont);
			backButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 500, 400, 50, "戻る", DX.GetColor(0, 103, 167), buttonFont);
			makeRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 570, 400, 50, "部屋を作る", DX.GetColor(0, 103, 167), buttonFont);
			joinRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 440, 640, 400, 50, "部屋に参加する", DX.GetColor(0, 103, 167), buttonFont);
			findRoomButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 1000, 570, 100, 50, "探す", DX.GetColor(0, 103, 167), buttonFont);
			submitButton = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 490, 780, 300, 50, "確定", DX.GetColor(0, 103, 167), buttonFont);
			roomName = new TextBox(490, 570, 500, 50, textBoxFont);
			roomName.FrameColor = DX.GetColor(50, 50, 50);
			roomName.Text = $"Room{rand.Next(100, 100000)}";
			playerName = new TextBox(490, 640, 500, 50, textBoxFont);
			playerName.FrameColor = DX.GetColor(50, 50, 50);
			playerName.Text = $"Player{rand.Next(100, 100000)}";
			playerNum = new TextBox(490, 710, 500, 50, textBoxFont);
			playerNum.FrameColor = DX.GetColor(50, 50, 50);
			playerNum.Text = "4";
			ipAddress = new TextBox(0, 0, 400, 50, textBoxFont);
			ipAddress.Text = Data.Address;
			portNumber = new TextBox(0, 50, 400, 50, textBoxFont);
			portNumber.Text = Data.Port.ToString();
			saveIpAddress = new TextureButton(TextureAsset.GetTextureHandle("ButtonBase1"), 0, 100, 400, 50, "IPを反映する", DX.GetColor(0, 103, 167), buttonFont);
			findRoomWindow = new FindRoomWindow(340, 180, 600, 610);
			loadTexture = new TextureFade(TextureAsset.Register("LoadImage", "../../../images/TitleLoad.png"), 590, 600, 100, 100, 60, 60, 1);
			isWaitJoin = false;
			DX.SetBackgroundColor(255, 255, 255); // 背景色を白に設定
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
			ipAddress.Update();
			portNumber.Update();
			if(saveIpAddress.LeftClicked())
			{
				if (IPAddress.TryParse(ipAddress.Text, out _)
					&& int.TryParse(portNumber.Text, out _))
				{
					SocketManager.SetAddress(ipAddress.Text, int.Parse(portNumber.Text));
				}
			}
			switch (state)
			{
				case State.Start:
					if (startButton.LeftClicked())
					{
						state = State.Select;
					}
					if (endButton.LeftClicked())
					{
						DX.DxLib_End();
						return;
					}
					break;

				case State.Select:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					if (makeRoomButton.LeftClicked())
					{
						state = State.MakeRoom;
					}
					if (joinRoomButton.LeftClicked())
					{
						state = State.FindRoom;
						roomName.Text = "";
					}
					break;

				case State.FindRoom:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					if (submitButton.LeftClicked())
					{
						if (roomName.Text.Length != 0
							&& playerName.Text.Length != 0)
						{
							state = State.Load;
							Data.PlayerName = playerName.Text;
							Data.RoomName = roomName.Text;
						}
						isWaitJoin = false;
					}
					if (findRoomButton.LeftClicked())
					{
						state = State.Popup;
						findRoomWindow.IsVisible = true;
					}
					roomName.Update();
					playerName.Update();
					break;

				case State.MakeRoom:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					if (submitButton.LeftClicked())
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
							isWaitJoin = false;
						}

					}
					roomName.Update();
					playerName.Update();
					playerNum.Update();
					break;

				case State.Load:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					if (!isWaitJoin)
					{
						isWaitJoin = true;
						Task.Run(()=>JoinMatch(Data.RoomName, Data.PlayerName));
					}
					if (!loadTexture.IsVisible())
					{
						loadTexture.Start();
					}
					loadTexture.Update();
					break;

				case State.Popup:
					findRoomWindow.Update();
					if (!findRoomWindow.IsVisible)
					{
						Data.MatchInfo = findRoomWindow.GetSelectedMatch();
						roomName.Text = Data.MatchInfo.MatchKey;
						state = State.FindRoom;
					}
					break;

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
			ipAddress.Draw();
			portNumber.Draw();
			saveIpAddress.Draw();
			switch (state)
			{
				case State.Start:
					startButton.Draw();
					startButton.DrawText();
					endButton.Draw();
					endButton.DrawText();
					break;

				case State.Select:
					backButton.Draw();
					backButton.DrawText();
					makeRoomButton.Draw();
					makeRoomButton.DrawText();
					joinRoomButton.Draw();
					joinRoomButton.DrawText();
					break;

				case State.FindRoom:
					backButton.Draw();
					backButton.DrawText();
					findRoomButton.Draw();
					findRoomButton.DrawText();
					submitButton.Draw();
					submitButton.DrawText();
					roomName.Draw();
					playerName.Draw();
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 250, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 250, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					break;

				case State.MakeRoom:
					backButton.Draw();
					backButton.DrawText();
					submitButton.Draw();
					submitButton.DrawText();
					roomName.Draw();
					playerName.Draw();
					playerNum.Draw();
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 260, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 260, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "人数", playerNum.x1 - 260, playerNum.y1 + 5, DX.GetColor(125, 125, 125));
					break;

				case State.Load:
					backButton.Draw();
					backButton.DrawText();
					loadTexture.Draw();
					break;

				case State.Popup:
					if (findRoomWindow.IsVisible)
					{
						findRoomWindow.Draw();
					}
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// 試合に参加する処理を行う関数
		/// </summary>
		/// <param name="roomName"></param>
		/// <param name="playerName"></param>
		private void JoinMatch(string roomName, string playerName)
		{
			if (IPAddress.TryParse(ipAddress.Text, out _)
					&& int.TryParse(portNumber.Text, out _))
			{
				SocketManager.SetAddress(ipAddress.Text, int.Parse(portNumber.Text));
			}
			else
			{
				isWaitJoin = false;
				return;
			}
			isWaitJoin = true;
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
				isWaitJoin = false;
				state = State.Start;
			}
		}
	}
}
