using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public interface ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId);
    }
}