using DxLibDLL;
using SugorokuClient.Scene;
using SugorokuClient.Util;
using SugorokuLibrary;
using System.Collections.Generic;

namespace SugorokuClient.UI
{
	/// <summary>
	/// ゲームシーンで利用するすごろくのパーツ
	/// </summary>
	public class SugorokuFrame
	{
		/// <summary>
		/// ProcessEventが処理中かどうか
		/// </summary>
		public bool IsProcessingEvent { get; private set; }
		
		/// <summary>
		/// 背景のテクスチャ識別子
		/// </summary>
		private int BackgroundTextureHandle { get; set; }
		
		/// <summary>
		/// マスの情報
		/// </summary>
		private Field Fld { get; set; }

		/// <summary>
		/// マスの状態
		/// </summary>
		private List<SquareState> SquareState { get; set; }

		/// <summary>
		/// マスのフレーム
		/// </summary>
		private List<SugorokuSquareFrame> SquareList { get; set; }

		/// <summary>
		/// プレイヤー情報のリスト
		/// </summary>
		private List<Player> Playerlist { get; set; }

		/// <summary>
		/// プレイヤーのIDとプレイヤー情報の辞書
		/// </summary>
		private Dictionary<int, Player> Players { get; set; }

		/// <summary>
		/// プレイヤーIDとプレイヤーのテクスチャ識別子の辞書
		/// </summary>
		public Dictionary<int, int> PlayerTextureHandle { get; private set; }

		/// <summary>
		/// プレイヤーIDとプレイヤーのテクスチャの辞書
		/// </summary>
		private Dictionary<int, AnimationTexture> PlayerAnimationTexture { get; set; }
		

		/// <summary>
		/// 順位を表示しているかどうか
		/// </summary>
		private bool IsDrawRanking { get; set; }

		/// <summary>
		/// 王冠のテクスチャ識別子
		/// </summary>
		private int KingTexture { get; set; }

		/// <summary>
		/// 表彰台のテクスチャ識別子
		/// </summary>
		private int HyosyodaiTextureHandle { get; set; }

		/// <summary>
		/// 表彰台の左上のX座標
		/// </summary>
		private int HyosyoudaiX { get; set; }

		/// <summary>
		/// 表彰台の左上のY座標
		/// </summary>
		private int HyosyoudaiY { get; set; }

		/// <summary>
		/// 表彰台の各順位の描画位置
		/// </summary>
		private List<(int, int)> HyosyoudaiPos { get; set; }

		/// <summary>
		/// プレイヤーのIDと表彰台での描画位置の辞書
		/// </summary>
		private Dictionary<int, (int, int)> PlayerRankResultPos { get; set; }
		

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SugorokuFrame()
		{
			IsDrawRanking = false;
			HyosyoudaiX = 180;
			HyosyoudaiY = 480;
			HyosyoudaiPos = new List<(int, int)>();
			HyosyoudaiPos.Add((HyosyoudaiX + 15, HyosyoudaiY - 200));
			HyosyoudaiPos.Add((HyosyoudaiX + 245, HyosyoudaiY - 100));
			HyosyoudaiPos.Add((HyosyoudaiX + 475, HyosyoudaiY - 0));
			HyosyoudaiPos.Add((HyosyoudaiX + 705, HyosyoudaiY + 100));
			PlayerRankResultPos = new Dictionary<int, (int, int)>();
			KingTexture = TextureAsset.Register("king", "../../../images/king.png");
			HyosyodaiTextureHandle = TextureAsset.Register("hyoushoudai", "../../../images/hyoushoudai.png");
			BackgroundTextureHandle = TextureAsset.Register("GameBackgroundImage", "../../../images/Map.png");
			SquareList = new List<SugorokuSquareFrame>();
			Playerlist = new List<Player>();
			Players = new Dictionary<int, Player>();
			PlayerTextureHandle = new Dictionary<int, int>();
			PlayerTextureHandle.Add(-1, TextureAsset.Register("Player1", "../../../images/koma_1.png"));
			PlayerTextureHandle.Add(-2, TextureAsset.Register("Player2", "../../../images/koma_2.png"));
			PlayerTextureHandle.Add(-3, TextureAsset.Register("Player3", "../../../images/koma_3.png"));
			PlayerTextureHandle.Add(-4, TextureAsset.Register("Player4", "../../../images/koma_4.png"));
			PlayerAnimationTexture = new Dictionary<int, AnimationTexture>();
			Fld = new Field();
			for (var i = 0; i < Fld.Squares.Length; i++)
			{
				SquareList.Add(new SugorokuSquareFrame(Fld.Squares[i], i));
			}
			IsProcessingEvent = false;
		}


		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="players">プレイヤー情報</param>
		public void Init(List<Player> players)
		{
			Playerlist = players;
			var idList = new List<int>();
			SquareState = new List<SquareState>();
			foreach (var id in Playerlist)
			{
				idList.Add(id.PlayerID);
			}
			for (var i = 0; i < 31; i++)
			{
				SquareState.Add(new SquareState(idList));
			}
			foreach (var id in idList)
			{
				SquareState[0].ExistsControl(id, true);
			}
			var retList = SquareState[0].GetPlayerIdAndPos(
				SquareList[0].CenterPos.Item1,
				SquareList[0].CenterPos.Item2);
			
			foreach (var ret in retList)
			{
				for (int i = 0; i < Playerlist.Count; i++)
				{
					if (ret.Item1 != Playerlist[i].PlayerID) continue;
					Players.Add(Playerlist[i].PlayerID, Playerlist[i]);
					var handle = PlayerTextureHandle[-(i + 1)];
					PlayerTextureHandle.Remove(-(i + 1));
					PlayerTextureHandle.Add(Playerlist[i].PlayerID, handle);
					PlayerAnimationTexture.Add(Playerlist[i].PlayerID,
						new AnimationTexture(handle, ret.Item2, ret.Item3, 70, 70));
				}
			}
		}

		
		/// <summary>
		/// 更新処理
		/// </summary>
		public void Update()
		{
			foreach (var anime in PlayerAnimationTexture)
			{
				anime.Value.Update();
				if (anime.Value.IsAnimationEndFrame())
				{
					SquareList[anime.Value.AnimationEndPos()].MessageBoxStart();
				}
			}
			foreach (var square in SquareList)
			{
				square.Update();
			}
		}


