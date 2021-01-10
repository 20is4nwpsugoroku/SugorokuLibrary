using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class DiceAgainEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.ActionSchedule.Insert(0, playerId);
        }
    }
}