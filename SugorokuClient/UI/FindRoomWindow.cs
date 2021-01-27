using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.ServerToClient;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	public class FindRoomWindow
	{
		private int X { get; set; }
		private int Y { get; set; }
		private int Width { get; set; }
		private int Height { get; set; }
		private int ListButtonHeight { get; set; }
		private bool IsHaveInfo { get; set; }
		private bool IsReloading { get; set; }
		public bool IsSelectedInfo { get; private set; }
		public bool IsVisible { get; set; }
		private uint WindowBaseColor { get; set; }
		private uint WindowFrameColor { get; set; }
		private uint TextColor { get; set; }
		private int TextFont { get; set; }
		private int ListButtonTexture { get; set; }
		private int ReloadButtonTexture { get; set; }
		private int CloseButtonTexture { get; set; }
		private Dictionary<string, MatchInfo> Matches { get; set; }
		private List<TextureButton> MatchesListButtons { get; set; }
		private TextureButton ReloadButton { get; set; }
		private TextureButton CloseButton { get; set; }
		private MatchInfo SelectedMatch { get; set; }



		private FindRoomWindow()
		{
			IsVisible = false;
			IsHaveInfo = false;
			IsReloading = false;
			IsSelectedInfo = false;
			ListButtonHeight = 50;
			WindowBaseColor = DX.GetColor(240, 240, 240);
			WindowFrameColor = DX.GetColor(125, 125, 125);
			TextColor = DX.GetColor(50, 50, 50);
			ListButtonTexture = TextureAsset.Register("FindRoomWindowList", "../../../images/FindRoomListButton.png");
			ReloadButtonTexture = TextureAsset.Register("FindRoomWindowReload", "../../../images/Reload.png");
			CloseButtonTexture = TextureAsset.Register("FindRoomWindowClose", "../../../images/Close.png");
		}


		public FindRoomWindow(int x, int y, int width, int height) : this()
		{
			X = x;
			Y = y;
			Width = (width > 130) ? width : 115;
			Height = (height > 110) ? height : 110;
			ReloadButton = new TextureButton(ReloadButtonTexture, x + width - 110, y + 5, 50, 50);
			CloseButton = new TextureButton(CloseButtonTexture, x + width - 55, y + 5, 50, 50);
			TextFont = FontAsset.Register("FindRoomWindowFont", size: 40);
			MatchesListButtons = new List<TextureButton>();
			Task.Run(() => Reload());
		}


		public void Update()
		{
			if (!IsVisible) return;
			if (ReloadButton.LeftClicked() && !IsReloading)
			{
				Task.Run(()=>Reload());
			}
			if (CloseButton.LeftClicked())
			{
				IsVisible = false;
				IsSelectedInfo = false;
				return;
			}
			if (IsReloading) return;
			foreach (var button in MatchesListButtons)
			{
				if (button.LeftClicked())
				{
					SelectedMatch = Matches[button.Text];
					IsVisible = false;
					IsSelectedInfo = true;
					break;
				}
			}
		}


		public void Draw()
		{
			if (!IsVisible) return;
			DX.DrawBox(X-1, Y-1, X + Width + 1, Y + Height + 1, WindowBaseColor, DX.TRUE);
			DX.DrawBox(X-1, Y-1, X + Width + 1, Y + Height + 1, WindowFrameColor, DX.FALSE);
			ReloadButton.Draw();
			CloseButton.Draw();
			if (!IsHaveInfo)
			{
				FontAsset.Draw(TextFont, "Please Reload",
					X + Width / 2 - FontAsset.GetDrawTextWidth(TextFont, "Please Reload") / 2,
					Y + 60, TextColor
					);
			}
			else if (IsReloading)
			{
				FontAsset.Draw(TextFont, "Loading...",
					X + Width / 2 - FontAsset.GetDrawTextWidth(TextFont, "Loading...") / 2,
					Y + 60, TextColor
					);
			}
			else
			{
				foreach (var button in MatchesListButtons)
				{
					button.Draw();
					button.DrawText();
				}
			}
		}



		private bool GetAllMatchInfo()
		{
			var getAll = new GetAllMatchesMessage();
			var jsonMsg = JsonConvert.SerializeObject(getAll);
			var (result, msg) = SocketManager.SendRecv(jsonMsg);
			if (result)
			{
				Matches = JsonConvert.DeserializeObject<Dictionary<string, MatchInfo>>(msg);
			}
			return result;
		}


		private void Reload()
		{
			IsReloading = true;
			IsHaveInfo = GetAllMatchInfo();
			MatchesListButtons.Clear();
			if (!IsHaveInfo)
			{
				IsReloading = false;
				return;
			}
			var matchNum = 0;
			foreach(var match in Matches)
			{
				matchNum++;
				var listButtonPosY = matchNum * ListButtonHeight + 50 + Y;
				if (listButtonPosY + ListButtonHeight > Height) break;

				MatchesListButtons.Add(
					new TextureButton(ListButtonTexture, X, listButtonPosY, Width, ListButtonHeight,
					match.Key, TextColor, TextFont)
				);
			}
			IsReloading = false;
		}


		public MatchInfo GetSelectedMatch()
		{
			if (IsSelectedInfo)
			{
				return SelectedMatch;
			}
			else
			{
				return  new MatchInfo();
			}
		}
	}
}
