using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class PrevDiceEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.ActionSchedule.Insert(0, playerId);
        }
    }
}