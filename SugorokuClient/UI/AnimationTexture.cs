using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	public class AnimationTexture
	{
		private int TextureHandle { get; set; }
		public float X { get; private set; }
		public float Y { get; private set; }
		public float Width { get; private set; }
		public float Height { get; private set; }
		public bool IsProcessingEvent { get; private set; }
		public int AnimationFrame { get; private set; }
		
		private int TargetX { get; set; }
		private int TargetY { get; set; }
		private int TargetWidth { get; set; }
		private int TargetHeight { get; set; }
		private float IncrementX { get; set; }
		private float IncrementY { get; set; }
		private float IncrementWidth { get; set; }
		private float IncrementHeight { get; set; }



		public AnimationTexture(int textureHandle, int x, int y, int width, int height)
		{
			TextureHandle = textureHandle;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			IsProcessingEvent = false;
			AnimationFrame = -1;
			TargetX = 0;
			TargetY = 0;
			TargetWidth = 0;
			TargetHeight = 0;
			IncrementX = 0;
			IncrementY = 0;
			IncrementWidth = 0;
			IncrementHeight = 0;
		}


		public void Update()
		{
			if(IsProcessingEvent && AnimationFrame > 0)
			{
				X += IncrementX;
				Y += IncrementY;
				Width += IncrementWidth;
				Height += IncrementHeight;
				AnimationFrame--;
			}
			else if (IsProcessingEvent && AnimationFrame == 0)
			{
				X = TargetX;
				Y = TargetY;
				Width = TargetWidth;
				Height = TargetHeight;
				AnimationFrame = 0;
				IsProcessingEvent = false;
			}
		}


		public void Draw()
		{
			TextureAsset.DrawModi(TextureHandle,
				(int)X, (int)Y,
				(int)(X + Width), (int)Y,
				(int)(X + Width), (int)(Y + Height),
				(int)X, (int)(Y + Height),
				DX.TRUE);
		}


		public void ChangePosition(int x, int y, int frame)
		{
			TargetX = x;
			TargetY = y;
			IncrementX = (frame != 0) ? (x - X) / frame : (x - X);
			IncrementY = (frame != 0) ? (y - Y) / frame : (y - Y);
			AnimationFrame = frame;
			IsProcessingEvent = false;
		}


		public void ChangeScale(int width, int height, int frame)
		{
			TargetWidth = width;
			TargetHeight = height;
			IncrementWidth = (frame != 0) ? (width - Width) / frame : (width - Width);
			IncrementHeight = (frame != 0) ? (height - Height) / frame : (height - Height);
			AnimationFrame = frame;
			IsProcessingEvent = false;
		}


		public void ChangePositionAndScale(int x, int y, int width, int height, int frame)
		{
			ChangePosition(x, y, frame);
			ChangeScale(width, height, frame);
		}


		public void Start()
		{
			IsProcessingEvent = true;
		}

		public void Stop()
		{
			IsProcessingEvent = false;
		}


		public void Reset()
		{
			IsProcessingEvent = false;
			TargetX = 0;
			TargetY = 0;
			TargetWidth = 0;
			TargetHeight = 0;
			IncrementX = 0;
			IncrementY = 0;
			IncrementWidth = 0;
			IncrementHeight = 0;
			AnimationFrame = -1;
		}
	}
}
