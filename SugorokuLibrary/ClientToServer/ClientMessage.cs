using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// クライアントからサーバーに送るメッセージの親クラスです。
	/// サーバーサイドはこのクラスをデシリアライズする際ジェネリクスとして指定することでメッセージの解析を行えます
	/// </summary>
	/// <code>
	/// ClientMessage clientMessage = JsonConvert.DeserializeObject&lt;ClientMessage&gt;(jsonText);
	/// if (clientMessage is DiceMessage dm) //sample
	/// </code>
	[JsonConverter(typeof(ClientMessageConverter))]
	public abstract class ClientMessage
	{
	}
}