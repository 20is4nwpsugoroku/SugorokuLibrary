using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;
using SugorokuClient.UI.Interfaces;


namespace SugorokuClient.UI
{
	/// <summary>
	/// 四角いボタン
	/// </summary>
	public class Button : IButton
	{
		/// <summary>
		/// 左上のX座標
		/// </summary>
		public int x1 { get; private set; }

		/// <summary>
		/// 左上のY座標
		/// </summary>
		public int y1 { get; private set; }

		/// <summary>
		/// 右下のX座標
		/// </summary>
		public int x2 { get; private set; }

		/// <summary>
		/// 右下のY座標
		/// </summary>
		public int y2 { get; private set; }

		
		/// <summary>
		/// ボタンの色
		/// </summary>
		public uint MainColor { get; set; } = DX.GetColor(128, 128, 128);


		/// <summary>
		/// ボタンの枠の色
		/// </summary>
		public uint FrameColor { get; set; } = DX.GetColor(128, 128, 128);


		/// <summary>
		/// ボタンに半透明の色を重ねて描画する際の色
		/// </summary>
		public uint MouseOverColor { get; set; } = DX.GetColor(50, 50, 50);


		/// <summary>
		/// ボタン内に配置されるテキストの色
		/// </summary>
		public uint TextColor { get; set; } = DX.GetColor(30, 30, 30);


		/// <summary>
		/// ボタンに内に描画されるテキストの内容
		/// </summary>
		public string Text { get; set; } = "";

		/// <summary>
		/// フォントのハンドル
		/// </summary>
		protected int FontHandle { get; set; } = -1;


		/// <summary>
		/// フォントの描画位置のX座標
		/// </summary>
		protected int TextPosX { get; set; } = 0;


		/// <summary>
		/// フォン多の描画位置のY座標
		/// </summary>
		protected int TextPosY { get; set; } = 0;



		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">ボタンの幅</param>
		/// <param name="height">ボタンの高さ</param>
		public Button(int x, int y, int width, int height)
		{
			this.x1 = x;
			this.y1 = y;
			this.x2 = width + x1;
			this.y2 = height + y1;
		}


		/// <summary>
		/// デフォルトコンストラクタ+色指定
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mainColor"></param>
		public Button(int x, int y, int width, int height, uint mainColor) : this(x, y, width, height)
		{
			MainColor = mainColor;
			FrameColor = mainColor;
		}


		/// <summary>
		/// デフォルトコンストラクタ+文字列指定+色指定
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="mainColor"></param>
		/// <param name="text">ボタン上に描画する文字列</param>
		/// <param name="textColor"></param>
		public Button(int x, int y, int width, int height, uint mainColor, string text, uint textColor, int fontHandle) : this(x, y, width, height, mainColor)
		{
			TextColor = textColor;
			Text = text;
			FontHandle = fontHandle;
			var textWidth = FontAsset.GetDrawTextWidth(fontHandle, text);
			var textHeight = FontAsset.GetDrawTextWidth(fontHandle, text, DX.TRUE);
			TextPosX = x + width / 2 - textWidth / 2;
			TextPosY = y + height / 2 - textHeight / 3;
		}


		/// <summary>
		/// ボタンにマウスが重なっているかどうか
		/// </summary>
		/// <returns>true: ボタンがマウスに重なっている</returns>
		public bool MouseOver()
		{
			var pos = InputManager.GetMousePos();
			return (pos.Item1 >= x1 && pos.Item1 <= x2
				&& pos.Item2 >= y1 && pos.Item2 <= y2);
		}


		/// <summary>
		/// マウスが重なっている場合、重ねて色を描画する
		/// </summary>
		public void MouseOverDraw()
		{
			if (!MouseOver()) return;
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 100);
			DX.DrawBox(x1, y1, x2, y2, MouseOverColor, DX.TRUE);
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
		}


		/// <summary>
		/// マウスが重なっている場合、重ねて色を描画する。透明度設定なし
		/// </summary>
		public void MouseOverDrawWithoutMODESETTING()
		{
			if (!MouseOver()) return;
			DX.DrawBox(x1, y1, x2, y2, MouseOverColor, DX.TRUE);
		}


		/// <summary>
		/// マウスが重なっている場合、重ねて色をテキストにのみ描画する
		/// </summary>
		public void MouseOverDrawText()
		{
			if (!MouseOver()) return;
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 100);
			if (FontHandle >= 0)
			{
				FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, MouseOverColor);
			}
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
		}


		/// <summary>
		/// マウスが重なっている場合、重ねて色をテキストのみ描画する。透明度設定なし
		/// </summary>
		public void MouseOverDrawTextWithoutMODESETTING()
		{
			if (!MouseOver()) return;
			if (FontHandle >= 0)
			{
				FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, MouseOverColor);
			}
		}


		/// <summary>
		/// ボタンを描画する
		/// </summary>
		public void Draw()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, DX.TRUE);
			DrawText();
		}


		/// <summary>
		/// テキストのみ描画する
		/// </summary>
		public void DrawText()
		{
			if (FontHandle >= 0)
			{
				FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, TextColor);
			}
		}


		/// <summary>
		/// ボタンの枠を描画する
		/// </summary>
		public void DrawFrame()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, DX.FALSE);
		}


		/// <summary>
		/// ボタンが左クリックされたかどうか
		/// </summary>
		/// <returns>true: ボタンが左クリックされた</returns>
		public bool LeftClicked()
		{
			return this.MouseOver() && InputManager.MouseL_Down();
		}

	}
}
