using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using SugorokuClient.Util;

namespace SugorokuClient.Scene
{
	/// <summary>
	/// シーンの管理に使用するクラス
	/// </summary>
	public static class SceneManager
	{
		/// <value> 現在のシーン </value>
		private static IScene CurrentScene { get; set; }

		/// <value> 使用可能なシーン </value>
		private static Dictionary<string, IScene> Scenes { get; set; }

		/// <value> 描画タイミングの調整をするクラス </value>
		private static FPSAdjuster FpsAdjuster { get; set; }


		/// <summary>
		/// シーンマネージャーを初期化する
		/// </summary>
		public static void Initialize()
		{	
			CurrentScene = null;
			Scenes = new Dictionary<string, IScene>();
			Scenes.Clear();
			FpsAdjuster = new FPSAdjuster();
		}


		/// <summary>
		/// 現在のシーンの更新と描画処理を行う
		/// </summary>
		public static void Update()
		{
			FpsAdjuster.WaitNextFrame();
			InputManager.UpdateInput();
			CurrentScene.Update();
			if (FpsAdjuster.IsDraw())
			{
				DX.ClearDrawScreen();
				CurrentScene.Draw();
			}

		}


		/// <summary>
		/// 指定した名前でシーンを追加する
		/// </summary>
		/// <param name="sceneName">シーンの名前</param>
		/// <param name="scene">対応するシーンのインスタンス</param>
		public static void AddScene(string sceneName, IScene sceneInstance)
		{
			if (!Scenes.ContainsKey(sceneName))
			{
				Scenes.Add(sceneName, sceneInstance);
			}
		}
		

		/// <summary>
		/// sceneNameで指定したシーンを削除する
		/// </summary>
		/// <param name="sceneName"></param>
		public static void DeleteScene(string sceneName)
		{
			Scenes.Remove(sceneName);
		}


		/// <summary>
		/// 指定したシーンに遷移する
		/// </summary>
		/// <param name="sceneName">AddSceneで指定したシーンの名前</param>
		public static void ChangeScene(string sceneName)
		{
			CurrentScene = Scenes[sceneName];
			CurrentScene.Init();
		}


		// <summary>
		/// シーンの初期化を行わずに指定したシーンに遷移する
		/// </summary>
		/// <param name="sceneName">AddSceneで指定したシーンの名前</param>
		public static void ChangeSceneNoInit(string sceneName)
		{
			CurrentScene = Scenes[sceneName];
		}
	}
}
