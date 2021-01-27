using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	public class DiceTexture : Button
	{
		private int CurrentTexture { get; set; }
		private List<int> DiceTexturelist { get; set; }
		private Random Rand { get; set; }
		public int AnimationFrame { get; private set; }
		private int Dice { get; set; }




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
			AnimationFrame = 60;
			Rand = new Random();
		}


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
				CurrentTexture = DiceTexturelist[Dice - 1];
				AnimationFrame = -1;
			}
		}


		public new void Draw()
		{
			TextureAsset.Draw(CurrentTexture, x1, y1, x2 - x1, y2 - y1, DX.TRUE);
		}


		public void AnimationStart(int dice)
		{
			Dice = dice;
			AnimationFrame = 60;
		}
	}
}
