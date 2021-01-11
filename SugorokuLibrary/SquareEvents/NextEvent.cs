using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class NextEvent : ISquareEvent
    {
        public int NextCount { get; set; }
        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position += NextCount;
        }

        public override string ToString()
        {
            return $"Next {NextCount}";
        }
    }
}