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
	/// <summary>
	/// 部屋を探すポップアップ画面のパーツ
	/// </summary>
	public class FindRoomWindow
	{
		/// <summary>
		/// 部屋の情報を持っているかどうか
		/// </summary>
		private bool IsHaveInfo { get; set; }

		/// <summary>
		/// リロード処理を行っているかどうか
		/// </summary>
		private bool IsReloading { get; set; }

		/// <summary>
		/// 部屋の情報が選択されたかどうか
		/// </summary>
		public bool IsSelectedInfo { get; private set; }

		/// <summary>
		/// 描画されているかどうか
		/// </summary>
		public bool IsVisible { get; set; }

		// <summary>
		/// 左上のX座標
		/// </summary>
		private int X { get; set; }

		/// <summary>
		/// 左上のY座標
		/// </summary>
		private int Y { get; set; }

		/// <summary>
		/// 幅
		/// </summary>
		private int Width { get; set; }

		/// <summary>
		/// 高さ
		/// </summary>
		private int Height { get; set; }

		/// <summary>
		/// 各ボタンの高さ
		/// </summary>
		private int ListButtonHeight { get; set; }

		/// <summary>
		/// 背景色
		/// </summary>
		private uint WindowBaseColor { get; set; }

		/// <summary>
		/// 輪郭の色
		/// </summary>
		private uint WindowFrameColor { get; set; }

		/// <summary>
		/// テキストの色
		/// </summary>
		private uint TextColor { get; set; }

		/// <summary>
		/// フォントの識別子
		/// </summary>
		private int TextFont { get; set; }

		/// <summary>
		/// 各ボタンのテクスチャの識別子
		/// </summary>
		private int ListButtonTexture { get; set; }

		/// <summary>
		/// リロードボタンのテクスチャの識別子
		/// </summary>
		private int ReloadButtonTexture { get; set; }

		/// <summary>
		/// 閉じるボタンのテクスチャの識別子
		/// </summary>
		private int CloseButtonTexture { get; set; }

		/// <summary>
		/// 部屋の名前と試合情報の辞書
		/// </summary>
		private Dictionary<string, MatchInfo> Matches { get; set; }

		/// <summary>
		/// 一覧のボタン
		/// </summary>
		private List<TextureButton> MatchesListButtons { get; set; }

		/// <summary>
		/// リロードボタン
		/// </summary>
		private TextureButton ReloadButton { get; set; }

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		private TextureButton CloseButton { get; set; }

		/// <summary>
		/// 選択された試合情報
		/// </summary>
		private MatchInfo SelectedMatch { get; set; }


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
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


		/// <summary>
		/// 描画位置を指定するコンストラクタ
		/// </summary>
		/// <param name="x">左上のX座標</param>
		/// <param name="y">右上のY座標</param>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
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


		/// <summary>
		/// 更新処理
		/// </summary>
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


		/// <summary>
		/// サーバーからすべての試合情報を取得する
		/// </summary>
		/// <returns></returns>
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


		/// <summary>
		/// リロード処理
		/// </summary>
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


		/// <summary>
		/// このUIで選択された試合情報を取得する関数
		/// </summary>
		/// <returns></returns>
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
