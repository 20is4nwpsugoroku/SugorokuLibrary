using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;


namespace SugorokuClient
{
	public partial class Form1 : Form
	{
		// 画像を左右に動かす処理のための変数を初期化
		int X = 0, XAdd = 8;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.ClientSize = new Size(640, 480);
			DX.SetUserWindow(this.Handle); //DxLibの親ウインドウをこのフォームウインドウにセット
			DX.DxLib_Init();
			// 描画先を裏画面に変更
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
		}

		//ループする関数
		public void MainLoop()
		{
			// 画面をクリア
			DX.ClearDrawScreen();
			// 画像を描画する座標を更新
			X += XAdd;
			if (X < 0 || X > 640 - 32)
			{
				XAdd = -XAdd;
			}
			// 四角を描画
			DX.DrawBox(X, 32 * 5, X + 32, 32 * 6, DX.GetColor(255, 255, 255), 1);
			// 裏画面の内容を表画面に反映する
			DX.ScreenFlip();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			DX.DxLib_End();
		}
	}
}
