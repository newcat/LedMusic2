using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class BoolOption : BaseOption
    {

        public ReactivePrimitive<bool> Value = new ReactivePrimitive<bool>(false);

        public BoolOption(string name) : base(name, NodeOptionType.BOOL) {
            RegisterCommand("setValue", (p) => setValue(p));
        }

        private void setValue(JToken payload)
        {
            Value.Set(payload.Value<bool>());
            RaiseValueChanged();
        }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
