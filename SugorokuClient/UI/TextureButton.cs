using System;
using System.Collections.Generic;
using System.Text;
using SugorokuClient.UI.Interfaces;
using SugorokuClient.Util;
using DxLibDLL;

namespace SugorokuClient.UI
{
	public class TextureButton : IButton
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
		/// 右上のX座標
		/// </summary>
		public int x2 { get; private set; }


		/// <summary>
		/// 右上のY座標
		/// </summary>
		public int y2 { get; private set; }


		/// <summary>
		/// 右下のX座標
		/// </summary>
		public int x3 { get; private set; }


		/// <summary>
		/// 右下のY座標
		/// </summary>
		public int y3 { get; private set; }


		/// <summary>
		/// 左下のX座標
		/// </summary>
		public int x4 { get; private set; }


		/// <summary>
		/// 左下のY座標
		/// </summary>
		public int y4 { get; private set; }


		/// <summary>
		/// テクスチャの識別子
		/// </summary>
		private int TextureHandle { get; set; }



		#region あたり判定用

		private bool isRect { get; set; } = true;

		private DX.VECTOR v1; // 1-2のベクトル
		private DX.VECTOR v2; // 2-3のベクトル
		private DX.VECTOR v3; // 3-4のベクトル
		private DX.VECTOR v4; // 4-1のベクトル

		// 座標を設定する
		private void SetVector(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			v1.x = x2 - x1; v1.y = y2 - y1; v1.z = 0;
			v2.x = x3 - x2; v2.y = y3 - y2; v2.z = 0;
			v3.x = x4 - x3; v3.y = y4 - y3; v3.z = 0;
			v4.x = x1 - x4; v4.y = y1 - y4; v4.z = 0;
		}

		#endregion


		/// <summary>
		/// クラス内部用のコンストラクタ
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		private TextureButton(int textureHandle)
		{
			TextureHandle = textureHandle;
			isRect = true;
		}


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		public TextureButton(int textureHandle, int x, int y) : this(textureHandle)
		{		
			int width, height;
			TextureAsset.GetTextureSize(textureHandle, out width, out height);
			x1 = x;
			y1 = y;
			x2 = x + width;
			y2 = y;
			x3 = x + width;
			y3 = y + height;
			x4 = x;
			y4 = y + height;
			SetVector(x1, y1, x2, y2, x3, y3, x4, y4);
		}


		/// <summary>
		///デフォルトコンストラクタ+テクスチャの幅と高さを指定する
		/// </summary>
		/// <param name="textureHandle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public TextureButton(int textureHandle, int x, int y, int width, int height) : this(textureHandle)
		{
			x1 = x; y1 = y;
			x2 = x + width; y2 = y;
			x3 = x + width;	y3 = y + height;
			x4 = x;	y4 = y + height;
			SetVector(x1, y1, x2, y2, x3, y3, x4, y4);
		}


		/// <summary>
		/// 画像の識別子と、角の座標を指定する
		/// </summary>
		/// <param name="textureHandle">画像の識別子</param>
		/// <param name="x1">左上のX座標</param>
		/// <param name="y1">左上のY座標</param>
		/// <param name="x2">右上のX座標</param>
		/// <param name="y2">右上のY座標</param>
		/// <param name="x3">右下のX座標</param>
		/// <param name="y3">右下のY座標</param>
		/// <param name="x4">左下のX座標</param>
		/// <param name="y4">左下のY座標</param>
		public TextureButton(int textureHandle, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) : this(textureHandle)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			this.x3 = x3;
			this.y3 = y3;
			this.x4 = x4;
			this.y4 = y4;
			SetVector(x1, y1, x2, y2, x3, y3, x4, y4);
			isRect = false;
		}


		/// <summary>
		/// ボタンにマウスが重なっているかどうか
		/// </summary>
		/// <returns>ボタンにマウスが重なっているかどうか</returns>
		public bool MouseOver()
		{
			var pos = InputManager.GetMousePos();
			if (isRect)
			{
				return (pos.Item1 >= x1 && pos.Item1 <= x2
					&& pos.Item2 >= y1 && pos.Item2 <= y2);
			}
			else
			{
				DX.VECTOR mouseVec1, mouseVec2, mouseVec3, mouseVec4;
				mouseVec1.x = pos.Item1 - x1;
				mouseVec1.y = pos.Item2 - y1;
				mouseVec1.z = 0;
				mouseVec2.x = pos.Item1 - x2;
				mouseVec2.y = pos.Item2 - y2;
				mouseVec2.z = 0;
				mouseVec3.x = pos.Item1 - x3;
				mouseVec3.y = pos.Item2 - y3;
				mouseVec3.z = 0;
				mouseVec4.x = pos.Item1 - x4;
				mouseVec4.y = pos.Item2 - y4;
				mouseVec4.z = 0;
				var b1 = DX.VCross(v1, mouseVec1).z > 0;
				var b2 = DX.VCross(v2, mouseVec1).z > 0;
				var b3 = DX.VCross(v3, mouseVec1).z > 0;
				var b4 = DX.VCross(v4, mouseVec1).z > 0;
				return b1 == b2 && b2 == b3 && b3 == b4 && b4 == b1;
			}			
		}


		/// <summary>
		/// マウスが重なっている場合、重ねて色を描画する
		/// </summary>
		public void MouseOverDraw()
		{
			if (!MouseOver()) return;
			DX.SetDrawBright(180, 180, 180);
			TextureAsset.DrawModi(TextureHandle, x1, y1, x2, y2, x3, y3, x4, y4, DX.TRUE);
			DX.SetDrawBright(255, 255, 255);
		}


		/// <summary>
		/// ボタンを描画する
		/// </summary>
		public void Draw()
		{
			TextureAsset.DrawModi(TextureHandle, x1, y1, x2, y2, x3, y3, x4, y4, DX.TRUE);
		}


		/// <summary>
		/// ボタンが左クリックされたかどうか
		/// </summary>
		/// <returns>true: ボタンが左クリックされた</returns>
		public bool LeftClicked()
		{
			return MouseOver() || InputManager.MouseL_Down();
		}
	}
}
