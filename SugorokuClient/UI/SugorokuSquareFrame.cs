﻿using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuLibrary;
using SugorokuLibrary.SquareEvents;
using SugorokuClient.Util;


namespace SugorokuClient.UI
{
	/// <summary>
	/// すごろくのマスのフレーム
	/// </summary>
	public class SugorokuSquareFrame
	{
		/// <summary>
		/// クリックされたかどうか
		/// </summary>
		public bool IsFirstClicked { get; set; }

		/// <summary>
		/// マスのボタン
		/// </summary>
		private TextureButton SquareButton { get; set; }

		/// <summary>
		/// マスのイベントの説明分
		/// </summary>
		public TextureFade DescriptionMessage { get; private set; }

		/// <summary>
		/// マスのイベント
		/// </summary>
		public SquareEvent Square { get; private set; }

		/// <summary>
		/// マスの中央の座標
		/// </summary>
		public (int, int) CenterPos { get; private set; }


		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="square">マスのイベント</param>
		/// <param name="index">マスの場所</param>
		public SugorokuSquareFrame(SquareEvent square, int index)
		{
			(this.CenterPos, this.SquareButton) = GenerateSquareButton(index);
			int x, y, w, h, handle;
			GetMessageBoxTextureInfo(index, CenterPos, out handle, out x, out y, out w, out h);
			var text = GetMessageBoxText(index);
			var fontHandle = (text != string.Empty) ? FontAsset.Register("MessageBoxFont", size: 18) : -1;
			this.DescriptionMessage = new TextureFade(handle, x, y, w, h, 5, 30, 120, fontHandle, text, DX.GetColor(50, 50, 50));
			this.Square = square;
			IsFirstClicked = false;
		}


		/// <summary>
		/// 更新処理
		/// </summary>
		public void Update()
		{
			if (DescriptionMessage.Text == string.Empty) return;
			if (SquareButton.LeftClicked() && DescriptionMessage.Text != string.Empty)
			{
				IsFirstClicked = true;
				DescriptionMessage.Start();
			}
			if (IsFirstClicked) DescriptionMessage.Update();
		}


		/// <summary>
		/// 描画処理
		/// </summary>
		public void Draw()
		{
			SquareButton.MouseOverDraw();
			if (IsFirstClicked) DescriptionMessage.Draw();
		}


		/// <summary>
		/// メッセージボックスの処理を開始する
		/// </summary>
		public void MessageBoxStart()
		{
			IsFirstClicked = true;
			DescriptionMessage.Start();
		}


		/// <summary>
		/// マス用のテクスチャと中央の座標を生成する
		/// </summary>
		/// <param name="index"></param>
		/// <returns>(int, int)は中央の座標</returns>
		private static ((int, int), TextureButton) GenerateSquareButton(int index)
		{
			var squareImageHandle = TextureAsset.Register("squareImage", "../../../images/toumei.png");
			TextureButton button;
			(int, int) centerPos;
			switch (index)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
					var pos = GetRectSquarePos(index);
					button = new TextureButton(squareImageHandle, pos.Item1, pos.Item2, 120, 120);
					centerPos = (button.x1 + 60, button.y1 + 60);
					break;

				case 9:
					button = new TextureButton(squareImageHandle, 1112, 174, 1242, 277, 1177, 330, 1112, 296);
					centerPos = (1170, 242);
					break;

				case 10:
					button = new TextureButton(squareImageHandle, 1177, 330, 1242, 277, 1243, 434, 1178, 378);
					centerPos = (1230, 360);
					break;

				case 11:
					button = new TextureButton(squareImageHandle, 1112, 414, 1178, 378, 1243, 434, 1111, 514);
					centerPos = (1164, 443);
					break;

				case 20:
					button = new TextureButton(squareImageHandle, 35, 458, 155, 393, 155, 480, 94, 514);
					centerPos = (103, 454);
					break;

				case 21:
					button = new TextureButton(squareImageHandle, 35, 458, 94, 514, 94, 564, 33, 614);
					centerPos = (45, 537);
					break;

				case 22:
					button = new TextureButton(squareImageHandle, 94, 564, 152, 598, 151, 675, 33, 614);
					centerPos = (101, 621);
					break;

				case 30:
					button = new TextureButton(squareImageHandle, 1000, 548, 222, 170);
					centerPos = (button.x2 - 120, button.y2 + 85);
					break;

				default:
					button = new TextureButton(squareImageHandle, 0, 0, 10, 10);
					centerPos = (0, 0);
					break;
			}
			return (centerPos, button);

		}


