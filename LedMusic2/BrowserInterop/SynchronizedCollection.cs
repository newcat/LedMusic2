using System.Collections.Generic;

namespace LedMusic2.BrowserInterop
{
    public class SynchronizedCollection<T> : List<T>, ISynchronizable
        where T : ISimpleSynchronizable
    {

        public new void Add(T item)
        {
            base.Add(item);
        }

    }
}
