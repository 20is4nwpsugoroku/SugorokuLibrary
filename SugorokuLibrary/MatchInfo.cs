﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuLibrary
{
	/// <summary>
	/// 試合情報のメタ情報のクラス
	/// </summary>
	class MatchInfo
	{
		///<value>試合を識別するID</value>
		public int MatchID { get; set; }

		///<value>プレイヤーIDの配列</value>
		public int[] PlayerIDs { get; set; }

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
