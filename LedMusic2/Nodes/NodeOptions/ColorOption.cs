using LedMusic2.BrowserInterop;
using LedMusic2.LedColors;

namespace LedMusic2.Nodes.NodeOptions
{
    public class ColorOption : BaseOption
    {

        public override NodeOptionType Type => NodeOptionType.COLOR;
        public override string ReactiveName => "ColorOption";

        public ReactivePrimitive<LedColor> Value = new ReactivePrimitive<LedColor>("Value", new LedColorRGB(0, 0, 0));

        public ColorOption(string name) : base(name) { }

    }
}
