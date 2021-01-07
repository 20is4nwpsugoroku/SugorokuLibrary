using System;
using System.Collections.Generic;
using System.Linq;

namespace SugorokuLibrary.Match
{
	/// <summary>
	/// 一つの試合を管理するクラス
	/// </summary>
	public class MatchCore
	{
		/// <value> 試合のメタ情報 </value>
		public MatchInfo MatchInfo { get; set; }

		/// <value> すごろくのマスの情報 </value>
		public Field Field { get; set; }

		/// <value> プレイヤー情報、プレイヤーIDをKeyとするPlayerクラスの辞書</value>
		public Dictionary<int, Player> Players { get; set; }

		/// <value> 行動の順番をPlayerIDの並びで格納する(3順分くらい) </value>
		public Queue<int> ActionSchedule { get; set; }

		/// <value> 順位を格納する </value>
		public Queue<int> Ranking { get; private set; }

		/// <value> 乱数生成用のクラス </value>
		private Random Rand { get; set; }


		#region コンストラクタ

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		MatchCore()
		{
			MatchInfo = new MatchInfo();
			Field = new Field();
			Players = new Dictionary<int, Player>();
			ActionSchedule = new Queue<int>();
			Ranking = new Queue<int>();
			Rand = new Random();
		}

		/// <summary>
		/// 規定のフィールドでMatchCoreを利用する場合のコンストラクタ
		/// </summary>
		/// <param name="matchInfo">事前に準備した試合情報</param>
		/// <param name="players">プレイヤー情報の配列</param>
		public MatchCore(MatchInfo matchInfo, Player[] players) : this()
		{
			MatchInfo = matchInfo;
			ResetPlayerInfo(players);
		}

		public MatchCore(MatchInfo matchInfo) : this()
		{
			MatchInfo = matchInfo;
			ResetPlayerInfo(matchInfo.Players);
		}

		#endregion


		#region 試合の進行関連の関数

		/// <summary>
		/// 試合を開始する処理
		/// </summary>
		public void Start()
		{
			MatchInfo.Turn++;
			MatchInfo.NextPlayerID = ActionSchedule.Peek();
		}


		/// <summary>
		/// 引数のプレイヤーの行動を試合に反映する
		/// </summary>
		/// <param name="playerAction">プレイヤーの行動</param>
		public void ReflectAction(PlayerAction playerAction)
		{
			// エラー処理関連
			if (playerAction.PlayerID != MatchInfo.NextPlayerID 
				|| !Players.ContainsKey(playerAction.PlayerID))
			{
				return; // 行動できるプレイヤーではないのでリターン
			}
			if (playerAction.Length < Constants.ActionMinLength
				|| playerAction.Length > Constants.ActionMaxLength)
			{
				playerAction.Length = Rand.Next(Constants.ActionMinLength, 7); // Lengthが不正な場合、さいころをふる
			}

			// プレイヤーの移動
			var prePos = Players[playerAction.PlayerID].Position;
			var nextPos = prePos + playerAction.Length;
			if (nextPos >= Constants.GoalPosition)
			{
				Goal(MatchInfo.NextPlayerID);
			}

			// イベントの実行
			Field.Squares[nextPos].Event.Event();

			// 次のターンに進める
			if (ActionSchedule.Count != 0)
			{
				IncrementTurn();
			}
		}


		/// <summary>
		/// 試合を終了させる
		/// </summary>
		public void End()
		{
			MatchInfo.NextPlayerID = Constants.InvalidPlayerID;
		}


		/// <summary>
		/// 行動できるプレイヤーを次に進める
		/// </summary>
		private void IncrementTurn()
		{
			ActionSchedule.Enqueue(ActionSchedule.Dequeue());
			MatchInfo.NextPlayerID = ActionSchedule.Peek();
		}


		/// <summary>
		/// プレイヤーがゴールした際の処理
		/// </summary>
		/// <param name="playerID">ゴールに到着したプレイヤーのID</param>
		private void Goal(int playerID)
		{
			// キューから行動の順番を予定を消す処理
			var new_schedule = new Queue<int>();
			foreach (var element in ActionSchedule)
			{
				if (element == playerID) continue;
				new_schedule.Enqueue(element);
			}
			ActionSchedule.Clear();
			ActionSchedule = new_schedule;

			// プレイヤーの位置をゴール位置に合わせる
			Players[playerID].Position = Constants.GoalPosition;
			Ranking.Enqueue(playerID);
			
			// 全プレイヤーがゴールしたので、終了処理に移る
			if (new_schedule.Count == 0)
			{
				End();
			}
		}

		#endregion 


		#region プレイヤー情報の初期化関連

		/// <summary>
		/// プレイヤーに関連する情報を初期化する
		/// </summary>
		/// <param name="players"></param>
		private void ResetPlayerInfo(IReadOnlyList<Player> players)
		{
			if (!players.Any())
			{
				throw new ArgumentNullException();
			}
			ResetPlayersDictionary(players);
			ResetActionSchedule(ActionSchedule, players);
		}


		/// <summary>
		/// ActionScheduleを初期化する
		/// </summary>
		/// <param name="schedule">PlayerIDの並びで行動の順番を格納するキュー</param>
		/// <param name="players">Player情報の配列、この配列の順番が行動の順番となる</param>
		private static void ResetActionSchedule(Queue<int> schedule, IReadOnlyList<Player> players)
		{
			schedule.Clear();
			for (var i = 0; i < players.Count * 3; i++)
			{
				schedule.Enqueue(players[i % players.Count].PlayerID);
			}
		}


		/// <summary>
		/// DictionaryのPlayersを初期化する
		/// </summary>
		/// <param name="players"></param>
		private void ResetPlayersDictionary(IEnumerable<Player> players)
		{
			Players.Clear();
			foreach (var player in players)
			{
				Players.Add(player.PlayerID, player);
			}
		}

		#endregion

	}
}
