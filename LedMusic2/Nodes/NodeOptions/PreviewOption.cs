using LedMusic2.Reactive;
using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeOptions
{
    class PreviewOption : BaseOption
    {

        public ReactivePrimitive<LedColorArray> Value { get; } = new ReactivePrimitive<LedColorArray>();

        public PreviewOption(string name) : base(name, NodeOptionType.PREVIEW) { }

        public override object GetValue()
        {
            return Value.Get();
        }

    }
}
