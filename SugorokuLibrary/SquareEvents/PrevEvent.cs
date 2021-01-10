using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class PrevEvent : ISquareEvent
    {
        public int BackCount { get; set; }

        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position -= BackCount;
        }
    }
}