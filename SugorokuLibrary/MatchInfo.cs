using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuLibrary
{
	class MatchInfo
	{
		public int MatchID { get; set; }
		public int[] PlayerIDs { get; set; }
		public int Turn { get; set; }
		public long StartAtUnixTime { get; }
		public long EndAtUnixTime { get; }
	}
}
