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
		public Field Field { get; }

		public bool NextPlayerPrevDice { get; set; }

		/// <value> プレイヤー情報、プレイヤーIDをKeyとするPlayerクラスの辞書</value>
		public Dictionary<int, Player> Players { get; }

		/// <value> 行動の順番をPlayerIDの並びで格納する(3順分くらい) </value>
		public ListQueue<int> ActionSchedule { get; }

		/// <value> 順位を格納する </value>
		public IEnumerable<int>? Ranking { get; private set; }

		public int TopPlayerId { get; private set; }

		/// <value> 乱数生成用のクラス </value>
		private Random Rand { get; set; }

		public bool NoEnqueue { get; set; }

		#region コンストラクタ

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private MatchCore()
		{
			MatchInfo = new MatchInfo();
			Field = new Field();
			Players = new Dictionary<int, Player>();
			ActionSchedule = new ListQueue<int>();
			Rand = new Random();
		}

		/// <summary>
		/// 規定のフィールドでMatchCoreを利用する場合のコンストラクタ
		/// </summary>
		/// <param name="matchInfo">事前に準備した試合情報</param>
		/// <param name="players">プレイヤー情報の配列</param>
		public MatchCore(MatchInfo matchInfo, IReadOnlyList<Player> players) : this()
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
		public ReflectionStatus ReflectAction(PlayerAction playerAction)
		{
			// エラー処理関連
			if (MatchInfo.NextPlayerID == Constants.FinishedPlayerID)
			{
				// 誰かがゴール済みのとき、AlreadyFinishedを返す
				return ReflectionStatus.AlreadyFinished;
			}

			if (playerAction.PlayerID != MatchInfo.NextPlayerID
			    || !Players.ContainsKey(playerAction.PlayerID))
			{
				return ReflectionStatus.NotYourTurn; // 行動できるプレイヤーではないのでリターン
			}

			if (playerAction.Length < Constants.ActionMinLength
			    || playerAction.Length > Constants.ActionMaxLength)
			{
				// すごろくの値が不正な場合、エラーを返す
				return ReflectionStatus.Error;
			}

			if (NextPlayerPrevDice)
			{
				NextPlayerPrevDice = false;
				NoEnqueue = false;
				var prev = Players[playerAction.PlayerID].Position;
				var next = prev - playerAction.Length;
				Players[playerAction.PlayerID].Position = next;
				IncrementTurn();
				return ReflectionStatus.PrevDiceSuccess;
			}

			// プレイヤーの移動
			var prePos = Players[playerAction.PlayerID].Position;
			var nextPos = prePos + playerAction.Length;
			if (nextPos >= Constants.GoalPosition)
			{
				// Goal(MatchInfo.NextPlayerID);
				End(playerAction.PlayerID);
				Players[playerAction.PlayerID].Position = 30;
				return ReflectionStatus.PlayerGoal;
			}

			// イベントの実行
			Players[playerAction.PlayerID].Position = nextPos;
			Field.Squares[nextPos].Event(this, MatchInfo.NextPlayerID);

			// 次のターンに進める
			if (!NextPlayerPrevDice)
			{
				IncrementTurn();
			}

			return ReflectionStatus.NextSuccess;
		}


		/// <summary>
		/// 試合を終了させる
		/// </summary>
		private void End(int playerId)
		{
			TopPlayerId = playerId;
			Ranking = Players.OrderByDescending(p => p.Value.Position).Select(kvp => kvp.Value.PlayerID);
			MatchInfo.NextPlayerID = Constants.FinishedPlayerID;
		}


		/// <summary>
		/// 行動できるプレイヤーを次に進める
		/// </summary>
		private void IncrementTurn()
		{
			if (NoEnqueue)
			{
				NoEnqueue = false;
				return;
			}

			ActionSchedule.Enqueue(ActionSchedule.Dequeue());
			MatchInfo.NextPlayerID = ActionSchedule.Peek();
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
		private static void ResetActionSchedule(ListQueue<int> schedule, IReadOnlyList<Player> players)
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