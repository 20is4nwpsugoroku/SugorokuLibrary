using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;


namespace SugorokuClient.Util
{
	/// <summary>
	/// DXライブラリのテクスチャの識別子を管理するクラス
	/// </summary>
	public static class TextureAsset
	{
		/// <summary>
		/// 作成したテクスチャの名前と識別子の辞書
		/// </summary>
		private static Dictionary<string, int> TextureStore { get; set; } = new Dictionary<string, int>();


		/// <summary>
		/// テクスチャを作成する
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		/// <param name="path">画像のパス</param>
		/// <returns></returns>
		public static int Register(string assetName, string path)
		{
			var ret = DX.LoadGraph(path);
			TextureStore.Add(assetName, ret);
			return ret;
		}


		/// <summary>
		/// Registerで作成したテクスチャを名前を指定して取得する
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		/// <returns></returns>
		public static int GetTextureHandle(string assetName)
		{
			int ret = -1;
			if (!TextureStore.TryGetValue(assetName, out ret))
			{
				if (!TextureStore.ContainsKey("Default"))
				{
					TextureStore.Add("Default", DX.MakeGraph(1, 1));
				}
				TextureStore.TryGetValue("Default", out ret);
			}
			return ret;
		}


		/// <summary>
		/// アセット名を指定して画像を描画する関数
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		/// <param name="x">描画するX座標</param>
		/// <param name="y">描画するY座標</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void Draw(string assetName, int x, int y, int transFlag)
		{
			Draw(GetTextureHandle(assetName), x, y, transFlag);
		}


		/// <summary>
		/// テクスチャの識別子を指定して画像を描画する関数
		/// </summary>
		/// <param name="textureHandle">作成したテクスチャの識別子</param>
		/// <param name="x">描画するX座標</param>
		/// <param name="y">描画するY座標</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void Draw(int textureHandle, int x, int y, int transFlag)
		{
			DX.DrawGraph(x, y, textureHandle, transFlag);
		}


		/// <summary>
		/// アセット名とサイズを指定して画像を描画する関数
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void Draw(string assetName, int x, int y, int width, int height, int transFlag)
		{
			DX.DrawModiGraph(x, y, x + width, y, x + width, y + height, x, y + height, GetTextureHandle(assetName), transFlag);
		}


		/// <summary>
		/// テクスチャの識別子とサイズを指定して画像を描画する関数
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void Draw(int textureHandle, int x, int y, int width, int height, int transFlag)
		{
			DX.DrawModiGraph(x, y, x + width, y, x + width, y + height, x, y + height, textureHandle, transFlag);
		}


		/// <summary>
		/// アセット名と角の4点を指定して画像を描画する関数
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		/// <param name="x1">左上のX座標</param>
		/// <param name="y1">左上のY座標</param>
		/// <param name="x2">右上のX座標</param>
		/// <param name="y2">右上のY座標</param>
		/// <param name="x3">右下のX座標</param>
		/// <param name="y3">右下のY座標</param>
		/// <param name="x4">左下のX座標</param>
		/// <param name="y4">左下のY座標</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void DrawModi(string assetName, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int transFlag)
		{
			DX.DrawModiGraph(x1, y1, x2, y2, x3, y3, x4, y4, GetTextureHandle(assetName), transFlag);
		}


		/// <summary>
		/// テクスチャの識別子と角の4点を指定して画像を描画する関数
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x1">左上のX座標</param>
		/// <param name="y1">左上のY座標</param>
		/// <param name="x2">右上のX座標</param>
		/// <param name="y2">右上のY座標</param>
		/// <param name="x3">右下のX座標</param>
		/// <param name="y3">右下のY座標</param>
		/// <param name="x4">左下のX座標</param>
		/// <param name="y4">左下のY座標</param>
		/// <param name="transFlag">画像の透明度を有効にするかどうか</param>
		public static void DrawModi(int textureHandle, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int transFlag)
		{
			DX.DrawModiGraph(x1, y1, x2, y2, x3, y3, x4, y4, textureHandle, transFlag);
		}


		/// <summary>
		/// テクスチャの名前を指定してサイズを取得する
		/// </summary>
		/// <param name="assetName">テクスチャの名前</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		public static void GetTextureSize(string assetName, out int width, out int height)
		{
			GetTextureSize(GetTextureHandle(assetName), out width, out height);
		}


		/// <summary>
		/// テクスチャの識別子を指定してサイズを取得する
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		public static void GetTextureSize(int textureHandle, out int width, out int height)
		{
			DX.GetGraphSize(textureHandle, out width, out height);
		}


		/// <summary>
		/// アセット名を指定してテクスチャを削除する
		/// </summary>
		/// <param name="assetName">作成したテクスチャの名前</param>
		public static void DeleteTexture(string assetName)
		{
			DX.DeleteGraph(GetTextureHandle(assetName));
			TextureStore.Remove(assetName);
		}


		/// <summary>
		/// 登録されたすべてのテクスチャを削除する関数
		/// </summary>
		public static void ClearTexture()
		{
			DX.InitGraph();
			TextureStore.Clear();
		}

	}
}
