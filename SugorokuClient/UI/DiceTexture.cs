using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	/// <summary>
	/// さいころ用のテクスチャ
	/// </summary>
	public class DiceTexture : Button
	{
		/// <summary>
		/// 現在表示されているさいころのテクスチャ
		/// </summary>
		private int CurrentTexture { get; set; }

		/// <summary>
		/// 1~6までのさいころのテクスチャ
		/// </summary>
		private List<int> DiceTexturelist { get; set; }

		/// <summary>
		/// 乱数の生成に使うクラス
		/// </summary>
		private Random Rand { get; set; }

		/// <summary>
		/// アニメーションが処理されるフレーム数
		/// </summary>
		public int AnimationFrame { get; private set; }

		/// <summary>
		/// 出た目の数
		/// </summary>
		private int Dice { get; set; }


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">左上のY座標</param>
		/// <param name="width">テクスチャの幅</param>
		/// <param name="height">テクスチャの高さ</param>
		public DiceTexture(int x, int y, int width, int height)
			: base(x, y, width, height)
		{
			DiceTexturelist = new List<int>();
			DiceTexturelist.Add(TextureAsset.Register("dice1texture", "../../../images/saikoro_1.png"));
			DiceTexturelist.Add(TextureAsset.Register("dice2texture", "../../../images/saikoro_2.png"));
			DiceTexturelist.Add(TextureAsset.Register("dice3texture", "../../../images/saikoro_3.png"));
			DiceTexturelist.Add(TextureAsset.Register("dice4texture", "../../../images/saikoro_4.png"));
			DiceTexturelist.Add(TextureAsset.Register("dice5texture", "../../../images/saikoro_5.png"));
			DiceTexturelist.Add(TextureAsset.Register("dice6texture", "../../../images/saikoro_6.png"));
			CurrentTexture = DiceTexturelist[0];
			Dice = 1;
			AnimationFrame = -1;
			Rand = new Random();
		}


		/// <summary>
		/// 更新処理
		/// </summary>
		public void Update()
		{
			if (AnimationFrame > 0)
			{
				AnimationFrame--;
				if (AnimationFrame % 6 == 0)
				{
					CurrentTexture = DiceTexturelist[Rand.Next(0, 6)];
				}
			}
			else if (AnimationFrame == 0)
			{
				if (Math.Abs(Dice) < 1 || Math.Abs(Dice) > 6) Dice = 6;
				CurrentTexture = DiceTexturelist[Math.Abs(Dice) - 1];
				AnimationFrame = -1;
			}
		}


		/// <summary>
		/// 描画処理
		/// </summary>
		public new void Draw()
		{
			TextureAsset.Draw(CurrentTexture, x1, y1, x2 - x1, y2 - y1, DX.TRUE);
		}


		/// <summary>
		/// さいころのアニメーションを開始する
		/// </summary>
		/// <param name="dice">最終的な目の数</param>
		public void AnimationStart(int dice)
		{
			Dice = dice;
			AnimationFrame = 60;
		}
	}
}
