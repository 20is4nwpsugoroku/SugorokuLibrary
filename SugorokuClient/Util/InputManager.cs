using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;

namespace SugorokuClient.Util
{
	/// <summary>
	/// ユーザーの入力状態を管理するクラス
	/// </summary>
	public static class InputManager
	{
		/// <value> キーボード入力用のバッファー </value>
		private static byte[] KeyState { get; set; } = new byte[256];
		

		/// <value>
		/// マウスのX座標
		/// </value>
		public static int MousePosX { get; private set; }
		

		/// <value>
		/// マウスのY座標
		/// </value>
		public static int MousePosY { get; private set; }


		/// <value>
		/// マウスの左ボタンの入力状態
		/// </value>
		private static int MouseLPressedCount { get; set; } = 0;


		/// <value>
		/// マウスの左ボタンの入力状態
		/// </value>
		private static int MouseRPressedCount { get; set; } = 0;


		/// <value>
		/// マウスの左ボタンの入力状態
		/// </value>
		private static int MouseMPressedCount { get; set; } = 0;


		/// <summary>
		/// 入力状態を更新する
		/// </summary>
		public static void UpdateInput()
		{
			MouseInputUpdate();
			DX.GetHitKeyStateAll(KeyState);
		}


		/// <summary>
		/// マウスの入力状態を取得し、各種パラメータを更新する
		/// </summary>
		private static void MouseInputUpdate()
		{
			int bufX, bufY, mouseInput;
			DX.GetMousePoint(out bufX, out bufY);
			MousePosX = bufX;
			MousePosY = bufY;
			mouseInput = DX.GetMouseInput();
			MouseLPressedCount += ((mouseInput & DX.MOUSE_INPUT_LEFT) != 0) ? 1 : -MouseLPressedCount;
			MouseRPressedCount += ((mouseInput & DX.MOUSE_INPUT_RIGHT) != 0) ? 1 : -MouseRPressedCount;
			MouseMPressedCount += ((mouseInput & DX.MOUSE_INPUT_MIDDLE) != 0) ? 1 : -MouseMPressedCount;
		}


		/// <summary>
		/// マウスの左ボタンが押されているかどうか取得する
		/// </summary>
		/// <returns>true: マウスの左ボタンが押されている。もしくは、押され続けている。</returns>
		public static bool MouseL_Pressed()
		{
			return MouseLPressedCount > 0;
		}


		/// <summary>
		/// マウスの右ボタンが押されているかどうか取得する
		/// </summary>
		/// <returns>true: マウスの右ボタンが押されている。もしくは、押され続けている。</returns>
		public static bool MouseR_Pressed()
		{
			return MouseRPressedCount > 0;
		}


		/// <summary>
		/// マウスの中央ボタンが押されているかどうか取得する
		/// </summary>
		/// <returns>true: マウスの中央ボタンが押されている。もしくは、押され続けている。</returns>
		public static bool MouseM_Pressed()
		{
			return MouseMPressedCount > 0;
		}


		/// <summary>
		/// マウスの左ボタンが押されたか瞬間どうか取得する
		/// </summary>
		/// <returns>true: マウスの左ボタンが押された </returns>
		public static bool MouseL_Down()
		{
			return MouseLPressedCount == 1;
		}


		/// <summary>
		/// マウスの右ボタンが押されたか瞬間どうか取得する
		/// </summary>
		/// <returns>true: マウスの右ボタンが押された </returns>
		public static bool MouseR_Down()
		{
			return MouseRPressedCount == 1;
		}


		/// <summary>
		/// マウスの中央ボタンが押されたか瞬間どうか取得する
		/// </summary>
		/// <returns>true: マウスの中央ボタンが押された </returns>
		public static bool MouseM_Down()
		{
			return MouseMPressedCount == 1;
		}


		/// <summary>
		/// マウスのXY座標を取得する
		/// </summary>
		/// <returns>(X座標、Y座標)</returns>
		public static (int, int) GetMousePos()
		{
			return (MousePosX, MousePosY);
		}

	}
}
