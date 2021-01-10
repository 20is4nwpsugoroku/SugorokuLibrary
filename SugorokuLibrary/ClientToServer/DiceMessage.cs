using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(DiceMessageConverter))]
	public class DiceMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "dice";
		[JsonProperty("matchKey")] public string MatchKey { get; }
		[JsonProperty("playerId")] public int PlayerId { get; }
		[JsonProperty("nowPosition")] public int NowPosition { get; }

		public DiceMessage(string matchKey, int playerId, int nowPosition)
		{
			MatchKey = matchKey;
			PlayerId = playerId;
			NowPosition = nowPosition;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is DiceMessage di)) return false;
			return di.MatchKey == MatchKey && di.PlayerId == PlayerId && di.NowPosition == NowPosition;
		}
	}
}