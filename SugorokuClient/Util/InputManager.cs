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

		/// <summary>
		/// 入力状態を更新する
		/// </summary>
		public static void UpdateInput()
		{
			DX.GetHitKeyStateAll(KeyState);
		}
	}
}
