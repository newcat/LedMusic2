using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Models
{
    public class Keyframe : IComparable<Keyframe>, IEquatable<Keyframe>
    {

        public object Value { get; set; }
        public int Frame { get; set; }

        public Keyframe(int frame, object value)
        {
            Frame = frame;
            Value = value;
        }

        public int CompareTo(Keyframe other)
        {
            return Frame - other.Frame;
        }

        public bool Equals(Keyframe other)
        {
            return Frame == other.Frame;
        }

    }
}
