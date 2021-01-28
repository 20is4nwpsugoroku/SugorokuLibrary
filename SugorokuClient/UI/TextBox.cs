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
	/// <summary>
	/// テキストボックスのクラス
	/// </summary>
	public class TextBox : Button
	{
		/// <summary>
		/// キーボードからの文字入力用の識別子
		/// </summary>
		public int KeyInputHandle { get; private set; } = -1;


		/// <summary>
		/// このテキストボックスに対して文字の入力を行っているかどうか
		/// </summary>
		public bool IsInputActive { get; private set; } = false;


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テキストボックスの幅</param>
		/// <param name="height">テキストボックスの高さ</param>
		/// <param name="fontHandle">テキストボックスで利用するフォントの識別子</param>
		public TextBox(int x, int y, int width, int height, int fontHandle) 
			: base(x, y, width, height, DX.GetColor(255, 255, 255), string.Empty, DX.GetColor(50, 50, 50), fontHandle)
		{
			TextPosX = x + 10;
			FrameColor = TextColor;
		}


		/// <summary>
		/// テキストボックスの更新
		/// </summary>
		public void Update()
		{
			// キー入力が有効な場合
			if (IsInputActive)
			{
				int ret = DX.CheckKeyInput(KeyInputHandle);
				if (ret == 1 || ret == 2)
				{
					IsInputActive = false;
					StringBuilder stringBuilder = new StringBuilder();
					DX.GetKeyInputString(stringBuilder, KeyInputHandle);
					Text = stringBuilder.ToString();
					DX.DeleteKeyInput(KeyInputHandle);
				}
				else if (ret == -1)
				{
					IsInputActive = false;
				}
				else if (ret == 0)
				{
					IsInputActive = !(InputManager.MouseL_Down() && (!MouseOver()));
				}
			}
			else
			{
				// テキストボックスがクリックされたら入力を有効にする
				IsInputActive = LeftClicked();
				if (IsInputActive)
				{
					DX.DeleteKeyInput(KeyInputHandle);
					DX.SetActiveKeyInput(KeyInputHandle);
					KeyInputHandle = DX.MakeKeyInput(100, DX.TRUE, DX.FALSE, DX.FALSE);
					DX.SetActiveKeyInput(KeyInputHandle);
				}
			}
		}


		/// <summary>
		/// テキストボックスの描画を行う
		/// </summary>
		public new void Draw()
		{
			base.Draw();
			DrawFrame();
			if (IsInputActive)
			{
				DX.DrawKeyInputString(TextPosX, TextPosY, KeyInputHandle);
			}
		}
	}
}