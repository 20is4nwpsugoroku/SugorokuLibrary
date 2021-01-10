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

		public DiceMessage(string matchKey, int playerId)
		{
			MatchKey = matchKey;
			PlayerId = playerId;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is DiceMessage di)) return false;
			return di.MatchKey == MatchKey && di.PlayerId == PlayerId;
		}
	}
}