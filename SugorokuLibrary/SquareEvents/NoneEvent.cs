using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(NoneEventConverter))]
    public class NoneEvent : SquareEvent
    {
        public override void Event(MatchCore matchCore, int playerId)
        {
        }

        public override string Message => "";

        public override string ToString()
        {
            return "None";
        }
    }
}