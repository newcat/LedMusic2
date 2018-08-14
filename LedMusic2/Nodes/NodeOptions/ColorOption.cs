using LedMusic2.Reactive;
using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorOption : BaseOption
    {

        public ReactivePrimitive<LedColor> Value = new ReactivePrimitive<LedColor>(new LedColorRGB(0, 0, 0));

        public ColorOption() : base() { }
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
