using LedMusic2.Reactive;

namespace LedMusic2.Nodes.NodeOptions
{
    public class NumberOption : BaseOption
    {

        public ReactivePrimitive<double> Value = new ReactivePrimitive<double>(0.0);
        public ReactivePrimitive<double> MinValue = new ReactivePrimitive<double>(0.0);
        public ReactivePrimitive<double> MaxValue = new ReactivePrimitive<double>(1.0);

        public NumberOption(string name) : base(name, NodeOptionType.NUMBER) { }
        public NumberOption(string name, double min, double max) : this(name)
        {
            MinValue.Set(min);
            MaxValue.Set(max);
            Value.Set(min);
        }

    }
}
