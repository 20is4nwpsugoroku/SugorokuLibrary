using System.Collections.Generic;
using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(RankingMessageConverter))]
    public class RankingMessage : ServerMessage
    {
        public override string MethodType => "ranking";
        
        public IEnumerable<int> Ranking { get; }

        public RankingMessage(IEnumerable<int> ranking)
        {
            Ranking = ranking;
        }
    }
}