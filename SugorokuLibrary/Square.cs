using SugorokuLibrary.SquareEvents;

namespace SugorokuLibrary
{
    public class Square
    {
        public int Index { get; set; }
        public ISquareEvent Event { get; set; }
    }
}