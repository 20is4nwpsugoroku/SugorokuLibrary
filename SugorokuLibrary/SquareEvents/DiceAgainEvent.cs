using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(DiceAgainEventConverter))]
    public class DiceAgainEvent : SquareEvent
    {
        public override string Message { get; }

        public DiceAgainEvent(string message)
        {
            Message = message + "\nもう一度サイコロを振る";
        }

        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.ActionSchedule.Insert(0, playerId);
        }

        public override string ToString()
        {
            return "DiceAgain";
        }
    }
}