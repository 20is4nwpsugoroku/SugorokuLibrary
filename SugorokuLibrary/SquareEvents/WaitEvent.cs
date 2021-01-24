using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(WaitEventConverter))]
    public class WaitEvent : SquareEvent
    {
        public override string Message { get; }

        public WaitEvent(string message)
        {
            Message = message + "\n1回休み";
        }
        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.ActionSchedule.RemoveLast(playerId);
        }

        public override string ToString()
        {
            return "Wait";
        }
    }
}