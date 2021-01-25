using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// この要求を送ると現在開かれている部屋をすべて確認することができます。
	/// </summary>
	/// <code>
	/// var getAll = new GetAllMatchesMessage();
	/// var jsonMsg = JsonConverter.SerializeObject(getAll);
	/// var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, socket);
	/// if (result) var allMatches = JsonConvert.DeserializeObject&lt;Dictionary&lt;string, MatchInfo&gt;&gt;(msg);
	/// else var fail = JsonConvert.DeserializeObject&lt;FailedMessage&gt;(msg);
	/// </code>
	[JsonConverter(typeof(GetAllMatchesConverter))]
	public class GetAllMatchesMessage : ClientMessage
	{
		[JsonProperty("methodType")] public override string MethodType => "getAllMatches";
	}
}