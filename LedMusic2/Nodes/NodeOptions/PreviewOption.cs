using LedMusic2.BrowserInterop;
using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeOptions
{
    class PreviewOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.PREVIEW;
        public override string ReactiveName => "PreviewOption";

        public ReactivePrimitive<LedColor[]> Value { get; } = new ReactivePrimitive<LedColor[]>("Value");

        public PreviewOption(string name) : base(name) { }

    }
}