		/// <summary>
		/// 描画処理
		/// </summary>
		public void Draw()
		{
			TextureAsset.Draw(BackgroundTextureHandle, 0, 0, 1280, 800, DX.TRUE);
			foreach (var anime in PlayerAnimationTexture)
			{
				anime.Value.Draw();
			}
			foreach (var square in SquareList)
			{
				square.Draw();
			}
			if (IsDrawRanking)
			{
				DX.DrawBox(130, 70, 130 + 1020, 70 + 800, DX.GetColor(255, 255, 255), DX.TRUE);
				DX.DrawBox(130, 70, 130 + 1020, 70 + 800, DX.GetColor(0, 103, 167), DX.FALSE);
				TextureAsset.Draw(HyosyodaiTextureHandle, HyosyoudaiX, HyosyoudaiY, 920, 300, DX.TRUE);
				foreach (var hyo in PlayerRankResultPos)
				{
					TextureAsset.Draw(PlayerTextureHandle[hyo.Key], hyo.Value.Item1, hyo.Value.Item2, 200, 200, DX.TRUE);
				}
				TextureAsset.Draw(KingTexture, HyosyoudaiX + 15, HyosyoudaiY - 400, 200, 200, DX.TRUE);
			}
		}


		/// <summary>
		/// すべてのプレイヤーのアニメーションが終了しているかどうか
		/// </summary>
		/// <returns></returns>
		public bool IsEndAllAnimation()
		{
			var target = PlayerAnimationTexture.Count;
			var count = 0;
			foreach (var anime in PlayerAnimationTexture)
			{
				count += (anime.Value.IsProcessingEvent) ? 0 : 1;
			}
			return count == target;
		}


		/// <summary>
		/// 順位を描画するように設定する
		/// </summary>
		/// <param name="ranking">順位</param>
		public void SetDrawRanking(List<int> ranking)
		{
			for (int i = 0; i < ranking.Count; i++)
			{
				if (PlayerRankResultPos.ContainsKey(ranking[i])) continue;
				PlayerRankResultPos.Add(ranking[i], HyosyoudaiPos[i]);
			}
			IsDrawRanking = true;
		}


		/// <summary>
		/// SugorokuEventを処理する
		/// </summary>
		/// <param name="sugorokuEvent"></param>
		public void ProcessEvent(PlayerMoveEvent sugorokuEvent)
		{
			IsProcessingEvent = true;
			bool temp = false;
			int nowPosition = 0;
			for (var i = 0; i < SquareState.Count; i++)
			{
				temp = SquareState[i].PlayerExists[sugorokuEvent.PlayerId];
				if (temp)
				{
					nowPosition = i;
					break;
				}
			}
			SquareState[nowPosition].ExistsControl(sugorokuEvent.PlayerId, false);
			SquareState[sugorokuEvent.EventStartPos].ExistsControl(sugorokuEvent.PlayerId, true);
			var pos = SquareList[sugorokuEvent.EventStartPos].CenterPos;
			var movedPosList = SquareState[sugorokuEvent.EventStartPos].GetPlayerIdAndPos(pos.Item1, pos.Item2);
			foreach (var movedPos in movedPosList)
			{
				PlayerAnimationTexture[movedPos.Item1].AddChangePosition(movedPos.Item2, movedPos.Item3, 60, sugorokuEvent.EventStartPos);
				PlayerAnimationTexture[movedPos.Item1].Start();
			}

			if (sugorokuEvent.EventStartPos != sugorokuEvent.EventEndPos)
			{
				SquareState[sugorokuEvent.EventStartPos].ExistsControl(sugorokuEvent.PlayerId, false);
				SquareState[sugorokuEvent.EventEndPos].ExistsControl(sugorokuEvent.PlayerId, true);
				var pos_ = SquareList[sugorokuEvent.EventEndPos].CenterPos;
				var movedPosList_ = SquareState[sugorokuEvent.EventEndPos].GetPlayerIdAndPos(pos_.Item1, pos_.Item2);
				foreach (var movedPos_ in movedPosList_)
				{
					PlayerAnimationTexture[movedPos_.Item1].AddChangePosition(movedPos_.Item2, movedPos_.Item3, 60, 0);
					PlayerAnimationTexture[movedPos_.Item1].Start();
				}
			}
			IsProcessingEvent = false;
		}
	}
}
