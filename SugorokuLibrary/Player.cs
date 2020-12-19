namespace SugorokuLibrary
{
	/// <summary>
	/// プレイヤーの情報
	/// </summary>
	public class Player
	{
		///<value>マッチの識別キー</value>
		public string MatchKey { get; set; }

		///<value>プレイヤーを識別するためのID</value> 
		public int PlayerID { get; set; }

		///<value>プレイヤーの名前</value>
		public string PlayerName { get; set; }

		public bool IsHost { get; set; }

		///<value>プレイヤーの位置</value>
		public int Position { get; set; }
	}
}