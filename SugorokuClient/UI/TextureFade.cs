using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;

namespace SugorokuClient.UI
{
	/// <summary>
	/// フェードイン、フェードアウトを行うことができるテクスチャ
	/// </summary>
	public class TextureFade
	{
		/// <summary>
		/// フェードインが有効かどうか
		/// </summary>
		private bool fadein { get; set; }

		/// <summary>
		/// フェードアウトが有効かどうか
		/// </summary>
		private bool fadeout { get; set; }

		/// <summary>
		/// 現在フェードインの処理が行われているかどうか
		/// </summary>
		private bool nowFadein { get; set; }

		/// <summary>
		/// 現在フェードアウトの処理が行われているかどうか
		/// </summary>
		private bool nowFadeout { get; set; }

		/// <summary>
		/// フェードイン、アウト中の透明度の値
		/// </summary>
		private int alpha { get; set; }

		/// <summary>
		/// フェードインの演出が有効なフレーム数
		/// </summary>
		private int fadeinFrame { get; set; }

		/// <summary>
		/// フェードインの演出が何フレーム目か
		/// </summary>
		private int fadeinFrameCount { get; set; }

		/// <summary>
		/// フェードインの演出が有効なフレーム数
		/// </summary>
		private int fadeoutFrame { get; set; }

		/// <summary>
		/// フェードインの演出が何フレーム目か
		/// </summary>
		private int fadeoutFrameCount { get; set; }

		/// <summary>
		/// フェードイン、アウトの間のフレーム数
		/// </summary>
		private int notFadeFrame { get; set; }

		/// <summary>
		/// フェードイン、アウトの間の何フレーム目か
		/// </summary>
		private int notFadeFrameCount { get; set; }

		/// <summary>
		/// テクスチャの識別子
		/// </summary>
		private int textureHandle { get; set; }

		/// <summary>
		/// テクスチャの左上のX座標
		/// </summary>
		public int x { get; set; }

		/// <summary>
		/// テクスチャの左上のY座標
		/// </summary>
		public int y { get; set; }

		/// <summary>
		/// テクスチャの幅
		/// </summary>
		public int width { get; private set; }

		/// <summary>
		/// テクスチャの高さ
		/// </summary>
		public int height { get; private set; }

		/// <summary>
		/// テキストの色
		/// </summary>
		public uint TextColor { get; set; } = DX.GetColor(30, 30, 30);

		/// <summary>
		/// テキストの文字列
		/// </summary>
		public string Text { get; set; } = string.Empty;

		/// <summary>
		/// テキストに使用するフォントの識別子
		/// </summary>
		private int FontHandle { get; set; } = -1;

		/// <summary>
		/// テキストの描画位置の左上のX座標
		/// </summary>
		private int TextPosX { get; set; } = 0;

		/// <summary>
		/// テキストの描画位置の右上のY座標
		/// </summary>
		private int TextPosY { get; set; } = 0;


		/// <summary>
		/// テクスチャのみ利用する場合のコンストラクタ
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
		/// <param name="fadeinFrame">フェードインのフレーム数</param>
		/// <param name="fadeoutFrame">フェードアウトのフレーム数</param>
		/// <param name="notFadeFrame">フェードインとフェードアウトの間のフレーム数</param>
		public TextureFade(int textureHandle, int x, int y, int width, int height,
			uint fadeinFrame = 0, uint fadeoutFrame = 0, uint notFadeFrame = 0)
		{
			this.textureHandle = textureHandle;
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.fadeinFrame = (int)fadeinFrame;
			fadeinFrameCount = this.fadeinFrame;
			this.fadeoutFrame =(int)fadeoutFrame;
			fadeoutFrameCount = 0;
			this.notFadeFrame = (int)notFadeFrame;
			notFadeFrameCount = 0;
			fadein = fadeinFrame != 0;
			fadeout = fadeoutFrame != 0;
			alpha = 0;
		}


