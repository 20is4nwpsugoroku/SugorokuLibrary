using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SugorokuLibrary;
using SugorokuClient.Scene;
using SugorokuLibrary;



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
		public static KeyValuePair<string, MatchInfo> Match { get; set; }
		public static string RoomName { get; set; } = string.Empty;
		public static string PlayerName { get; set; } = string.Empty;

		public static Player Player { get; set; } = new Player();
	}
}
