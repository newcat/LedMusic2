using LedMusic2.Reactive;
using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorOption : BaseOption
    {

        public ReactivePrimitive<LedColor> Value = new ReactivePrimitive<LedColor>(new LedColorRGB(0, 0, 0));

        public ColorOption(string name) : base(name, NodeOptionType.COLOR) {
            RegisterCommand("setValue", (p) => setValue(p));
        }

        public override object GetValue()
        {
            return Value.Get();
        }

        private void setValue(JToken payload)
        {
            Value.HandleCommand("set", payload);
            RaiseValueChanged();
        }

    }
}
