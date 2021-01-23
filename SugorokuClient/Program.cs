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
			SceneManager.AddScene(SceneManager.SceneName.Title, title);
			SceneManager.AddScene(SceneManager.SceneName.Game, game);
			SceneManager.ChangeScene(SceneManager.SceneName.Title);
			while (DX.ProcessMessage() != -1)
			{
				MainLoop();
			}
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
