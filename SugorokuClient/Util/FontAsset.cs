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
		private static Dictionary<string, int> FontStore { get; set; } = new Dictionary<string, int>();


		/// <summary>
		/// フォントを作成する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <param name="fontName">フォント(字体)の名前</param>
		/// <param name="size">フォントのサイズ</param>
		/// <param name="thick">フォントの太さ</param>
		/// <param name="fontType">フォントのタイプ</param>
		/// <returns>フォントの識別子</returns>
		public static int Register(string assetName,string fontName = null, int size = -1, int thick = -1, int fontType = -1)
		{
			int ret;
			if (FontStore.ContainsKey(assetName))
			{
				ret = FontStore[assetName];
			}
			else
			{
				ret = DX.CreateFontToHandle(fontName, size, thick, fontType);
				FontStore.Add(assetName, ret);
			}
			return ret;
		}


		/// <summary>
		/// Registerで作成したフォントを名前を指定して取得する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <returns>フォントの識別子</returns>
		public static int GetFontHandle(string assetName)
		{
			int ret = 0;
			if (!FontStore.TryGetValue(assetName, out ret))
			{
				if (!FontStore.ContainsKey("Default"))
				{
					FontStore.Add("Default", Register("Default"));
				}
				FontStore.TryGetValue("Default", out ret);
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
			try
			{
				DX.DrawStringToHandle(x, y, text, color, fontHandle);
			}
			catch(Exception e)
			{
				Console.Error.WriteLine("Draw String Error :" + e);
			}
		}


		/// <summary>
		/// 描画される文字列の幅をアセット名を指定して取得する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <param name="text">文字列の内容</param>
		/// <returns>描画される文字列の幅</returns>
		public static int GetDrawTextWidth(string assetName, string text, int verticalFlag = DX.FALSE)
		{
			return GetDrawTextWidth(GetFontHandle(assetName), text, verticalFlag);

		}


		/// <summary>
		/// 描画される文字列の幅をフォントの識別子を指定して取得する
		/// </summary>
		/// <param name="fontHandle">作成したフォントの識別子</param>
		/// <param name="text">文字列の内容</param>
		/// <returns>描画される文字列の幅</returns>
		public static int GetDrawTextWidth(int fontHandle, string text, int verticalFlag = DX.FALSE)
		{
			return DX.GetDrawStringWidthToHandle(text, (int)DX.strlenDx(text), fontHandle, verticalFlag);
		}


		/// <summary>
		/// 描画される文字列の各種サイズをアセット名を指定して取得する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		/// <param name="text">文字列の内容</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		/// <param name="lineCount">行数</param>
		public static void GetDrawTextSize(string assetName, string text, out int width, out int height, out int lineCount)
		{
			GetDrawTextSize(GetFontHandle(assetName), text, out width, out height, out lineCount);
		}


		/// <summary>
		/// 描画される文字列の各種サイズをフォントの識別子を指定して取得する
		/// </summary>
		/// <param name="fontHandle">フォントの識別子</param>
		/// <param name="text">文字列の内容</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		/// <param name="lineCount">行数</param>
		public static void GetDrawTextSize(int fontHandle, string text, out int width, out int height, out int lineCount)
		{
			DX.GetDrawStringSizeToHandle(out width, out height, out lineCount, text, (int)DX.strlenDx(text), fontHandle);
		}


		/// <summary>
		/// アセット名を指定してフォントを削除する
		/// </summary>
		/// <param name="assetName">作成したフォントの名前</param>
		public static void DeleteFont(string assetName)
		{
			DX.DeleteFontToHandle(GetFontHandle(assetName));
			FontStore.Remove(assetName);
		}


		/// <summary>
		/// 登録されたすべてのフォントを削除する関数
		/// </summary>
		public static void ClearFont()
		{
			DX.InitFontToHandle();
			FontStore.Clear();
		}

	}
}