		/// <summary>
		/// テクスチャと文字列を利用する場合のコンストラクタ
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
		/// <param name="fadeinFrame">フェードインのフレーム数</param>
		/// <param name="fadeoutFrame">フェードアウトのフレーム数</param>
		/// <param name="notFadeFrame">フェードインとフェードアウトの間のフレーム数</param>
		/// <param name="fontHandle">フォントの識別子</param>
		/// <param name="text">描画する文字列</param>
		/// <param name="textColor">描画する文字の色</param>
		public TextureFade(int textureHandle, int x, int y, int width, int height,
			uint fadeinFrame = 0, uint fadeoutFrame = 0, uint notFadeFrame = 0,
			int fontHandle = -1, string text = "", uint textColor = 0)
			: this(textureHandle, x, y, width, height,fadeinFrame, fadeoutFrame, notFadeFrame)
		{
			Text = text;
			TextColor = textColor;
			FontHandle = fontHandle;
			int textWidth, textHeight;
			FontAsset.GetDrawTextSize(fontHandle, text, out textWidth, out textHeight, out _);
			TextPosX = x + width / 2 - textWidth / 2;
			TextPosY = y + height / 2 - textHeight / 2;
		}


		/// <summary>
		/// フェードインから演出をスタートする関数
		/// </summary>
		public void Start()
		{
			nowFadein = fadein;
			nowFadeout = false;
			fadeinFrameCount = fadeinFrame;
			fadeoutFrameCount = 0;
			notFadeFrameCount = 0;
			alpha = 0;
		}


		/// <summary>
		/// 演出の更新を行う関数
		/// </summary>
		public void Update()
		{
			if (nowFadein)
			{
				fadeinFrameCount--;
				nowFadein = fadeinFrameCount > 0;
				alpha = 255 - (255 * fadeinFrameCount / fadeinFrame);
			}
			else if (nowFadeout)
			{
				fadeoutFrameCount++;
				nowFadeout = fadeoutFrame > fadeoutFrameCount;
				alpha =  255 - (255 * fadeoutFrameCount / fadeoutFrame);
			}
			else if (notFadeFrame != 0)
			{
				notFadeFrameCount++;
				nowFadeout = notFadeFrame < notFadeFrameCount;
				alpha = 255;
			}
			else if (notFadeFrame == 0)
			{
				alpha = 255;
			}
		}


		/// <summary>
		/// 描画を行う関数
		/// </summary>
		public void Draw()
		{
			if (fadeoutFrame <= fadeoutFrameCount) return;
			if (nowFadein || nowFadeout)
			{
				DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, alpha);
				DX.DrawModiGraph(x, y, x + width, y, x + width, y + height, x, y + height, textureHandle, DX.TRUE);
				DrawTextWithoutMODESETTING();
				DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
			}
			else
			{
				DX.DrawModiGraph(x, y, x + width, y, x + width, y + height, x, y + height, textureHandle, DX.TRUE);
				DrawTextWithoutMODESETTING();
			}
		}


		/// <summary>
		/// 文字列のみ描画する関数
		/// </summary>
		public void DrawText()
		{
			if (FontHandle >= 0)
			{
				if (fadeoutFrame <= fadeoutFrameCount) return;
				if (nowFadein || nowFadeout)
				{
					DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, alpha);
					FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, TextColor);
					DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0);
				}
				else
				{
					FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, TextColor);
				}
			}
		}


		/// <summary>
		/// 文字列のみ描画する関数(透明度設定なし)
		/// </summary>
		private void DrawTextWithoutMODESETTING()
		{
			if (FontHandle >= 0)
			{
				FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, TextColor);
			}
		}


		/// <summary>
		/// 透明度が0以外で可視かどうか
		/// </summary>
		/// <returns></returns>
		public bool IsVisible()
		{
			return alpha != 0;
		}
	}
}
