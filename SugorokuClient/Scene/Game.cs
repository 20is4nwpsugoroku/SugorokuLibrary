using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.UI;
using SugorokuClient.Util;

namespace SugorokuClient.Scene
{
	public class Game : IScene
	{
		private SugorokuFrame SugorokuFrame { get; set; }


		public Game()
		{
		}

		public void Init()
		{
			DX.SetBackgroundColor(255, 255, 255);
			SugorokuFrame = new SugorokuFrame();
		}

		public void Update()
		{
			SugorokuFrame.Update();
		}

		public void Draw()
		{
			SugorokuFrame.Draw();
		}
	}
}
