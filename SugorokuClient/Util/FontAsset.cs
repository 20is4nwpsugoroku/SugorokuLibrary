using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;

namespace SugorokuClient.Util
{
	/// <summary>
	/// DXライブラリのフォントの識別子を管理するクラス
	/// </summary>
	public static class FontAsset
	{
		/// <summary>
		/// 作成したフォントの名前と識別子の辞書
		/// </summary>
		private static Dictionary<string, int> fontStore = new Dictionary<string, int>();


		/// <summary>
		/// フォントを作成する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <param name="fontName">フォント(字体)の名前</param>
		/// <param name="size">フォントのサイズ</param>
		/// <param name="thick">フォントの太さ</param>
		/// <param name="fontType">フォントのタイプ</param>
		/// <returns></returns>
		public static int Register(string assetName,string fontName = null, int size = -1, int thick = -1, int fontType = -1)
		{
			var ret = DX.CreateFontToHandle(fontName, size, thick, fontType);
			fontStore.Add(assetName, ret);
			return ret;
		}


		/// <summary>
		/// Registerで作成したフォントを名前を指定して取得する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <returns></returns>
		public static int GetFontHandle(string assetName)
		{
			int ret = 0;
			if (!fontStore.TryGetValue(assetName, out ret))
			{
				if (!fontStore.ContainsKey("Default"))
				{
					fontStore.Add("Default", Register("Default"));
				}
				fontStore.TryGetValue("Default", out ret);
			}
			return ret;
		}


		/// <summary>
		/// アセット名を指定して文字列を描画する関数
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <param name="text">描画する文字列</param>
		/// <param name="x">描画するX座標</param>
		/// <param name="y">描画するY座標</param>
		/// <param name="color">描画する文字列の色</param>
		public static void Draw(string assetName, string text, int x, int y, uint color)
		{
			Draw(GetFontHandle(assetName), text, x, y, color);
			
		}


		/// <summary>
		/// フォントの識別子を指定して文字列を描画する関数
		/// </summary>
		/// <param name="fontHandle">作成したフォントの識別子</param>
		/// <param name="text">描画する文字列</param>
		/// <param name="x">描画するX座標</param>
		/// <param name="y">描画するY座標</param>
		/// <param name="color">描画する文字列の色</param>
		public static void Draw(int fontHandle, string text, int x, int y, uint color)
		{
			DX.DrawStringToHandle(x, y, text, color, fontHandle);
		}


		/// <summary>
		/// 描画される文字列の幅をアセット名を指定して取得する
		/// </summary>
		/// <param name="assetName">作成しフォントの名前</param>
		/// <param name="text">文字列の内容</param>
		/// <returns>描画される文字列の幅</returns>
		public static int GetDrawnTextWidth(string assetName, string text)
		{
			return GetDrawTextWidth(GetFontHandle(assetName), text);

		}


		/// <summary>
		/// 描画される文字列の幅をフォントの識別子を指定して取得する
		/// </summary>
		/// <param name="fontHandle">作成したフォントの識別子</param>
		/// <param name="text">文字列の内容</param>
		/// <returns>描画される文字列の幅</returns>
		public static int GetDrawTextWidth(int fontHandle, string text)
		{
			return DX.GetDrawStringWidthToHandle(text, text.Length, fontHandle);
		}

	}
}
