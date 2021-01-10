using SugorokuLibrary.Match;

namespace SugorokuLibrary.SquareEvents
{
    public class WaitEvent : ISquareEvent
    {
        public void Event(MatchCore matchCore, int playerId)
        {
            matchCore.ActionSchedule.Remove(playerId);
        }
    }
}