using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	/// <summary>
	/// シーンクラス用のインターフェイス
	/// </summary>
	public interface IScene
	{
		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="data">各シーンで共用するデータ</param>
		public void Init(CommonData data);

		/// <summary>
		/// 更新を行う関数
		/// </summary>
		public void Update();
		
		/// <summary>
		/// 描画を行う関数
		/// </summary>
		public void Draw();
	}
}
