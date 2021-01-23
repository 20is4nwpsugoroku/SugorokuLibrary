using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(SquareEventConverter))]
    public abstract class SquareEvent
    {
        public abstract void Event(MatchCore matchCore, int playerId);
        public abstract string Message { get; }
    }
}