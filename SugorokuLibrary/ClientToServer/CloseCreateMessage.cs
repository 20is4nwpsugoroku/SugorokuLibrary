using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// 部屋への新規参加のプレイヤーを締め切るためのメッセージを作成するクラス
	/// </summary>
	/// <code>
	/// var closeMessage = new CloseCreateMessage("部屋名");
	/// var jsonMsg = JsonConvert.SerializeObject(closeMessage);
	/// var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, socket);
	/// if (result) var matchInfo = JsonConvert.DeserializeObject&lt;MatchInfo&gt;(msg);
	/// else var fail = JsonConvert.DeserializeObject&lt;FailedMessage&gt;(msg);
	/// </code>
	[JsonConverter(typeof(CloseCreateConverter))]
	public class CloseCreateMessage : ClientMessage
	{
		[JsonProperty("methodType")] public override string MethodType => "closeCreate";

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

		public override int GetHashCode()
		{
			return MatchKey.GetHashCode();
		}
	}
}