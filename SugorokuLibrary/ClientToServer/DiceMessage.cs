using System;
using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	/// <summary>
	/// すごろくを降る要求メッセージを作成するクラスです
	/// var dice = new DiceMessage("部屋名", プレイヤーID);
	/// var jsonMsg = JsonConvert.SerializeObject(dice);
	/// var (_, result, msg) = SugorokuLibrary.Protocol.Connection.SendAndRecvMessage(jsonMsg, socket);
	/// if (result) var diceResult = JsonConvert.DeserializeObject&lt;DiceResultMessage&gt;(msg);
	/// else var fail = JsonConvert.DeserializeObject&lt;FailedMessage&gt;(msg);
	/// </summary>
	[JsonConverter(typeof(DiceMessageConverter))]
	public class DiceMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "dice";
		[JsonProperty("matchKey")] public string MatchKey { get; }
		[JsonProperty("playerId")] public int PlayerId { get; }

		public DiceMessage(string matchKey, int playerId)
		{
			MatchKey = matchKey;
			PlayerId = playerId;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is DiceMessage di)) return false;
			return di.MatchKey == MatchKey && di.PlayerId == PlayerId;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(MatchKey, PlayerId);
		}
	}
}