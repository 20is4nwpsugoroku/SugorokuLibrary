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
		private Dictionary<int, int> PlayerTextureHandle { get; set; }
		private Dictionary<int, AnimationTexture> PlayerAnimationTexture { get; set; }
		private List<SquareState> SquareState { get; set; }



		public SugorokuFrame()
		{
			
			BackgroundTextureHandle = TextureAsset.Register("GameBackgroundImage",
				"../../../images/Map.png");
			SquareList = new List<SugorokuSquareFrame>();
			Playerlist = new List<Player>();
			Players = new Dictionary<int, Player>();
			PlayerTextureHandle = new Dictionary<int, int>();
			PlayerTextureHandle.Add(-1, TextureAsset.Register("Player1",
				"../../../images/koma_1.png"));
			PlayerTextureHandle.Add(-2, TextureAsset.Register("Player2",
				"../../../images/koma_2.png"));
			PlayerTextureHandle.Add(-3, TextureAsset.Register("Player3",
				"../../../images/koma_3.png"));
			PlayerTextureHandle.Add(-4, TextureAsset.Register("Player4",
				"../../../images/koma_4.png"));
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
					if (ret.Item1 != Playerlist[i].PlayerID) break;
					Players.Add(Playerlist[i].PlayerID, Playerlist[i]);
					var handle = PlayerTextureHandle[-(i + 1)];
					PlayerTextureHandle.Remove(-(i + 1));
					PlayerTextureHandle.Add(Playerlist[i].PlayerID, handle);
					PlayerAnimationTexture.Add(Playerlist[i].PlayerID,
						new AnimationTexture(handle, ret.Item1, ret.Item2, 70, 70));
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
				anime.Value.Update();
				if (anime.Value.IsAnimationEndFrame())
				{
					SquareList[anime.Value.AnimationEndPos()].DescriptionMessage.Start();
				}
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
		}


		public void ProcessEvent(SugorokuEvent sugorokuEvent)
		{
			DX.putsDx($"Dice:{sugorokuEvent.Dice}, StartPos:{sugorokuEvent.EventStartPos}, EndPos:{sugorokuEvent.EventEndPos}");
			IsProcessingEvent = true;
			SquareState[sugorokuEvent.EventStartPos - sugorokuEvent.Dice].ExistsControl(sugorokuEvent.PlayerId, false);
			SquareState[sugorokuEvent.EventStartPos].ExistsControl(sugorokuEvent.PlayerId, true);
			var pos = SquareList[sugorokuEvent.EventStartPos].CenterPos;
			var movedPosList = SquareState[sugorokuEvent.EventStartPos].GetPlayerIdAndPos(pos.Item1, pos.Item2);
			foreach (var movedPos in movedPosList)
			{
				DX.putsDx($"Id:{movedPos.Item1}, X:{movedPos.Item2}, Y:{movedPos.Item3}");
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
					PlayerAnimationTexture[movedPos.Item1].AddChangePosition(movedPos.Item2, movedPos.Item3, 60, sugorokuEvent.EventEndPos);
					PlayerAnimationTexture[movedPos.Item1].Start();
				}
			}
			IsProcessingEvent = false;
		}

	}
}
