using LedMusic2.BrowserInterop;

namespace LedMusic2.Nodes.NodeOptions
{
    public class NumberOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.NUMBER;
        public override string ReactiveName => "NumberOption";

        public ReactivePrimitive<double> Value = new ReactivePrimitive<double>("Value", 0.0);
        public ReactivePrimitive<double> MinValue = new ReactivePrimitive<double>("MinValue", 0.0);
        public ReactivePrimitive<double> MaxValue = new ReactivePrimitive<double>("MaxValue", 1.0);

        public NumberOption(string name) : base(name) { }
        public NumberOption(string name, double min, double max) : this(name)
        {
            MinValue.Set(min);
            MaxValue.Set(max);
            Value.Set(min);
        }

    }
}
