using LedMusic2.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.LedColors
{
    public class LedColorArray : List<LedColor>, IEquatable<LedColorArray>, ISerializable
    {

        public LedColorArray() : base() { }

        public LedColorArray(int capacity) : base(capacity)
        {
        }

        public LedColorArray(IEnumerable<LedColor> collection) : base(collection)
        {
        }

        public string Serialize()
        {
            return string.Join(",", this.Select((c) => c.Serialize()));
        }

        public void Deserialize(string s)
        {
            var parts = s.Split(',');
            Clear();
            foreach (var part in parts)
            {
                var c = new LedColorRGB(0, 0, 0);
                c.Deserialize(part);
                Add(c);
            }
        }

        public bool Equals(LedColorArray other)
        {
            if (Count != other.Count) return false;
            for (int i = 0; i < Count; i++)
            {
                if (!this[i].Equals(other[i])) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this
                .Select((c) => c.GetHashCode())
                .Aggregate((p, c) => p ^ c);
        }

    }
}
