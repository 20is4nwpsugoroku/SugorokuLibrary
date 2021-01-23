using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using DxLibDLL;
using SugorokuClient.UI;
using SugorokuClient.Util;
using SugorokuLibrary;
using SugorokuLibrary.ClientToServer;




namespace SugorokuClient.Scene
{
	public class Game : IScene
	{
		private SugorokuFrame SugorokuFrame { get; set; }
		private static Timer EventTimer { get; set; }
		private static State state { get; set; }

		enum State
		{
			WaitMatchStarted,
			WaitOtherPlayer,
		}

		






		public Game()
		{
		}

		public void Init()
		{
			DX.SetBackgroundColor(255, 255, 255);
			SugorokuFrame = new SugorokuFrame();
			EventTimer = new Timer();
			EventTimer.Elapsed += (o, e) => WaitStartMatchTask(o, e, CommonData.Player.IsHost);
			EventTimer.Interval = 5000;
			EventTimer.AutoReset = true;
			EventTimer.Enabled = true;
			EventTimer.Start();
		}

		public void Update()
		{
			SugorokuFrame.Update();
		}

		public void Draw()
		{
			SugorokuFrame.Draw();
		}


		private static void PlayingMatchTask(object source, ElapsedEventArgs e)
		{
			var (r, info) = GetMatchInfo(CommonData.MatchInfo.MatchKey);

		}


		private static void WaitStartMatchTask(object source, ElapsedEventArgs e, bool isHost)
		{
			var matchKey = CommonData.MatchInfo.MatchKey;
			if (isHost)
			{
				state = (IsAbleToStart(matchKey, CommonData.PlayerNum))
					? State.WaitOtherPlayer : State.WaitMatchStarted;
				if (state == State.WaitOtherPlayer) CloseJoinMatch(matchKey);
			}
			else
			{
				state = (IsMatchStarted(matchKey)) ? State.WaitMatchStarted : State.WaitOtherPlayer;
			}

			if (state == State.WaitOtherPlayer)
			{
				EventTimer.Stop();
				EventTimer.Elapsed += (o, e) => PlayingMatchTask(o, e);
				EventTimer.Start();
			}
		}


		private static bool IsMatchStarted(string matchKey)
		{
			var getInfo = new GetStartedMatchMessage(matchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (r, _) = SocketManager.SendRecv(json);
			return r;
		}


		private static bool IsAbleToStart(string matchKey, int playerNum)
		{
			var (r, info) = GetMatchInfo(matchKey);
			if (r)
			{
				r = info.Players.Count >= playerNum;
			}
			return r;
		}


		private static void CloseJoinMatch(string matchKey)
		{
			var getInfo = new CloseCreateMessage(CommonData.MatchInfo.MatchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (_, _) = SocketManager.SendRecv(json);
		}


		private static (bool, MatchInfo) GetMatchInfo(string matchKey)
		{
			var getInfo = new GetMatchInfoMessage(matchKey);
			var json = JsonConvert.SerializeObject(getInfo);
			var (r, msg) = SocketManager.SendRecv(json);
			return (r)
				? (r, JsonConvert.DeserializeObject<MatchInfo>(msg))
				: (r, new MatchInfo());
		}
	}
}
