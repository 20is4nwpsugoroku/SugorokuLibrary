using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// 1つのマッチ情報を確認するメッセージです
	/// </summary>
	/// <code>
	/// var getMatch = new GetMatchInfoMessage("部屋名");
	/// var jsonMsg = JsonConvert.SerializeObject(getMatch);
	/// var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, socket);
	/// if (result) var Matches = JsonConvert.DeserializeObject&lt;MatchInfo&gt;(msg);
	/// else var fail = JsonConvert.DeserializeObject&lt;FailedMessage&gt;(msg);
	/// </code>
	[JsonConverter(typeof(GetMatchInfoConverter))]
	public class GetMatchInfoMessage : ClientMessage
	{
		[JsonProperty("methodType")] public override string MethodType => "getMatchInfo";
		[JsonProperty("matchKey")] public string MatchKey { get; }

		public GetMatchInfoMessage(string matchKey)
		{
			MatchKey = matchKey;
		}
	}
}