namespace SugorokuLibrary.ClientToServer
{
	public class GetMatchInfoMessage : IClientMessage
	{
		public string MethodType => "getMatchInfo";
		public string MatchKey { get; }

		public GetMatchInfoMessage(string matchKey)
		{
			MatchKey = matchKey;
		}
	}
}