using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.UI.Interfaces
{
	public interface IButton
	{
		public bool MouseOver();
		public void MouseOverDraw();
		public void Draw();
	}
}
