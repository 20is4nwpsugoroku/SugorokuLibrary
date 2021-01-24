using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
    [JsonConverter(typeof(GetRankingConverter))]
    public class GetRankingMessage : ClientMessage
    {
        public override string MethodType => "getRanking";
        
        public string MatchKey { get; }

        public GetRankingMessage(string matchKey)
        {
            MatchKey = matchKey;
        }
    }
}