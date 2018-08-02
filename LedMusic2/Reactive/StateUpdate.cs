using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Reactive
{

    public interface IStateUpdate
    {
        string Name { get; }
        void Print(int depth);
        JToken ToJson();
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

        public JToken ToJson()
        {
            if (typeof(IStateUpdate).IsAssignableFrom(typeof(T)))
            {
                // Value is also a IStateUpdate
                return new JProperty(Name, (Value as IStateUpdate).ToJson());
            } else if (typeof(T) == typeof(StateUpdateCollection))
            {
                // Value is a StateUpdateCollection
                return new JProperty(Name, (Value as StateUpdateCollection)?.ToJson());
            } else
            {
                // Value is a class or primitive type
                return new JProperty(Name, Value);
            }
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
                (Value as StateUpdateCollection)?.Print(depth + 1);
            }
            else
                Console.WriteLine("{0}: {1}", Name, Value?.ToString() ?? "null");
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
        public JToken ToJson()
        {
            return new JObject(from el in this select el.ToJson());
        }
    }

}