		/// <summary>
		/// 長方形のマスの場所を取得する
		/// </summary>
		/// <param name="index"></param>
		/// <returns>左上のX座標とY座標</returns>
		private static (int, int) GetRectSquarePos(int index)
		{
			switch (index)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					return (43 + 120 * index, 174);

				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
					return (992 - 120 * (index - 12), 392);


				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
					return (152 + 120 * (index - 23), 597);

				default:
					return (0, 0);
			}
		}


		/// <summary>
		/// メッセージボックスのテクスチャを取得する
		/// </summary>
		/// <param name="index"></param>
		/// <param name="centerPos"></param>
		/// <param name="textureHandle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private static void GetMessageBoxTextureInfo(int index,
			(int, int) centerPos, out int textureHandle,
			out int x, out int y, out int width, out int height)
		{
			if (index >= 9 && index <= 11)
			{
				textureHandle = TextureAsset.Register("RigntDirectionMessageBox",
					"../../../images/RigntDirectionMessageBox.png");
				x = centerPos.Item1 - 320 - 35;
				y = centerPos.Item2 - 60;
				width = 320;
				height = 120;

			}
			else if (index >= 20 && index <= 22)
			{
				textureHandle = TextureAsset.Register("LeftDirectionMessageBox",
					"../../../images/LeftDirectionMessageBox.png");
				x = centerPos.Item1 + 35;
				y = centerPos.Item2 - 60;
				width = 320;
				height = 120;
			}
			else
			{
				textureHandle = TextureAsset.Register("DownDirectionMessageBox",
					"../../../images/DownDirectionMessageBox.png");
				x = centerPos.Item1 - 130;
				y = centerPos.Item2 - 180 - 35;
				width = 260;
				height = 180;
			}
		}


		/// <summary>
		/// イベントのメッセージを取得する
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static string GetMessageBoxText(int index)
		{
			return index switch
			{
				1 => "初任給で\n車を買う\n2マス進む",
				5 => "GOTOトラベルで\nアメリカへ\n3マス進む",
				6 => "ステイホーム！家で過ごそう\nスタートに戻る",
				7 => "昼寝のつもりが\n次の日の朝まで寝てた\nもう一度サイコロを振る",
				10 => "リモート授業に\n繋がらない\n1回休み",
				12 => "腹筋を割り切れないまま\n海へ\n2マス戻る",
				15 => "Blackout Tuesday\n今こそ前に進もう\n3マス進む",
				17 => "食欲が収まらず\nついに破産\nもう一度サイコロを振り\n出た目の数戻る",
				19 => "握手会に行くため\n欠席\n一回休み",
				21 => "これが噂の\n無限くら寿司\nもう一度サイコロを振る",
				24 => "ランサムウェア\nに感染\n4マス戻る",
				25 => "ワクチン開発に\n時間がかかる\n一回休み",
				26 => "届いたのは\nザ・ノースフォイスだった！\nもう一度サイコロを振り\n出た目の数戻る",
				28 => "１社目の内定に\n落ちた\nもう一度サイコロを振る",
				29 => "脱サラして\n焼き芋屋を開店\nスタートに戻る",
				30 => "ゴール",
				_ => string.Empty
			};
		}
	}
}
