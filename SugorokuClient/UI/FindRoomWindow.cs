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
		private int x;
		private int y;
		private int width;
		private int height;
		private int listButtonHeight = 50;
		public bool isVisible = false;
		private bool isHaveInfo = false;
		private bool isReloading = false;

		public bool isSelectedInfo { get; private set; } = false;

		private KeyValuePair<string, MatchInfo> selectedMatch;


		private uint windowBaseColor = DX.GetColor(240, 240, 240);
		private uint windowFrameColor = DX.GetColor(125, 125, 125);
		private uint textColor = DX.GetColor(50, 50, 50);
		private int textFont;

		private int listButtonTexture = -1;
		private int reloadButtonTexture = -1;
		private int closeButtonTexture = -1;
		private string listButtonPath = "E:\\workspace\\devs\\SugorokuLibrary\\dev\\haruto8631\\SugorokuClient\\images\\Image1.png";
		private string reloadButtonPath = "E:\\workspace\\devs\\SugorokuLibrary\\dev\\haruto8631\\SugorokuClient\\images\\Image1.png";
		private string closeButtonPath = "E:\\workspace\\devs\\SugorokuLibrary\\dev\\haruto8631\\SugorokuClient\\images\\Image1.png";

		private Dictionary<string, MatchInfo> matches { get; set; }
		private List<TextureButton> matchesListButtons { get; set; }
		private TextureButton reloadButton { get; set; }
		private TextureButton closeButton { get; set; }


		private FindRoomWindow()
		{
			isVisible = false;
			isHaveInfo = false;
			isReloading = false;
			listButtonTexture = TextureAsset.Register("FindRoomWindowList", listButtonPath);
			reloadButtonTexture = TextureAsset.Register("FindRoomWindowReload", reloadButtonPath);
			closeButtonTexture = TextureAsset.Register("FindRoomWindowClose", closeButtonPath);
			SocketManager.Connect(CommonData.Address, CommonData.Port);
		}


		public FindRoomWindow(int x, int y, int width, int height) : this()
		{
			this.x = x;
			this.y = y;
			this.width = (width > 130) ? width : 115;
			this.height = (height > 110) ? height : 110;
			reloadButton = new TextureButton(reloadButtonTexture, x + width - 110, y + 5, 50, 50);
			closeButton = new TextureButton(closeButtonTexture, x + width - 55, y + 5, 50, 50);
			textFont = FontAsset.Register("FindRoomWindowFont", size: listButtonHeight);
			matchesListButtons = new List<TextureButton>();
			Task.Run(() => Reload());
		}


		public void Update()
		{
			if (!isVisible) return;
			if (reloadButton.LeftClicked() && !isReloading)
			{
				Task.Run(()=>Reload());
			}
			if (closeButton.LeftClicked())
			{
				isVisible = false;
				isSelectedInfo = false;
			}
			if (isReloading) return;
			foreach (var button in matchesListButtons)
			{
				if (button.LeftClicked())
				{
					selectedMatch = new KeyValuePair<string, MatchInfo>(
						button.Text, matches[button.Text]
						);
					isVisible = false;
					isSelectedInfo = true;
					break;
				}
			}
		}


		public void Draw()
		{
			if (!isVisible) return;
			DX.DrawBox(x-1, y-1, x + width + 1, y + height + 1, windowBaseColor, DX.TRUE);
			DX.DrawBox(x-1, y-1, x + width + 1, y + height + 1, windowFrameColor, DX.FALSE);
			reloadButton.Draw();
			closeButton.Draw();
			if (!isHaveInfo)
			{
				FontAsset.Draw(textFont, "Error",
					x + width / 2 - FontAsset.GetDrawTextWidth(textFont, "Error") / 2,
					y + 60, DX.GetColor(255, 0, 0)
					);
			}
			else if (isReloading)
			{
				FontAsset.Draw(textFont, "Loading",
					x + width / 2 - FontAsset.GetDrawTextWidth(textFont, "Loading") / 2,
					y + 60, textColor
					);
			}
			else
			{
				foreach (var button in matchesListButtons)
				{
					button.Draw();
					button.DrawText();
				}
			}
		}



		private bool GetMatchInfo()
		{
			var getAll = new GetAllMatchesMessage();
			var jsonMsg = JsonConvert.SerializeObject(getAll);
			var (result, msg) = SocketManager.SendRecv(jsonMsg);
			if (result)
			{
				matches = JsonConvert.DeserializeObject<Dictionary<string, MatchInfo>>(msg);
			}
			isHaveInfo = result;
			return result;
		}


		private void Reload()
		{
			isReloading = true;
			GetMatchInfo();
			matchesListButtons.Clear();
			if (!isHaveInfo) return;
			var matchNum = 0;
			foreach(var match in matches)
			{
				matchNum++;
				var listButtonPosY = matchNum * listButtonHeight + 50 + y;
				if (listButtonPosY + listButtonHeight > height) break;

				matchesListButtons.Add(
					new TextureButton(listButtonTexture, x, listButtonPosY, width, listButtonHeight,
					match.Key, textColor, textFont)
				);
			}
			isReloading = false;
		}


		public KeyValuePair<string, MatchInfo> GetSelectedMatch()
		{
			if (isSelectedInfo)
			{
				return selectedMatch;
			}
			else
			{
				return new KeyValuePair<string, MatchInfo>(string.Empty, new MatchInfo());
			}
		}

	}

}
