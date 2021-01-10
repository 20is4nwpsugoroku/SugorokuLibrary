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
        [JsonProperty("nowPosition")] public int NowPosition { get; }

        public PrevDiceMessage(string matchKey, int playerId, int nowPosition)
        {
            MatchKey = matchKey;
            PlayerId = playerId;
            NowPosition = nowPosition;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is PrevDiceMessage pr)) return false;
            return pr.MatchKey == MatchKey && pr.PlayerId == PlayerId && pr.NowPosition == NowPosition;
        }
    }
}