using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuClient.UI;
using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuClient.Scene
{
	public class Title : IScene
	{
		private int LogoImageHandle;
		private const int LogoImageHeight = 300;
		private const int LogoImageWidth = 300;
		private const int LogoImageX = 490;
		private const int LogoImageY = 90;

		


		private TextureButton startButton { get; set; }
		private TextureButton endButton { get; set; }
		private TextureButton backButton { get; set; }
		private TextureButton submitButton { get; set; }
		private TextureButton makeRoomButton { get; set; }
		private TextureButton joinRoomButton { get; set; }
		private TextureButton findRoomButton { get; set; }

		private TextBox roomName { get; set; }
		private TextBox playerName { get; set; }
		private TextBox playerNum { get; set; }

		private FindRoomWindow findRoomWindow { get; set; }

		private TextureFade loadTexture { get; set; }

		private Random rand { get; set; }
		private bool isWaitJoin { get; set; }
		private static CommonData Data { get; set; }


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

		private State state;


		public Title()
		{

		}

		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		public void Init(CommonData data)
		{
			rand = new Random();
			Data = data;
			state = State.Start;
			LogoImageHandle = TextureAsset.Register("Logo", "../../../images/TitleLogo.png");
			var buttonFont = FontAsset.Register("Default", size: 40);
			var textBoxFont = FontAsset.Register("TextBox", size: 20);
			startButton = new TextureButton(TextureAsset.Register("button1Base", "../../../images/StartButton.png"), 540, 500, 200, 50, "はじめる", DX.GetColor(222, 222, 222), buttonFont);
			endButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 540, 570, 200, 50, "終わる", DX.GetColor(222, 222, 222), buttonFont);
			backButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 540, 500, 200, 50, "戻る", DX.GetColor(222, 222, 222), buttonFont);
			makeRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 490, 570, 300, 50, "部屋を作る", DX.GetColor(222, 222, 222), buttonFont);
			joinRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 490, 640, 300, 50, "部屋に参加する", DX.GetColor(222, 222, 222), buttonFont);
			findRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 1000, 570, 100, 50, "探す", DX.GetColor(222, 222, 222), buttonFont);
			submitButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 540, 780, 200, 50, "確定", DX.GetColor(222, 222, 222), buttonFont);
			roomName = new TextBox(490, 570, 500, 50, textBoxFont);
			roomName.FrameColor = DX.GetColor(50, 50, 50);
			roomName.Text = $"Room{rand.Next(100, 100000)}";
			playerName = new TextBox(490, 640, 500, 50, textBoxFont);
			playerName.FrameColor = DX.GetColor(50, 50, 50);
			playerName.Text = $"Player{rand.Next(100, 100000)}";
			playerNum = new TextBox(490, 710, 500, 50, textBoxFont);
			playerNum.FrameColor = DX.GetColor(50, 50, 50);
			playerNum.Text = "4";
			findRoomWindow = new FindRoomWindow(340, 180, 600, 610);
			loadTexture = new TextureFade(TextureAsset.GetTextureHandle("button1Base"), 590, 600, 100, 100, 60, 60, 1);
			isWaitJoin = false;
			SocketManager.Connect(Data.Address, Data.Port);
			DX.SetBackgroundColor(255, 255, 255); // 背景色を白に設定
		}


		/// <summary>
		/// シーン内の更新処理
		/// </summary>
		public void Update()
		{
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
						findRoomWindow.isVisible = true;
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
					if (!findRoomWindow.isVisible)
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
			TextureAsset.Draw(LogoImageHandle, LogoImageX, LogoImageY, LogoImageWidth, LogoImageHeight, DX.FALSE);
			
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
					if (findRoomWindow.isVisible)
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
			isWaitJoin = true;
			var cpmsg = new CreatePlayerMessage(playerName, roomName);
			var json = JsonConvert.SerializeObject(cpmsg);
			var (result, msg) = SocketManager.SendRecv(json);
			if (result)
			{
				Data.Player = JsonConvert.DeserializeObject<Player>(msg);
				DX.putsDx(json);
				DX.putsDx(msg);
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
