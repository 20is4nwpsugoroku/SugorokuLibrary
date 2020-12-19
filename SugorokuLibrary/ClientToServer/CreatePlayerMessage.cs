namespace SugorokuLibrary.ClientToServer
{
	public class CreatePlayerMessage : IClientMessage
	{
		public string MethodType => "createPlayer";
		public string PlayerName { get; }
		public string MatchKey { get; }

		public CreatePlayerMessage(string playerName, string matchKey)
		{
			PlayerName = playerName;
			MatchKey = matchKey;
		}
	}
}