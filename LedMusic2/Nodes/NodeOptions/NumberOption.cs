using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class NumberOption : BaseOption
    {

        public ReactivePrimitive<double> Value { get; }
            = new ReactivePrimitive<double>(0.0);
        public ReactivePrimitive<double> MinValue { get; }
            = new ReactivePrimitive<double>(0.0);
        public ReactivePrimitive<double> MaxValue { get; }
            = new ReactivePrimitive<double>(1.0);
        public ReactivePrimitive<bool> UseMinMax { get; }
            = new ReactivePrimitive<bool>(false);

        public NumberOption() : base() { }
        public NumberOption(string name) : base(name, NodeOptionType.NUMBER) { }
        public NumberOption(string name, double min, double max) : this(name)
        {
            MinValue.Set(min);
            MaxValue.Set(max);
            Value.Set(min);
            UseMinMax.Set(true);
        }

        protected override void SetValue(JToken payload)
        {
            var val = payload.Value<double>();
            if (!UseMinMax.Get() || (val < MaxValue.Get() && val > MinValue.Get()))
                Value.Set(val);
        }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
