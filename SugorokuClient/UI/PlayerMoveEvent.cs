using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	/// <summary>
	///  プレイヤーがどんな移動をしたかどうか格納するクラス
	/// </summary>
	public class PlayerMoveEvent
	{
		/// <summary>
		/// プレイヤーがダイスによって移動した位置
		/// </summary>
		public int EventStartPos { get; private set; }

		/// <summary>
		/// プレイヤーが移動したことによって発生した
		/// イベントによる影響を受けた後の位置
		/// </summary>
		public int EventEndPos { get; private set; }

		/// <summary>
		/// プレイヤーのID
		/// </summary>
		public int PlayerId { get; private set; }

		/// <summary>
		/// ダイスで出た数
		/// </summary>
		public int Dice { get; private set; }

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="startPos">移動の開始位置</param>
		/// <param name="endPos">移動の終了位置</param>
		/// <param name="playerId">プレイヤーのID</param>
		/// <param name="dice">ダイスで出た目の数</param>
		public PlayerMoveEvent(int startPos, int endPos, int playerId, int dice)
		{
			EventStartPos = startPos;
			EventEndPos = endPos;
			PlayerId = playerId;
			Dice = dice;
		}
	}
}
