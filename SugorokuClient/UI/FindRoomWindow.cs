using System;
using System.Collections.Generic;
using System.Text;
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
		private FailedMessage failedMessage { get; set; }
		private List<TextureButton> matchesListButtons { get; set; }
		private TextureButton reloadButton { get; set; }
		private TextureButton closeButton { get; set; }


		public FindRoomWindow()
		{
			isVisible = false;
			listButtonTexture = TextureAsset.Register("FindRoomWindowList", listButtonPath);
			reloadButtonTexture = TextureAsset.Register("FindRoomWindowReload", reloadButtonPath);
			closeButtonTexture = TextureAsset.Register("FindRoomWindowClose", closeButtonPath);
			CommonData.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			CommonData.socket.Connect(CommonData.address, CommonData.port);
		}


		public FindRoomWindow(int x, int y, int width, int height) : this()
		{
			this.x = x;
			this.y = y;
			this.width = (width > 130) ? width : 115;
			this.height = (height > 110) ? height : 110;
			reloadButton = new TextureButton(reloadButtonTexture, x + width - 110, 5, 50, 50);
			closeButton = new TextureButton(closeButtonTexture, x + width - 55, 5, 50, 50);
			textFont = FontAsset.Register("FindRoomWindowFont", size: listButtonHeight);
			GetMatchInfo();
			Reload();
		}


		public void Update()
		{
			if (!isVisible) return;
			if (reloadButton.LeftClicked())
			{
				Reload();
			}
			if (closeButton.LeftClicked())
			{
				isVisible = false;
				isSelectedInfo = false;
			}
			foreach (var button in matchesListButtons)
			{
				if (button.LeftClicked())
				{
					selectedMatch = new KeyValuePair<string, MatchInfo>(
						button.Text, matches.GetValueOrDefault(button.Text)
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
			foreach (var button in matchesListButtons)
			{
				button.Draw();
			}
			if (isHaveInfo)
			{
				FontAsset.Draw(textFont, "Error",
					x + width / 2 - FontAsset.GetDrawTextWidth(textFont, "Error"),
					60, DX.GetColor(255, 0, 0)
					);
			}
		}



		private bool GetMatchInfo()
		{
			var getAll = new GetAllMatchesMessage();
			var jsonMsg = JsonConvert.SerializeObject(getAll);
			var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, CommonData.socket);
			if (result)
			{
				matches = JsonConvert.DeserializeObject<Dictionary<string, MatchInfo>>(msg);
			}
			else
			{
				failedMessage = JsonConvert.DeserializeObject<FailedMessage>(msg);
			}
			isHaveInfo = result;
			return result;
		}


		private void Reload()
		{
			matchesListButtons.Clear();
			if (!isHaveInfo) return;
			var matchNum = 0;
			foreach(var match in matches)
			{
				matchNum++;
				var listButtonPosY = matchNum * listButtonHeight + 50;
				if (listButtonPosY + listButtonHeight > height) break;

				matchesListButtons.Add(
					new TextureButton(listButtonTexture, x, listButtonPosY, width, listButtonHeight,
					match.Key, textColor, textFont)
				);
			}
		}


		public KeyValuePair<string, MatchInfo> GetSelectedMatch()
		{
			if (isSelectedInfo)
			{
				return selectedMatch;
			}
			else
			{
				return new KeyValuePair<string, MatchInfo>();
			}
		}

	}

}
