namespace SugorokuLibrary
{
    public class NextEvent : ISquareEvent
    {
        public int NextCount { get; set; }

        public void Event()
        {
            throw new System.NotImplementedException();
        }
    }
}