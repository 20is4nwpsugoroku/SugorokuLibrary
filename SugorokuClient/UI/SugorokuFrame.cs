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
		private List<SugorokuSquareFrame> SquaresList { get; set; }
		private Field Fld { get; set; }
		public bool IsProcessingEvent { get; private set; }
		private List<Player> PlayerIds { get; set; }
		private Dictionary<int, Player> Players { get; set; }
		private Dictionary<int, int> PlayerTextureHandle { get; set; }

		public SugorokuFrame()
		{
			
			BackgroundTextureHandle = TextureAsset.Register("GameBackgroundImage",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png");



			SquaresList = new List<SugorokuSquareFrame>();
			PlayerIds = new List<Player>();
			Players = new Dictionary<int, Player>();
			PlayerTextureHandle = new Dictionary<int, int>();
			PlayerTextureHandle.Add(0, TextureAsset.Register("Player1",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png"));
			PlayerTextureHandle.Add(1, TextureAsset.Register("Player2",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png"));
			PlayerTextureHandle.Add(2, TextureAsset.Register("Player3",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png"));
			PlayerTextureHandle.Add(3, TextureAsset.Register("Player4",
				"E:/workspace/devs/SugorokuLibrary/dev/haruto8631/SugorokuClient/images/Image1.png"));
			Fld = new Field();
			for (var i = 0; i < Fld.Squares.Length; i++)
			{
				SquaresList.Add(new SugorokuSquareFrame(Fld.Squares[i], i));
			}
		}


		public void Init(List<Player> playerIds)
		{
			PlayerIds = playerIds;
			for (int i = 0; i < PlayerIds.Count; i++)
			{
				Players.Add(PlayerIds[i].PlayerID, PlayerIds[i]);
				var handle = PlayerTextureHandle[i];
				PlayerTextureHandle.Remove(i);
				PlayerTextureHandle.Add(PlayerIds[i].PlayerID, handle);
			}
		}


		public void Update()
		{
			foreach(var square in SquaresList)
			{
				square.Update();
			}
		}


		public void Draw()
		{
			TextureAsset.Draw(BackgroundTextureHandle, 0, 0, 1280, 800, DX.TRUE);
			foreach (var square in SquaresList)
			{
				square.Draw();
			}
		}


		public void ProcessEvent(SugorokuEvent sugorokuEvent)
		{
			IsProcessingEvent = true;

		}

	}
}
