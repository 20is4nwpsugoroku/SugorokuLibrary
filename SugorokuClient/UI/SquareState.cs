using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	/// <summary>
	/// すごろくのマスの状態
	/// </summary>
	public class SquareState
	{
		/// <summary>
		/// マスにいるプレイヤーの人数
		/// </summary>
		public int PlayerNum { get; private set; }

		/// <summary>
		/// プレイヤーのIDとそのプレイヤーがマスに存在するかどうかの辞書
		/// </summary>
		public Dictionary<int, bool> PlayerExists { get; private set; }

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="PlayerIds">プレイヤーIDのリスト</param>
		public SquareState(List<int> PlayerIds)
		{
			PlayerNum = 0;
			PlayerExists = new Dictionary<int, bool>();
			foreach(var id in PlayerIds)
			{
				PlayerExists.Add(id, false);
			}
		}


		/// <summary>
		/// プレイヤーのIDを指定し、マスにいるかどうかの状態を変更する関数
		/// </summary>
		/// <param name="playerId">プレイヤーID</param>
		/// <param name="exists">マスに存在するかどうか true: 存在する</param>
		public void ExistsControl (int playerId, bool exists)
		{
			PlayerExists[playerId] = exists;
			PlayerNum += (exists) ? 1 : -1;
		}

		/// <summary>
		/// マスの上のプレイヤーを描画するのに、
		/// どのプレイヤーをどの位置で描画するべきかどうかを返す関数
		/// </summary>
		/// <param name="centerX">中心のX座標</param>
		/// <param name="centerY">中心のY座標</param>
		/// <returns>(プレイヤーID, X座標, Y座標)のタプルのリスト</returns>
		public List<(int, int, int)> GetPlayerIdAndPos(int centerX, int centerY)
		{
			var posList = new Queue<(int, int)>();
			const int textureWidth = 70;
			const int textureHeight = 70;
			const int merginHeight = 5;
			switch (PlayerNum)
			{
				// 駒の画像ファイルが70px*70pxで上下だけ10px離して配置する
				case 1:
					posList.Enqueue((centerX - textureWidth / 2, centerY - textureHeight / 2));
					break;
				case 2:
					posList.Enqueue((centerX - textureWidth, centerY - textureHeight / 2));
					posList.Enqueue((centerX, centerY - centerY - textureHeight / 2));
					break;
				case 3:
					posList.Enqueue((centerX - textureWidth, centerY - textureHeight - merginHeight));
					posList.Enqueue((centerX, centerY - textureHeight - merginHeight));
					posList.Enqueue((centerX - textureWidth / 2, centerY + merginHeight));
					break;

				case 4:
					posList.Enqueue((centerX - textureWidth, centerY - textureHeight + merginHeight));
					posList.Enqueue((centerX, centerY - textureHeight + merginHeight));
					posList.Enqueue((centerX - textureWidth, centerY + merginHeight));
					posList.Enqueue((centerX, centerY + merginHeight));
					break;
				default:
					for (int i = 0; i < PlayerNum; i++)
					{
						posList.Enqueue((centerX - textureWidth / 2, centerY - textureHeight / 2));
					}
					break;
			}
			var list = new List<(int, int, int)>();
			foreach (var i in PlayerExists)
			{
				if (i.Value)
				{
					var pos = (centerX, centerY);
					if (posList.Count != 0)
					{
						pos = posList.Dequeue();
					}
					list.Add((i.Key, pos.Item1, pos.Item2));
				}
			}
			return list;
		}

	}
}
