using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	public class AnimationTexture
	{
		private class Animation
		{
			public Animation(int animationFrame,
				int targetX, int targetY,
				int targetWidth, int targetHeight,
				float incrementX, float incrementY,
				float incrementWidth, float incrementHeight)
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
			public int TargetX { get; set; }
			public int TargetY { get; set; }
			public int TargetWidth { get; set; }
			public int TargetHeight { get; set; }
			public float IncrementX { get; set; }
			public float IncrementY { get; set; }
			public float IncrementWidth { get; set; }
			public float IncrementHeight { get; set; }
		}

		private class SugorokuAnimation : Animation
		{
			public SugorokuAnimation(int animationFrame,
				int targetX, int targetY,
				int targetWidth, int targetHeight,
				float incrementX, float incrementY,
				float incrementWidth, float incrementHeight,
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
		public float X { get; private set; }
		public float Y { get; private set; }
		public float Width { get; private set; }
		public float Height { get; private set; }
		public bool IsProcessingEvent { get; private set; }
		public bool IsStopped { get; private set; }
		private Queue<Animation> AnimationSchedule { get; set; }
		private Animation ProcessingAnimation { get; set; }



		//public int AnimationFrame { get; private set; }
		//private int TargetX { get; set; }
		//private int TargetY { get; set; }
		//private int TargetWidth { get; set; }
		//private int TargetHeight { get; set; }
		//private float IncrementX { get; set; }
		//private float IncrementY { get; set; }
		//private float IncrementWidth { get; set; }
		//private float IncrementHeight { get; set; }



		public AnimationTexture(int textureHandle, int x, int y, int width, int height)
		{
			TextureHandle = textureHandle;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			IsProcessingEvent = false;
			IsStopped = true;
			AnimationSchedule = new Queue<Animation>();
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
			TextureAsset.DrawModi(TextureHandle,
				(int)X, (int)Y,
				(int)(X + Width), (int)Y,
				(int)(X + Width), (int)(Y + Height),
				(int)X, (int)(Y + Height),
				DX.TRUE);
		}


		public bool IsAnimationEndFrame()
		{
			return IsProcessingEvent && ProcessingAnimation.AnimationFrame == 0;
		}


		public void AddChangePosition(int x, int y, int frame, int animationEndPosition)
		{
			var incrementX = (frame != 0) ? (x - X) / frame : (x - X);
			var incrementY = (frame != 0) ? (y - Y) / frame : (y - Y);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, (int)Width, (int)Height,
				incrementX, incrementY, 0, 0, animationEndPosition));
		}


		public void AddChangeScale(int width, int height, int frame, int animationEndPosition)
		{
			var incrementWidth = (frame != 0) ? (width - Width) / frame : (width - Width);
			var incrementHeight = (frame != 0) ? (height - Height) / frame : (height - Height);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, (int)X, (int)Y, width, height, 0, 0,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		public void AddChangePositionAndScale(int x, int y, int width, int height, int frame, int animationEndPosition)
		{
			var incrementX = (frame != 0) ? (x - X) / frame : (x - X);
			var incrementY = (frame != 0) ? (y - Y) / frame : (y - Y);
			var incrementWidth = (frame != 0) ? (width - Width) / frame : (width - Width);
			var incrementHeight = (frame != 0) ? (height - Height) / frame : (height - Height);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, width, height, incrementX, incrementY,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		public void Start()
		{
			IsProcessingEvent = true;
			IsStopped = false;
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
