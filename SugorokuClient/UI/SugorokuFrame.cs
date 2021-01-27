using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuClient.Scene;
using SugorokuClient.UI;
using SugorokuClient.Util;

namespace SugorokuClient.UI
{
	public class SugorokuFrame
	{
		private int BackgroundTextureHandle { get; set; }
		private List<SugorokuSquareFrame> SquareList { get; set; }
		private Field Fld { get; set; }
		public bool IsProcessingEvent { get; private set; }
		private List<Player> Playerlist { get; set; }
		private Dictionary<int, Player> Players { get; set; }
		public Dictionary<int, int> PlayerTextureHandle { get; private set; }
		private Dictionary<int, AnimationTexture> PlayerAnimationTexture { get; set; }
		private List<SquareState> SquareState { get; set; }

		private int HyosyodaiTextureHandle { get; set; }
		private int hyosyoudaiX { get; set; }
		private int hyosyoudaiY { get; set; }
		private List<(int, int)> hyosyoudaiPos { get; set; }
		private Dictionary<int, (int, int)> hyosyou { get; set; }
		private bool isResult = false;
		private int kingTexture { get; set; }


		public SugorokuFrame()
		{
			isResult = false;
			hyosyoudaiX = 180;
			hyosyoudaiY = 480;
			hyosyoudaiPos = new List<(int, int)>();
			hyosyoudaiPos.Add((hyosyoudaiX + 15, hyosyoudaiY - 200));
			hyosyoudaiPos.Add((hyosyoudaiX + 245, hyosyoudaiY - 100));
			hyosyoudaiPos.Add((hyosyoudaiX + 475, hyosyoudaiY - 0));
			hyosyoudaiPos.Add((hyosyoudaiX + 705, hyosyoudaiY + 100));
			hyosyou = new Dictionary<int, (int, int)>();
			kingTexture = TextureAsset.Register("king", "../../../images/king.png");
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
			foreach(var square in SquareList)
			{
				square.Update();
			}
			foreach (var anime in PlayerAnimationTexture)
			{
				if (anime.Value.IsAnimationEndFrame())
				{
					SquareList[anime.Value.AnimationEndPos()].MessageBoxStart();
				}
				anime.Value.Update();
			}
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
			if (isResult)
			{
				DX.DrawBox(130, 70, 130 + 1020, 70 + 800, DX.GetColor(255, 255, 255), DX.TRUE);
				DX.DrawBox(130, 70, 130 + 1020, 70 + 800, DX.GetColor(0, 103, 167), DX.FALSE);
				TextureAsset.Draw(HyosyodaiTextureHandle, hyosyoudaiX, hyosyoudaiY, 920, 300, DX.TRUE);
				foreach (var hyo in hyosyou)
				{
					TextureAsset.Draw(PlayerTextureHandle[hyo.Key], hyo.Value.Item1, hyo.Value.Item2, 200, 200, DX.TRUE);
				}
				TextureAsset.Draw(kingTexture, hyosyoudaiX + 15, hyosyoudaiY - 400, 200, 200, DX.TRUE);
			}
		}


		public void DrawRanking(List<int> ranking)
		{
			for (int i = 0; i < ranking.Count; i++)
			{
				if (hyosyou.ContainsKey(ranking[i])) continue;
				hyosyou.Add(ranking[i], hyosyoudaiPos[i]);
			}
			isResult = true;
		}


		public void ProcessEvent(SugorokuEvent sugorokuEvent)
		{
			IsProcessingEvent = true;
			bool temp = false;
			int nowPosition = 0;
			for (var i = 0; i < SquareState.Count; i++)
			{
				temp = SquareState[i].PlayerExists[sugorokuEvent.PlayerId];
				if (!temp) nowPosition = i;
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
				pos  = SquareList[sugorokuEvent.EventEndPos].CenterPos;
				movedPosList = SquareState[sugorokuEvent.EventEndPos].GetPlayerIdAndPos(pos.Item1, pos.Item2);
				foreach (var movedPos in movedPosList)
				{
					PlayerAnimationTexture[movedPos.Item1].AddChangePosition(movedPos.Item2, movedPos.Item3, 60, 0);
				}
			}
			IsProcessingEvent = false;
		}

	}
}
