using LedMusic2.Reactive;
using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Nodes.NodeOptions
{
    class PreviewOption : BaseOption
    {

        public ReactivePrimitive<LedColorArray> Value { get; } = new ReactivePrimitive<LedColorArray>();

        public PreviewOption(JToken j) : base(j) { }
        public PreviewOption(string name) : base(name, NodeOptionType.PREVIEW) { }

        protected override void SetValue(JToken value)
        {
            throw new System.NotImplementedException();
        }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
