using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// プレイヤーの作成・参加を行うメッセージを作成するクラス
	/// </summary>
	/// <code>
	/// var create = new CreatePlayerMessage("名前", "部屋名");
	/// var jsonMsg = JsonConvert.SerializeObject(create);
	/// var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, socket);
	/// if (result) var player = JsonConvert.DeserializeObject&lt;Player&gt;(msg);
	/// else var fail = JsonConvert.DeserializeObject&lt;FailedMessage&gt;(msg);
	/// </code>
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