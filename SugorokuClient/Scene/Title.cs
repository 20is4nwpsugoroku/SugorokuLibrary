using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;

namespace SugorokuClient.Scene
{
	public class Title : IScene
	{
		// 画像を左右に動かす処理のための変数を初期化
		int X = 0, XAdd = 8;

		public Title()
		{

		}

		/// <summary>
		/// シーンの初期化処理
		/// </summary>
		public void Init()
		{

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
		}


		/// <summary>
		/// シーン内の描画処理
		/// </summary>
		public void Draw()
		{
			// 四角を描画
			DX.DrawBox(X, 32 * 5, X + 32, 32 * 6, DX.GetColor(255, 255, 255), 1);
			// 裏画面の内容を表画面に反映する
			DX.ScreenFlip();
		}
	}
}
