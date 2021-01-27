﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SugorokuLibrary;
using SugorokuLibrary.Match;
using SugorokuClient.Scene;
using SugorokuClient.Util;



namespace SugorokuClient.Scene
{
	/// <summary>
	/// 共用する変数など
	/// </summary>
	public class CommonData
	{
		public CommonData()
		{
			//Address = "10.127.72.183";
			Address = "127.0.0.1";
			Port = 9500;
			RoomName = string.Empty;
			PlayerName = string.Empty;
			PlayerNum = 0;
			Player = new Player();
			MatchInfo = new MatchInfo();
			MatchManager = new MatchCommunicationManager();
			PlayerEvents = new Queue<SugorokuEvent>();
			Ranking = new List<int>();
		}

		public string Address { get; } = "127.0.0.1";
		public int Port { get; } = 9500;
		public string RoomName { get; set; } = string.Empty;
		public string PlayerName { get; set; } = string.Empty;
		public int PlayerNum { get; set; } = 4;
		public Player Player { get; set; } = new Player();
		public MatchInfo MatchInfo { get; set; } = new MatchInfo();
		public string MatchInfoStr { get; set; } = "";
		public MatchCore Match { get; set; }
		//public MatchCore MatchStr { get; set; }
		public MatchCommunicationManager MatchManager { get; set; }
		public Queue<SugorokuEvent> PlayerEvents { get; set; }
		public List<int> Ranking { get; set; }

	}
}
