using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(GetMatchInfoConverter))]
	public class GetMatchInfoMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "getMatchInfo";
		[JsonProperty("matchKey")] public string MatchKey { get; }

		public GetMatchInfoMessage(string matchKey)
		{
			MatchKey = matchKey;
		}
	}
}