using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;

namespace SugorokuClient.UI
{
	public class TextureFade
	{
		private bool fadein { get; set; }
		private bool fadeout { get; set; }
		private bool nowFadein { get; set; }
		private bool nowFadeout { get; set; }
		private int alpha { get; set; }
		private int fadeinFrame { get; set; }
		private int fadeinFrameCount { get; set; }
		private int fadeoutFrame { get; set; }
		private int fadeoutFrameCount { get; set; }
		private int notFadeFrame { get; set; }
		private int notFadeFrameCount { get; set; }
		private int textureHandle { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public int width { get; private set; }
		public int height { get; private set; }


		public uint TextColor { get; set; } = DX.GetColor(30, 30, 30);
		public string Text { get; set; } = string.Empty;
		private int FontHandle { get; set; } = -1;
		private int TextPosX { get; set; } = 0;
		private int TextPosY { get; set; } = 0;


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


		public void Start()
		{
			nowFadein = fadein;
			nowFadeout = false;
			fadeinFrameCount = fadeinFrame;
			fadeoutFrameCount = 0;
			notFadeFrameCount = 0;
			alpha = 0;
		}


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


		private void DrawTextWithoutMODESETTING()
		{
			if (FontHandle >= 0)
			{
				FontAsset.Draw(FontHandle, Text, TextPosX, TextPosY, TextColor);
			}
		}


		public bool IsVisible()
		{
			return alpha != 0;
		}
	}
}
