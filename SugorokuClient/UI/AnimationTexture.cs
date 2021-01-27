using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;
using SugorokuLibrary;


namespace SugorokuClient.UI
{
	public class AnimationTexture
	{
		private class Animation
		{
			public Animation(int animationFrame,
				double targetX, double targetY,
				double targetWidth, double targetHeight,
				double incrementX, double incrementY,
				double incrementWidth, double incrementHeight)
			{
				AnimationFrame = animationFrame;
				TargetX = targetX;
				TargetY = targetY;
				TargetWidth = targetWidth;
				TargetHeight = targetHeight;
				IncrementX = incrementX;
				IncrementY = incrementY;
				IncrementWidth = incrementWidth;
				IncrementHeight = incrementHeight;
			}

			public int AnimationFrame { get; set; }
			public double TargetX { get; set; }
			public double TargetY { get; set; }
			public double TargetWidth { get; set; }
			public double TargetHeight { get; set; }
			public double IncrementX { get; set; }
			public double IncrementY { get; set; }
			public double IncrementWidth { get; set; }
			public double IncrementHeight { get; set; }
		}

		private class SugorokuAnimation : Animation
		{
			public SugorokuAnimation(int animationFrame,
				double targetX, double targetY,
				double targetWidth, double targetHeight,
				double incrementX, double incrementY,
				double incrementWidth, double incrementHeight,
				int animationEndPos) 
				: base(animationFrame, targetX, targetY,
				targetWidth, targetHeight,  incrementX, incrementY,
				incrementWidth, incrementHeight)
			{
				AnimationEndPos = animationEndPos;
			}

			public int AnimationEndPos { get; set; }
		}




		private int TextureHandle { get; set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public bool IsProcessingEvent { get; private set; }
		public bool IsStopped { get; private set; }
		private ListQueue<SugorokuAnimation> AnimationSchedule { get; set; }
		private SugorokuAnimation ProcessingAnimation { get; set; }



		//public double AnimationFrame { get; private set; }
		//private double TargetX { get; set; }
		//private double TargetY { get; set; }
		//private double TargetWidth { get; set; }
		//private double TargetHeight { get; set; }
		//private double IncrementX { get; set; }
		//private double IncrementY { get; set; }
		//private double IncrementWidth { get; set; }
		//private double IncrementHeight { get; set; }



		public AnimationTexture(int textureHandle, double x, double y, double width, double height)
		{
			TextureHandle = textureHandle;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			IsProcessingEvent = false;
			IsStopped = true;
			AnimationSchedule = new ListQueue<SugorokuAnimation>();
			ProcessingAnimation = new SugorokuAnimation(0, x, y, width, height, 0, 0, 0, 0, 0);
			//AnimationFrame = -1;
			//TargetX = 0;
			//TargetY = 0;
			//TargetWidth = 0;
			//TargetHeight = 0;
			//IncrementX = 0;
			//IncrementY = 0;
			//IncrementWidth = 0;
			//IncrementHeight = 0;
		}


		public void Update()
		{
			if(IsProcessingEvent && ProcessingAnimation.AnimationFrame > 0)
			{
				X += ProcessingAnimation.IncrementX;
				Y += ProcessingAnimation.IncrementY;
				Width += ProcessingAnimation.IncrementWidth;
				Height += ProcessingAnimation.IncrementHeight;
				ProcessingAnimation.AnimationFrame--;
			}
			else if (IsProcessingEvent && ProcessingAnimation.AnimationFrame == 0)
			{
				X = ProcessingAnimation.TargetX;
				Y = ProcessingAnimation.TargetY;
				Width = ProcessingAnimation.TargetWidth;
				Height = ProcessingAnimation.TargetHeight;
				ProcessingAnimation.AnimationFrame = 0;
				IsProcessingEvent = false;
			}
			else if (!IsProcessingEvent && AnimationSchedule.Count != 0 && !IsStopped)
			{
				ProcessingAnimation = AnimationSchedule.Dequeue();
				IsProcessingEvent = true;
			}
		}


		public void Draw()
		{
			TextureAsset.Draw(TextureHandle,
				(int)X, (int)Y,
				(int)(Width), (int)(Height),
				DX.TRUE);
		}


		public bool IsAnimationEndFrame()
		{
			return IsProcessingEvent && ProcessingAnimation.AnimationFrame == 0;
		}


		public int AnimationEndPos()
		{
			return ProcessingAnimation.AnimationEndPos;
		}


		public void AddChangePosition(int x, int y, int frame, int animationEndPosition)
		{
			double incrementX, incrementY, baseX, baseY, baseWidth, baseHeight;
			if (AnimationSchedule.Count != 0)
			{
				var anime = AnimationSchedule[^1];
				baseX = anime.TargetX;
				baseY = anime.TargetY;
				baseWidth = anime.TargetWidth;
				baseHeight = anime.TargetHeight;
			}
			else
			{
				baseX = X;
				baseY = Y;
				baseWidth = Width;
				baseHeight = Height;
			}
			incrementX = (frame != 0) ? (x - baseX) / frame : (x - baseX);
			incrementY = (frame != 0) ? (y - baseY) / frame : (y - baseY);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, baseWidth, baseHeight,
				incrementX, incrementY, 0, 0, animationEndPosition));
		}


		public void AddChangeScale(int width, int height, int frame, int animationEndPosition)
		{
			double incrementWidth, incrementHeight, baseX, baseY, baseWidth, baseHeight;
			if (AnimationSchedule.Count != 0)
			{
				var anime = AnimationSchedule[^1];
				baseX = anime.TargetX;
				baseY = anime.TargetY;
				baseWidth = anime.TargetWidth;
				baseHeight = anime.TargetHeight;
			}
			else
			{
				baseX = X;
				baseY = Y;
				baseWidth = Width;
				baseHeight = Height;
			}
			incrementWidth = (frame != 0) ? (width - baseWidth) / frame : (width - baseWidth);
			incrementHeight = (frame != 0) ? (height - baseHeight) / frame : (height - baseHeight);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, baseX, baseY, width, height, 0, 0,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		public void AddChangePositionAndScale(int x, int y, int width, int height, int frame, int animationEndPosition)
		{
			double incrementX, incrementY, incrementWidth, incrementHeight, baseX, baseY, baseWidth, baseHeight;
			if (AnimationSchedule.Count != 0)
			{
				var anime = AnimationSchedule[^1];
				baseX = anime.TargetX;
				baseY = anime.TargetY;
				baseWidth = anime.TargetWidth;
				baseHeight = anime.TargetHeight;
			}
			else
			{
				baseX = X;
				baseY = Y;
				baseWidth = Width;
				baseHeight = Height;
			}
			incrementX = (frame != 0) ? (x - baseX) / frame : (x - baseX);
			incrementY = (frame != 0) ? (y - baseY) / frame : (y - baseY);
			incrementWidth = (frame != 0) ? (width - baseWidth) / frame : (width - baseWidth);
			incrementHeight = (frame != 0) ? (height - baseHeight) / frame : (height - baseHeight);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, width, height, incrementX, incrementY,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		public void Start()
		{
			IsProcessingEvent = true;
			IsStopped = false;
			if (AnimationSchedule.Count != 0)
			{
				ProcessingAnimation = AnimationSchedule.Dequeue();
			}
		}

		public void Stop()
		{
			IsProcessingEvent = false;
			IsStopped = true;
		}


		public void Reset()
		{
			AnimationSchedule.Clear();
			ProcessingAnimation.AnimationFrame = -1;
			IsProcessingEvent = false;
			IsProcessingEvent = true;
		}
	}
}
