using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SugorokuClient.UI;
using SugorokuClient.Util;
using DxLibDLL;


namespace SugorokuClient.UI
{
	public class TextBox : Button
	{
		public int KeyInputHandle { get; private set; } = -1;


		public bool isInputActive { get; private set; } = false;


		public TextBox(int x, int y, int width, int height, int fontHandle) 
			: base(x, y, width, height, DX.GetColor(255, 255, 255), "", DX.GetColor(50, 50, 50), fontHandle)
		{
			TextPosX = x;
			FrameColor = TextColor;
		}


		public void Update()
		{
			
			if (isInputActive)
			{
				int ret = DX.CheckKeyInput(KeyInputHandle);
				if (ret == 1 || ret == 2)
				{
					isInputActive = false;
					StringBuilder stringBuilder = new StringBuilder();
					DX.GetKeyInputString(stringBuilder, KeyInputHandle);
					Text = stringBuilder.ToString();
					DX.DeleteKeyInput(KeyInputHandle);
				}
				else if (ret == -1)
				{
					isInputActive = false;
				}
				else if (ret == 0)
				{
					isInputActive = !(InputManager.MouseL_Down() && (!MouseOver()));
				}
			}
			else
			{
				isInputActive = LeftClicked();
				if (isInputActive)
				{
					DX.DeleteKeyInput(KeyInputHandle);
					DX.SetActiveKeyInput(KeyInputHandle);
					KeyInputHandle = DX.MakeKeyInput(100, DX.TRUE, DX.FALSE, DX.FALSE);
					DX.SetActiveKeyInput(KeyInputHandle);
				}
			}
		}


		public void Draw()
		{
			base.Draw();
			base.DrawFrame();
			if (isInputActive)
			{
				DX.DrawKeyInputString(TextPosX, TextPosY, KeyInputHandle);
			}
		}
	}
}