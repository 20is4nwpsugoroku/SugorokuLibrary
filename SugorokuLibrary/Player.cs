using Newtonsoft.Json;

namespace SugorokuLibrary
{
	/// <summary>
	/// プレイヤーの情報
	/// </summary>
	public class Player
	{
		///<value>マッチの識別キー</value>
		[JsonProperty("matchKey")]
		public string MatchKey { get; set; }

		///<value>プレイヤーを識別するためのID</value>
		[JsonProperty("playerID")]
		public int PlayerID { get; set; }

		///<value>プレイヤーの名前</value>
		[JsonProperty("playerName")]
		public string PlayerName { get; set; }
		
		[JsonIgnore]
		public bool Wait { get; set; }

		[JsonProperty("isHost")] public bool IsHost { get; set; }

		///<value>プレイヤーの位置</value>
		[JsonProperty("position")]
		public int Position { get; set; }

		public override bool Equals(object? obj)
		{
			if (!(obj is Player player)) return false;

			return player.IsHost == IsHost && player.Position == Position &&
			       player.MatchKey == MatchKey && player.PlayerName == PlayerName &&
			       player.PlayerID == PlayerID;
		}
	}
}