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

        private readonly bool useMinMax = false;

        public NumberOption(string name) : base(name, NodeOptionType.NUMBER) {
            RegisterCommand("setValue", (p) => setValue(p));
        }

        public NumberOption(string name, double min, double max) : this(name)
        {
            MinValue.Set(min);
            MaxValue.Set(max);
            Value.Set(min);
            useMinMax = true;
        }

        private void setValue(JToken payload)
        {
            var val = payload.Value<double>();
            if (!useMinMax || (val < MaxValue.Get() && val > MinValue.Get()))
            {
                Value.Set(val);
                RaiseValueChanged();
            }
        }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
