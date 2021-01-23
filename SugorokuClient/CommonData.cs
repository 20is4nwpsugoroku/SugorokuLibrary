using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SugorokuLibrary;
using SugorokuClient.Scene;



namespace SugorokuClient
{
	/// <summary>
	/// 共用する変数など
	/// </summary>
	public static class CommonData
	{
		public static Socket socket;
		public static string Address { get; } = "127.0.0.1";
		public static int Port { get; } = 9500;
		public static string RoomName { get; set; } = string.Empty;
		public static string PlayerName { get; set; } = string.Empty;
		public static int PlayerNum { get; set; } = 4;
		public static Player Player { get; set; } = new Player();
		public static MatchInfo MatchInfo { get; set; } = new MatchInfo();
	}
}
