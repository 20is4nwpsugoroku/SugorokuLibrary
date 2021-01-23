using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(NextEventConverter))]
    public class NextEvent : SquareEvent
    {
        public int NextCount { get; }
        public override string Message { get; }

        public NextEvent(int nextCount, string message)
        {
            NextCount = nextCount;
            Message = message + $"\n{nextCount}マス進む";
        }
        
        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position += NextCount;
        }

        public override string ToString()
        {
            return $"Next {NextCount}";
        }
    }
}