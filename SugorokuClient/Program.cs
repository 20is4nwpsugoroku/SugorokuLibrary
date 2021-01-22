using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;
using SugorokuClient.Util;
using SugorokuClient.Scene;

namespace SugorokuClient
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//Application.SetHighDpiMode(HighDpiMode.SystemAware);
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);

			//Form1 form = new Form1();
			//form.Show();
			// ウインドウモードで起動

			DX.ChangeWindowMode(DX.TRUE);
			DX.SetGraphMode(1280, 960, 32);
			DX.SetDoubleStartValidFlag(DX.TRUE);
			DX.SetMainWindowClassName("すごろくゲーム start at " + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
			DX.SetMainWindowText("すごろくゲーム");
			DX.DxLib_Init();

			// 描画先を裏画面に変更
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);			
			SceneManager.Initialize();
			IScene title = new Title();
			IScene game = new Game();
			SceneManager.AddScene("title", title);
			SceneManager.AddScene("game", game);
			SceneManager.ChangeScene("title");
			while (DX.ProcessMessage() != -1) //Application.Runしないで自分でループを作る
			{
				MainLoop();
				//Application.DoEvents();
			}
			//Application.Run(new Form1());
		}

		//ループする関数
		private static void MainLoop()
		{
			if (SceneManager.Update() == -1)
			{
				DX.DxLib_End();
			}
		}
	}
}
