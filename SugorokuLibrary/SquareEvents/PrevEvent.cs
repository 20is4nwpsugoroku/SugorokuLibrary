namespace SugorokuLibrary.SquareEvents
{
    public class PrevEvent : ISquareEvent
    {
        public int BackCount { get; set; }

        public void Event()
        {
            throw new System.NotImplementedException();
        }
    }
}