using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;
using SugorokuLibrary;


namespace SugorokuClient.UI
{
	/// <summary>
	/// 拡大縮小が可能なテクスチャ
	/// </summary>
	public class AnimationTexture
	{
		/// <summary>
		/// AnimationTextureクラス用のアニメーションの情報を保存するクラス
		/// </summary>
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

			/// <summary>
			/// アニメーションが実行されるフレーム数
			/// </summary>
			public int AnimationFrame { get; set; }

			/// <summary>
			/// アニメーション後のテクスチャのX座標
			/// </summary>
			public double TargetX { get; set; }

			/// <summary>
			/// アニメーション後のテクスチャのY座標
			/// </summary>
			public double TargetY { get; set; }

			/// <summary>
			/// アニメーション後のテクスチャの幅
			/// </summary>
			public double TargetWidth { get; set; }

			/// <summary>
			/// アニメーション後のテクスチャの高さ
			/// </summary>
			public double TargetHeight { get; set; }

			/// <summary>
			/// 各フレームで移動するX座標
			/// </summary>
			public double IncrementX { get; set; }

			/// <summary>
			/// 各フレームで移動するY座標
			/// </summary>
			public double IncrementY { get; set; }

			/// <summary>
			/// 各フレームで増える幅
			/// </summary>
			public double IncrementWidth { get; set; }

			/// <summary>
			/// 各フレームで増える高さ
			/// </summary>
			public double IncrementHeight { get; set; }
		}

		/// <summary>
		/// Animationクラスに、アニメーションが終了するすごろくの盤面上での位置を付加したクラス
		/// </summary>
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

