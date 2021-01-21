using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DxLibDLL;
using SugorokuClient.UI;
using SugorokuClient.Util;

namespace SugorokuClient.Scene
{
	public class Title : IScene
	{
		#region Temp
		private const string DummyImagePath = "E:\\workspace\\devs\\SugorokuLibrary\\dev\\haruto8631\\SugorokuClient\\images\\Image1.png";
		#endregion

		private int LogoImageHandle;
		private const string LogoImagePath = DummyImagePath;
		private const int LogoImageHeight = 300;
		private const int LogoImageWidth = 300;
		private const int LogoImageX = 490;
		private const int LogoImageY = 90;

		


		private TextureButton startButton { get; set; }
		private TextureButton endButton { get; set; }
		private TextureButton backButton { get; set; }
		private TextureButton makeRoomButton { get; set; }
		private TextureButton joinRoomButton { get; set; }
		private TextureButton findRoomButton { get; set; }





		private TextBox hostName { get; set; }
		private TextBox roomName { get; set; }
		private TextBox playerName { get; set; }
		private TextBox playerNum { get; set; }
		private TextBox textBox { get; set; }


		enum State
		{
			Start,
			Select,
			MakeRoom,
			FindRoom,
			Load,
			Popup
		}

		private State state;


		public Title()
		{

		}

		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		public void Init()
		{
			state = State.Start;
			LogoImageHandle = TextureAsset.Register("Logo", LogoImagePath);
			var buttonFont = FontAsset.Register("Default", size: 40);
			var textBoxFont = FontAsset.Register("TextBox", size: 20);
			startButton = new TextureButton(TextureAsset.Register("button1Base", DummyImagePath), 540, 500, 200, 50, "はじめる", DX.GetColor(222, 222, 222), buttonFont);
			endButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 540, 570, 200, 50, "終わる", DX.GetColor(222, 222, 222), buttonFont);
			backButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 540, 500, 200, 50, "戻る", DX.GetColor(222, 222, 222), buttonFont);
			makeRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 490, 570, 300, 50, "部屋を作る", DX.GetColor(222, 222, 222), buttonFont);
			joinRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 490, 640, 300, 50, "部屋に参加する", DX.GetColor(222, 222, 222), buttonFont);
			findRoomButton = new TextureButton(TextureAsset.GetTextureHandle("button1Base"), 1040, 640, 100, 50, "探す", DX.GetColor(222, 222, 222), buttonFont);
			hostName = new TextBox(490, 570, 500, 50, textBoxFont);
			roomName = new TextBox(490, 640, 500, 50, textBoxFont);
			playerName = new TextBox(490, 710, 500, 50, textBoxFont);
			playerNum = new TextBox(490, 780, 500, 50, textBoxFont);



			//button = new Button(160, 160, 200, 200, DX.GetColor(255, 0, 0), "Aue", DX.GetColor(0, 0, 255), fonthandle);
			//textBox = new TextBox(0, 0, 160, 160, fonthandle);
			DX.SetBackgroundColor(0, 255, 255); // 背景色を白に設定
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
					hostName.Update();
					roomName.Update();
					playerName.Update();
					break;
				case State.MakeRoom:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					hostName.Update();
					roomName.Update();
					playerName.Update();
					playerNum.Update();
					break;
				case State.Load:
					if (backButton.LeftClicked())
					{
						state = State.Start;
					}
					break;
				case State.Popup:
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
					hostName.Draw();
					roomName.Draw();
					playerName.Draw();
					FontAsset.Draw("button1Base", "ホスト名", hostName.x1 -250, hostName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 250, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 250, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					break;
				case State.MakeRoom:
					backButton.Draw();
					backButton.DrawText();
					hostName.Draw();
					roomName.Draw();
					playerName.Draw();
					playerNum.Draw();
					FontAsset.Draw("button1Base", "ホスト名", hostName.x1 - 260, hostName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "部屋名", roomName.x1 - 260, roomName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "プレイヤー名", playerName.x1 - 260, playerName.y1 + 5, DX.GetColor(125, 125, 125));
					FontAsset.Draw("button1Base", "人数", playerNum.x1 - 260, playerNum.y1 + 5, DX.GetColor(125, 125, 125));
					break;
				case State.Load:
					backButton.Draw();
					backButton.DrawText();
					break;
				case State.Popup:
					break;
				default:
					break;
			}
		}
	}
}
