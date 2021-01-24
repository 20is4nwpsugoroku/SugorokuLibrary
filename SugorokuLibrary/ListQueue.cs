using System.Collections.Generic;

namespace SugorokuLibrary
{
    public class ListQueue<T> : List<T>
    {
        public void Enqueue(T item)
        {
            Add(item);
        }

        public T Dequeue()
        {
            var t = base[0];
            RemoveAt(0);
            return t;
        }

        public bool RemoveLast(T item)
        {
            Reverse();
            var result = Remove(item);
            Reverse();
            return result;
        }

        public T Peek()
        {
            return base[0];
        }
    }
}