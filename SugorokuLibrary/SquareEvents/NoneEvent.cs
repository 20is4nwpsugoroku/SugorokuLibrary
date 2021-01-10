using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class NoneEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
        }
    }
}