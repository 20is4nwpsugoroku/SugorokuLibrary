using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(CloseCreateConverter))]
	public class CloseCreateMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "closeCreate";

		[JsonProperty("matchKey")] public string MatchKey { get; }

		public CloseCreateMessage(string matchKey)
		{
			MatchKey = matchKey;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is CloseCreateMessage cl)) return false;
			return cl.MatchKey == MatchKey;
		}
	}
}