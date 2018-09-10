using LedMusic2.Reactive;
using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorOption : BaseOption
    {

        public ReactivePrimitive<LedColor> Value { get; } = new ReactivePrimitive<LedColor>(new LedColor(0, 0, 0));

        public ColorOption(JToken j) : base(j) { }
        public ColorOption(string name) : base(name, NodeOptionType.COLOR) { }

        public override object GetValue()
        {
            return Value.Get();
        }

        protected override void SetValue(JToken payload)
        {
            Value.HandleCommand("set", payload);
        }

    }
}
