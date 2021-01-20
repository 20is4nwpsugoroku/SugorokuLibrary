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
using SugorokuClient.Scene;


namespace SugorokuClient
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.ClientSize = new Size(640, 480);
			DX.SetUserWindow(this.Handle); //DxLibの親ウインドウをこのフォームウインドウにセット
			DX.SetGraphMode(640, 480, 32);
			DX.DxLib_Init();
			// 描画先を裏画面に変更
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetMainWindowText("○×ゲーム");
			SceneManager.Initialize();
			IScene title = new Title();
			SceneManager.AddScene("title", title);
			SceneManager.ChangeScene("title");
		}

		//ループする関数
		public void MainLoop()
		{
			if (SceneManager.Update() == -1)
			{
				DX.DxLib_End();
				this.Close();
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			DX.DxLib_End();
		}
	}
}
