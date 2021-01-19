using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class GoStartEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.Players[playerId].Position = 0;
        }

        public override string ToString()
        {
            return "GoStart";
        }
    }
}