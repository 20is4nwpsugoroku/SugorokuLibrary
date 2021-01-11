using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(CreatePlayerConverter))]
	public class CreatePlayerMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "createPlayer";

		[JsonProperty("playerName")] public string PlayerName { get; }

		[JsonProperty("matchKey")] public string MatchKey { get; }

		public CreatePlayerMessage(string playerName, string matchKey)
		{
			PlayerName = playerName;
			MatchKey = matchKey;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is CreatePlayerMessage cr)) return false;
			return cr.MatchKey == MatchKey && cr.PlayerName == PlayerName;
		}
	}
}