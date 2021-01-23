using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(GoStartEventConverter))]
    public class GoStartEvent : SquareEvent
    {
        public override string Message { get; }

        public GoStartEvent(string message)
        {
            Message = message + "\nスタートへ戻る";
        }
        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position = 0;
        }

        public override string ToString()
        {
            return "GoStart";
        }
    }
}