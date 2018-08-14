using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    class TextOption : BaseOption
    {

        public ReactivePrimitive<string> Value { get; } = new ReactivePrimitive<string>();

        public TextOption() : base() { }
        public TextOption(string name) : base(name, NodeOptionType.TEXT) { }

        public override object GetValue()
        {
            return Value.Get();
        }

        protected override void SetValue(JToken value)
        {
            throw new System.NotImplementedException();
        }

    }
}
