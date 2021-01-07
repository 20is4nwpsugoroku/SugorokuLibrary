using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.UI.Interfaces;


namespace SugorokuClient.UI
{
	public class Button : IButton
	{
		private int x1;
		private int y1;
		private int x2;
		private int y2;


		public uint MainColor { get; set; } = DX.GetColor(128, 128, 128);
		public uint TextColor { get; set; } = DX.GetColor(30, 30, 30);


		public Button(int x, int y, int width, int height)
		{
			this.x1 = x;
			this.x2 = y;
			this.x2 = width - x1;
			this.y2 = height - y1;
		}


		public Button(int x, int y, int width, int height, uint mainColor) : this(x, y, width, height)
		{
			MainColor = mainColor;
		}


		public Button(int x, int y, int width, int height, uint mainColor, uint textColor) : this(x, y, width, height, mainColor)
		{
			TextColor = textColor;
		}


		public bool MouseOver()
		{
			
		}


		public void MouseOverDraw()
		{

		}


		public void Draw()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, 1);
		}


		public void DrawFrame()
		{
			DX.DrawBox(x1, y1, x2, y2, MainColor, 0);
		}


	}
}
