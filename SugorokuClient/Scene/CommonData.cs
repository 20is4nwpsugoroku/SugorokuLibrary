using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.Match;
using System.Collections.Generic;


namespace SugorokuClient.Scene
{
	/// <summary>
	/// 共用する変数など
	/// </summary>
	public class CommonData
	{
		/// <summary>
		/// サーバーのIPアドレス
		/// </summary>
		public string Address { get; set;  }

		/// <summary>
		/// サーバーのポート
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// 部屋の名前
		/// </summary>
		public string RoomName { get; set; }

		/// <summary>
		/// 自分のプレイヤー名
		/// </summary>
		public string PlayerName { get; set; }

		/// <summary>
		/// 試合部屋の人数
		/// </summary>
		public int PlayerNum { get; set; }

		/// <summary>
		/// 自分のプレイヤー情報
		/// </summary>
		public Player Player { get; set; }

		/// <summary>
		/// 試合情報
		/// </summary>
		public MatchInfo MatchInfo { get; set; }

		/// <summary>
		/// 試合情報のJson
		/// </summary>
		public string MatchInfoStr { get; set; }

		/// <summary>
		/// 試合情報(未使用)
		/// </summary>
		public MatchCore Match { get; set; }

		/// <summary>
		/// 試合中の通信関連のクラス
		/// </summary>
		public MatchCommunicationManager MatchManager { get; set; }

		/// <summary>
		/// すべてのプレイヤーの未処理の行動情報が格納される
		/// </summary>
		public Queue<PlayerMoveEvent> PlayerEvents { get; set; }

		/// <summary>
		/// ゲーム終了時の順位を格納する
		/// </summary>
		public List<int> Ranking { get; set; }


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CommonData()
		{
			//Address = "10.127.72.183";
			Address = "127.0.0.1";
			Port = 9500;
			RoomName = string.Empty;
			PlayerName = string.Empty;
			PlayerNum = 4;
			Player = new Player();
			MatchInfo = new MatchInfo();
			MatchInfoStr = string.Empty;
			MatchManager = new MatchCommunicationManager();
			PlayerEvents = new Queue<PlayerMoveEvent>();
			Ranking = new List<int>();
		}
	}
}
