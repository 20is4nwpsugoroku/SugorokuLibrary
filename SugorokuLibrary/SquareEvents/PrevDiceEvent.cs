using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(PrevDiceEventConverter))]
    public class PrevDiceEvent : SquareEvent
    {
        public override string Message { get; }

        public PrevDiceEvent(string message)
        {
            Message = message + "\nサイコロで出た数だけ戻る";
        }

        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.NextPlayerPrevDice = true;
            matchCore.ActionSchedule.Insert(0, playerId);
        }

        public override string ToString()
        {
            return "PrevDice";
        }
    }
}