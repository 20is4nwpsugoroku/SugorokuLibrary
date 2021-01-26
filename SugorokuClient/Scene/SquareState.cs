using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	public class SquareState
	{
		public int PlayerNum { get; private set; }
		public Dictionary<int, bool> PlayerExists { get; private set; }


		public SquareState(List<int> PlayerIds)
		{
			PlayerNum = 0;
			PlayerExists = new Dictionary<int, bool>();
			foreach(var id in PlayerIds)
			{
				PlayerExists.Add(id, false);
			}
		}


		public void ExistsControl (int playerId, bool exists)
		{
			PlayerExists[playerId] = exists;
			PlayerNum += (exists) ? 1 : -1;
		}


		public List<(int, int, int)> GetPlayerIdAndPos(int centerX, int centerY)
		{
			var posList = new Queue<(int, int)>();
			switch (PlayerNum)
			{
				// 駒の画像ファイルが70px*70pxで上下だけ10px離して配置する
				case 1:
					posList.Enqueue((centerX - 35, centerY - 35));
					break;
				case 2:
					posList.Enqueue((centerX - 70, centerY - 35));
					posList.Enqueue((centerX, centerY - 35));
					break;
				case 3:
					posList.Enqueue((centerX - 70, centerY - 75));
					posList.Enqueue((centerX, centerY - 75));
					posList.Enqueue((centerX - 35, centerY + 5));
					break;

				case 4:
					posList.Enqueue((centerX - 70, centerY - 75));
					posList.Enqueue((centerX, centerY - 75));
					posList.Enqueue((centerX - 70, centerY + 5));
					posList.Enqueue((centerX, centerY + 5));
					break;
				default:
					for (int i = 0; i < PlayerNum; i++)
					{
						posList.Enqueue((centerX - 35, centerY - 35));
					}
					break;
			}

			var list = new List<(int, int, int)>();
			foreach (var i in PlayerExists)
			{
				if (i.Value)
				{
					var pos = posList.Dequeue();
					list.Add((i.Key, pos.Item1, pos.Item2));
				}
			}
			return list;
		}

	}
}
