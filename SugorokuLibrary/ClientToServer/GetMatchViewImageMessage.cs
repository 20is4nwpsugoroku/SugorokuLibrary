using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
    [JsonConverter(typeof(GetMatchViewImageMessageConverter))]
    public class GetMatchViewImageMessage : ClientMessage
    {
        public override string MethodType => "getMatchViewImage";
        
        public string MatchKey { get; }

        public GetMatchViewImageMessage(string matchKey)
        {
            MatchKey = matchKey;
        }
    }
}