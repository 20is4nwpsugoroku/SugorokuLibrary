using System;
using System.Collections.Generic;
using System.Text;

namespace SugorokuClient.Scene
{
	public class SugorokuEvent
	{
		public int EventStartPos { get; private set; }
		public int EventEndPos { get; private set; }
		public int PlayerId { get; private set; }
		public int Dice { get; private set; }

		public SugorokuEvent(int startPos, int endPos, int playerId, int dice)
		{
			EventStartPos = startPos;
			EventEndPos = endPos;
			PlayerId = playerId;
			Dice = dice;
		}
	}
}
