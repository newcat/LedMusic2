using LedMusic2.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedMusic2.LedColors
{
    public struct LedColorArray : IEquatable<LedColorArray>, ISerializable
    {

        private LedColor[] values;
        public int Length => values?.Length ?? 0;

        public LedColorArray(IEnumerable<LedColor> collection)
        {
            values = collection.ToArray();
        }

        public string Serialize()
        {
            return values == null ? "" : string.Join(",", values.Select((c) => c.Serialize()));
        }

        public object Deserialize(string s)
        {
            var c = new LedColor(0, 0, 0);
            var parts = s.Split(',');
            var l = new List<LedColor>(parts.Length);
            foreach (var part in parts)
            {                
                c.Deserialize(part);
                l.Add(c);
            }
            return new LedColorArray(l);
        }

        public bool Equals(LedColorArray other)
        {
            if (Length != other.Length) return false;
            for (int i = 0; i < Length; i++)
            {
                if (!this[i].Equals(other[i])) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return values
                .Select((c) => c.GetHashCode())
                .Aggregate((p, c) => p ^ c);
        }

        public LedColor this[int i]
        {
            get { return values == null ? default(LedColor) : values[i]; }
            set { values[i] = value; }
        }

    }
}
