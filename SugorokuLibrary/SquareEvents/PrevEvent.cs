using Newtonsoft.Json;
using SugorokuLibrary.Match;
using SugorokuLibrary.SquareEvents.Converter;

namespace SugorokuLibrary.SquareEvents
{
    [JsonConverter(typeof(PrevEventConverter))]
    public class PrevEvent : SquareEvent
    {
        public int BackCount { get; }
        public override string Message { get; }

        public PrevEvent(int backCount, string message)
        {
            BackCount = backCount;
            Message = message + $"\n{backCount}マス戻る";
        }

        public override void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position -= BackCount;
        }

        public override string ToString()
        {
            return $"Prev {BackCount}";
        }
    }
}