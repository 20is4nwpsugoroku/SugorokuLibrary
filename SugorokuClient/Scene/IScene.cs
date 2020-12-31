using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	public interface IScene
	{
		public void Init();

		public void Update();

		public void Draw();
	}
}
