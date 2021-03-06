using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(GetStartedMatchConverter))]
	public class GetStartedMatchMessage : ClientMessage
	{
		public override string MethodType => "getStartedMatch";
		public string MatchKey { get; }

		public GetStartedMatchMessage(string matchKey)
		{
			MatchKey = matchKey;
		}
	}
}