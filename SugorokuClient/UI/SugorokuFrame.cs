using DxLibDLL;
using SugorokuClient.Scene;
using SugorokuClient.Util;
using SugorokuLibrary;
using System.Collections.Generic;

namespace SugorokuClient.UI
{
	public class SugorokuFrame
	{
		public bool IsProcessingEvent { get; private set; }
		private int BackgroundTextureHandle { get; set; }
		
		private List<SugorokuSquareFrame> SquareList { get; set; }
		private Field Fld { get; set; }
		
		private List<Player> Playerlist { get; set; }
		private Dictionary<int, Player> Players { get; set; }
		public Dictionary<int, int> PlayerTextureHandle { get; private set; }
		private Dictionary<int, AnimationTexture> PlayerAnimationTexture { get; set; }
		private List<SquareState> SquareState { get; set; }
		private bool IsDrawRanking { get; set; }
		private int KingTexture { get; set; }
		private int HyosyodaiTextureHandle { get; set; }
		private int hyosyoudaiX { get; set; }
		private int hyosyoudaiY { get; set; }
		private List<(int, int)> hyosyoudaiPos { get; set; }
		private Dictionary<int, (int, int)> hyosyou { get; set; }
		


		public SugorokuFrame()
		{
			IsDrawRanking = false;
			hyosyoudaiX = 180;
			hyosyoudaiY = 480;
			hyosyoudaiPos = new List<(int, int)>();
			hyosyoudaiPos.Add((hyosyoudaiX + 15, hyosyoudaiY - 200));
			hyosyoudaiPos.Add((hyosyoudaiX + 245, hyosyoudaiY - 100));
			hyosyoudaiPos.Add((hyosyoudaiX + 475, hyosyoudaiY - 0));
			hyosyoudaiPos.Add((hyosyoudaiX + 705, hyosyoudaiY + 100));
			hyosyou = new Dictionary<int, (int, int)>();
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


		public bool IsEndAllAnimation()
		{
			var target = PlayerAnimationTexture.Count;
			var count = 0;
			foreach (var anime in PlayerAnimationTexture)
			{
				count += (anime.Value.IsProcessingEvent) ? 1 : 0;
			}
			return count == target;
		}


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
				TextureAsset.Draw(HyosyodaiTextureHandle, hyosyoudaiX, hyosyoudaiY, 920, 300, DX.TRUE);
				foreach (var hyo in hyosyou)
				{
					TextureAsset.Draw(PlayerTextureHandle[hyo.Key], hyo.Value.Item1, hyo.Value.Item2, 200, 200, DX.TRUE);
				}
				TextureAsset.Draw(KingTexture, hyosyoudaiX + 15, hyosyoudaiY - 400, 200, 200, DX.TRUE);
			}
		}


		public void SetDrawRanking(List<int> ranking)
		{
			for (int i = 0; i < ranking.Count; i++)
			{
				if (hyosyou.ContainsKey(ranking[i])) continue;
				hyosyou.Add(ranking[i], hyosyoudaiPos[i]);
			}
			IsDrawRanking = true;
		}


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
