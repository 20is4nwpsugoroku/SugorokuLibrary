using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(DiceMessageConverter))]
	public class DiceMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "dice";
		[JsonProperty("matchId")] public string MatchId { get; }
		[JsonProperty("playerId")] public int PlayerId { get; }
		[JsonProperty("nowPosition")] public int NowPosition { get; }

		public DiceMessage(string matchId, int playerId, int nowPosition)
		{
			MatchId = matchId;
			PlayerId = playerId;
			NowPosition = nowPosition;
		}
	}
}