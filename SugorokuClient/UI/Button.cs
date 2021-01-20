﻿using System;
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
		private int x1;

		/// <summary>
		/// 左上のY座標
		/// </summary>
		private int y1;

		/// <summary>
		/// 右下のX座標
		/// </summary>
		private int x2;

		/// <summary>
		/// 右下のY座標
		/// </summary>
		private int y2;

		
		/// <summary>
		/// ボタンの色
		/// </summary>
		public uint MainColor { get; set; } = DX.GetColor(128, 128, 128);


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
		private string Text { get; set; } = "";


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
			this.x2 = y;
			this.x2 = width - x1;
			this.y2 = height - y1;
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
		public Button(int x, int y, int width, int height, uint mainColor, string text, uint textColor) : this(x, y, width, height, mainColor)
		{
			TextColor = textColor;
		}


		public bool MouseOver()
		{
			var pos = InputManager.GetMousePos();
			return (pos.Item1 >= x1 && pos.Item1 <= x2
				&& pos.Item2 >= y1 && pos.Item2 <= y2);
		}


		public void MouseOverDraw()
		{
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 100);
			DX.DrawBox(x1, y1, x2, y2, MouseOverColor, 1);
			DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
		}


		public void MouseOverDrawWithoutMODESETTING()
		{
			DX.DrawBox(x1, y1, x2, y2, MouseOverColor, 1);
		}


		public void Draw()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, 1);
		}


		public void DrawFrame()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, 0);
		}


		public bool Clicked()
		{
			return this.MouseOver() || InputManager.MouseL_Down();
		}

	}
}
