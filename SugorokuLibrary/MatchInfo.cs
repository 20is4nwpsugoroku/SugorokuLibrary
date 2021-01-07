using System.Collections.Generic;

namespace SugorokuLibrary
{
	/// <summary>
	/// 試合情報のメタ情報のクラス
	/// </summary>
	public class MatchInfo
	{
		///<value>試合を識別するID</value>
		public string MatchKey { get; set; }

		///<value>プレイヤーIDの配列</value>
		public List<Player> Players { get; set; }

		///<value>ホストのプレイヤーID</value>
		public int HostPlayerID { get; set; }
		
		///<value>ユーザー追加を終了しているか</value>
		public bool CreatePlayerClosed { get; set; }

		///<value>現在のターン数</value>
		public int Turn { get; set; }

		///<value>試合の開始時間</value>
		public long StartAtUnixTime { get; set; }

		///<value>試合の終了時間</value>
		public long EndAtUnixTime { get; set; }

		///<value>次行動するプレイヤーのID</value>
		public int NextPlayerID { get; set; }
	}
}
