using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class NoneEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
        }

        public override string ToString()
        {
            return "None";
        }
    }
}