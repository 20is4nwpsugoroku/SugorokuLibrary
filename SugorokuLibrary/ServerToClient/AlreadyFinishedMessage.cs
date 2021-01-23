using System.Collections.Generic;
using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(AlreadyFinishedMessageConverter))]
    public class AlreadyFinishedMessage : ServerMessage
    {
        public override string MethodType => "alreadyFinished";
        public int GoaledPlayerId { get; }
        public IEnumerable<int> Ranking { get; }

        public AlreadyFinishedMessage(int goaledPlayerId, IEnumerable<int> ranking)
        {
            GoaledPlayerId = goaledPlayerId;
            Ranking = ranking;
        }
    }
}