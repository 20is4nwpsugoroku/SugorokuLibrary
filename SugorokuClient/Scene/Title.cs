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
		// 画像を左右に動かす処理のための変数を初期化
		int X = 0, XAdd = 8;
		Button button;
		TextBox textBox;


		public Title()
		{

		}

		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		public void Init()
		{
			var fonthandle = FontAsset.Register("Default", size: 20);
			button = new Button(0, 100, 100, 100, DX.GetColor(255, 0, 0), "Aue", DX.GetColor(0, 0, 255), fonthandle);
			textBox = new TextBox(0, 0, 100, 100, fonthandle);
		}


		/// <summary>
		/// シーン内の更新処理
		/// </summary>
		public void Update()
		{
			X += XAdd;
			if (X < 0 || X > 640 - 32)
			{
				XAdd = -XAdd;
			}
			textBox.Update();
		}


		/// <summary>
		/// シーン内の描画処理
		/// </summary>
		public void Draw()
		{
			// 四角を描画
			DX.DrawBox(X, 32 * 5, X + 32, 32 * 6, DX.GetColor(255, 255, 255), 1);
			// 裏画面の内容を表画面に反映する
			button.Draw();
			button.DrawFrame();
			button.MouseOverDraw();
			textBox.Draw();
			DX.ScreenFlip();
		}
	}
}
