using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    public class BoolOption : BaseOption
    {

        public ReactivePrimitive<bool> Value = new ReactivePrimitive<bool>(false);

        public BoolOption() : base() { Initialize(); }
        public BoolOption(string name) : base(name, NodeOptionType.BOOL) { }

        protected override void SetValue(JToken payload)
        {
            Value.Set(payload.Value<bool>());
        }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
