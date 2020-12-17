using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuLibrary
{
	/// <summary>
	/// プレイヤーの情報
	/// </summary>
	public class Player
	{
		///<value>プレイヤーを識別するためのID</value> 
		public int PlayerID { get; set; }

		///<value>プレイヤーの名前</value>
		public int PlayerName { get; set; }

		///<value>プレイヤーの位置</value>
		public int Position { get; set; }

	}
}
