using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Util
{
	/// <summary>
	/// DXライブラリ用の描画タイミング調整クラス
	/// </summary>
	public class FPSAdjuster
	{
        ///<value> フレーム時刻の基準となる時刻（単位：ms）</value>
        private long BaseTickCount { get; set; }

        ///<value> 前回のフレームの時刻（単位：µs）</value>
        private long PrevTickCount { get; set; }

        ///<value> 現在のフレームの時刻（単位：μs）</value>
        private long NowTickCount { get; set; }

        ///<value> 1フレームの時間（単位：μs）</value>
        private int Period { get; set; }

        ///<value> FPS計測用のカウンタ値 </value>
        private int FpsCount { get; set; }

        ///<value> FPS測定用の時刻（単位：ms）</value>
        private int FpsTickCount { get; set; }

        ///<value> 目標のFPS </value>
        public int Fps { get; private set; }

        ///<value> 計測した（実際の）FPS </value>
        public int FpsReal { get; private set; }


        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        FPSAdjuster()
        {
            BaseTickCount = 0;
            PrevTickCount = 0;
            NowTickCount = 0;
            Period = 0;
            Fps = 0;
            FpsReal = 0;
            FpsCount = 0;
            FpsTickCount = 0;
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fps">目標のFPS（省略時は60FPS）</param>
        public FPSAdjuster(int fps = 60) : this()
        {
            BaseTickCount = System.Environment.TickCount;
            this.Fps = fps;
            Period = 1000 * 1000 / fps;
        }

        /// <summary>
        /// 次のフレームを待つ。ゲームループの先頭で呼び出す。
        /// </summary>
        public void WaitNextFrame()
        {
            // フレームの時刻を更新する
            PrevTickCount += Period;

            // 基準となる時刻からの差分を求める。
            NowTickCount = (System.Environment.TickCount - BaseTickCount) * 1000;

            // 次のフレームまで到達しているか？
            if (NowTickCount >= (PrevTickCount + Period))
            {
                return;
            }

            // 次のフレームまで到達していなければ、スリープする
            while (NowTickCount < (PrevTickCount + Period))
            {
                System.Threading.Thread.Sleep(1);
                NowTickCount = (System.Environment.TickCount - BaseTickCount) * 1000;
            }
        }


        /// <summary>
        /// このフレームの描画が行えるかどうかを返す。
        /// 現在の時刻と次のフレームの時刻の差から描画する余裕があれば ture，
        /// 余裕がなければ false を返す
        /// </summary>
        /// <returns>true:描画可能 false:描画不可</returns>
        public bool IsDraw()
        {
            // 基準となる時刻からの差分を求める。
            NowTickCount = (System.Environment.TickCount - BaseTickCount) * 1000;

            // 描画する時間がある場合は描画可能
            if (NowTickCount < (PrevTickCount + Period * 2))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// FPSを計算する。描画後に使用する
        /// FPSを計測しない場合は呼ぶ必要はない。
        /// </summary>
        public void CalcFps()
        {
            // カウンタを増やす
            FpsCount++;

            // 前回の計測時刻から1秒以上経過していれば、フレームレートを計算
            int tickCount = System.Environment.TickCount;
            if (tickCount - FpsTickCount >= 1000)
            {
                FpsReal = (FpsCount * 1000) / (tickCount - FpsTickCount);
                FpsTickCount = tickCount;
                FpsCount = 0;
            }
        }
    }
}
