using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuClient.UI;
using SugorokuClient.Util;

namespace SugorokuClient.UI
{
	public class SugorokuFrame
	{
		private List<SugorokuSquareFrame> SquaresList { get; set; }
		private int BackgroundTextureHandle { get; set; }


		public SugorokuFrame()
		{
			BackgroundTextureHandle = TextureAsset.Register("GameBackgroundImage",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png");
			SquaresList = new List<SugorokuSquareFrame>();
			foreach (var square in Field.Squares)
			{
				SquaresList.Add(new SugorokuSquareFrame(square));
			}
		}


		public void Update()
		{
			foreach(var square in SquaresList)
			{
				square.Update();
			}
		}


		public void Draw()
		{
			TextureAsset.Draw(BackgroundTextureHandle, 0, 0, 1280, 800, DX.TRUE);
			foreach (var square in SquaresList)
			{
				square.Draw();
			}
		}

	}
}
