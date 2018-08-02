using LedMusic2.Reactive;
using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeOptions
{
    class PreviewOption : BaseOption
    {

        public ReactivePrimitive<LedColor[]> Value { get; } = new ReactivePrimitive<LedColor[]>();

        public PreviewOption(string name) : base(name, NodeOptionType.PREVIEW) { }

    }
}
