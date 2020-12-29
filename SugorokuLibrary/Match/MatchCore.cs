using System;
using System.Collections.Generic;
using System.Text;
using SugorokuLibrary.Match;

namespace SugorokuLibrary.Match
{
	public class MatchCore
	{
		/// <value> 試合のメタ情報 </value>
		public MatchInfo MatchInfo { get; set; }

		/// <value> 試合のメタ情報 </value>
		public Field Field { get; set; }

		/// <value> 試合のメタ情報 </value>
		public Dictionary<int, Player> Players { get; set; }


		/// <value> 行動の順番をPlayerIDの並びで格納する(3順分くらい)</value>
		private Queue<int> ActionSchedule { get; set; }


		private Random Rand { get; set; }


		MatchCore()
		{
			MatchInfo = new MatchInfo();
			Field = new Field();
			Players = new Dictionary<int, Player>();
			Rand = new Random();
			ActionSchedule = new Queue<int>();
		}


		/// <summary>
		/// 試合を開始する処理
		/// </summary>
		public void Start()
		{
			
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerAction">プレイヤーの行動</param>
		public void ReflectAciton(PlayerAction playerAction)
		{
			int prePos, nextPos;
			int currentPlayerID = MatchInfo.NextPlayerID;

			// エラー処理関連
			if (playerAction.PlayerID != MatchInfo.NextPlayerID 
				|| !Players.ContainsKey(playerAction.PlayerID))
			{
				return; // 行動できるプレイヤーではないのでリターン
			}

			if (playerAction.Length <= 0 || playerAction.Length >= 7)
			{
				playerAction.Length = Rand.Next(1, 7); // Lengthが不正な場合、さいころをふる
			}

			// プレイヤーの移動
			prePos = Players[playerAction.PlayerID].Position;
			nextPos = prePos + playerAction.Length;
			if (nextPos >= Constants.GoalPosition)
			{
				Goal(currentPlayerID);
			}

			// イベントの実行
			Field.Squares[nextPos].Event.Event();

			// 次のターンに進める
			IncrementTurn();
		}


		/// <summary>
		/// 行動できるプレイヤーを次に進める処理
		/// </summary>
		public void IncrementTurn()
		{
			ActionSchedule.Enqueue(ActionSchedule.Dequeue());
			MatchInfo.NextPlayerID = ActionSchedule.Peek();
		}


		/// <summary>
		/// プレイヤーがゴールした際の処理
		/// </summary>
		/// <param name="playerID">ゴールに到着したプレイヤーのID</param>
		public void Goal(int playerID)
		{	
			// キューから行動の順番を予定を消す処理
			// プレイヤーの位置をゴール位置に合わせる
		}


		/// <summary>
		/// 試合を終了させる処理
		/// </summary>
		public void End()
		{

		}


		/// <summary>
		/// 試合終了後、試合結果のみを取得するときの関数
		/// </summary>
		public void GetResult()
		{

		}




	}
}
