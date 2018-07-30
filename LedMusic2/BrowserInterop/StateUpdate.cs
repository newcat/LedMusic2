using System;
using System.Collections.Generic;

namespace LedMusic2.BrowserInterop
{

    public interface IStateUpdate
    {
        string Name { get; }
        void Print(int depth);
    }

    public class StateUpdate<T> : IStateUpdate
    {
        public string Name { get; }
        public T Value { get; }
        public StateUpdate(string path, T value)
        {
            Name = path;
            Value = value;
        }

        public void Print(int depth)
        {
            for (int i = 0; i < depth; i++)
                Console.Write("  ");
            if (typeof(IStateUpdate).IsAssignableFrom(typeof(T)))
            {
                Console.WriteLine("{0}:", Name);
                (Value as IStateUpdate).Print(depth + 1);
            }
            else if (typeof(T) == typeof(StateUpdateCollection))
            {
                Console.WriteLine("{0}:", Name);
                (Value as StateUpdateCollection).Print(depth + 1);
            }
            else
                Console.WriteLine("{0}: {1}", Name, Value.ToString());
        }

    }

    public class StateUpdateCollection : List<IStateUpdate>
    {
        public StateUpdateCollection() { }
        public StateUpdateCollection(IEnumerable<IStateUpdate> updates) : base(updates) { }
        public StateUpdateCollection(params IStateUpdate[] updates) : base(updates) { }
        public void Print(int depth)
        {
            foreach (var u in this)
                u?.Print(depth);
        }
    }

}
