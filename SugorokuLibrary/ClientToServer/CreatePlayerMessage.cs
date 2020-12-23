using Newtonsoft.Json;

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
	}
}