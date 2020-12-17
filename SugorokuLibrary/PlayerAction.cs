using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuLibrary
{
	/// <summary>
	/// プレイヤーの行動のクラス
	/// </summary>
	public class PlayerAction
	{
		///<value>プレイヤーの識別ID</value>
		public int PlayerID { get; set; }

		///<value>プレイヤーが移動する距離</value>
		public int Legnth { get; set; }

	}
}
