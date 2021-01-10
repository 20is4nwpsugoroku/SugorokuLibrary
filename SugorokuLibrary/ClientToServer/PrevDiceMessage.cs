using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
    [JsonConverter(typeof(PrevDiceMessageConverter))]
    public class PrevDiceMessage : ClientMessage
    {
        [JsonProperty("methodType")] public string MethodType => "prevDice";
        [JsonProperty("matchKey")] public string MatchKey { get; }
        [JsonProperty("playerId")] public int PlayerId { get; }

        public PrevDiceMessage(string matchKey, int playerId)
        {
            MatchKey = matchKey;
            PlayerId = playerId;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is PrevDiceMessage pr)) return false;
            return pr.MatchKey == MatchKey && pr.PlayerId == PlayerId;
        }
    }
}