			/// <summary>
			/// アニメーションが終了したときのすごろくでのマスの位置
			/// </summary>
			public int AnimationEndPos { get; set; }
		}


		/// <summary>
		/// テクスチャの識別子
		/// </summary>
		private int TextureHandle { get; set; }

		/// <summary>
		/// テクスチャの左上のX座標
		/// </summary>
		public double X { get; private set; }

		/// <summary>
		/// テクスチャの左上のY座標
		/// </summary>
		public double Y { get; private set; }

		/// <summary>
		/// テクスチャの幅
		/// </summary>
		public double Width { get; private set; }

		/// <summary>
		/// テクスチャの高さ
		/// </summary>
		public double Height { get; private set; }

		/// <summary>
		/// アニメーションを処理しているかどうか
		/// </summary>
		public bool IsProcessingEvent { get; private set; }

		/// <summary>
		/// アニメーションの処理が止められたかどうか
		/// </summary>
		public bool IsStopped { get; private set; }

		/// <summary>
		/// アニメーションのスケジュール
		/// </summary>
		private ListQueue<SugorokuAnimation> AnimationSchedule { get; set; }

		/// <summary>
		/// 現在処理しているアニメーション
		/// </summary>
		private SugorokuAnimation ProcessingAnimation { get; set; }


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="textureHandle">テクスチャの識別子</param>
		/// <param name="x">テクスチャの左上のX座標</param>
		/// <param name="y">テクスチャの左上のy座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
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
		}


		/// <summary>
		/// 更新処理
		/// </summary>
		public void Update()
		{
			if (IsProcessingEvent && ProcessingAnimation.AnimationFrame > 0)
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

			if (!IsProcessingEvent && AnimationSchedule.Count != 0 && !IsStopped)
			{
				ProcessingAnimation = AnimationSchedule.Dequeue();
				IsProcessingEvent = true;
			}
		}


		/// <summary>
		/// 描画処理
		/// </summary>
		public void Draw()
		{
			TextureAsset.Draw(TextureHandle, (int)X, (int)Y,
				(int)(Width), (int)(Height), DX.TRUE);
		}


		/// <summary>
		/// アニメーションが終了するフレームかどうか
		/// </summary>
		/// <returns>true: アニメーションが終了するフレーム</returns>
		public bool IsAnimationEndFrame()
		{
			return IsProcessingEvent && ProcessingAnimation.AnimationFrame == 1;
		}


		/// <summary>
		/// 現在のアニメーションが終了した際のすごろくのマス上での位置
		/// </summary>
		/// <returns>アニメーションが終了した際のすごろくのマス上での位置</returns>
		public int AnimationEndPos()
		{
			return ProcessingAnimation.AnimationEndPos;
		}


		/// <summary>
		/// AnimationScheduleにいくつ格納されているか
		/// </summary>
		/// <returns>未処理のアニメーションの数</returns>
		public int GetScheduleNum()
		{
			return AnimationSchedule.Count;
		}


		/// <summary>
		/// 座標を変化させるアニメーションを追加する
		/// </summary>
		/// <param name="x">移動後のX座標</param>
		/// <param name="y">移動後のY座標</param>
		/// <param name="frame">アニメーションを実行するフレーム数</param>
		/// <param name="animationEndPosition">アニメーション終了時のすごろくのマス上での位置</param>
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
				baseX = ProcessingAnimation.TargetX;
				baseY = ProcessingAnimation.TargetY;
				baseWidth = ProcessingAnimation.TargetWidth;
				baseHeight = ProcessingAnimation.TargetHeight;
			}
			incrementX = (frame != 0) ? (x - baseX) / frame : (x - baseX);
			incrementY = (frame != 0) ? (y - baseY) / frame : (y - baseY);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, baseWidth, baseHeight,
				incrementX, incrementY, 0, 0, animationEndPosition));
		}


		/// <summary>
		/// テクスチャの幅と高さを変化させるアニメーションを追加する
		/// </summary>
		/// <param name="width">アニメーション処理後のテクスチャの幅</param>
		/// <param name="height">アニメーションの処理後のテクスチャの高さ</param>
		/// <param name="frame">アニメーションを実行するフレーム数</param>
		/// <param name="animationEndPosition">アニメーションを実行した後のすごろくのマス上での位置</param>
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
				baseX = ProcessingAnimation.TargetX;
				baseY = ProcessingAnimation.TargetY;
				baseWidth = ProcessingAnimation.TargetWidth;
				baseHeight = ProcessingAnimation.TargetHeight;
			}
			incrementWidth = (frame != 0) ? (width - baseWidth) / frame : (width - baseWidth);
			incrementHeight = (frame != 0) ? (height - baseHeight) / frame : (height - baseHeight);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, baseX, baseY, width, height, 0, 0,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		/// <summary>
		/// 座標の移動とテクスチャの大きさの変化があるアニメーションを追加する
		/// </summary>
		/// <param name="x">移動後のX座標</param>
		/// <param name="y">移動後のY座標</param>
		/// <param name="width">アニメーション処理後のテクスチャの幅</param>
		/// <param name="height">アニメーションの処理後のテクスチャの高さ</param>
		/// <param name="frame">アニメーションを実行するフレーム数</param>
		/// <param name="animationEndPosition">アニメーションを実行した後のすごろくのマス上での位置</param>
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
				baseX = ProcessingAnimation.TargetX;
				baseY = ProcessingAnimation.TargetY;
				baseWidth = ProcessingAnimation.TargetWidth;
				baseHeight = ProcessingAnimation.TargetHeight;
			}
			incrementX = (frame != 0) ? (x - baseX) / frame : (x - baseX);
			incrementY = (frame != 0) ? (y - baseY) / frame : (y - baseY);
			incrementWidth = (frame != 0) ? (width - baseWidth) / frame : (width - baseWidth);
			incrementHeight = (frame != 0) ? (height - baseHeight) / frame : (height - baseHeight);
			AnimationSchedule.Enqueue(new SugorokuAnimation(
				frame, x, y, width, height, incrementX, incrementY,
				incrementWidth, incrementHeight, animationEndPosition));
		}


		/// <summary>
		/// アニメーションの処理を開始する
		/// </summary>
		public void Start()
		{
			IsProcessingEvent = true;
			IsStopped = false;
			if (AnimationSchedule.Count != 0 && ProcessingAnimation.AnimationFrame == 0)
			{
				ProcessingAnimation = AnimationSchedule.Dequeue();
			}
		}


		/// <summary>
		/// アニメーションの処理を停止する
		/// </summary>
		public void Stop()
		{
			IsProcessingEvent = false;
			IsStopped = true;
		}


		/// <summary>
		/// 未処理のアニメーションを削除し、アニメーションの処理を停止する
		/// </summary>
		public void Reset()
		{
			AnimationSchedule.Clear();
			ProcessingAnimation.AnimationFrame = -1;
			IsProcessingEvent = false;
			IsProcessingEvent = true;
		}
	}
}
